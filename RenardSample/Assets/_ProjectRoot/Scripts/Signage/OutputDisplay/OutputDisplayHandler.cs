using System;
using UnityEngine;
using UnityEngine.UI;
using Unity.Cinemachine;
using TMPro;
using Renard;

namespace SignageHADO
{
    using UI;

    [Serializable]
    public class OutputDisplayHandler : MonoBehaviourCustom
    {
        [SerializeField] private Camera externalCamera = null;
        public Camera ExternalCamera => externalCamera;

        [SerializeField] private CinemachineBrain cinemachineCamera = null;
        public Camera DisplayCamera => cinemachineCamera != null ? cinemachineCamera.OutputCamera : null;
        public CinemachineBrain CinemachineCamera => cinemachineCamera;

        [SerializeField] private ExternalDisplayHandler externalDisplay = null;
        [SerializeField] private UI3DCanvasScaler ui3DCanvasScaler = null;
        [SerializeField] private UI3DFrame ui3DFrame = null;

        [Header("-- SettingsUI")]
        [SerializeField] private Button btnSetDisplay = null;
        [SerializeField] private TMP_InputField inputDisplaySizeW = null;
        [SerializeField] private TMP_InputField inputDisplaySizeH = null;
        [SerializeField] private Button btnSizeExchange = null;
        [SerializeField] private TextMeshProUGUI _displayRoll = null;
        [SerializeField] private Button btnDisplayLeftRoll = null;
        [SerializeField] private Button btnDisplayRightRoll = null;

        [Header("-- DebugUI")]
        [SerializeField] private CanvasGroup _canvasGroupDebugUI = null;
        [SerializeField] private TextMeshProUGUI _debugDisplaySize = null;

        protected const int loadWaitTimeMilliseconds = 3 * 1000;

        protected bool visibleDebugUI
        {
            get => _canvasGroupDebugUI != null ? (_canvasGroupDebugUI.alpha > 0f) : false;
            set
            {
                if (_canvasGroupDebugUI != null)
                {
                    _canvasGroupDebugUI.alpha = value ? 1f : 0f;
                    _canvasGroupDebugUI.blocksRaycasts = false;
                }
            }
        }

        protected bool isViewDebug => SystemHandler.DebugMode;

        private Vector2 displaySize = Vector2.zero;
        private string strDisplaySizeW => $"{displaySize.x:#,##0}";
        private string strDisplaySizeH => $"{displaySize.y:#,##0}";

        private float displayRoll = 0f;
        private string strDisplayRoll => $"Angle= {displayRoll:#0}";

        protected string strDisplaySize
        {
            get
            {
                if (externalDisplay != null)
                    return $"{externalDisplay.DisplaySize.x:#,##0} x {externalDisplay.DisplaySize.y:#,##0}";
                return string.Empty;
            }
        }

        [HideInInspector] private int _intParse = 0;
        [HideInInspector] private Vector2 _tmpDisplaySize = Vector2.zero;

        private void Start()
        {
            externalCamera.enabled = true;

            OpenSettings();

            btnSizeExchange.onClick.AddListener(() => OnExchangeDisplaySize());
            btnSetDisplay.onClick.AddListener(OnChangeDisplaySize);

            btnDisplayLeftRoll.onClick.AddListener(() => OnChangeDisplayRoll(displayRoll - 90f));
            btnDisplayRightRoll.onClick.AddListener(() => OnChangeDisplayRoll(displayRoll + 90f));

            inputDisplaySizeW.onEndEdit.AddListener((value) => OnChangedValueInt(value, (x) => displaySize.x = x));
            inputDisplaySizeH.onEndEdit.AddListener((value) => OnChangedValueInt(value, (y) => displaySize.y = y));

            externalDisplay?.Setup(DisplayCamera, displaySize, displayRoll);

            OnChangeDisplaySize();
        }

        private void Update()
        {
            UpdateDisplaySettings();

            SystemHandler.SetCoverRoll(displayRoll);

            UpdateView();
        }

        private void UpdateDisplaySettings()
        {
            if (_displayRoll != null && _displayRoll.text != strDisplayRoll)
                _displayRoll.text = strDisplayRoll;

            if (btnDisplayLeftRoll != null && btnDisplayLeftRoll.interactable != (displayRoll > -180f))
                btnDisplayLeftRoll.interactable = (displayRoll > -180f);

            if (btnDisplayRightRoll != null && btnDisplayRightRoll.interactable != (displayRoll < 180f))
                btnDisplayRightRoll.interactable = (displayRoll < 180f);
        }

        private void UpdateView()
        {
            if (ui3DCanvasScaler != null && ui3DCanvasScaler.ResolutionSize != displaySize)
                ui3DCanvasScaler.Set(displaySize, ui3DCanvasScaler.PixelsPerUnit, ui3DCanvasScaler.MatchMode);

            if (ui3DFrame != null && ui3DFrame.TargetPixel != displaySize)
                ui3DFrame.TargetPixel = displaySize;

            if (visibleDebugUI != isViewDebug)
                visibleDebugUI = isViewDebug;

            if (_debugDisplaySize != null && _debugDisplaySize.text != strDisplaySize)
                _debugDisplaySize.text = strDisplaySize;
        }

        public void OpenSettings()
        {
            displaySize = new Vector2(ConfigSignageHADO.DisplaySizeW, ConfigSignageHADO.DisplaySizeH);
            displayRoll = ConfigSignageHADO.DisplayRoll;
            OnUpdateDisplaySizeInput();
        }

        private void OnExchangeDisplaySize()
        {
            _tmpDisplaySize = displaySize;

            displaySize.x = _tmpDisplaySize.y;
            displaySize.y = _tmpDisplaySize.x;

            OnUpdateDisplaySizeInput();
        }

        private void OnUpdateDisplaySizeInput()
        {
            if (inputDisplaySizeW != null && inputDisplaySizeW.text != strDisplaySizeW)
                inputDisplaySizeW.text = strDisplaySizeW;

            if (inputDisplaySizeH != null && inputDisplaySizeH.text != strDisplaySizeH)
                inputDisplaySizeH.text = strDisplaySizeH;
        }

        private void OnChangeDisplaySize()
        {
            if (ConfigSignageHADO.DisplaySizeW != displaySize.x ||
                ConfigSignageHADO.DisplaySizeH != displaySize.y ||
                ConfigSignageHADO.DisplayRoll != displayRoll)
            {
                ConfigSignageHADO.DisplaySizeW = (int)displaySize.x;
                ConfigSignageHADO.DisplaySizeH = (int)displaySize.y;
                ConfigSignageHADO.DisplayRoll = displayRoll;
                ConfigSignageHADO.Save();
            }

            externalDisplay?.SetDisplaySize(displaySize, displayRoll);
        }

        private void OnChangeDisplayRoll(float rollValue)
        {
            displayRoll = Mathf.Clamp(rollValue, -180f, 180f);
        }

        private void OnChangedValueInt(string value, Action<int> changeAction)
        {
            if (!int.TryParse(value, out _intParse))
                return;

            if (changeAction != null)
                changeAction(_intParse);
        }
    }
}
