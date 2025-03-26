using System;
using UnityEngine;
using Renard;

[Serializable]
[DisallowMultipleComponent]
public class ConfigSignageHADO : ConfigScript<ConfigSignageHADO>
{
    /// <summary>生成</summary>
    public static void CreateLoad()
    {
        Create("ConfigSignageHADO");
        Load();
    }

    public const int DefaultDisplaySizeW = 1080;
    public const int DefaultDisplaySizeH = 1920;
    public const string DefaultHostAddress = "192.168.1.2";
    public const int DefaultHostPort = 11029;

    #region データ定義

    protected string lastBootTime = string.Empty;
    public static string LastBootTime
    {
        get { return singleton != null ? singleton.lastBootTime : string.Empty; }
        set
        {
            if (singleton == null) return;
            singleton.lastBootTime = value;
        }
    }

    protected int displaySizeW = DefaultDisplaySizeW;
    public static int DisplaySizeW
    {
        get { return singleton != null ? singleton.displaySizeW : DefaultDisplaySizeW; }
        set
        {
            if (singleton == null) return;
            singleton.displaySizeW = value;
        }
    }

    protected int displaySizeH = DefaultDisplaySizeH;
    public static int DisplaySizeH
    {
        get { return singleton != null ? singleton.displaySizeH : DefaultDisplaySizeH; }
        set
        {
            if (singleton == null) return;
            singleton.displaySizeH = value;
        }
    }

    protected float displayRoll = 0f;
    public static float DisplayRoll
    {
        get { return singleton != null ? singleton.displayRoll : 0f; }
        set
        {
            if (singleton == null) return;
            singleton.displayRoll = value;
        }
    }

    protected int cameraDeviceGyroActive = 0;
    public static bool CameraDeviceGyroActive
    {
        get { return singleton != null ? (singleton.cameraDeviceGyroActive == 1) : false; }
        set
        {
            if (singleton == null) return;
            singleton.cameraDeviceGyroActive = (value ? 1 : 0);
        }
    }

    protected float cameraDevicePitch = 0f;
    public static float CameraDevicePitch
    {
        get { return singleton != null ? singleton.cameraDevicePitch : 0f; }
        set
        {
            if (singleton == null) return;
            singleton.cameraDevicePitch = value;
        }
    }

    protected int debugMode = 0;
    public static bool DebugMode
    {
        get { return singleton != null ? (singleton.debugMode == 1) : false; }
        set
        {
            if (singleton == null) return;
            singleton.debugMode = (value ? 1 : 0);
        }
    }

    protected int keepShotAction = 0;
    public static bool KeepShotAction
    {
        get { return singleton != null ? (singleton.keepShotAction == 1) : false; }
        set
        {
            if (singleton == null) return;
            singleton.keepShotAction = (value ? 1 : 0);
        }
    }

    protected string connectionDeviceName = string.Empty;
    public static string ConnectionDeviceName
    {
        get { return singleton != null ? singleton.connectionDeviceName : string.Empty; }
        set
        {
            if (singleton == null) return;
            singleton.connectionDeviceName = value;
        }
    }

    protected string hostAddress = DefaultHostAddress;
    public static string HostAddress
    {
        get { return singleton != null ? singleton.hostAddress : DefaultHostAddress; }
        set
        {
            if (singleton == null) return;
            singleton.hostAddress = value;
        }
    }

    protected int hostPort = DefaultHostPort;
    public static int HostPort
    {
        get { return singleton != null ? singleton.hostPort : DefaultHostPort; }
        set
        {
            if (singleton == null) return;
            singleton.hostPort = value;
        }
    }

    #endregion

    protected override void OnLoad()
    {
        lastBootTime = PlayerPrefs.GetString(nameof(lastBootTime), string.Empty);
        displaySizeW = PlayerPrefs.GetInt(nameof(displaySizeW), DefaultDisplaySizeW);
        displaySizeH = PlayerPrefs.GetInt(nameof(displaySizeH), DefaultDisplaySizeH);
        displayRoll = PlayerPrefs.GetFloat(nameof(displayRoll), 0f);
        cameraDeviceGyroActive = PlayerPrefs.GetInt(nameof(cameraDeviceGyroActive), 0);
        cameraDevicePitch = PlayerPrefs.GetFloat(nameof(cameraDevicePitch), 0f);
        debugMode = PlayerPrefs.GetInt(nameof(debugMode), 0);
        keepShotAction = PlayerPrefs.GetInt(nameof(keepShotAction), 0);
        connectionDeviceName = PlayerPrefs.GetString(nameof(connectionDeviceName), string.Empty);
        hostAddress = PlayerPrefs.GetString(nameof(hostAddress), DefaultHostAddress);
        hostPort = PlayerPrefs.GetInt(nameof(hostPort), DefaultHostPort);
    }

    protected override void OnSave()
    {
        PlayerPrefs.SetString(nameof(lastBootTime), lastBootTime);
        PlayerPrefs.SetInt(nameof(displaySizeW), displaySizeW);
        PlayerPrefs.SetInt(nameof(displaySizeH), displaySizeH);
        PlayerPrefs.SetFloat(nameof(displayRoll), displayRoll);
        PlayerPrefs.SetInt(nameof(cameraDeviceGyroActive), cameraDeviceGyroActive);
        PlayerPrefs.SetFloat(nameof(cameraDevicePitch), cameraDevicePitch);
        PlayerPrefs.SetInt(nameof(debugMode), debugMode);
        PlayerPrefs.SetInt(nameof(keepShotAction), keepShotAction);
        PlayerPrefs.SetString(nameof(connectionDeviceName), connectionDeviceName);
        PlayerPrefs.SetString(nameof(hostAddress), hostAddress);
        PlayerPrefs.SetInt(nameof(hostPort), hostPort);
    }

    protected override void OnDelete()
    {
        PlayerPrefs.DeleteKey(nameof(lastBootTime));
        PlayerPrefs.DeleteKey(nameof(displaySizeW));
        PlayerPrefs.DeleteKey(nameof(displaySizeH));
        PlayerPrefs.DeleteKey(nameof(displayRoll));
        PlayerPrefs.DeleteKey(nameof(cameraDeviceGyroActive));
        PlayerPrefs.DeleteKey(nameof(cameraDevicePitch));
        PlayerPrefs.DeleteKey(nameof(debugMode));
        PlayerPrefs.DeleteKey(nameof(keepShotAction));
        PlayerPrefs.DeleteKey(nameof(connectionDeviceName));
        PlayerPrefs.DeleteKey(nameof(hostAddress));
        PlayerPrefs.DeleteKey(nameof(hostPort));
    }
}