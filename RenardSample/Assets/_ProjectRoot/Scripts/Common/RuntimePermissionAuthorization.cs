// iOS/Androidプラットフォームで動作するランタイムパーミッション要求のサンプル
// 参考URL: https://forum.unity.com/threads/requestuserauthorization-as-a-coroutine-bugged.380666/

using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
#if UNITY_ANDROID && UNITY_2018_3_OR_NEWER
using UnityEngine.Android;
#endif // UNITY_ANDROID && UNITY_2018_3_OR_NEWER

public class RuntimePermissionAuthorization : MonoBehaviourCustom
{
    [Header("Permissions")]
    [SerializeField] private bool usedCamera = false;
    [SerializeField] private bool usedMicrophone = false;

    private bool _isRequesting = false;
    private CancellationTokenSource _onSetupToken = null;

    private void Start()
    {
        OnDisposeSetup();
        _onSetupToken = new CancellationTokenSource();
        OnSetupAsync(_onSetupToken.Token).Forget();
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        OnFixedRequesting(this.GetCancellationTokenOnDestroy(), hasFocus).Forget();
    }

    private void OnDestroy()
    {
        OnDisposeSetup();
    }

    private void OnDisposeSetup()
    {
        _onSetupToken?.Cancel();
        _onSetupToken?.Dispose();
        _onSetupToken = null;

        _isRequesting = false;
    }

    private async UniTask OnFixedRequesting(CancellationToken token, bool hasFocus)
    {
        // iOSにおいて１フレーム待たないと正しい状態が拾えないため
        await UniTask.NextFrame(token);
        token.ThrowIfCancellationRequested();

        if (_isRequesting && hasFocus)
            _isRequesting = false;
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
            throw new Exception($"not target platform. platform={Application.platform}, unityVersion={Application.unityVersion}");
#endif
        }
        catch (Exception ex)
        {
            Log(DebugerLogType.Info, "GetAuthorizationCamera", $"{ex.Message}");
            return false;
        }
    }

    // カメラパーミッション設定
    private async UniTask RequestAuthorizationCameraAsync(CancellationToken token)
    {
        try
        {
#if UNITY_IOS && UNITY_2018_1_OR_NEWER
            await RequestUserAuthorizationAsync(token, UserAuthorization.WebCam);
#elif UNITY_ANDROID && UNITY_2018_3_OR_NEWER
            await RequestUserAuthorizationAsync(token, Permission.Camera);
#else
            throw new Exception($"not target platform. platform={Application.platform}, unityVersion={Application.unityVersion}");
#endif
        }
        catch (Exception ex)
        {
            Log(DebugerLogType.Info, "RequestAuthorizationCamera", $"{ex.Message}");
        }
    }

    // マイクパーミッション
    private bool GetAuthorizationMicrophone()
    {
        try
        {
#if UNITY_IOS && UNITY_2018_1_OR_NEWER
            return Application.HasUserAuthorization(UserAuthorization.Microphone);
#elif UNITY_ANDROID && UNITY_2018_3_OR_NEWER
            return Permission.HasUserAuthorizedPermission(Permission.Microphone);
#else
            throw new Exception($"not target platform. platform={Application.platform}, unityVersion={Application.unityVersion}");
#endif
        }
        catch (Exception ex)
        {
            Log(DebugerLogType.Info, "GetAuthorizationMicrophone", $"{ex.Message}");
            return false;
        }
    }

    // マイクパーミッション設定
    private async UniTask RequestAuthorizationMicrophoneAsync(CancellationToken token)
    {
        try
        {
#if UNITY_IOS && UNITY_2018_1_OR_NEWER
            await RequestUserAuthorizationAsync(token, UserAuthorization.Microphone);
#elif UNITY_ANDROID && UNITY_2018_3_OR_NEWER
            await RequestUserAuthorizationAsync(token, Permission.Microphone);
#else
            throw new Exception($"not target platform. platform={Application.platform}, unityVersion={Application.unityVersion}");
#endif
        }
        catch (Exception ex)
        {
            Log(DebugerLogType.Info, "RequestAuthorizationMicrophoneAsync", $"{ex.Message}");
        }
    }

    private async UniTask OnSetupAsync(CancellationToken token)
    {
        try
        {
            // カメラパーミッション確認
            if (usedCamera && !GetAuthorizationCamera())
                await RequestAuthorizationCameraAsync(token);

            token.ThrowIfCancellationRequested();

            // マイクパーミッション確認
            if (usedMicrophone && !GetAuthorizationMicrophone())
                await RequestAuthorizationMicrophoneAsync(token);

            token.ThrowIfCancellationRequested();

            // パーミッションの許可状態を確認する
            if ((usedCamera && !GetAuthorizationCamera()) || (usedMicrophone && !GetAuthorizationMicrophone()))
                throw new Exception($"permission error. platform={Application.platform}, unityVersion={Application.unityVersion}, usedCamera={usedCamera}|{GetAuthorizationCamera()}, usedMicrophone={usedMicrophone}|{GetAuthorizationMicrophone()}");
        }
        catch (Exception ex)
        {
            Log(DebugerLogType.Info, "OnSetupAsync", $"{ex.Message}");
        }
    }

#if UNITY_IOS
    private async UniTask RequestUserAuthorizationAsync(CancellationToken token, UserAuthorization mode)
    {
        try
        {
            _isRequesting = true;

            await Application.RequestUserAuthorization(mode).ToUniTask(cancellationToken: token);
            token.ThrowIfCancellationRequested();

            await UniTask.Delay(TimeSpan.FromMilliseconds(500), cancellationToken: token);
            token.ThrowIfCancellationRequested();
        }
        catch (Exception ex)
        {
            Log(DebugerLogType.Info, "RequestUserAuthorizationAsync", $"{ex.Message}");
        }
        finally
        {
            _isRequesting = false;
        }
    }
#endif //UNITY_IOS

#if UNITY_ANDROID
    private async UniTask RequestUserAuthorizationAsync(CancellationToken token, string permission)
    {
        try
        {
            _isRequesting = true;

            Permission.RequestUserPermission(permission);

            await UniTask.Delay(TimeSpan.FromMilliseconds(500), cancellationToken: token);
            token.ThrowIfCancellationRequested();
        }
        catch (Exception ex)
        {
            Log(DebugerLogType.Info, "RequestUserAuthorizationAsync", $"{ex.Message}");
        }
        finally
        {
            _isRequesting = false;
        }
    }
#endif //UNITY_ANDROID
}
