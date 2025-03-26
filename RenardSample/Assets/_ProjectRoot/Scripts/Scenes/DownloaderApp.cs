using System;
using System.Text;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UniRx;
using TMPro;

namespace SignageHADO
{
    using Net;

    [Serializable]
    public class DownloaderApp : MonoBehaviourCustom
    {
        [SerializeField] private DownloadHandler downloadHandler = null;
        [Header("SetupUI")]
        [SerializeField] private CanvasGroup _canvasGroupSetupUI = default;
        [SerializeField] private TMP_Text txtIPAddress = null;
        [SerializeField] private TMP_InputField inputPort = null;
        [SerializeField] private Button btnSetup = null;
        [Header("View")]
        [SerializeField] private CanvasGroup _canvasGroupView = default;
        [SerializeField] private TMP_Text address = null;
        [SerializeField] private TMP_Text accessLog = null;

        [SerializeField] private EventSystem _debugEventSystem = null;

        private bool _visibleSetupUI = false;
        protected bool visibleSetupUI
        {
            get { return _visibleSetupUI; }
            set
            {
                _visibleSetupUI = value;

                if (_canvasGroupSetupUI != null)
                {
                    _canvasGroupSetupUI.alpha = _visibleSetupUI ? 1f : 0f;
                    _canvasGroupSetupUI.blocksRaycasts = _visibleSetupUI;
                }
            }
        }

        private bool _visibleView = false;
        protected bool visibleView
        {
            get { return _visibleView; }
            set
            {
                _visibleView = value;

                if(_canvasGroupView != null)
                {
                    _canvasGroupView.alpha = _visibleView ? 1f : 0f;
                    _canvasGroupView.blocksRaycasts = false;
                }
            }
        }

        private string ipAddress = string.Empty;
        private int port = 11029;
        private string strAddress => $"{ipAddress} : {port}";

        private string resourcesPath
        {
            get
            {
#if UNITY_EDITOR
                return $"{Application.dataPath}/Editor/.CreateAssetBundles";
#else
                return $"{Application.dataPath}/..";
#endif
            }
        }

        private const int maxLogLength = 3000;

        private string setupProgress => downloadHandler != null ? downloadHandler.SetupProgress : string.Empty;
        private string setupErrorMessages => downloadHandler != null ? downloadHandler.SetupErrorMessages : string.Empty;
        private string requestCallMessage => downloadHandler != null ? downloadHandler.RequestCallMessage : string.Empty;

        private int _intParse = 0;
        private bool _nowSetup = false;
        private StringBuilder strLog = new StringBuilder();
        private CancellationTokenSource _onSetupToken = null;
        private CancellationTokenSource _onUpdateToken = null;

        private void Awake()
        {
            // EventSystemがなかったら追加する
            if (_debugEventSystem != null)
            {
                if (FindObjectsOfType<EventSystem>(true).Length <= 0)
                    Instantiate(_debugEventSystem);
            }
        }

        private void Start()
        {
            ipAddress = NetConfigs.GetIPv4();
            port = ConfigSignageHADO.HostPort;

            inputPort.onEndEdit.AddListener((value) => OnChangedValueInt(value, (x) => port = x));
            inputPort.text = port.ToString();

            btnSetup.onClick.AddListener(OnSetup);

            this.ObserveEveryValueChanged(x => setupProgress)
                .Subscribe(SetLog)
                .AddTo(this);

            this.ObserveEveryValueChanged(x => setupErrorMessages)
                .Subscribe(SetLog)
                .AddTo(this);

            this.ObserveEveryValueChanged(x => requestCallMessage)
                .Subscribe(SetLog)
                .AddTo(this);

            visibleSetupUI = true;
            visibleView = false;
        }

        private void Update()
        {
            if (txtIPAddress != null && txtIPAddress.text != ipAddress)
                txtIPAddress.text = ipAddress;
        }

        private void OnDestroy()
        {
            OnDisposeSetup();
            OnDisposeUpdate();
        }

        private void OnSetup()
        {
            if (_nowSetup)
                return;

            OnDisposeSetup();
            _onSetupToken = new CancellationTokenSource();
            OnSetupAsync(_onSetupToken.Token).Forget();
        }

        private void OnDisposeSetup()
        {
            _onSetupToken?.Dispose();
            _onSetupToken = null;
        }

        private async UniTask OnSetupAsync(CancellationToken token)
        {
            _nowSetup = true;

            strLog.Length = 0;

            try
            {
                if (!await downloadHandler.SetupHostAsync(token, ipAddress, port, resourcesPath))
                    throw new Exception("downloadHandler setup error.");

                if (address != null && address.text != strAddress)
                    address.text = strAddress;

                if (ConfigSignageHADO.HostPort != port)
                {
                    ConfigSignageHADO.HostPort = port;
                    ConfigSignageHADO.Save();
                }

                visibleSetupUI = false;
                visibleView = true;

                OnDisposeUpdate();
                _onUpdateToken = new CancellationTokenSource();
                OnUpdateAsync(_onUpdateToken.Token).Forget();
            }
            catch (Exception ex)
            {
                Log(DebugerLogType.Info, "OnSetupAsync", $"{ex.Message}");
            }
            finally
            {
                _nowSetup = false;
            }
        }

        private void OnChangedValueInt(string value, Action<int> changeAction)
        {
            if (!int.TryParse(value, out _intParse))
                return;

            if (changeAction != null)
                changeAction(_intParse);
        }

        private void SetLog(string log)
        {
            if (string.IsNullOrEmpty(log))
                return;

            if (strLog.Length + log.Length > maxLogLength)
                strLog.Remove(0, log.Length);

            strLog.AppendLine(log);
        }

        private void OnDisposeUpdate()
        {
            _onUpdateToken?.Dispose();
            _onUpdateToken = null;
        }

        private async UniTask OnUpdateAsync(CancellationToken token)
        {
            while (_onUpdateToken != null)
            {
                if (accessLog != null && accessLog.text != strLog.ToString())
                    accessLog.text = strLog.ToString();

                await UniTask.Yield(PlayerLoopTiming.Update, token);
                token.ThrowIfCancellationRequested();
            }
        }
    }
}
