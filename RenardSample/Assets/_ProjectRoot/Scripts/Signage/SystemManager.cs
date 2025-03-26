using System;
using System.Text;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Unity.Cinemachine;
using UniRx;
using TMPro;
using Renard;
using SignageHADO;

public class SystemHandler
{
    private static SystemManager manager => SystemManager.Singleton;

    public static bool Init
        => manager != null ? manager.Init : false;

    public static bool DebugMode => manager != null ? manager.DebugMode : false;

    public static float FrameRate => manager != null ? manager.FrameRate : 0f;

    public static Camera DisplayCamera
        => manager != null ? manager.DisplayCamera : null;

    public static CinemachineBrain DrawCamera
        => manager != null ? manager.CinemachineCamera : null;

    public static void ResetSystem()
        => manager?.ResetSystem();

    #region Screen

    private static CoverScreen coverScreen
        => manager != null ? manager.CoverScreen : null;

    public static bool VisibleCover
        => coverScreen != null ? coverScreen.Visible : false;

    public static void CoverShow(float fade = 0f)
        => coverScreen?.Show(fade);

    public static void CoverShow(float fade, Color background)
        => coverScreen?.Show(fade, background);

    public static void CoverHide(float fade = 0f)
        => coverScreen?.Hide(fade);

    public static void SetCoverRoll(float roll)
        => coverScreen?.SetRoll(roll);

    #endregion
}

namespace SignageHADO
{
    using Game;
    using Net;

    [Serializable]
    public class SystemManager : SingletonMonoBehaviourCustom<SystemManager>
    {
        [SerializeField] private Camera mainCamera = null;
        [SerializeField] private OutputDisplayHandler outputDisplayHandler = null;
        [SerializeField] private TextMeshProUGUI txtAssetsCommitHash = null;
        [SerializeField] private SignageLogHandler signageLogHandler = null;

        [Header("-- SettingsUI")]
        [SerializeField] private CanvasGroup canvasGroupSettingsConsole = null;
        [SerializeField] private Button btnSettings = null;
        [SerializeField] private Button btnSetOffset = null;
        [SerializeField] private TMP_InputField inputCameraDevicePitch = null;
        [SerializeField] private Button btnGyroActive = null;
        [SerializeField] private Image imgGyroActive = null;

        [Header("-- DeviceRolling")]
        [SerializeField] private CanvasGroup canvasGroupDeviceRolling = null;
        [SerializeField] private Image imgPitchGage = null;
        [SerializeField] private Image imgPitchGageInvert = null;
        [SerializeField] private Image imgPitchDevice = null;
        [SerializeField] private TextMeshProUGUI txtPitch = null;
        [SerializeField] private Image imgYawGage = null;
        [SerializeField] private Image imgYawGageInvert = null;
        [SerializeField] private Image imgYawDevice = null;
        [SerializeField] private TextMeshProUGUI txtYaw = null;
        [SerializeField] private Image imgRollGage = null;
        [SerializeField] private Image imgRollGageInvert = null;
        [SerializeField] private Image imgRollDevice = null;
        [SerializeField] private TextMeshProUGUI txtRoll = null;

        [Header("起動設定")]
        [SerializeField] private string assetNameScenes = "asset_scenes";
        [SerializeField] private string additiveSceneName = "SignageScene";
        [Header("-> 無反応リセット時間[s]")]
        [SerializeField] private float escLostTrackingTime = 15f;
        [SerializeField] private float escFadeInTime = 1f;
        [Header("-> ログ出力先 ※[build=persistentDataPath, editor=dataPath/../..]から")]
        [SerializeField, TextArea] private string outputLogPath = "Logs";

        protected const int loadWaitTimeMilliseconds = 3 * 1000;

        public bool Init { get; private set; } = false;

        private bool _debugMode = false;
        public bool DebugMode
        {
            get => _debugMode;
            set
            {
                _debugMode = value;

                if (ConfigSignageHADO.DebugMode != _debugMode)
                {
                    ConfigSignageHADO.DebugMode = _debugMode;
                    ConfigSignageHADO.Save();
                }
            }
        }

        protected bool isMobile
        {
            get
            {
#if !UNITY_EDITOR
                if (Application.platform == RuntimePlatform.IPhonePlayer ||
                    Application.platform == RuntimePlatform.Android)
                {
                    return true;
                }
#endif
                return false;
            }
        }

