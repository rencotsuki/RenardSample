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

            await UniTask.Yield(PlayerLoopTiming.Update, token);
            token.ThrowIfCancellationRequested();
        }
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
}
