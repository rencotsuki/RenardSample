using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Renard;

public class DisplaySettings : MonoBehaviourCustom
{
    [SerializeField] private Transform gameWorld = null;
    [SerializeField] private ExternalDisplayHandler displayHandler = null;
    [SerializeField] private Camera arCamera = null;
    [SerializeField] private TextMeshProUGUI txtARCameraPos = null;
    [SerializeField] private TMP_InputField inputMonitorPosY = null;
    [SerializeField] private TMP_InputField inputMonitorWidth = null;
    [SerializeField] private TMP_InputField inputDisplaySizeW = null;
    [SerializeField] private TMP_InputField inputDisplaySizeH = null;
    [SerializeField] private Button btnSet = null;

    /// <summary>フィールドの横幅[m](600x266cm)</summary>
    public const float WorldWidth = 6f;
    /// <summary>モニタスケール(43ｲﾝﾁ-100x55cmと考え)</summary>
    public const float DefaultMonitorWidth = 100f;
    /// <summary>モニタの高さ[m](床から画面中心)</summary>
    public const float DefaultMonitorPosY = 1.0f;

    private string strARCameraPos => arCamera != null ? $"ARCam: {arCamera.transform.position.ToString("#0.0")}|{arCamera.transform.eulerAngles.ToString("#0.0")}" : "ARCam: ---|---";

    private float monitorPosY = DefaultMonitorPosY;
    private string strMonitorPosY => $"{monitorPosY:#0.0#}";

    private float monitorWidth = DefaultMonitorWidth;
    private string strMonitorWidth => $"{monitorWidth}";

    private Vector2 displaySize = Vector2.zero;
    private string strDisplaySizeW => displayHandler != null ? $"{displaySize.x}" : "0";
    private string strDisplaySizeH => displayHandler != null ? $"{displaySize.y}" : "0";

    [HideInInspector] private int _intParse = 0;
    [HideInInspector] private float _tmpScale = 0f;

    private void Start()
    {
        monitorPosY = DefaultMonitorPosY;
        monitorWidth = DefaultMonitorWidth;
        displaySize = displayHandler.DisplaySize;
        btnSet.onClick.AddListener(OnSet);

        inputMonitorPosY.onEndEdit.AddListener((value) => OnChangedValue(value, (h) => monitorPosY = h));

        if (inputMonitorPosY != null && inputMonitorPosY.text != strMonitorPosY)
            inputMonitorPosY.text = strMonitorPosY;

        inputMonitorWidth.onEndEdit.AddListener((value) => OnChangedValue(value, (h) => monitorWidth = h));

        if (inputMonitorWidth != null && inputMonitorWidth.text != strMonitorWidth)
            inputMonitorWidth.text = strMonitorWidth;

        inputDisplaySizeW.onEndEdit.AddListener((value) => OnChangedValue(value, (x) => displaySize.x = x));
        inputDisplaySizeH.onEndEdit.AddListener((value) => OnChangedValue(value, (y) => displaySize.y = y));

        if (inputDisplaySizeW != null && inputDisplaySizeW.text != strDisplaySizeW)
            inputDisplaySizeW.text = strDisplaySizeW;

        if (inputDisplaySizeH != null && inputDisplaySizeH.text != strDisplaySizeH)
            inputDisplaySizeH.text = strDisplaySizeH;

        OnSet();
    }

    private void Update()
    {
        if (txtARCameraPos != null && txtARCameraPos.text != strARCameraPos)
            txtARCameraPos.text = strARCameraPos;
    }

    private void OnChangedValue(string value, Action<int> changeAction)
    {
        if (!int.TryParse(value, out _intParse))
            return;

        if (changeAction != null)
            changeAction(_intParse);
    }

    private void OnSet()
    {
        displayHandler?.SetDisplaySize(displaySize);

        _tmpScale = (monitorWidth / (WorldWidth * 100f));
       //SetGameWorld(_tmpScale, monitorPosY);
    }

    private void SetGameWorld(float scale, float height)
    {
        if (gameWorld != null)
        {
            gameWorld.transform.localScale = Vector3.one * scale;
            gameWorld.transform.localPosition = Vector3.up * height;
        }

        GameHandler.SetWorldScale(scale);
    }
}