        private bool _openSettings = false;
        public bool OpenSettings
        {
            get => _openSettings;
            protected set
            {
                _openSettings = value;

                if (canvasGroupSettingsConsole != null)
                {
                    canvasGroupSettingsConsole.alpha = _openSettings ? 1f : 0f;
                    canvasGroupSettingsConsole.blocksRaycasts = _openSettings;
                }

                if (_openSettings)
                {
                    cameraDevicePitch = ConfigSignageHADO.CameraDevicePitch;
                    outputDisplayHandler?.OpenSettings();
                }
            }
        }

        private bool _visibleDeviceRolling = false;
        protected bool visibleDeviceRolling
        {
            get => _visibleDeviceRolling;
            set
            {
                _visibleDeviceRolling = value;

                if (canvasGroupDeviceRolling != null)
                {
                    canvasGroupDeviceRolling.alpha = _visibleDeviceRolling ? 1f : 0f;
                    canvasGroupDeviceRolling.blocksRaycasts = false;
                }
            }
        }

        public Camera MainCamera => mainCamera;
        public Camera ExternalCamera => outputDisplayHandler != null ? outputDisplayHandler.ExternalCamera : null;
        public Camera DisplayCamera => outputDisplayHandler != null ? outputDisplayHandler.DisplayCamera : null;
        public CinemachineBrain CinemachineCamera => outputDisplayHandler != null ? outputDisplayHandler.CinemachineCamera : null;

        protected bool isHeadTracking => TrackingHandler.IsHeadTracking;
        protected bool isLeftHandTracking => TrackingHandler.IsLeftHandTracking;
        protected bool isRightHandTracking => TrackingHandler.IsRightHandTracking;

        protected SignageManager signageManager => SignageManager.Singleton;
        protected bool initSignageHandler => signageManager != null ? signageManager.Init : false;
        protected SignageStatus signageStatus => signageManager != null ? signageManager.Status : SignageStatus.None;
        protected SignageStatus signageRestStatus => signageManager != null ? signageManager.RestStatus : SignageStatus.None;

        protected GameManager gameManager => GameManager.Singleton;
        protected bool initGameHandler => gameManager != null ? gameManager.Init : false;
        protected bool activeManualAction => gameManager != null ? gameManager.ActiveManualAction : false;

        private bool cameraDeviceGyroActive = false;

        private float cameraDevicePitch = 0f;
        protected string strCameraDevicePitch => $"{cameraDevicePitch:0.0}";

        protected Vector3 cameraDeviceEulerAngles => TrackingHandler.CameraDeviceEulerAngles;

        #region FPS

        [HideInInspector] private int _frameCount = 0;
        [HideInInspector] private float _prevTime = 0f;
        [HideInInspector] private float _frameTime = 0f;
        [HideInInspector] private float _frameRate = 0f;

        public float FrameRate
        {
            get
            {
                if (_frameRate > 0)
                {
                    if (Application.targetFrameRate > 0 && _frameRate > Application.targetFrameRate)
                        return Application.targetFrameRate;
                    return _frameRate;
                }
                return 0f;
            }
        }

        #endregion

        [HideInInspector] private int _intParse = 0;
        [HideInInspector] private float _floatParse = 0f;
        [HideInInspector] private float _trackingLostBegin = 0f;
        [HideInInspector] private int _rollingAngle180 = 0;
        [HideInInspector] private float _rollingFillAmount = 0f;
        [HideInInspector] private float _rollingFillAmountInvert = 0f;
        [HideInInspector] private Vector3 _rollingEulerAngles = Vector3.zero;

        private StringBuilder _strRollingAngles = new StringBuilder();

        private CancellationTokenSource _onSetupToken = null;
        private CancellationTokenSource _onUpdateToken = null;
        private CancellationTokenSource _onCallSystemEscToken = null;

        protected override void Initialized()
        {
            base.Initialized();
            OnResetFPS();

            _debugMode = ConfigSignageHADO.DebugMode;

            coverScreen?.Show();

            OnDisposeSetup();
            _onSetupToken = new CancellationTokenSource();
            OnSetupAsync(_onSetupToken.Token).Forget();
        }

