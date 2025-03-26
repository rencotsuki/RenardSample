using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Playables;
using Unity.Cinemachine;
using UniRx;
using SignageHADO;

public enum SignageStatus : int
{
    None = 0,

    /// <summary>待機中</summary>
    DemoPart,
    /// <summary>チュートリアル</summary>
    TutorialPart,
    /// <summary>ゲーム中</summary>
    GamePart,
    /// <summary>広告中</summary>
    InfoPart,

    /// <summary>テストモード</summary>
    TestLoop
}

public static class SignageHandler
{
    private static SignageManager manager => SignageManager.Singleton;

    public static bool Init => manager != null ? manager.Init : false;

    public static SignageStatus SignageStatus => manager != null ? manager.Status : SignageStatus.None;
    public static SignageStatus SignageBeforeStatus => manager != null ? manager.BeforeStatus : SignageStatus.None;

    public static CinemachineCamera SignageCamera1
        => manager != null ? manager.SignageCamera1 : null;

    public static CinemachineCamera SignageCamera2
        => manager != null ? manager.SignageCamera2 : null;

    public static CinemachineCamera SignageCamera3
        => manager != null ? manager.SignageCamera3 : null;

    public static TimelineMotionHandler NavigateAnimator
        => manager != null ? manager.NavigateAnimator : null;
}

[Serializable]
public class NavigatorData
{
    public string AssetName = "asset_navigator";
    public string ObjectName = "NavigatorObject";

    [Header("★ VRM直入れの場合使用")]
    public bool VRMRuntimeLoad = false;
    [Header("※Application.persistentDataPathからのカレントパス(空OK)")]
    [TextArea] public string VRMFilePath = "Navigator";
    public string VRMFileName = "Suzy_1.0.0_high";
    [Header("ポーズ初期化用のAnimationClip")]
    public AnimationClip PoseResetClip = null;
}

/*
 * アプリケーションの演出進行を管理する
 */

namespace SignageHADO
{
    [Serializable]
    public class SignageManager : SingletonMonoBehaviourCustom<SignageManager>
    {
        [Serializable]
        protected class CinemachineCameraSettings
        {
            public bool ResetCamera = false;
            public CinemachineCamera Camera = null;
        }

        [SerializeField] protected CinemachineCameraSettings signageCamera1 = null;
        [SerializeField] protected CinemachineCameraSettings signageCamera2 = null;
        [SerializeField] protected CinemachineCameraSettings signageCamera3 = null;
        [SerializeField] private SignageTimelineHandler timelineHandler = null;
        [SerializeField] private NavigatorHandler navigatorHandler = null;
        [SerializeField] private SignageStatus resetStatus = SignageStatus.DemoPart;
        [Header("仮データ")]
        [SerializeField] private NavigatorData navigatorData = default;
        [Header("テストデータ")]
        [SerializeField] private PlayableAsset testPlayable = default;

        protected const int loadWaitTimeMilliseconds = 3 * 1000;

        public bool Init { get; private set; } = false;

        public SignageStatus Status { get; private set; } = SignageStatus.None;
        public SignageStatus BeforeStatus { get; private set; } = SignageStatus.None;
        public SignageStatus RestStatus => resetStatus;

        protected bool isHeadTracking => TrackingHandler.IsHeadTracking;
        protected bool isLeftHandTracking => TrackingHandler.IsLeftHandTracking;
        protected bool isRightHandTracking => TrackingHandler.IsRightHandTracking;
        protected bool activeManualAction => GameHandler.ActiveManualAction;

        public CinemachineCamera SignageCamera1 => signageCamera1.Camera;
        public CinemachineCamera SignageCamera2 => signageCamera2.Camera;
        public CinemachineCamera SignageCamera3 => signageCamera3.Camera;

        public TimelineMotionHandler NavigateAnimator => navigatorHandler != null ? navigatorHandler.TimelineAnimator : null;

        public Subject<SignageStatus> OnChangedSignageStatusSubject { get; protected set; } = new Subject<SignageStatus>();

        private CancellationTokenSource _onSetupToken = null;

        protected override void Initialized()
        {
            base.Initialized();
        }

