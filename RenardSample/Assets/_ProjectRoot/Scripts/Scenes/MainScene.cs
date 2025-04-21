using System;
using System.Text;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using SignageHADO;
using SignageHADO.Game;

[Serializable]
public class MainScene : MonoBehaviourCustom
{
    [Header("-- Controller")]
    [SerializeField] private Image imgActiveHeadTracking = null;
    [SerializeField] private Image imgActiveLeftHandTracking = null;
    [SerializeField] private Image imgActiveRightHandTracking = null;
    [SerializeField] private GameObject objectDetection = null;
    [SerializeField] private TextMeshProUGUI txtObjectDetection = null;
    [SerializeField] private Toggle toggleKeepShotAction = null;
    [SerializeField] private Image imgBatteryGage = null;
    [SerializeField] private TextMeshProUGUI txtBatteryLevel = null;
    [SerializeField] private Color colorBatteryNormal = Color.green;
    [SerializeField] private Color colorBatteryLow = Color.red;
    [Header("-- DebugUI")]
    [SerializeField] private EventSystem _debugEventSystem = null;
    [SerializeField] private Button _btnDebugMode = null;
    [SerializeField] private Image _imgDebugMode = null;
    [SerializeField] private TextMeshProUGUI _debugLastBoot = null;
    [SerializeField] private TextMeshProUGUI _debugStatus = null;
    [SerializeField] private TextMeshProUGUI _debugGameLog = null;
    [SerializeField] private CanvasGroup _canvasGroupDebugUI = null;
    [SerializeField] private DebugMainCameraView _debugMainCameraView = null;
    [SerializeField] private RawImage _mainScreenView = null;
    [SerializeField] private Toggle _toggleDisplayView = null;
    [SerializeField] private Toggle _toggleManualControl = null;
    [SerializeField] private GameObject _manualControl = null;
    [SerializeField] private VariableJoystick _joystick = null;
    [SerializeField] private Button _btnShot = null;
    [Header("-- ScreenSaver")]
    [SerializeField] private CanvasGroup _canvasGroupScreenSaver = null;
    [SerializeField] private Button _btnScreenSaver = null;
    [SerializeField] private Image _imgScreenSaver = null;
    [SerializeField] private TMP_InputField _inputScreenSaverTime = null;
    [SerializeField] private Vector2 _screenSaverSpeed = new Vector2(200f, 150f);

    protected const int loadWaitTimeMilliseconds = 3 * 1000;

    private SystemManager systemManager => SystemManager.Singleton;
    private bool debugMode
    {
        get => systemManager != null ? systemManager.DebugMode : false;
        set
        {
            if (systemManager != null)
                systemManager.DebugMode = value;
        }
    }

    private GameManager gameManager => GameManager.Singleton;
    private GameStatus gameStatus => gameManager != null ? gameManager.Status : GameStatus.None;
    private GameScore gameScore => gameManager != null ? gameManager.GameScore : default;
    private float gameTime => gameManager != null ? gameManager.GameTime : 0f;
    private float gameCount => gameManager != null ? gameManager.GameCount : 0f;
    private int comboCount => gameManager != null ? gameManager.ComboCount : 0;
    protected bool keepShotAction => gameManager != null ? gameManager.KeepShotAction : false;
    protected bool activeManualAction => gameManager != null ? gameManager.ActiveManualAction : false;

    private SignageManager signageManager => SignageManager.Singleton;
    private SignageStatus sinageStatus => signageManager != null ? signageManager.Status : SignageStatus.None;

    protected bool visibleDebugUI
    {
        get => _canvasGroupDebugUI != null ? (_canvasGroupDebugUI.alpha > 0f) : false;
        set
        {
            if (_canvasGroupDebugUI != null)
            {
                _canvasGroupDebugUI.alpha = value ? 1f : 0f;
                _canvasGroupDebugUI.blocksRaycasts = value;
            }

            if (_debugMainCameraView != null)
                _debugMainCameraView.IsDebug = value;
        }
    }

    private bool _isViewDisplay = false;
    protected bool isViewDisplay
    {
        get
        {
            if (visibleDebugUI)
            {
                if (_debugMainCameraView != null && _debugMainCameraView.IsDebug)
                    return _isViewDisplay;
            }
            return false;
        }
        set => _isViewDisplay = value;
    }