        private void Start()
        {
            OpenSettings = false;

            cameraDevicePitch = ConfigSignageHADO.CameraDevicePitch;
            cameraDeviceGyroActive = ConfigSignageHADO.CameraDeviceGyroActive;

            btnSettings.onClick.AddListener(() => OpenCloseSettings());
            btnSetOffset.onClick.AddListener(OnChangeViewTrackingOffset);
            btnGyroActive.onClick.AddListener(OnChangeGyroActive);

            inputCameraDevicePitch.onEndEdit.AddListener((value) => OnChangedValueFloat(value, (x) => cameraDevicePitch = x));

            if (inputCameraDevicePitch != null)
            {
                if (inputCameraDevicePitch.text != strCameraDevicePitch)
                    inputCameraDevicePitch.text = strCameraDevicePitch;

                if (inputCameraDevicePitch.gameObject.activeSelf != !isMobile)
                    inputCameraDevicePitch.gameObject.SetActive(!isMobile);
            }

            if (btnSetOffset != null && btnSetOffset.gameObject.activeSelf != !isMobile)
                btnSetOffset.gameObject.SetActive(!isMobile);

            if (btnGyroActive != null && btnGyroActive.gameObject.activeSelf != isMobile)
                btnGyroActive.gameObject.SetActive(isMobile);

            OnChangeViewTrackingOffset();
            OnChangeGyroActive();

            visibleDeviceRolling = false;

            if (btnUpdateAssets != null)
            {
                btnUpdateAssets.onClick.AddListener(() => UpdateAssets());
                btnUpdateAssets.interactable = false;
            }

            hostAddress = ConfigSignageHADO.HostAddress;
            hostPort = ConfigSignageHADO.HostPort;

            inputAddress.onEndEdit.AddListener((value) => hostAddress = value);
            inputAddress.text = hostAddress;

            inputPort.onEndEdit.AddListener((value) => OnChangedValueInt(value, (x) => hostPort = x));
            inputPort.text = hostPort.ToString();

            visibleDownload = false;

            OnDisposeUpdate();
            _onUpdateToken = new CancellationTokenSource();
            OnUpdateAsync(_onUpdateToken.Token).Forget();
        }

        protected override void OnDestroy()
        {
            OnDisposeSetup();
            OnDisposeUpdate();
            OnDisposeCallSystemEsc();
            OnDisposeUpdateAssets();

            base.OnDestroy();
        }

        private void OnResetFPS()
        {
            _frameCount = 0;
            _prevTime = 0f;
            _frameTime = 0f;
            _frameRate = 0f;
        }

        private void Update()
        {
            // FPS計測
            {
                ++_frameCount;
                _frameTime = Time.realtimeSinceStartup - _prevTime;

                if (_frameTime >= 0.5f)
                {
                    _frameRate = _frameCount > 0 ? _frameCount / _frameTime : 0f;
                    _frameCount = 0;
                    _prevTime = Time.realtimeSinceStartup;
                }
            }
        }

        private void OnDisposeSetup()
        {
            _onSetupToken?.Dispose();
            _onSetupToken = null;
        }

