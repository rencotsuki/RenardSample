using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace SignageHADO
{
    [Serializable]
    [DisallowMultipleComponent]
    public class NavigatorHandler : MonoBehaviourCustom
    {
        [SerializeField] protected LipSyncScript lipSync = null;
        [SerializeField] protected Transform standPos = null;
        [Header("Model")]
        [SerializeField] protected string fileExtension = "vrm";
        [Header("※Editor起動用：Assetsからのカレントパス")]
        [SerializeField, TextArea] protected string editorOriginFilePath = "Editor/AssetBundleResources/GameObjects/Navigator/VRM";

        protected const int loadWaitTimeMilliseconds = 3 * 1000;

        public NavigatorObject NavigatorObject { get; protected set; } = null;
        protected Transform modelRoot => NavigatorObject != null ? NavigatorObject.transform : null;
        protected float lookHeight => NavigatorObject != null ? NavigatorObject.LookAtHeight : 1.3f;
        public TimelineMotionHandler TimelineAnimator => NavigatorObject != null ? NavigatorObject.TimelineMotion : null;

        protected FacialExpression facial = new FacialExpression();
        protected FacialExpression preFacial = new FacialExpression();
        protected float facialBlendTime = 0f;
        private float _facialBlendCount = 0f;

        public bool Visible
        {
            get => NavigatorObject != null ? NavigatorObject.Visible : false;
            set
            {
                NavigatorObject?.SetVisible(value);
            }
        }

        private CancellationTokenSource _onUpdateToken = null;

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
            var deltaTime = 0f;

            while (_onUpdateToken != null)
            {
                deltaTime = Time.deltaTime;

                if (Visible)
                {
                    if (facialBlendTime > 0)
                    {
                        if (_facialBlendCount - deltaTime > 0)
                        {
                            _facialBlendCount -= deltaTime;
                            preFacial.Weight = Mathf.Clamp01(_facialBlendCount / facialBlendTime);
                            facial.Weight = 1f - preFacial.Weight;
                        }
                        else
                        {
                            EndFacialBlend();
                        }
                    }

                    NavigatorObject?.UpdateFacialExpression(facial, preFacial);
                    NavigatorObject?.UpdateBlink(deltaTime);
                    NavigatorObject?.UpdateLipSync();
                }

                await UniTask.Yield(PlayerLoopTiming.Update, token);
                token.ThrowIfCancellationRequested();
            }
        }

        protected void OnClearLoadObject()
        {
            if (NavigatorObject != null)
                Destroy(NavigatorObject.gameObject);

            OnDisposeUpdate();
        }

        protected void OnSetupModel()
        {
            if (NavigatorObject == null)
                return;

            NavigatorObject.transform.SetPositionAndRotation(standPos.position, standPos.rotation);
            NavigatorObject.SetLipSync(lipSync);

            OnDisposeUpdate();
            _onUpdateToken = new CancellationTokenSource();
            OnUpdateAsync(_onUpdateToken.Token).Forget();
        }

        protected string GetFilePath(string path, string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return string.Empty;

            if (Application.isEditor)
            {
                return $"{Application.dataPath}/{editorOriginFilePath}/{fileName}.{fileExtension}";
            }
            else
            {
                return string.IsNullOrEmpty(path) ?
                        $"{Application.persistentDataPath}/{fileName}.{fileExtension}" :
                        $"{Application.persistentDataPath}/{path}/{fileName}.{fileExtension}";
            }
        }

        public async UniTask<bool> SetupVRMRuntimeLoadAsync(CancellationToken token, string path, string fileName, AnimationClip poseResetClip = null)
        {
            try
            {
                OnClearLoadObject();

                NavigatorObject = await NavigatorObject.CreateVRMRuntimeLoadAsync(token, GetFilePath(path, fileName), transform, poseResetClip, IsDebugLog);

                // Startが呼ばれてから処理されるように次のフレームに送る
                await UniTask.NextFrame(token);

                return (NavigatorObject != null);
            }
            catch (Exception ex)
            {
                Log(DebugerLogType.Info, "SetupVRMRuntimeLoadAsync", $"{ex.Message}");

                if (NavigatorObject != null)
                {
                    Destroy(NavigatorObject.gameObject);
                    NavigatorObject = null;
                }

                return false;
            }
            finally
            {
                OnSetupModel();
            }
        }

        protected async UniTask<NavigatorObject> OnLoadNavigatorObjectAsync(CancellationToken token, string assetBundleName, string fileName)
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
                    throw new Exception($"file load error. {assetBundleName}:{fileName}");

                var obj = loader.GetAsset<GameObject>();
                if (obj == null)
                    throw new Exception($"failed load gameObject. {assetBundleName}:{fileName}");

                var instance = Instantiate(loader.GetAsset<GameObject>());
                if (instance == null)
                    throw new Exception($"failed instance gameObject. {assetBundleName}:{fileName}, object={obj.name}");

                instance.name = obj.name;
                instance.transform.SetParent(transform);
                instance.transform.localPosition = Vector3.zero;
                instance.transform.localRotation = Quaternion.identity;
                instance.transform.localScale = Vector3.one;

                var result = instance.GetComponent<NavigatorObject>();
                if (result == null)
                {
                    Destroy(instance);
                    throw new Exception($"failed getComponent. not target object. {assetBundleName}:{fileName}");
                }

                return result;
            }
            catch (Exception ex)
            {
                Log(DebugerLogType.Info, "OnLoadNavigatorObjectAsync", $"{ex.Message}");
            }
            return null;
        }

        public async UniTask<bool> SetupAssetBundleLoadAsync(CancellationToken token, string assetName, string fileName)
        {
            try
            {
                OnClearLoadObject();

                var waitTimeoutToken = new CancellationTokenSource();
                waitTimeoutToken.CancelAfterSlim(TimeSpan.FromMilliseconds(loadWaitTimeMilliseconds));
                var waitLinkedToken = CancellationTokenSource.CreateLinkedTokenSource(token, waitTimeoutToken.Token);

                await UniTask.WaitUntil(() => AssetBundleHandler.IsInit, cancellationToken: waitLinkedToken.Token);

                NavigatorObject = await OnLoadNavigatorObjectAsync(token, assetName, fileName);

                // Startが呼ばれてから処理されるように次のフレームに送る
                await UniTask.NextFrame(token);

                return (NavigatorObject != null);
            }
            catch (Exception ex)
            {
                Log(DebugerLogType.Info, "SetupAssetBundleLoadAsync", $"{ex.Message}");
                return false;
            }
            finally
            {
                OnSetupModel();
            }
        }

        public void ResetAction()
        {
            facial.Clear();
            preFacial.Clear();
            EndFacialBlend();

            NavigatorObject?.ResetFacialExpression();

            lipSync?.StopVoice();
        }

        public void SetLookAtTarget(Transform target)
            => NavigatorObject?.SetLookAtTarget(target);

        public void SetFacialExpression(ExpressionEnum expression, float blendTime)
        {
            if (facial.Expression == expression)
                return;

            preFacial.Copy(facial);
            facial.Expression = expression;

            if (blendTime <= 0)
            {
                EndFacialBlend();
            }
            else
            {
                _facialBlendCount = facialBlendTime = blendTime;
                facial.Weight = 0f;
            }
        }

        private void EndFacialBlend()
        {
            facial.Weight = 1f;
            preFacial.Copy(facial);

            _facialBlendCount = facialBlendTime = 0f;
        }

        public void PlayVoice(AudioClip clip, float volume = 1f)
            => lipSync?.PlayVoice(clip, volume);
    }
}
