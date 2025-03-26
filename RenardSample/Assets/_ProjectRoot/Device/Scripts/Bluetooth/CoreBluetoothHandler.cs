#if UNITY_EDITOR_OSX || UNITY_IOS
#define BLUETOOTH_ACTIVE
#endif
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UniRx;
#if BLUETOOTH_ACTIVE
using UnityCoreBluetooth;
#endif

namespace SignageHADO
{
    public class CoreBluetoothHandler : MonoBehaviourCustom
    {
        [SerializeField]
        private string[] uuidList = new string[]
        {
            "000099D0-BE61-4658-8275-27620E352019",
            "0000FFE5-0000-1000-8000-00805F9A34FB"
        };

        public bool IsScanning { get; private set; } = false;

        public bool IsConnecting
        {
            get
            {
#if BLUETOOTH_ACTIVE
                if (_connectedBluetooth != null)
                    return true;
#endif
                return false;
            }
        }

        public string DeviceName
        {
            get
            {
#if BLUETOOTH_ACTIVE
                if (_peripheral != null)
                    return _peripheral.name;
#endif
                return string.Empty;
            }
        }

        public Subject<string[]> OnCompletedScanSubject { get; private set; } = new Subject<string[]>();

        public Subject<bool> OnConnectedSubject { get; private set; } = new Subject<bool>();

        public Subject<byte[]> OnUpdateCharacteristicSubject { get; private set; } = new Subject<byte[]>();

        protected bool poweredOn { get; private set; } = false;

        private const int powerOnWaitMillisecond = 5 * 1000;
        private const int scanMilliseconds = 3 * 1000; // スキャン時間[ms]

#if BLUETOOTH_ACTIVE
        private Dictionary<string, CoreBluetoothPeripheral> _bluetoothList = new Dictionary<string, CoreBluetoothPeripheral>();

        [HideInInspector] private CoreBluetoothManager _manager = null;
        [HideInInspector] private CoreBluetoothPeripheral _peripheral = null;
        [HideInInspector] private CoreBluetoothService _service = null;
        [HideInInspector] private CoreBluetoothCharacteristic _connectedBluetooth = null;
        [HideInInspector] private string[] _usage = null;
#endif
        [HideInInspector] private int _characteristic = 0;
        [HideInInspector] private int _updateCharacteristic = 0;
        private bool _trigger = false;
        private bool _triggerDown = false;
        private bool _triggerUp = false;

        private CancellationTokenSource _onScanToken = null;

        private void Start()
        {
            OnReset();
        }

        private void OnSetup()
        {
            poweredOn = false;

#if BLUETOOTH_ACTIVE
            _manager?.Stop();

            _manager = CoreBluetoothManager.Shared;
            _manager?.OnUpdateState(OnUpdateState);
            _manager?.OnDiscoverPeripheral(OnDiscoverPeripheral);
            _manager?.OnConnectPeripheral(OnConnectPeripheral);
            _manager?.OnDiscoverService(OnDiscoverService);
            _manager?.OnDiscoverCharacteristic(OnDiscoverCharacteristic);
            _manager?.OnUpdateValue(OnUpdateValue);

            _manager?.Start();
#endif
        }

        private void OnDestroy()
        {
            Stop();
        }

        private void OnReset()
        {
#if BLUETOOTH_ACTIVE
            _bluetoothList?.Clear();

            _connectedBluetooth?.SetNotifyValue(false);
            _connectedBluetooth = null;

            _service = null;
            _peripheral = null;
#endif
        }

#if BLUETOOTH_ACTIVE
        private void OnUpdateState(string state)
        {
            Log(DebugerLogType.Info, "OnUpdateState", $"state: {state}");
            poweredOn = (state == "poweredOn");
        }
        
        private async UniTask OnStanbyScanAsync(CancellationToken token, string deviceName)
        {
            OnReset();

            try
            {
                OnSetup();

                var waitTimeoutToken = new CancellationTokenSource();
                waitTimeoutToken.CancelAfterSlim(TimeSpan.FromMilliseconds(powerOnWaitMillisecond));
                var waitLinkedToken = CancellationTokenSource.CreateLinkedTokenSource(token, waitTimeoutToken.Token);

                await UniTask.WaitUntil(() => poweredOn, PlayerLoopTiming.Update, waitLinkedToken.Token);

                _manager?.StartScan();

                await UniTask.Delay(TimeSpan.FromMilliseconds(scanMilliseconds), cancellationToken: token);

                _manager?.StopScan();

                if (_bluetoothList != null && _bluetoothList.Count > 0)
                    SelectDevice(deviceName);
            }
            catch (Exception ex)
            {
                Log(DebugerLogType.Info, "OnStanbyScanAsync", $"{ex.Message}");

                OnReset();
            }
        }