        private async UniTask OnSetupAsync(CancellationToken token)
        {
            Init = false;

            try
            {
                if (!await AssetBundleHandler.SetupAsync(token))
                    throw new Exception("failed setup assetBundle manager.");

                if (!await AssetBundleHandler.LoadHashListAsync(token))
                    throw new Exception("failed Load assetBundle hashList.");

                if (await AssetBundleHandler.FixedDifference(token))
                    Log(DebugerLogType.Info, "OnSetupAsync", "there is a difference in the assetbundle.");

                var waitTimeoutToken = new CancellationTokenSource();
                waitTimeoutToken.CancelAfterSlim(TimeSpan.FromMilliseconds(loadWaitTimeMilliseconds));
                var waitLinkedToken = CancellationTokenSource.CreateLinkedTokenSource(token, waitTimeoutToken.Token);

                await UniTask.WaitUntil(() => AssetBundleHandler.IsInit, cancellationToken: waitLinkedToken.Token);
                token.ThrowIfCancellationRequested();

                if (!await OnLoadSceneAssetBundleAsync(token, assetNameScenes, additiveSceneName))
                    throw new Exception($"load assetScene error. {assetNameScenes}:{additiveSceneName}");

                await UniTask.NextFrame(token);
                token.ThrowIfCancellationRequested();

                signageManager.OnChangedSignageStatusSubject
                    .Subscribe(OnChangedSinageStatus)
                    .AddTo(signageManager);

#if UNITY_EDITOR
                var logPath = string.IsNullOrEmpty(outputLogPath) ? $"{Application.dataPath}/../.." : $"{Application.dataPath}/../../{outputLogPath}";
#else
                var logPath = string.IsNullOrEmpty(outputLogPath) ? $"{Application.persistentDataPath}" : $"{Application.persistentDataPath}/{outputLogPath}";
#endif
                var assetsCommitHash = AssetBundleHandler.AssetBundleHashList != null ? AssetBundleHandler.AssetBundleHashList.CommitHash :string.Empty;
                var licenseDate = LicenseHandler.ExpiryDateTime.ToString("yyyy/MM/dd");

                // ログ収集開始
                signageLogHandler.Setup(logPath, assetsCommitHash, licenseDate);

                ViewAssetsCommitHash(assetsCommitHash);

                Init = true;
            }
            catch (Exception ex)
            {
                Log(DebugerLogType.Info, "OnSetupAsync", $"{ex.Message}");
            }
        }

        private async UniTask<bool> OnLoadSceneAssetBundleAsync(CancellationToken token, string assetBundleName, string sceneName)
        {
            try
            {
                if (string.IsNullOrEmpty(assetBundleName) || string.IsNullOrEmpty(sceneName))
                    throw new Exception($"scene Name error. {assetBundleName}:{sceneName}");

                var loader = AssetBundleHandler.LoadLevelAsync(assetBundleName, sceneName, true);
                if (loader == null)
                    throw new Exception($"load assetScene not found. {assetBundleName}:{sceneName}");

                var loadTimeoutToken = new CancellationTokenSource();
                loadTimeoutToken.CancelAfterSlim(TimeSpan.FromMilliseconds(AssetBundleHandler.LoadingTimeOutMilliseconds));
                var loadLinkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(token, loadTimeoutToken.Token);

                await loader.ToUniTask(PlayerLoopTiming.Update, loadLinkedTokenSource.Token);

                token.ThrowIfCancellationRequested();

                if (!loader.IsDone())
                    throw new Exception("file load error.");

                return true;
            }
            catch (Exception ex)
            {
                Log(DebugerLogType.Info, "OnLoadSceneAssetBundleAsync", $"{ex.Message}");
            }
            return false;
        }

        private void ViewAssetsCommitHash(string commitHash)
        {
            if (txtAssetsCommitHash != null)
                txtAssetsCommitHash.text = (string.IsNullOrEmpty(commitHash) ? "---" : commitHash.Substring(0, (commitHash.Length < 7 ? commitHash.Length : 7)));
        }

        private void OnDisposeUpdate()
        {
            _onUpdateToken?.Dispose();
            _onUpdateToken = null;
        }

        private async UniTask OnUpdateAsync(CancellationToken token)
        {
            try
            {
                while (_onUpdateToken != null)
                {
                    if (OpenSettings || !initSignageHandler || !initGameHandler)
                    {
                        coverScreen?.Show(coverFade);
                    }
                    else
                    {
                        coverScreen?.Hide(coverFade);
                    }

                    DrawCameraDeviceRolling();
                    DrawUpdateAssets();

                    if (Init)
                    {
                        // RestStatus以外は離脱を監視してリセット、設定を開いている時は無視
                        if (signageStatus != signageRestStatus && !OpenSettings)
                        {
                            if (isHeadTracking || activeManualAction)
                            {
                                _trackingLostBegin = 0f;
                            }
                            else
                            {
                                if (_trackingLostBegin <= 0)
                                {
                                    _trackingLostBegin = Time.realtimeSinceStartup;
                                }
                                else if (Time.realtimeSinceStartup - _trackingLostBegin >= escLostTrackingTime)
                                {
                                    CallSystemEsc();
                                    _trackingLostBegin = 0f;
                                }
                            }
                        }
                        else
                        {
                            _trackingLostBegin = 0f;
                        }
                    }

                    await UniTask.Yield(PlayerLoopTiming.Update, token);
                    token.ThrowIfCancellationRequested();
                }
            }
            catch (Exception ex)
            {
                Log(DebugerLogType.Info, "OnUpdateAsync", $"{ex.Message}");
            }
        }