        private void Start()
        {
            timelineHandler?.OnEndTimelineSubject
                .ThrottleFrame(3)
                .Subscribe(_ => OnNextSinageStatus())
                .AddTo(this);

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
            Init = false;

            try
            {
                var waitTimeoutToken = new CancellationTokenSource();
                waitTimeoutToken.CancelAfterSlim(TimeSpan.FromMilliseconds(loadWaitTimeMilliseconds));
                var waitLinkedToken = CancellationTokenSource.CreateLinkedTokenSource(token, waitTimeoutToken.Token);

                await UniTask.WaitUntil(() => AssetBundleHandler.IsInit, cancellationToken: waitLinkedToken.Token);

                if (navigatorData.VRMRuntimeLoad)
                {
                    if (!await navigatorHandler.SetupVRMRuntimeLoadAsync(token, navigatorData.VRMFilePath, navigatorData.VRMFileName, navigatorData.PoseResetClip))
                        throw new Exception($"failed load model. name={navigatorData.VRMFileName}");
                }
                else
                {
                    if (!await navigatorHandler.SetupAssetBundleLoadAsync(token, navigatorData.AssetName, navigatorData.ObjectName))
                        throw new Exception($"failed load model. asset={navigatorData.AssetName}, name={navigatorData.ObjectName}");
                }

                await UniTask.WaitUntil(() => SystemHandler.Init, cancellationToken: token);
                token.ThrowIfCancellationRequested();

                navigatorHandler.SetLookAtTarget(SystemHandler.DisplayCamera.transform);
                navigatorHandler.Visible = true;

                ResetStatus();

                Init = true;
            }
            catch (Exception ex)
            {
                Log(DebugerLogType.Info, "OnSetupAsync", $"{ex.Message}");
            }
        }

        protected override void OnDestroy()
        {
            OnDisposeSetup();
            base.OnDestroy();
        }

        private void Update()
        {
            if (Init)
            {
                if (Status != SignageStatus.None)
                {
                    if (timelineHandler.EnabledPlayableDirector)
                    {
                        ChangedSignageCamera();
                    }
                }
            }
        }

        private void ResetSignageCamera()
        {
            // バーチャルカメラのLive状態を見てEnabledを変更させる

            if (signageCamera1 != null)
                signageCamera1.Camera.enabled = signageCamera1.ResetCamera;

            if (signageCamera2 != null)
                signageCamera2.Camera.enabled = signageCamera2.ResetCamera;

            if (signageCamera3 != null)
                signageCamera3.Camera.enabled = signageCamera3.ResetCamera;
        }

        private void ChangedSignageCamera()
        {
            // バーチャルカメラのLive状態を見てEnabledを変更させる

            if (signageCamera1 != null && signageCamera1.Camera.enabled != signageCamera1.Camera.IsLive)
                signageCamera1.Camera.enabled = signageCamera1.Camera.IsLive;

            if (signageCamera2 != null && signageCamera2.Camera.enabled != signageCamera2.Camera.IsLive)
                signageCamera2.Camera.enabled = signageCamera2.Camera.IsLive;

            if (signageCamera3 != null && signageCamera3.Camera.enabled != signageCamera3.Camera.IsLive)
                signageCamera3.Camera.enabled = signageCamera3.Camera.IsLive;
        }

        private void OnChangedSinageStatus(SignageStatus status)
        {
            BeforeStatus = Status;

            if (Status != status)
            {
                OnChangedSignageStatusSubject?.OnNext(status);
            }

            Status = status;

            if (Status == SignageStatus.TestLoop)
            {
                timelineHandler?.DebugPlay(testPlayable);
            }
            else if (Status != SignageStatus.None)
            {
                if (Status == resetStatus)
                    ResetSignageCamera();

                timelineHandler?.Play(Status);
            }
            else
            {
                timelineHandler?.Stop();
            }
        }

        private bool CheckActiveTracking()
        {
            // 頭もしくは両手が反応していたら認める
            if (isHeadTracking || (isLeftHandTracking && isRightHandTracking))
                return true;

            if (activeManualAction)
                return true;

            return false;
        }

        private void OnNextSinageStatus()
        {
            if (Status == SignageStatus.TestLoop)
                return;

            if (Status == SignageStatus.DemoPart)
            {
                if (CheckActiveTracking())
                {
                    OnChangedSinageStatus(SignageStatus.TutorialPart);
                }
                else
                {
                    OnChangedSinageStatus(SignageStatus.DemoPart);
                }
            }
            else if (Status == SignageStatus.TutorialPart)
            {
                OnChangedSinageStatus(SignageStatus.GamePart);
            }
            else if (Status == SignageStatus.GamePart)
            {
                OnChangedSinageStatus(SignageStatus.InfoPart);
            }
            else
            {
                OnChangedSinageStatus(SignageStatus.DemoPart);
            }
        }

        public void Esc()
        {
            timelineHandler?.Stop(false);
            navigatorHandler?.ResetAction();
            BeforeStatus = Status = SignageStatus.None;
            ResetSignageCamera();
        }

        public void ResetStatus()
        {
            Esc();
            OnChangedSinageStatus(resetStatus);
        }

        public void SetFacialExpression(ExpressionEnum expression, float blendTime = 0.3f)
            => navigatorHandler?.SetFacialExpression(expression, blendTime);

        public void PlayVoice(AudioClip clip, float volume = 1f)
            => navigatorHandler?.PlayVoice(clip, volume);
    }
}
