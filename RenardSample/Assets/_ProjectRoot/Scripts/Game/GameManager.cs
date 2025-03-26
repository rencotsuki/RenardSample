using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Unity.Cinemachine;
using UniRx;

namespace SignageHADO.Game
{
    /*
     * ゲーム部分を管理する
     */

    [Serializable]
    public class GameManager : SingletonMonoBehaviourCustom<GameManager>
    {
        public CinemachineCamera PlayerCamera = null;
        [SerializeField] private VirtualCourt virtualCourt = null;
        [Header("ゲーム設定")]
        [SerializeField] private float attackRange = 15.5f;
        [SerializeField] private float attackInterval = 0.2f;
        [SerializeField] private int attackPower = 1000;
        [Header("Assets")]
        [SerializeField] private string _fieldAssetName = "asset_virtual_field";
        [SerializeField] private string _enemyAssetName = "asset_enemy";
        [SerializeField] private string _seAssetName = "asset_se";

        public const float RecoverySpeed = 3.0f - 0.44f - 0.2f; //TODO: トータルで3秒になるようにしてある
        public const float DeathWaitTime = 0.44f;    //TODO: 素材依存しているので注意！
        public const float RespawnWaitTime = 0.2f;   //TODO: 素材依存しているので注意！

        protected float bulletScale => GameHandler.BulletScale;

        protected const int loadWaitTimeMilliseconds = 3 * 1000;

        public bool Init { get; private set; } = false;

        public GameStatus Status { get; private set; } = GameStatus.None;

        public float GameTime { get; private set; } = 0f;
        public float GameCount { get; private set; } = 0f;
        public float FillAmountGameTime { get; private set; } = 0f;

        public float ResultTime { get; private set; } = 0f;
        public float FillAmountResultTime { get; private set; } = 0f;

        public Vector3 WorldAnchorPos => virtualCourt != null ? virtualCourt.transform.position : Vector3.zero;

        public float WorldScale { get; private set; } = 1f;

        public int ComboCount { get; private set; } = 0;

        public GameScore GameScore = new GameScore();
        public int Point => GameScore.Point;
        public int BreakEnemy => GameScore.BreakEnemy;

        public Subject<Unit> OnEntrySubject { get; private set; } = new Subject<Unit>();
        public Subject<Unit> OnStartSubject { get; private set; } = new Subject<Unit>();
        public Subject<Unit> OnPlaySubject { get; private set; } = new Subject<Unit>();
        public Subject<Unit> OnEndSubject { get; private set; } = new Subject<Unit>();
        public Subject<Unit> OnEscSubject { get; private set; } = new Subject<Unit>();
        public Subject<GameScore> ReportGameResultSubject { get; private set; } = new Subject<GameScore>();

        private float _resultCount = 0f;
        private CancellationTokenSource _onSetupToken = null;
        private CancellationTokenSource _onUpdateToken = null;

        #region View

        [Header("View")]
        [SerializeField] private SignalResultView gameSignalResultView = null;

        public SignalResultView GameSignalResultView => gameSignalResultView;

        #endregion

        #region PlayerHandler

        [Header("Player")]
        [SerializeField] private PlayerHandler playerHandler = null;

        public bool KeepShotAction => playerHandler != null ? playerHandler.KeepShotAction : false;

        public bool ActiveManualAction => playerHandler != null ? playerHandler.ActiveManualAction : false;

        public void ShotBullet()
            => playerHandler?.ShotBullet();

        public void SetKeepShotAction(bool value)
            => playerHandler?.SetKeepShotAction(value);

        public void ActiveManualControl(bool active)
            => playerHandler?.ActiveManualControl(active);

        public void SetManualControl(float vertical, float horizontal)
            => playerHandler?.SetManualControl(vertical, horizontal);

        #endregion

        #region EnemyHandler

        [Header("Enemy")]
        [SerializeField] private EnemyHandler enemyHandler = null;

        public void ReturnEnemy(EnemyObject enemy)
            => enemyHandler?.Return(enemy);

        public EnemyObject SpawnEnemy(int enemyNo, EmenyAction action, float duration, float runWait = 0f)
            => enemyHandler != null ? enemyHandler.Spawn(enemyNo, action, duration, runWait) : null;

        #endregion

        #region BulletHandler

        [Header("Bullet")]
        [SerializeField] private BulletHandler bulletHandler = null;
        [SerializeField] private string _seNameShot = "se_player_attack_shot"; // TODO: あとでどこか外に持たせる

