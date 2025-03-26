using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Playables;
using UniRx;

namespace SignageHADO.Game
{
    [Serializable]
    public class EmenyAction
    {
        public Vector3 BeginPos = Vector3.zero;
        public Vector3 EndPos = Vector3.zero;
    }

    [Serializable]
    public class EnemyHandler : MonoBehaviourCustom
    {
        [SerializeField] private Transform spawnField = null;
        [SerializeField] private Transform poolRoot = null;
        [SerializeField] private EnemyTimelineHandler timelineHandler = null;
        [SerializeField] private EnemyGameUI enemyGameUIPrefab = null;

        [Header("テスト用")]
        [SerializeField] private PlayableAsset testPlayableAsset = null;

        private const float loadTimeout = 5f;
        private const string resourcesPrefabPath = "Prefabs";

        protected string enemyAssetName = string.Empty;
        protected string seAssetName = string.Empty;
        protected float worldScale = 1f;

        protected EnemyConfigObject enemyConfig = null;

        protected Dictionary<int, EnemyObject> enemyPrefabs = new Dictionary<int, EnemyObject>();
        protected Dictionary<int, EnemySEClip> enemySEClip = new Dictionary<int, EnemySEClip>();
        protected Dictionary<int, Queue<EnemyObject>> enemisePool = new Dictionary<int, Queue<EnemyObject>>();

        protected List<EnemyScoreBonus> comboBonus = new List<EnemyScoreBonus>();
        protected List<EnemyScoreBonus> killBonus = new List<EnemyScoreBonus>();
        protected List<EnemyScoreRank> scoreRank = new List<EnemyScoreRank>();

        public Subject<(int, int, bool)> OnAddScoreSubject { get; protected set; } = new Subject<(int, int, bool)>();

        private EnemyData _tmpData = null;
        private EnemyObject _tmpPrefab = null;
        private EnemyObject _tmpObject = null;
        private EnemySEClip _tmpSEClip = null;

        public EnemyData GetEmenyData(int enemyNo)
            => enemyConfig != null ? enemyConfig?.GetEnemyData(enemyNo) : null;

        protected PlayableAsset GetTimelineData(int index)
            => enemyConfig != null ? enemyConfig?.GetTimelineData(index) : null;

        public async UniTask<bool> SetupAsync(CancellationToken token, string enemyAssetName, string seAssetName, float worldScale)
        {
            this.enemyAssetName = enemyAssetName;
            this.seAssetName = seAssetName;
            this.worldScale = worldScale <= 0 ? 1f : worldScale;

            IsDebugLog = true;

            try
            {
                enemyConfig = await OnLoadAssetBundlePrefabAsync<EnemyConfigObject>(token, this.enemyAssetName, EnemyConfigObject.FileName);

                comboBonus?.Clear();
                comboBonus = enemyConfig.ComboBonus.OrderByDescending(x => x.Count).ToList();

                killBonus?.Clear();
                killBonus = enemyConfig.KillBonus.OrderByDescending(x => x.Count).ToList();

                scoreRank?.Clear();
                scoreRank = enemyConfig.ScoreRank.OrderByDescending(x => x.Point).ToList();

                if (!await OnCreatePoolObjectsAsync(token, enemyConfig.EnemyList))
                    throw new Exception("failed create bullet pool.");

                return true;
            }
            catch (Exception ex)
            {
                Log(DebugerLogType.Info, "SetupAsync", $"{ex.Message}");
            }
            return false;
        }