        private void OnChangedSinageStatus(SignageStatus status)
        {
            if (status == SignageStatus.None)
                ResetView();
        }

        private void OpenCloseSettings()
        {
            OpenSettings = !OpenSettings;

            if (OpenSettings)
            {
                // 開いたら処理を止める
                Esc();
            }
            else
            {
                // 閉じたらリセットする
                ResetSystem();
            }
        }

        private void OnChangeViewTrackingOffset()
        {
            // 今はPitch以外は使わない
            TrackingHandler.SetEulerAngles(new Vector3(cameraDevicePitch, 0f, 0f));

            if (ConfigSignageHADO.CameraDevicePitch != cameraDevicePitch)
            {
                ConfigSignageHADO.CameraDevicePitch = cameraDevicePitch;
                ConfigSignageHADO.Save();
            }
        }

        private void OnChangeGyroActive()
        {
            cameraDeviceGyroActive = !cameraDeviceGyroActive;

            TrackingHandler.SetGyroActive(cameraDeviceGyroActive);

            if (ConfigSignageHADO.CameraDeviceGyroActive != cameraDeviceGyroActive)
            {
                ConfigSignageHADO.CameraDeviceGyroActive = cameraDeviceGyroActive;
                ConfigSignageHADO.Save();
            }
        }

        private void OnChangedValueInt(string value, Action<int> changeAction)
        {
            if (!int.TryParse(value, out _intParse))
                return;

            if (changeAction != null)
                changeAction(_intParse);
        }

        private void OnChangedValueFloat(string value, Action<float> changeAction)
        {
            if (!float.TryParse(value, out _floatParse))
                return;

            if (changeAction != null)
                changeAction(_floatParse);
        }

        private void CallSystemEsc()
        {
            OnDisposeCallSystemEsc();
            _onCallSystemEscToken = new CancellationTokenSource();
            OnCallSystemEscAsync(_onCallSystemEscToken.Token).Forget();
        }

        private void OnDisposeCallSystemEsc()
        {
            _onCallSystemEscToken?.Dispose();
            _onCallSystemEscToken = null;
        }

        private async UniTask OnCallSystemEscAsync(CancellationToken token)
        {
            try
            {
                sceneTransition?.PlayFadeIn(escFadeInTime);

                await UniTask.Delay(TimeSpan.FromSeconds(escFadeInTime), cancellationToken: token);
                token.ThrowIfCancellationRequested();

                ResetSystem();
            }
            catch (Exception ex)
            {
                Log(DebugerLogType.Info, "OnCallSystemEscAsync", $"{ex.Message}");
            }
        }

