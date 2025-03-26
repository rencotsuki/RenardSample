using System;
using System.Text;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UniRx;
using TMPro;
using SignageHADO;

[Serializable]
[DisallowMultipleComponent]
public class DeviceSensorScript : MonoBehaviourCustom
{
    [SerializeField] private WitMotionDevice witMotionDevice = null;
    [SerializeField] private SensorActionShake _sensorActionShake = default;
    [SerializeField] private AnomalyDetector _anomalyDetector = default;
    [SerializeField] private GyroVisualizer _gyroVisualizer = default;
    [SerializeField] private TMP_Text _logDevice = default;

    public bool UseWitMotion = true;

    public ScreenOrientation Orientation => ScreenOrientation.LandscapeLeft;

    private Gyroscope gyro => Input.gyro;

    /// <summary>加速度</summary>
    public Vector3 Acceleration
    {
        get
        {
            if (UseWitMotion && witMotionDevice != null)
            {
                return witMotionDevice.AngularVelocity;
            }
            else if (gyro != null)
            {
                if (Orientation == ScreenOrientation.LandscapeLeft)
                {
                    return CoodinateTransformer.LandscapeLeftToUnity(gyro.userAcceleration);
                }
                else
                {
                    return CoodinateTransformer.LandscapeRightToUnity(gyro.userAcceleration);
                }
            }
            return Vector3.zero;
        }
    }

    /// <summary>重力</summary>
    public Vector3 Gravity
    {
        get
        {
            if (UseWitMotion && witMotionDevice != null)
            {
                return witMotionDevice.Accelerometer;
            }
            else if (gyro != null)
            {
                if (Orientation == ScreenOrientation.LandscapeLeft)
                {
                    return CoodinateTransformer.LandscapeLeftToUnity(gyro.gravity);
                }
                else
                {
                    return CoodinateTransformer.LandscapeRightToUnity(gyro.gravity);
                }
            }
            return Vector3.zero;
        }
    }

    /// <summary>傾き</summary>
    public Quaternion Attitude
    {
        get
        {
            if (UseWitMotion && witMotionDevice != null)
            {
                return witMotionDevice.Angle;
            }
            if (witMotionDevice != null)
            {
                if (Orientation == ScreenOrientation.LandscapeLeft)
                {
                    return CoodinateTransformer.LandscapeLeftToUnity(gyro.attitude);
                }
                else
                {
                    return CoodinateTransformer.LandscapeRightToUnity(gyro.attitude);
                }
            }
            return Quaternion.identity;
        }
    }

    public float CalcuratedValue => _anomalyDetector != null ? _anomalyDetector.CalcuratedValue : 0f;

    public bool IsAnomaly => _anomalyDetector != null ? _anomalyDetector.IsAnomaly : false;

    public bool IsBeginner => true;

    protected bool isDrawGyro
    {
        get
        {
            if (UseWitMotion && witMotionDevice != null)
                return witMotionDevice.IsActive;

            if (gyro != null)
                return gyro.enabled;

            return false;
        }
    }

    public Subject<(SensorActionEnum, SensorArmDirectionEnum, bool)> OnUpdateSensorSubject = new Subject<(SensorActionEnum, SensorArmDirectionEnum, bool)>();

    private StringBuilder _logString = new StringBuilder();
    private float _deltaTime = 0f;
    private SensorActionEnum _sensorAction = SensorActionEnum.None;
    private SensorArmDirectionEnum _sensorArmDirection = SensorArmDirectionEnum.None;
    private bool _overAction = false;
    private CancellationTokenSource _onUpdateSensorToken = null;

    private void Start()
    {
        IsDebugLog = true;

        if (_logDevice != null)
            _logDevice.text = string.Empty;

        OnDisposeUpdateSensor();
        _onUpdateSensorToken = new CancellationTokenSource();
        OnUpdateSensorAsync(_onUpdateSensorToken.Token).Forget();
    }

    private void OnDestroy()
    {
        OnDisposeUpdateSensor();
    }

    private void OnDisposeUpdateSensor()
    {
        _onUpdateSensorToken?.Cancel();
        _onUpdateSensorToken?.Dispose();
        _onUpdateSensorToken = null;
    }

    private async UniTask OnUpdateSensorAsync(CancellationToken token)
    {
        while (_onUpdateSensorToken != null)
        {
            _deltaTime = Time.deltaTime;

            try
            {
                _sensorAction = SensorActionEnum.None;
                _sensorArmDirection = SensorArmDirection.GetArmDirection(UseWitMotion, Orientation, Gravity);

                if (_sensorActionShake != null && _sensorActionShake.CheckSensorAction(this, _deltaTime))
                    _sensorAction = _sensorAction.AddTo(_sensorActionShake.SensorAction);

                _anomalyDetector?.UpdateAnomalySensor(this, _deltaTime);
                _overAction = IsAnomaly;

                OnUpdateSensorSubject?.OnNext((_sensorAction, _sensorArmDirection, _overAction));

                _gyroVisualizer?.UpdateGyro(Attitude, IsAnomaly, isDrawGyro);
            }
            catch (Exception ex)
            {
                Log(DebugerLogType.Info, "OnUpdateSensorAsync", $"{ex.Message}");
            }

            UpdateDeviceLog();

            await UniTask.Yield(PlayerLoopTiming.Update, token);
            token.ThrowIfCancellationRequested();
        }
    }

    private void UpdateDeviceLog()
    {
        try
        {
            if (_logString == null || !IsDebugLog)
                return;

            _logString.Length = 0;
            _logString
                .Append($"[{(UseWitMotion ? "WitMotion" : isDrawGyro ? "InputGyro" : "---")}]")
                .AppendLine($"Acceleration={Acceleration}")
                .AppendLine($"Gravity={Gravity}")
                .AppendLine($"Attitude={Attitude}")
                .AppendLine("[SensorAction]")
                .AppendLine($"Action={_sensorAction}, ")
                .Append($"Direction={_sensorArmDirection}, ")
                .Append($"{(_overAction ? "OverAction" : "")}");

            if (_logDevice != null && _logDevice.text != _logString.ToString())
                _logDevice.text = _logString.ToString();
        }
        catch (Exception ex)
        {
            Log(DebugerLogType.Info, "UpdateDeviceLog", $"{ex.Message}");
        }
    }
}
