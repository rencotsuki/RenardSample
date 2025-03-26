using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace SignageHADO.Game
{
    [Serializable]
    public class BulletHandler : MonoBehaviourCustom
    {
        [SerializeField] private Transform spawnField = null;
        [SerializeField] private Transform poolRoot = null;
        [SerializeField] private int poolNum = 12;
        [SerializeField] private BulletScript defaultPrefab = null;
        [SerializeField] private AudioClip defaultShotSE = null;

        private const float loadTimeout = 5f;

        protected Queue<BulletScript> bulletPool = new Queue<BulletScript>();

        protected string seAssetName = string.Empty;
        protected float worldScale = 1f;
        private BulletScript bulletPrafab = null;
        protected AudioClip clipShotSE = null;

        private int _addCreateCount = 0;
        private BulletScript _tmpBullet = null;

        public async UniTask<bool> SetupAsync(CancellationToken token, string seAssetName, float worldScale, string shotSEName = "")
        {
            this.seAssetName = seAssetName;
            this.worldScale = worldScale <= 0 ? 1f : worldScale;

            try
            {
                if (!await OnLoadSEAsync(token, this.seAssetName, shotSEName))
                    throw new Exception("failed se.");

                if (!await OnCreatePoolObjectsAsync(token))
                    throw new Exception("failed create bullet pool.");

                return true;
            }
            catch (Exception ex)
            {
                Log(DebugerLogType.Info, "SetupAsync", $"{ex.Message}");
            }
            return false;
        }

        private async UniTask<bool> OnLoadSEAsync(CancellationToken token, string assetName, string shotSEName)
        {
            try
            {
                if (!string.IsNullOrEmpty(shotSEName))
                    clipShotSE = await OnLoadAssetBundleAudioClipAsync(token, assetName, shotSEName);

                return true;
            }
            catch (Exception ex)
            {
                Log(DebugerLogType.Info, "OnLoadSEAsync", $"{ex.Message}");
            }
            finally
            {
                if (clipShotSE == null)
                    clipShotSE = defaultShotSE;
            }
            return false;
        }

        private async UniTask<AudioClip> OnLoadAssetBundleAudioClipAsync(CancellationToken token, string assetBundleName, string objectName)
        {
            GameObject registerObject = null;

            try
            {
                if (string.IsNullOrEmpty(assetBundleName) || string.IsNullOrEmpty(objectName))
                    throw new Exception($"object Name error. {assetBundleName}:{objectName}");

                var loader = AssetBundleHandler.LoadAssetAsync(assetBundleName, objectName);
                if (loader == null)
                    throw new Exception($"load asset not found. {assetBundleName}:{objectName}");

                var loadTimeoutToken = new CancellationTokenSource();
                loadTimeoutToken.CancelAfterSlim(TimeSpan.FromSeconds(loadTimeout));
                var loadLinkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(token, loadTimeoutToken.Token);

                await loader.ToUniTask(PlayerLoopTiming.Update, loadLinkedTokenSource.Token);

                if (!loader.IsDone())
                    throw new Exception($"load asset task error. {assetBundleName}:{objectName}");

                return loader.GetAsset<AudioClip>(); ;
            }
            catch (Exception ex)
            {
                Log(DebugerLogType.Info, "OnLoadAssetBundleAudioClipAsync", $"{ex.Message}");

                if (registerObject != null)
                    Destroy(registerObject);

                return null;
            }
        }

        private async UniTask<bool> OnCreatePoolObjectsAsync(CancellationToken token)
        {
            try
            {
                // TODO: AssetBundleからプレハブをロードする処理

                bulletPrafab = defaultPrefab;

                var serialCount = 1;
                BulletScript tmpBullet = null;
                for (int i = 0; i < poolNum; i++)
                {
                    tmpBullet = OnCreateBullet(bulletPrafab, serialCount);
                    if (tmpBullet == null)
                        continue;

                    bulletPool.Enqueue(tmpBullet);
                    serialCount += 1;
                }

                return true;
            }
            catch (Exception ex)
            {
                Log(DebugerLogType.Info, "OnCreatePoolObjectsAsync", $"{ex.Message}");
            }
            return false;
        }

        private BulletScript OnCreateBullet(BulletScript origin, int serialNo)
        {
            try
            {
                if (origin == null)
                    throw new Exception($"create origin null.");

                var bullet = Instantiate(origin, poolRoot);
                if (bullet == null)
                    throw new Exception($"failed instantiate. prafab={(origin != null ? origin.name : "null")}");

                bullet.gameObject.name = $"bullet{serialNo}";
                bullet.transform.localPosition = Vector3.zero;
                bullet.transform.localRotation = Quaternion.identity;
                return bullet;
            }
            catch (Exception ex)
            {
                Log(DebugerLogType.Info, "OnCreateBullet", $"{ex.Message}");
            }
            return null;
        }

        public void Clear()
        {
            foreach (var item in GetComponentsInChildren<BulletScript>())
            {
                item?.Return();
            }

            _addCreateCount = 0;
        }

        public void Return(BulletScript bullet)
        {
            if (bullet == null)
                return;

            bullet.transform.SetParent(poolRoot);

            if (bullet.SerialNo <= poolNum)
            {
                bullet.ResetObject();
                bullet.transform.localPosition = Vector3.zero;
                bullet.transform.localRotation = Quaternion.identity;

                bulletPool.Enqueue(bullet);
                return;
            }

            Destroy(bullet.gameObject);
        }

        public bool Shot(int attack, Vector3 pos, Vector3 direct)
        {
            try
            {
                if (bulletPool != null && bulletPool.Count > 0)
                    _tmpBullet = bulletPool.Dequeue();

                if (_tmpBullet == null)
                {
                    _tmpBullet = OnCreateBullet(bulletPrafab, _addCreateCount + 1);
                    if (_tmpBullet == null)
                        throw new Exception("failed rent bullet.");

                    _addCreateCount += 1;
                }

                _tmpBullet.transform.SetParent(spawnField);
                _tmpBullet.Shot(attack, pos, direct);

                SoundHandler.PlaySE(clipShotSE);
                return true;
            }
            catch (Exception ex)
            {
                Log(DebugerLogType.Info, "Shot", $"{ex.Message}");
            }
            return false;
        }
    }
}