        public void ReturnBullet(BulletScript bullet)
            => bulletHandler?.Return(bullet);

        public void ShotBullet(int attack, Vector3 pos, Vector3 direct, bool right)
        {
            if (bulletHandler == null)
                return;

            if (bulletHandler.Shot(attack, pos, direct))
            {
                if (Status == GameStatus.Playing)
                {
                    if (right)
                    {
                        GameScore.RightShot += 1;
                    }
                    else
                    {
                        GameScore.LeftShot += 1;
                    }
                }
            }
        }

        #endregion

        protected override void Initialized()
        {
            base.Initialized();
        }

        private void Start()
        {
            enemyHandler?.OnAddScoreSubject
                .Subscribe(x => AddScore(x.Item1, x.Item2, x.Item3))
                .AddTo(this);

            OnDisposeSetup();
            _onSetupToken = new CancellationTokenSource();
            OnSetupAsync(_onSetupToken.Token).Forget();
        }

        protected override void OnDestroy()
        {
            OnDisposeSetup();
            OnDisposeUpdate();
            base.OnDestroy();
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

                playerHandler?.Setup(bulletScale, attackRange, attackInterval, attackPower);

                if (!await bulletHandler.SetupAsync(token, _seAssetName, WorldScale, _seNameShot))
                    throw new Exception("failed setup bulletHandler.");

                if (!await enemyHandler.SetupAsync(token, _enemyAssetName, _seAssetName, WorldScale))
                    throw new Exception("failed setup enemyHandler.");

                if (virtualCourt != null)
                {
                    var courtTextures = new Texture2D[5];

                    courtTextures[0] = await OnLoadTextureAssetBundleAsync(token, _fieldAssetName, "virtualcourt_tex_1");
                    courtTextures[1] = await OnLoadTextureAssetBundleAsync(token, _fieldAssetName, "virtualcourt_tex_2");
                    courtTextures[2] = await OnLoadTextureAssetBundleAsync(token, _fieldAssetName, "virtualcourt_tex_3");
                    courtTextures[3] = await OnLoadTextureAssetBundleAsync(token, _fieldAssetName, "virtualcourt_tex_4");
                    courtTextures[4] = await OnLoadTextureAssetBundleAsync(token, _fieldAssetName, "virtualcourt_tex_5");

                    virtualCourt?.SetPlaneTextures(courtTextures);
                }

                await UniTask.WaitUntil(() => SystemHandler.Init, cancellationToken: token);
                token.ThrowIfCancellationRequested();

                Esc();

                OnDisposeUpdate();
                _onUpdateToken = new CancellationTokenSource();
                OnUpdateAsync(_onUpdateToken.Token).Forget();

                Init = true;
            }
            catch (Exception ex)
            {
                Log(DebugerLogType.Info, "OnSetupAsync", $"{ex.Message}");
            }
        }

        private async UniTask<Texture2D> OnLoadTextureAssetBundleAsync(CancellationToken token, string assetBundleName, string fileName)
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