        private async UniTask<bool> OnCreatePoolObjectsAsync(CancellationToken token, params EnemyData[] enemise)
        {
            try
            {
                EnemyObject tmpPrefab = null;
                EnemyObject tmpEnemy = null;

                enemySEClip?.Clear();

                foreach (var enemy in enemise)
                {
                    if (enemy.No <= 0)
                        continue;

                    if (string.IsNullOrEmpty(enemyAssetName))
                    {
                        tmpPrefab = await OnLoadResourcesPrefabAsync<EnemyObject>(token, resourcesPrefabPath, enemy.ObjectName);
                    }
                    else
                    {
                        tmpPrefab = await OnLoadAssetBundlePrefabAsync<EnemyObject>(token, enemyAssetName, enemy.ObjectName);
                    }

                    if (tmpPrefab == null)
                        continue;

                    enemyPrefabs.Add(enemy.No, tmpPrefab);

                    _tmpSEClip = await OnLoadSEAsync(token, seAssetName, enemy);

                    if (_tmpSEClip != null)
                        enemySEClip.Add(enemy.No, _tmpSEClip);

                    if (enemy.PoolNum <= 0)
                        continue;

                    for (int i = 0; i < enemy.PoolNum; i++)
                    {
                        tmpEnemy = OnCreateEnemy(enemy, tmpPrefab, _tmpSEClip);
                        if (tmpEnemy == null)
                            continue;

                        if (!enemisePool.ContainsKey(enemy.No))
                            enemisePool.Add(enemy.No, new Queue<EnemyObject>());

                        enemisePool[enemy.No].Enqueue(tmpEnemy);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Log(DebugerLogType.Info, "OnCreatePoolObjectsAsync", $"{ex.Message}");
            }
            return false;
        }

        private async UniTask<T> OnLoadResourcesPrefabAsync<T>(CancellationToken token, string resourcesPath, string objectName) where T : UnityEngine.Object
        {
            try
            {
                if (string.IsNullOrEmpty(objectName))
                    throw new Exception($"object Name error. {objectName}");

                var loader = Resources.LoadAsync<T>($"{(string.IsNullOrEmpty(resourcesPath) ? "" : resourcesPath + "/")}{objectName}");

                await loader.ToUniTask(null, PlayerLoopTiming.Update, token)
                            .Timeout(TimeSpan.FromSeconds(loadTimeout));

                return (loader.isDone && loader.asset != null) ? loader.asset as T : null;
            }
            catch (Exception ex)
            {
                Log(DebugerLogType.Info, "OnLoadResourcesPrefabAsync", $"{ex.Message}");
                return null;
            }
        }

        private async UniTask<T> OnLoadAssetBundlePrefabAsync<T>(CancellationToken token, string assetBundleName, string objectName) where T : UnityEngine.Object
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

                var obj = loader.GetAsset<UnityEngine.Object>();
                if (obj == null || obj.name != objectName)
                    throw new Exception($"load target error. {assetBundleName}:{objectName} => {(obj != null ? obj.name : "null")}");

                registerObject = Instantiate(obj, poolRoot) as GameObject;
                if (registerObject == null)
                    throw new Exception($"load object registe error. {assetBundleName}:{objectName}");

                registerObject.name = $"{objectName}(Register)";
                registerObject.SetActive(false);

                var result = registerObject.GetComponent<T>();
                if (result == null)
                    throw new Exception($"getComponent not found target object. {assetBundleName}:{objectName} => {(obj != null ? obj.name : "null")}");

                return result;
            }
            catch (Exception ex)
            {
                Log(DebugerLogType.Info, "OnLoadAssetBundlePrefabAsync", $"{ex.Message}");

                if (registerObject != null)
                    Destroy(registerObject);

                return null;
            }
        }