    private bool _isViewScreenSaver = false;
    protected bool isViewScreenSaver
    {
        get => _isViewScreenSaver;
        set
        {
            _isViewScreenSaver = value;

            if (_canvasGroupScreenSaver != null)
            {
                _canvasGroupScreenSaver.alpha = _isViewScreenSaver ? 1f : 0f;
                _canvasGroupScreenSaver.blocksRaycasts = _isViewScreenSaver;
            }
        }
    }

    private float batteryLevel => BatteryInfo.Level;

    private bool activeHeadTracking => TrackingHandler.IsHeadTracking;
    private bool activeLeftHandTracking => TrackingHandler.IsLeftHandTracking;
    private bool activeRightHandTracking => TrackingHandler.IsRightHandTracking;
    private bool enabledDetection => TrackingHandler.EnabledDetection;
    private int detectionTargetCount => TrackingHandler.DetectionTargetCount;

    private float joystickVertical => _joystick != null ? _joystick.Vertical : 0f;
    private float joystickHorizontal => _joystick != null ? _joystick.Horizontal : 0f;

    private StringBuilder strBattery = new StringBuilder();
    private StringBuilder gameLog = new StringBuilder();

    private float screenSaverTime = 0f;

    private float _floatParse = 0f;
    private float _lastActionTime = 0f;
    private Vector2 _screenSaverDirection = Vector2.one;
    private Vector2 _screenSaverMoveAmount = Vector2.zero;
    private Vector2 _screenSaverPosition = Vector2.zero;
    private Vector2 _screenSaverMinBounds = Vector2.zero;
    private Vector2 _screenSaverMaxBounds = Vector2.zero;

    private CancellationTokenSource _onUpdateToken = null;

    private void Awake()
    {
#if UNITY_EDITOR
        // デバッグ時にEventSystemがなかったら追加する
        if (_debugEventSystem != null)
        {
            if (FindObjectsOfType<EventSystem>(true).Length <= 0)
                Instantiate(_debugEventSystem);
        }
#endif
    }

    private void Start()
    {
        visibleDebugUI = debugMode;

        toggleKeepShotAction.isOn = keepShotAction;
        toggleKeepShotAction.onValueChanged.AddListener((isOn) => gameManager?.SetKeepShotAction(isOn));

        _toggleDisplayView.isOn = isViewDisplay;
        _toggleDisplayView.onValueChanged.AddListener((isOn) => isViewDisplay = isOn);

        _toggleManualControl.isOn = activeManualAction;
        _toggleManualControl.onValueChanged.AddListener((isOn) => gameManager?.ActiveManualControl(isOn));

        _btnShot.onClick.AddListener(() => gameManager?.ShotBullet());

        _btnDebugMode.onClick.AddListener(() => debugMode = !debugMode);

        if (_debugLastBoot != null)
            _debugLastBoot.text = $"BootTime: {DateTime.FromBinary(long.Parse(ConfigSignageHADO.LastBootTime)).ToString()}";

        _inputScreenSaverTime.onEndEdit.AddListener((value) => OnChangedValueFloat(value, (x) => SetScreenSaverTime(x)));

        OnDisposeUpdate();
        _onUpdateToken = new CancellationTokenSource();
        OnUpdateAsync(_onUpdateToken.Token).Forget();
    }

    private void OnDestroy()
    {
        OnDisposeUpdate();
    }

    private void OnDisposeUpdate()
    {
        _onUpdateToken?.Dispose();
        _onUpdateToken = null;
    }

