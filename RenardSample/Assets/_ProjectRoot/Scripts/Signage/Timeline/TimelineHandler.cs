using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UniRx;

namespace SignageHADO.Timeline
{
    [Serializable]
    public class TimelineDirectionData
    {
        public SignageStatus TargetSinageStatus = SignageStatus.None;
        [HideInInspector] public PlayableAsset PlayableAsset = null;

        public TimelineDirectionData(SignageStatus status)
        {
            TargetSinageStatus = status;
        }
    }

    [RequireComponent(typeof(PlayableDirector))]
    public abstract class TimelineHandler : MonoBehaviourCustom
    {
        [SerializeField] protected string assetName = "asset_timeline";

        protected const int loadWaitTimeMilliseconds = 3 * 1000;

        protected PlayableDirector playableDirector { get; private set; } = null;

        public bool EnabledPlayableDirector => playableDirector != null ? playableDirector.enabled : false;

        public Subject<Unit> OnEndTimelineSubject { get; private set; } = new Subject<Unit>();

        private CancellationTokenSource _onPlayToken = null;

        private void Awake()
        {
            try
            {
                playableDirector = GetComponent<PlayableDirector>();
                playableDirector.playOnAwake = false;
                playableDirector.enabled = false;
            }
            catch (Exception ex)
            {
                Log(DebugerLogType.Info, "Awake", $"{ex.Message}");
            }

            Stop(false);
        }

        private void OnDestroy()
        {
            OnDisposePlay();
        }

        private void Update()
        {
            if (playableDirector == null || !playableDirector.enabled)
                return;

            if (playableDirector.duration <= 0)
                return;

            if (playableDirector.time > 0 && playableDirector.duration <= playableDirector.time)
                Stop(true);
        }

        public void Play(TimelineDirectionData data)
            => OnPlay(data, null, DirectorWrapMode.Hold);

        protected void OnPlay(TimelineDirectionData data, PlayableAsset localPlayable, DirectorWrapMode wrapMode)
        {
            OnDisposePlay();
            _onPlayToken = new CancellationTokenSource();
            OnPlayAsync(_onPlayToken.Token, data, localPlayable, wrapMode).Forget();
        }

        private void OnDisposePlay()
        {
            _onPlayToken?.Dispose();
            _onPlayToken = null;
        }

        private async UniTask OnPlayAsync(CancellationToken token, TimelineDirectionData data, PlayableAsset localPlayable, DirectorWrapMode wrapMode)
        {
            try
            {
                if (playableDirector == null)
                    throw new Exception("null playableDirector.");

                TimelineAsset timelineAsset = null;

                if (localPlayable != null)
                {
                    timelineAsset = localPlayable as TimelineAsset;
                    if (timelineAsset == null)
                        throw new Exception("null timeline asset.");
                }
                else
                {
                    data.PlayableAsset = await OnLoadPlayableAssetAsync(token, assetName, $"{data.TargetSinageStatus}");

                    if (data.PlayableAsset == null)
                        throw new Exception("failed load playableAsset.");

                    timelineAsset = data.PlayableAsset != null ? data.PlayableAsset as TimelineAsset : null;
                    if (timelineAsset == null)
                        throw new Exception("null timeline asset.");
                }

                playableDirector.name = $"{timelineAsset.name}";
                playableDirector.playableAsset = timelineAsset;

                // 継承先での拡張処理
                await OnSetPlayableAssetsAsync(token, timelineAsset);

                playableDirector.time = 0f;
                playableDirector.extrapolationMode = wrapMode;
                playableDirector.enabled = true;
                playableDirector.Play();
            }
            catch (Exception ex)
            {
                Log(DebugerLogType.Info, "OnPlayAsync", $"{ex.Message}");
            }
        }

        protected virtual async UniTask OnSetPlayableAssetsAsync(CancellationToken token, TimelineAsset timelineAsset)
        {
            // 必要があればここを拡張する
        }

        protected async UniTask<PlayableAsset> OnLoadPlayableAssetAsync(CancellationToken token, string assetBundleName, string fileName)
        {
            try
            {
                if (string.IsNullOrEmpty(assetBundleName) || string.IsNullOrEmpty(fileName))
                    throw new Exception($"object Name error. {assetBundleName}:{fileName}");

                var loader = AssetBundleHandler.LoadAssetAsync(assetBundleName, fileName);
                if (loader == null)
                    throw new Exception($"load asset not found. {assetBundleName}:{fileName}");

                var loadTimeoutToken = new CancellationTokenSource();
                loadTimeoutToken.CancelAfterSlim(TimeSpan.FromMilliseconds(AssetBundleHandler.LoadingTimeOutMilliseconds));
                var loadLinkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(token, loadTimeoutToken.Token);

                await loader.ToUniTask(PlayerLoopTiming.Update, loadLinkedTokenSource.Token);

                token.ThrowIfCancellationRequested();

                if (!loader.IsDone())
                    throw new Exception("file load error.");

                return loader.GetAsset<PlayableAsset>();
            }
            catch (Exception ex)
            {
                Log(DebugerLogType.Info, "OnLoadPlayableAssetAsync", $"{ex.Message}");
            }
            return null;
        }

        public void Stop(bool subject = true)
        {
            try
            {
                if (playableDirector == null)
                    throw new Exception("null playableDirector.");

                playableDirector.Stop();

                if (subject && playableDirector.enabled)
                    OnEndTimelineSubject?.OnNext(Unit.Default);

                playableDirector.enabled = false;
            }
            catch (Exception ex)
            {
                Log(DebugerLogType.Info, "Stop", $"{ex.Message}");
            }
        }
    }
}