        private void DrawCameraDeviceRolling()
        {
            if (imgGyroActive != null && imgGyroActive.enabled != cameraDeviceGyroActive)
                imgGyroActive.enabled = cameraDeviceGyroActive;

            if (isMobile)
            {
                if (visibleDeviceRolling != Init && cameraDeviceGyroActive)
                    visibleDeviceRolling = Init && cameraDeviceGyroActive;
            }
            else
            {
                if (visibleDeviceRolling != Init)
                    visibleDeviceRolling = Init;
            }

            if (!visibleDeviceRolling)
                return;

            // Pitch
            {
                _rollingAngle180 = (int)(Mathf.Repeat(cameraDeviceEulerAngles.x + 180f, 360f) - 180f);
                _rollingFillAmount = _rollingAngle180 >= 0 ? (_rollingAngle180 / 360f) : 0f;
                _rollingFillAmountInvert = _rollingAngle180 < 0 ? Mathf.Abs(_rollingAngle180 / 360f) : 0f;
                _rollingEulerAngles = Vector3.zero;
                _rollingEulerAngles.z = cameraDeviceEulerAngles.x;
                _strRollingAngles.Length = 0;
                _strRollingAngles.Append(_rollingAngle180);

                if (txtPitch != null && txtPitch.text != _strRollingAngles.ToString())
                    txtPitch.text = _strRollingAngles.ToString();

                if (imgPitchGage != null && imgPitchGage.fillAmount != _rollingFillAmount)
                    imgPitchGage.fillAmount = _rollingFillAmount;

                if (imgPitchGageInvert != null && imgPitchGageInvert.fillAmount != _rollingFillAmountInvert)
                    imgPitchGageInvert.fillAmount = _rollingFillAmountInvert;

                if (imgPitchDevice != null && imgPitchDevice.rectTransform.localEulerAngles != _rollingEulerAngles)
                    imgPitchDevice.rectTransform.localEulerAngles = _rollingEulerAngles;
            }

            // Yaw
            {
                _rollingAngle180 = (int)(Mathf.Repeat(cameraDeviceEulerAngles.y + 180f, 360f) - 180f);
                _rollingFillAmount = _rollingAngle180 >= 0 ? (_rollingAngle180 / 360f) : 0f;
                _rollingFillAmountInvert = _rollingAngle180 < 0 ? Mathf.Abs(_rollingAngle180 / 360f) : 0f;
                _rollingEulerAngles = Vector3.zero;
                _rollingEulerAngles.y = cameraDeviceEulerAngles.y;
                _strRollingAngles.Length = 0;
                _strRollingAngles.Append(_rollingAngle180);

                if (txtYaw != null && txtYaw.text != _strRollingAngles.ToString())
                    txtYaw.text = _strRollingAngles.ToString();

                if (imgYawGage != null && imgYawGage.fillAmount != _rollingFillAmount)
                    imgYawGage.fillAmount = _rollingFillAmount;

                if (imgYawGageInvert != null && imgYawGageInvert.fillAmount != _rollingFillAmountInvert)
                    imgYawGageInvert.fillAmount = _rollingFillAmountInvert;

                if (imgYawDevice != null && imgYawDevice.rectTransform.localEulerAngles != _rollingEulerAngles)
                    imgYawDevice.rectTransform.localEulerAngles = _rollingEulerAngles;
            }

            // Roll
            {
                _rollingAngle180 = (int)(Mathf.Repeat(cameraDeviceEulerAngles.z + 180f, 360f) - 180f);
                _rollingFillAmount = _rollingAngle180 >= 0 ? (_rollingAngle180 / 360f) : 0f;
                _rollingFillAmountInvert = _rollingAngle180 < 0 ? Mathf.Abs(_rollingAngle180 / 360f) : 0f;
                _rollingEulerAngles = Vector3.zero;
                _rollingEulerAngles.z = cameraDeviceEulerAngles.z;
                _strRollingAngles.Length = 0;
                _strRollingAngles.Append(_rollingAngle180);

                if (txtRoll != null && txtRoll.text != _strRollingAngles.ToString())
                    txtRoll.text = _strRollingAngles.ToString();

                if (imgRollGage != null && imgRollGage.fillAmount != _rollingFillAmount)
                    imgRollGage.fillAmount = _rollingFillAmount;

                if (imgRollGageInvert != null && imgRollGageInvert.fillAmount != _rollingFillAmountInvert)
                    imgRollGageInvert.fillAmount = _rollingFillAmountInvert;

                if (imgRollDevice != null && imgRollDevice.rectTransform.localEulerAngles != _rollingEulerAngles)
                    imgRollDevice.rectTransform.localEulerAngles = _rollingEulerAngles;
            }
        }

        public void Esc()
        {
            gameManager?.Esc();
            signageManager?.Esc();
            ResetView();

            SoundHandler.Stop();
        }

        public void ResetSystem()
        {
            Esc();
            signageManager?.ResetStatus();
        }

        public void ResetView()
        {
            signageView?.ResetUI();
            tutorialView?.ResetUI();
        }

        #region Screen

        [Header("Screen")]
        [SerializeField] private CoverScreen coverScreen = null;

        protected const float coverFade = 0.15f;

        public CoverScreen CoverScreen => coverScreen;

        [SerializeField] private SceneTransition sceneTransition = null;

        public TransitionParameter DefaultFadeIn => sceneTransition.FadeIn;

        public TransitionParameter DefaultFadeOut => sceneTransition.FadeOut;

        public void ScreenTransitionFadeIn(Color graphicColor, float fadeTime, float delayTime = 0)
            => sceneTransition?.PlayFadeIn(graphicColor, fadeTime, delayTime);