                return loader.GetAsset<Texture2D>();
            }
            catch (Exception ex)
            {
                Log(DebugerLogType.Info, "OnLoadTextureAssetBundleAsync", $"{ex.Message}");
            }
            return null;
        }

        private void OnDisposeUpdate()
        {
            _onUpdateToken?.Dispose();
            _onUpdateToken = null;
        }

        private async UniTask OnUpdateAsync(CancellationToken token)
        {
            try
            {
                while (_onUpdateToken != null)
                {
                    playerHandler?.UpdatePlayer(Status);

                    await UniTask.Yield(PlayerLoopTiming.Update, token);
                    token.ThrowIfCancellationRequested();
                }
            }
            catch (Exception ex)
            {
                Log(DebugerLogType.Info, "OnUpdateAsync", $"{ex.Message}");
            }
        }

        public void SetWorldScale(float scale)
        {
            WorldScale = scale;
        }

        private void ResetTimer()
        {
            GameCount = 0f;
            FillAmountGameTime = 0f;

            _resultCount = 0f;
            FillAmountResultTime = 0f;
        }

        public void SetGameResultTime(float gameTime, float resultTime)
        {
            GameTime = gameTime;
            ResultTime = resultTime;
        }

        protected void ChangedStatus(GameStatus status)
        {
            if (Status == status)
                return;

            Status = status;

            if (Status == GameStatus.Entry)
                OnEntrySubject?.OnNext(Unit.Default);

            if (Status == GameStatus.Start)
                OnStartSubject?.OnNext(Unit.Default);

            if (Status == GameStatus.Playing)
                OnPlaySubject?.OnNext(Unit.Default);

            if (Status == GameStatus.End)
                OnEndSubject?.OnNext(Unit.Default);

            if (Status == GameStatus.Result)
                ReportGameResultSubject?.OnNext(GameScore);

            if (Status == GameStatus.None)
                OnEscSubject?.OnNext(Unit.Default);
        }

        public void SetGameStatus(GameStatus status)
        {
            switch (status)
            {
                case GameStatus.Entry:
                    {
                        OnEntry();
                    }
                    break;

                case GameStatus.Start:
                    {
                        OnStart();
                    }
                    break;

                case GameStatus.Playing:
                    {
                        OnPlay();
                    }
                    break;

                case GameStatus.End:
                    {
                        OnEnd();
                    }
                    break;

                case GameStatus.Result:
                    {
                        OnResult();
                    }
                    break;

                case GameStatus.None:
                default:
                    {
                        Esc();
                    }
                    break;
            }
        }

        private void OnResetGame()
        {
            playerHandler?.ResetPlayer();
            bulletHandler?.Clear();
            enemyHandler?.Clear();

            SoundHandler.StopSE();

            ResetTimer();
            ResetScore();
        }

        public void Esc()
        {
            ChangedStatus(GameStatus.None);
            OnResetGame();
        }

        private void OnEntry()
        {
            ChangedStatus(GameStatus.Entry);
            OnResetGame();
        }

        private void OnStart()
        {
            ChangedStatus(GameStatus.Start);
            OnResetGame();

            enemyHandler?.Play();

            FillAmountGameTime = 1f;
        }

        private void OnPlay()
        {
            FillAmountGameTime = 1f;
            ChangedStatus(GameStatus.Playing);
        }

        private void OnEnd()
        {
            ChangedStatus(GameStatus.End);

            bulletHandler?.Clear();
            enemyHandler?.Clear();

            ResetTimer();
            ScoreTally();
        }

        private void OnResult()
        {
            ChangedStatus(GameStatus.Result);
        }

        private float GetGameCount(float time)
        {
            if (Status == GameStatus.Playing)
            {
                return time > 0 ? GameTime - time : GameTime;
            }
            else if (Status == GameStatus.Start)
            {
                return GameTime;
            }
            return 0f;
        }

        private float GetResultCount(float time)
        {
            if (Status == GameStatus.Result)
            {
                return time > 0 ? ResultTime - time : ResultTime;
            }
            else if (Status == GameStatus.Start ||
                     Status == GameStatus.Playing ||
                     Status == GameStatus.End)
            {
                return ResultTime;
            }
            return 0f;
        }

        public void UpdateStatusTimer(float time)
        {
            GameCount = GetGameCount(time);
            FillAmountGameTime = GameTime > 0 ? GameCount / GameTime : 0f;

            _resultCount = GetResultCount(time);
            FillAmountResultTime = ResultTime > 0 ? _resultCount / ResultTime : 0f;
        }

        public void MissShot()
        {
            if (Status != GameStatus.Playing)
                return;

            if (GameScore.MaxCombo < ComboCount)
                GameScore.MaxCombo = ComboCount;

            ComboCount = 0;
        }

        public void AddScore(int enemyNo, int point, bool kill)
        {
            if (Status != GameStatus.Playing)
                return;

            if (kill)
                GameScore.BreakEnemy += 1;

            GameScore.Hit += 1;
            GameScore.Point += point;
            ComboCount += 1;
        }

        private void ResetScore()
        {
            ComboCount = 0;
            GameScore.Reset();
        }

        private void ScoreTally()
        {
            if (GameScore.MaxCombo < ComboCount)
                GameScore.MaxCombo = ComboCount;

            // ボーナス計算
            GameScore.ComboBonus = enemyHandler.GetPointComboBonus(GameScore.MaxCombo);
            GameScore.KillBonus = enemyHandler.GetPointKillBonus(GameScore.BreakEnemy);

            // ランク判定
            GameScore.Rank = enemyHandler.GetScoreRank(GameScore.TotalPoint);
        }
    }
}