    private async UniTask OnUpdateAsync(CancellationToken token)
    {
        SetupScreenSaver();

        while (_onUpdateToken != null)
        {
            if (_debugStatus != null && _debugStatus.text != $"{sinageStatus}::{gameStatus}")
                _debugStatus.text = $"{sinageStatus}::{gameStatus}";

            if (imgActiveHeadTracking != null && imgActiveHeadTracking.enabled != activeHeadTracking)
                imgActiveHeadTracking.enabled = activeHeadTracking;

            if (imgActiveLeftHandTracking != null && imgActiveLeftHandTracking.enabled != activeLeftHandTracking)
                imgActiveLeftHandTracking.enabled = activeLeftHandTracking;

            if (imgActiveRightHandTracking != null && imgActiveRightHandTracking.enabled != activeRightHandTracking)
                imgActiveRightHandTracking.enabled = activeRightHandTracking;

            if (objectDetection != null && objectDetection.activeSelf != enabledDetection)
                objectDetection.SetActive(enabledDetection);

            if (txtObjectDetection != null && txtObjectDetection.text != $"{detectionTargetCount}")
                txtObjectDetection.text = $"{detectionTargetCount}";

            // ゲーム中はバッテリー表示更新はしない
            if (gameStatus != GameStatus.Playing)
            {
                strBattery.Length = 0;
                strBattery.Append((batteryLevel < 0 ? "--" : $"{Mathf.Clamp((int)(batteryLevel * 100f), 0, 100)}"));

                if (batteryLevel < 0)
                {
                    if (imgBatteryGage != null)
                    {
                        if (imgBatteryGage.fillAmount != 1)
                            imgBatteryGage.fillAmount = 1f;

                        if (imgBatteryGage.color != colorBatteryNormal)
                            imgBatteryGage.color = colorBatteryNormal;
                    }
                }
                else
                {
                    if (imgBatteryGage != null)
                    {
                        if (imgBatteryGage.fillAmount != batteryLevel)
                            imgBatteryGage.fillAmount = batteryLevel;

                        if (imgBatteryGage.color != (batteryLevel > BatteryInfo.LowLevel ? colorBatteryNormal : colorBatteryLow))
                            imgBatteryGage.color = (batteryLevel > BatteryInfo.LowLevel ? colorBatteryNormal : colorBatteryLow);
                    }
                }

                if (txtBatteryLevel != null && txtBatteryLevel.text != strBattery.ToString())
                    txtBatteryLevel.text = strBattery.ToString();
            }

            gameManager?.SetManualControl(joystickVertical, joystickHorizontal);

            if (visibleDebugUI != debugMode)
                visibleDebugUI = debugMode;

            if (_imgDebugMode != null && _imgDebugMode.enabled != debugMode)
                _imgDebugMode.enabled = debugMode;

            if (_mainScreenView != null)
            {
                if ((_mainScreenView.texture != null ? _mainScreenView.texture.GetInstanceID() : 0) !=
                    (_debugMainCameraView.OutputTexture != null ? _debugMainCameraView.OutputTexture.GetInstanceID() : 0))
                {
                    _mainScreenView.texture = _debugMainCameraView.OutputTexture;
                }

                if (_mainScreenView.enabled != (isViewDisplay && _mainScreenView.texture != null))
                    _mainScreenView.enabled = (isViewDisplay && _mainScreenView.texture != null);
            }

            if (visibleDebugUI)
                UpdateGameLog();

            UpdateScreenSaver();

            await UniTask.Yield(PlayerLoopTiming.Update, token);
            token.ThrowIfCancellationRequested();
        }
    }

    private void OnChangedValueFloat(string value, Action<float> changeAction)
    {
        if (!float.TryParse(value, out _floatParse))
            return;

        if (changeAction != null)
            changeAction(_floatParse);
    }

    private void UpdateGameLog()
    {
        gameLog.Length = 0;

        if (gameManager != null)
        {
            if (gameStatus != GameStatus.None && gameStatus != GameStatus.Entry)
            {
                gameLog.AppendLine($"GameStatus: {gameStatus}");

                if (gameStatus == GameStatus.Start || gameStatus == GameStatus.Playing)
                    gameLog.AppendLine($"GameTime: {gameCount:0.0}/{gameTime:0.0}s");

                gameLog.AppendLine($"BreakEnemy: {gameScore.BreakEnemy}");
                gameLog.AppendLine($"Shot: {gameScore.Shot}([L]{gameScore.LeftShot}, [R]{gameScore.RightShot})");
                gameLog.AppendLine($"Hit: {gameScore.Hit}");

                if (gameStatus == GameStatus.Playing)
                {
                    gameLog.AppendLine($"Point: {gameScore.Point}");
                    gameLog.AppendLine($"Combo: {comboCount} max[{gameScore.MaxCombo}]");
                }
                else
                {
                    gameLog.AppendLine($"HitRate: {gameScore.HitRate:0.0}");
                    gameLog.AppendLine($"MaxCombo: {gameScore.MaxCombo}");
                }

                if (gameStatus == GameStatus.Result)
                {
                    gameLog.AppendLine();
                    gameLog.AppendLine($"Point: {gameScore.Point}");
                    gameLog.AppendLine($"ComboBonus: {gameScore.ComboBonus}");
                    gameLog.AppendLine($"KillBonus: {gameScore.KillBonus}");
                    gameLog.AppendLine();
                    gameLog.AppendLine($"TotalPoint: {gameScore.TotalPoint}");
                    gameLog.AppendLine($"Rank: {gameScore.Rank}");
                }
            }
        }

        if (_debugGameLog != null && _debugGameLog.text != gameLog.ToString())
            _debugGameLog.text = gameLog.ToString();
    }