        public void ScreenTransitionFadeOut(Color graphicColor, float fadeTime, float delayTime = 0)
            => sceneTransition?.PlayFadeOut(graphicColor, fadeTime, delayTime);

        public void ScreenTransition(Color graphicColor, TransitionParameter parameter, float fadeTime, float delayTime = 0)
            => sceneTransition?.Play(graphicColor, parameter, fadeTime, delayTime);

        [SerializeField] private SignageView signageView = null;

        public void ShowInfomation(Sprite sprite, float fadeTime = 0f)
            => signageView?.ShowInfomation(sprite, fadeTime);

        public void HideInfomation(float fadeTime = 0f)
            => signageView?.HideInfomation(fadeTime);

        public void ShowNamePlate(Sprite plate, Sprite name, Sprite nameRole, float animeTime = 0f)
            => signageView?.ShowNamePlate(plate, name, nameRole, animeTime);

        public void HideNamePlate(float animeTime = 0f)
            => signageView?.HideNamePlate(animeTime);

        public void ShowLines(string lines)
            => signageView?.ShowLines(lines);

        public void HideLines()
            => signageView?.HideLines();

        [SerializeField] private TutorialView tutorialView = null;

        public void ShowTutorial(Sprite sprite, float fadeTime = 0f)
            => tutorialView?.ShowTutorial(sprite, fadeTime);

        public void HideTutorial(float fadeTime = 0f)
            => tutorialView?.HideTutorial(fadeTime);

        #endregion

        #region UpdateAssets

        [Header("-- UpdateAssets")]
        [SerializeField] private Button btnUpdateAssets = null;
        [SerializeField] private TMP_InputField inputAddress = null;
        [SerializeField] private TMP_InputField inputPort = null;
        [SerializeField] private DownloadHandler downloadHandler = null;
        [Header("-- DownloadUI")]
        [SerializeField] private CanvasGroup _canvasGroupDownload = default;
        [SerializeField] private TextMeshProUGUI _txtDownloadStatus = default;
        [SerializeField] private GameObject _downloadCountFrame = default;
        [SerializeField] private Image _imgDownloadCountGage = default;
        [SerializeField] private TextMeshProUGUI _txtDownloadCountProgress = default;
        [SerializeField] private GameObject _downloadDataFrame = default;
        [SerializeField] private Image _imgDownloadDataGage = default;
        [SerializeField] private TextMeshProUGUI _txtDownloadDataProgress = default;

        private bool _visibleDownload = false;
        protected bool visibleDownload
        {
            get { return _visibleDownload; }
            set
            {
                _visibleDownload = value;
                _canvasGroupDownload.alpha = _visibleDownload ? 1f : 0f;
                _canvasGroupDownload.blocksRaycasts = false;
            }
        }

        private DownloadProgressInfo downloadProgress => downloadHandler.DownloadProgress != null ? downloadHandler.DownloadProgress : null;
        private bool isViewDownloadProgress => downloadProgress != null ? downloadProgress.IsActive : false;

        [HideInInspector] private float _maxDataLength = 0f;
        [HideInInspector] private float _currentLength = 0f;
        [HideInInspector] private string hostAddress = string.Empty;
        [HideInInspector] private int hostPort = 0;
        private CancellationTokenSource _onUpdateAssetsToken = null;

        private void UpdateAssets()
        {
            if (downloadHandler != null)
            {
                if (!string.IsNullOrEmpty(hostAddress) && hostPort > 0)
                {
                    var deviceAddress = NetConfigs.GetIPv4();
                    if (!string.IsNullOrEmpty(deviceAddress))
                    {
                        OnDisposeUpdateAssets();
                        _onUpdateAssetsToken = new CancellationTokenSource();
                        OnUpdateAssetsAsync(_onUpdateAssetsToken.Token, deviceAddress, hostAddress, hostPort).Forget();

                        return;
                    }
                }
            }

            FailedUpdateAssets();
        }

        private void SuccessUpdateAssets()
        {
            SystemConsoleHandler.SystemWindow.Close();

            // 成功したら接続情報を保存
            if (ConfigSignageHADO.HostAddress != hostAddress ||
                ConfigSignageHADO.HostPort != hostPort)
            {
                ConfigSignageHADO.HostAddress = hostAddress;
                ConfigSignageHADO.HostPort = hostPort;
                ConfigSignageHADO.Save();
            }

            SystemConsoleHandler.SystemWindow
                .SetMessage("Asset更新", "アプリケーションを再起動させてください")
                .OnActionDone(() =>
                {
#if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
#else
                        Application.Quit();
#endif
                },
                "O K", true)
                .Show();
        }

