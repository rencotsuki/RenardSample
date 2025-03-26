using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
#if UNITY_ANDROID && UNITY_2018_3_OR_NEWER
using UnityEngine.Android;
#endif // UNITY_ANDROID && UNITY_2018_3_OR_NEWER

public class SampleCamera : MonoBehaviourCustom
{
    [SerializeField] private Vector2Int size = new Vector2Int(1920, 1080);
    [SerializeField] private CanvasScaler canvasScaler = null;
    [SerializeField] private RawImage rawImage = null;

    private bool _isSetup = false;
    private WebCamTexture _webCamTexture = null;

    private void Awake()
    {
        if (rawImage != null)
            rawImage.enabled = false;
    }

    private void Update()
    {
        if (_isSetup) return;
        SetupWebCam();
    }

    // カメラパーミッション
    private bool GetAuthorizationCamera()
    {
        try
        {
#if UNITY_IOS && UNITY_2018_1_OR_NEWER
            return Application.HasUserAuthorization(UserAuthorization.WebCam);
#elif UNITY_ANDROID && UNITY_2018_3_OR_NEWER
            return Permission.HasUserAuthorizedPermission(Permission.Camera);
#else
            return (Application.platform == RuntimePlatform.WindowsEditor ||
                    Application.platform == RuntimePlatform.WindowsPlayer ||
                    Application.platform == RuntimePlatform.OSXEditor ||
                    Application.platform == RuntimePlatform.OSXPlayer);
#endif
        }
        catch (Exception ex)
        {
            Log(DebugerLogType.Info, "GetAuthorizationCamera", $"{ex.Message}");
            return false;
        }
    }

    private void SetupWebCam()
    {
        if (WebCamTexture.devices.Length > 0)
        {
            if (!GetAuthorizationCamera())
                return;

            _webCamTexture = new WebCamTexture(WebCamTexture.devices[0].name, size.x, size.y);

            if (canvasScaler != null)
            {
                canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Shrink;
                canvasScaler.referenceResolution = size;
            }

            if (rawImage != null)
            {
                rawImage.texture = _webCamTexture;
                rawImage.enabled = (rawImage.texture != null);
            }

            _webCamTexture?.Play();
        }

        _isSetup = true;
    }
}