    private void SetupScreenSaver()
    {
        if (_imgScreenSaver != null)
        {
            _screenSaverMinBounds = -new Vector2(Screen.width, Screen.height) / 2 + _imgScreenSaver.rectTransform.sizeDelta / 2;
            _screenSaverMaxBounds = new Vector2(Screen.width, Screen.height) / 2 - _imgScreenSaver.rectTransform.sizeDelta / 2;

            _imgScreenSaver.rectTransform.anchoredPosition = Vector2.zero;
            _screenSaverPosition = _imgScreenSaver.rectTransform.anchoredPosition;
        }

        screenSaverTime = ConfigSignageHADO.ScreenSaverTime;
        SetScreenSaverTime(screenSaverTime);

        _lastActionTime = Time.realtimeSinceStartup;
        isViewScreenSaver = false;
    }

    private void SetScreenSaverTime(float time)
    {
        if (time >= 0)
        {
            if (screenSaverTime != time)
            {
                screenSaverTime = time;
                ConfigSignageHADO.ScreenSaverTime = screenSaverTime;
                ConfigSignageHADO.Save();
            }
        }

        if (_inputScreenSaverTime.text != screenSaverTime.ToString())
            _inputScreenSaverTime.text = screenSaverTime.ToString();
    }

    private bool IsActionScreenSaver()
    {
        if (Application.platform == RuntimePlatform.Android ||
            Application.platform == RuntimePlatform.IPhonePlayer)
        {
            // 画面タップされたら
            if (Input.touchCount > 0)
                return true;
        }

        // 何かしらのキーが押されたら
        if (Input.anyKey || Input.GetMouseButton(0) || Input.GetMouseButton(1) || Input.GetMouseButton(2))
            return true;

        return false;
    }

    private void UpdateScreenSaver()
    {
        if (screenSaverTime <= 0)
            return;

        if (IsActionScreenSaver())
        {
            HideScreenSaver(Time.realtimeSinceStartup);
            return;
        }

        if (isViewScreenSaver)
        {
            DrawScreenSaver(Time.deltaTime);
        }
        else if (Time.realtimeSinceStartup - _lastActionTime > screenSaverTime)
        {
            isViewScreenSaver = true;

            // 移動開始方向をランダムにする
            _screenSaverDirection = new Vector2(UnityEngine.Random.Range(0, 100) > 50 ? 1f : -1f, UnityEngine.Random.Range(0, 100) > 50 ? 1f : -1f);
        }
    }

    private void HideScreenSaver(float realtimeSinceStartup)
    {
        _lastActionTime = realtimeSinceStartup;

        if (isViewScreenSaver)
            isViewScreenSaver = false;
    }

    private void DrawScreenSaver(float deltaTime)
    {
        if (_imgScreenSaver == null)
            return;

        _screenSaverMoveAmount = _screenSaverSpeed * _screenSaverDirection * deltaTime;
        _screenSaverPosition += _screenSaverMoveAmount;

        if (_screenSaverPosition.x <= _screenSaverMinBounds.x || _screenSaverPosition.x >= _screenSaverMaxBounds.x)
        {
            _screenSaverDirection.x *= -1;
            _screenSaverPosition.x = Mathf.Clamp(_screenSaverPosition.x, _screenSaverMinBounds.x, _screenSaverMaxBounds.x);
        }

        if (_screenSaverPosition.y <= _screenSaverMinBounds.y || _screenSaverPosition.y >= _screenSaverMaxBounds.y)
        {
            _screenSaverDirection.y *= -1;
            _screenSaverPosition.y = Mathf.Clamp(_screenSaverPosition.y, _screenSaverMinBounds.y, _screenSaverMaxBounds.y);
        }

        _imgScreenSaver.rectTransform.anchoredPosition = _screenSaverPosition;
    }
}