        private async UniTask<EnemySEClip> OnLoadSEAsync(CancellationToken token, string assetName, EnemyData data)
        {
            try
            {
                if (data == null || data.No <= 0)
                    throw new Exception("not enemyData.");

                var loadSE = EnemySEClip.Create(data);

                if (!string.IsNullOrEmpty(loadSE.SpawnSE))
                    loadSE.SpawnSEClip = await OnLoadAssetBundleAudioClipAsync(token, assetName, loadSE.SpawnSE);

                if (!string.IsNullOrEmpty(loadSE.HitSE))
                    loadSE.HitSEClip = await OnLoadAssetBundleAudioClipAsync(token, assetName, loadSE.HitSE);

                if (!string.IsNullOrEmpty(loadSE.BreakSE))
                    loadSE.BreakSEClip = await OnLoadAssetBundleAudioClipAsync(token, assetName, loadSE.BreakSE);

                return loadSE;
            }
            catch (Exception ex)
            {
                Log(DebugerLogType.Info, "OnLoadSEAsync", $"{ex.Message}");
            }
            return null;
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

        private EnemyObject OnCreateEnemy(EnemyInfo info, EnemyObject origin, EnemySEClip seClip)
        {
            try
            {
                if (origin == null)
                    throw new Exception($"create origin null.");

                var bullet = Instantiate(origin, poolRoot);
                if (bullet == null)
                    throw new Exception($"failed instantiate. prafab={(origin != null ? origin.name : "null")}");

                bullet.gameObject.name = origin.name;
                bullet.gameObject.SetActive(true);
                bullet.transform.localPosition = Vector3.zero;
                bullet.transform.localRotation = Quaternion.identity;
                bullet.Setup(info, seClip, enemyGameUIPrefab, OnHitAction);
                return bullet;
            }
            catch (Exception ex)
            {
                Log(DebugerLogType.Info, "OnCreateEnemy", $"{ex.Message}");
            }
            return null;
        }

        public void Clear()
        {
            timelineHandler?.Stop();

            foreach (var item in GetComponentsInChildren<EnemyObject>())
            {
                item?.Return();
            }
        }

        public void Play()
        {
            if (SignageHandler.SignageStatus == SignageStatus.TestLoop)
            {
                timelineHandler?.SetData(testPlayableAsset);
            }
            else
            {
                timelineHandler?.SetData(GetTimelineData(0));
            }

            timelineHandler?.Play();
        }

        public void Return(EnemyObject enemy)
        {
            if (enemy == null)
                return;

            enemy.transform.SetParent(poolRoot);

            if (enemisePool.ContainsKey(enemy.EnemyNo))
            {
                _tmpData = GetEmenyData(enemy.EnemyNo);
                if (_tmpData != null)
                {
                    if (enemisePool[enemy.EnemyNo].Count < _tmpData.PoolNum)
                    {
                        enemy?.ResetObject();
                        enemy.transform.localPosition = Vector3.zero;
                        enemy.transform.localRotation = Quaternion.identity;

                        enemisePool[enemy.EnemyNo].Enqueue(enemy);
                        return;
                    }
                }
            }

            Destroy(enemy.gameObject);
        }

        public EnemyObject Spawn(int enemyNo, EmenyAction action, float duration, float runWait = 0f)
        {
            try
            {
                _tmpData = GetEmenyData(enemyNo);
                if (_tmpData == null)
                    throw new Exception($"not found enemy. no={enemyNo}");

                if (enemisePool.ContainsKey(enemyNo))
                {
                    if (enemisePool[enemyNo].Count > 0)
                        _tmpObject = enemisePool[enemyNo].Dequeue();
                }

                if (_tmpObject == null)
                {
                    if (!enemyPrefabs.ContainsKey(enemyNo))
                        throw new Exception($"not found prefab. no={enemyNo}");

                    _tmpPrefab = enemyPrefabs[enemyNo];

                    _tmpSEClip = enemySEClip != null && enemySEClip.ContainsKey(enemyNo) ? enemySEClip[enemyNo] : null;

                    _tmpObject = OnCreateEnemy(_tmpData, _tmpPrefab, _tmpSEClip);
                    if (_tmpObject == null)
                        throw new Exception("failed rent enemy.");
                }

                _tmpObject.transform.SetParent(spawnField);
                _tmpObject.Spawn(action, duration, runWait);

                return _tmpObject;
            }
            catch (Exception ex)
            {
                Log(DebugerLogType.Info, "Spawn", $"{ex.Message}");
            }
            return null;
        }

        protected void OnHitAction(int enemyNo, int point, bool kill)
        {
            OnAddScoreSubject?.OnNext((enemyNo, point, kill));
        }

        public int GetPointComboBonus(int count)
        {
            if (comboBonus.Count > 0 && count > 0)
            {
                for (int i = 0; i < comboBonus.Count; i++)
                {
                    if (comboBonus[i].Count <= count)
                        return comboBonus[i].Point;
                }
            }
            return 0;
        }

        public int GetPointKillBonus(int count)
        {
            if (killBonus.Count > 0 && count > 0)
            {
                for (int i = 0; i < killBonus.Count; i++)
                {
                    if (killBonus[i].Count <= count)
                        return killBonus[i].Point;
                }
            }
            return 0;
        }

        public GameScoreRank GetScoreRank(int scorePoint)
        {
            if (scoreRank.Count > 0 && scorePoint > 0)
            {
                for (int i = 0; i < scoreRank.Count; i++)
                {
                    if (scoreRank[i].Point <= scorePoint)
                        return scoreRank[i].Rank;
                }
            }

            return GameScoreRank.C;
        }
    }
}