        private void FailedUpdateAssets()
        {
            SystemConsoleHandler.SystemWindow.Close();

            SystemConsoleHandler.SystemWindow
                .SetMessage("Asset更新", "更新に失敗しました")
                .OnActionDone(null, "O K", true)
                .Show();
        }

        private void OnDisposeUpdateAssets()
        {
            _onUpdateAssetsToken?.Dispose();
            _onUpdateAssetsToken = null;
        }

        private async UniTask OnUpdateAssetsAsync(CancellationToken token, string deviceAddress, string hostAddress, int hostPort)
        {
            try
            {
                SystemConsoleHandler.SystemWindow.Close();

                SystemConsoleHandler.SystemWindow
                    .SetMessage("Asset更新", "更新中...")
                    .Show();

                if (!await downloadHandler.SetupClientAsync(token, deviceAddress, hostAddress, hostPort, Application.persistentDataPath))
                    throw new Exception("downloadHandler setup error.");

                SuccessUpdateAssets();
            }
            catch (Exception ex)
            {
                Log(DebugerLogType.Info, "OnUpdateAssetsAsync", $"{ex.Message}");

                if (!token.IsCancellationRequested)
                    FailedUpdateAssets();
            }
        }

        private void DrawUpdateAssets()
        {
            if (btnUpdateAssets != null && btnUpdateAssets.interactable != AssetBundleHandler.IsSetup)
                btnUpdateAssets.interactable = AssetBundleHandler.IsSetup;

            UpdateDownloadView();
        }

        public void UpdateDownloadView()
        {
            if (visibleDownload != isViewDownloadProgress)
                visibleDownload = isViewDownloadProgress;

            if (!visibleDownload)
                return;

            if (!string.IsNullOrEmpty(downloadProgress.DataTitle) && downloadProgress.MaxDataLength > 0)
            {
                if (_txtDownloadStatus.text != $"{downloadProgress.Title}\n\r{downloadProgress.DataTitle}")
                    _txtDownloadStatus.text = $"{downloadProgress.Title}\n\r{downloadProgress.DataTitle}";
            }
            else
            {
                if (_txtDownloadStatus.text != $"{downloadProgress.Title}\n\r{downloadProgress.SubTitle}")
                    _txtDownloadStatus.text = $"{downloadProgress.Title}\n\r{downloadProgress.SubTitle}";
            }

            if (_downloadCountFrame != null && _downloadCountFrame.activeSelf != (downloadProgress.MaxCount > 0))
                _downloadCountFrame.SetActive((downloadProgress.MaxCount > 0));

            if (_downloadDataFrame != null && _downloadDataFrame.activeSelf != (downloadProgress.MaxDataLength > 0))
                _downloadDataFrame.SetActive((downloadProgress.MaxDataLength > 0));

            if (_imgDownloadCountGage.fillAmount != downloadProgress.Value)
                _imgDownloadCountGage.fillAmount = downloadProgress.Value;

            if (_txtDownloadCountProgress.text != $"{downloadProgress.CurrentCount: #,##0}/{downloadProgress.MaxCount: #,##0}")
                _txtDownloadCountProgress.text = $"{downloadProgress.CurrentCount: #,##0}/{downloadProgress.MaxCount: #,##0}";

            if (_imgDownloadDataGage.fillAmount != downloadProgress.ValueData)
                _imgDownloadDataGage.fillAmount = downloadProgress.ValueData;

            _currentLength = downloadProgress != null ? (float)downloadProgress.CurrentDataLength / 1024f : 0f;
            _maxDataLength = downloadProgress != null ? (float)downloadProgress.MaxDataLength / 1024f : 0f;

            if (_txtDownloadDataProgress.text != $"{_currentLength: #,##0.0}/{_maxDataLength: #,##0.0} KB")
                _txtDownloadDataProgress.text = $"{_currentLength: #,##0.0}/{_maxDataLength: #,##0.0} KB";
        }

        #endregion
    }
}