        private async UniTask OnStartScanAsync(CancellationToken token)
        {
            OnReset();

            IsScanning = true;

            try
            {
                OnSetup();

                var waitTimeoutToken = new CancellationTokenSource();
                waitTimeoutToken.CancelAfterSlim(TimeSpan.FromMilliseconds(powerOnWaitMillisecond));
                var waitLinkedToken = CancellationTokenSource.CreateLinkedTokenSource(token, waitTimeoutToken.Token);

                await UniTask.WaitUntil(() => poweredOn, PlayerLoopTiming.Update, waitLinkedToken.Token);

                _manager?.StartScan();

                await UniTask.Delay(TimeSpan.FromMilliseconds(scanMilliseconds), cancellationToken: token);

                _manager?.StopScan();

                OnCompletedScanSubject?.OnNext(_bluetoothList != null && _bluetoothList.Count > 0 ? _bluetoothList.Keys.ToArray() : null);
            }
            catch (Exception ex)
            {
                Log(DebugerLogType.Info, "OnStartScanAsync", $"{ex.Message}");

                OnReset();
            }
            finally
            {
                IsScanning = false;
            }
        }

        private void OnDiscoverPeripheral(CoreBluetoothPeripheral peripheral)
        {
            Log(DebugerLogType.Info, "OnDiscoverPeripheral", $"{peripheral.name}");

            if (string.IsNullOrEmpty(peripheral.name))
                return;

            if (_bluetoothList.ContainsKey(peripheral.name))
                return;

            _bluetoothList.Add(peripheral.name, peripheral);

            Log(DebugerLogType.Info, "OnDiscoverPeripheral", $"{peripheral.name}");
        }

        private void OnConnectPeripheral(CoreBluetoothPeripheral peripheral)
        {
            Log(DebugerLogType.Info, "OnConnectPeripheral", $"{peripheral.name}");

            _peripheral = peripheral;
            _peripheral?.discoverServices();
        }

        private void OnDiscoverService(CoreBluetoothService service)
        {
            Log(DebugerLogType.Info, "OnDiscoverService", $"{service.uuid}");

            if (service.uuid == string.Empty)
                return;

            if (uuidList == null || !uuidList.Contains(service.uuid))
                return;

            _service = service;
            _service?.discoverCharacteristics();
        }

        private void OnDiscoverCharacteristic(CoreBluetoothCharacteristic characteristic)
        {
            if (characteristic == null)
                return;

            _usage = characteristic.Propertis;

            for (int i = 0; i < _usage.Length; i++)
            {
                if (_usage[i] == "notify")
                {
                    _connectedBluetooth = characteristic;
                    break;
                }
            }

            _connectedBluetooth?.SetNotifyValue(true);
            OnConnectedSubject?.OnNext(IsConnecting);
        }

        private void OnUpdateValue(CoreBluetoothCharacteristic characteristic, byte[] data)
        {
            try
            {
                OnUpdateCharacteristicSubject?.OnNext(data);

                _updateCharacteristic = BitConverter.ToInt32(data, 0);
            }
            catch (Exception ex)
            {
                Log(DebugerLogType.Info, "OnUpdateValue", $"{ex.Message}");
            }
        }
#endif

        private void OnDisposeScan()
        {
            _onScanToken?.Cancel();
            _onScanToken?.Dispose();
            _onScanToken = null;
        }

        public void StanbyScan(string deviceName)
        {
            if (string.IsNullOrEmpty(deviceName))
                return;

#if BLUETOOTH_ACTIVE
            OnDisposeScan();
            _onScanToken = new CancellationTokenSource();
            OnStanbyScanAsync(_onScanToken.Token, deviceName).Forget();
#endif
        }

        public void Scan()
        {
#if BLUETOOTH_ACTIVE
            OnDisposeScan();
            _onScanToken = new CancellationTokenSource();
            OnStartScanAsync(_onScanToken.Token).Forget();
#endif
        }

        public bool SelectDevice(string name)
        {
#if BLUETOOTH_ACTIVE
            if (!_bluetoothList.ContainsKey(name))
                return false;

            _peripheral = _bluetoothList[name];
            _manager?.ConnectToPeripheral(_peripheral);

            return true;
#else
            return false;
#endif
        }

        public void Stop()
        {
            OnDisposeScan();

#if BLUETOOTH_ACTIVE
            _manager?.Stop();
#endif

            OnReset();

            poweredOn = false;
        }
    }
}
