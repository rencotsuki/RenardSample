using System;
using UnityEngine;
using UniRx;
using DG.Tweening;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SignageHADO.Game
{
    [Serializable]
    public class EnemySEClip : EnemySE
    {
        public AudioClip SpawnSEClip = null;
        public AudioClip HitSEClip = null;
        public AudioClip BreakSEClip = null;

        public static EnemySEClip Create(EnemyData data)
        {
            return new EnemySEClip()
            {
                SpawnSE = data != null && data.EnemySE != null ? data.EnemySE.SpawnSE : string.Empty,
                HitSE = data != null && data.EnemySE != null ? data.EnemySE.HitSE : string.Empty,
                BreakSE = data != null && data.EnemySE != null ? data.EnemySE.BreakSE : string.Empty
            };
        }
    }

    [Serializable]
    public class EnemyObject : MonoBehaviourCustom
    {
        [SerializeField] private EnemyMover enemyMover = null;
        [SerializeField] private Renderer enemyBody = null;
        [SerializeField] private MeshRenderer enemyShadow = null;
        [SerializeField] private Transform lookAt = null;
        [SerializeField] private Transform gameUIRoot = null;
        [SerializeField] private Animator animator = null;
        [SerializeField] private Renderer[] modelRenderers = null;
        [SerializeField] private ColliderSupport[] colliderSupports = null;
        [SerializeField] private ParticleSystem[] dissolveEffects = null;
        [SerializeField] private float breakReturnWait = 0.5f;

        private int animatorStatusIdle => Animator.StringToHash("Idle");
        private int triggerSpawn => Animator.StringToHash("Spawn");
        private int triggerHit => Animator.StringToHash("Hit");
        private int triggerDeath => Animator.StringToHash("Death");

        protected const int loadWaitTimeMilliseconds = 3 * 1000;

        public EnemyInfo EnemyInfo { get; protected set; } = null;
        public int EnemyNo => EnemyInfo != null ? EnemyInfo.No : 0;
        public int Point => EnemyInfo != null ? EnemyInfo.Point : 0;

        protected EnemySEClip enemySE { get; private set; } = null;
        protected AudioClip spawnSEClip => enemySE != null ? enemySE.SpawnSEClip : null;
        protected AudioClip hitSEClip => enemySE != null ? enemySE.HitSEClip : null;
        protected AudioClip breakSEClip => enemySE != null ? enemySE.BreakSEClip : null;

        protected Camera uiCanvasCamera => GameHandler.ViewCamera;

        public bool IsReturn { get; protected set; } = false;

        public bool IsBreak { get; protected set; } = false;

        protected Material materialBody = null;
        protected Material materialDissolve = null;
        protected Action<int, int, bool> hitAction = null;
        protected RigidbodySupport rigidbodySupport = null;
        protected EmenyAction enemyAction = null;
        protected EnemyGameUI enemyGameUI = null;

        private float _dissolveAmount = 0f;
        protected float dissolveAmount
        {
            get => _dissolveAmount;
            set
            {
                _dissolveAmount = value;
                materialDissolve?.SetFloat("_DissolveAmount", _dissolveAmount);
            }
        }

        private BulletScript _tmpBullet = null;
        private Tweener _onDissolveTween = null;
        private IDisposable _onRunWait = null;
        private IDisposable _onReturnWait = null;

        private void Awake()
        {
            if (enemyBody != null)
            {
                if (enemyBody.materials.Length == 2)
                {
                    materialBody = new Material(enemyBody.materials[0]);
                    materialBody.CopyPropertiesFromMaterial(enemyBody.materials[0]);

                    materialDissolve = new Material(enemyBody.materials[1]);
                    materialDissolve.CopyPropertiesFromMaterial(enemyBody.materials[1]);

                    enemyBody.materials = new Material[] { materialBody, materialDissolve };
                }
            }

            foreach (var collider in colliderSupports)
            {
                collider.Setup(this);
                collider.SetupOwner(true);
                collider.Enabled = false;
            }

            rigidbodySupport = gameObject.AddComponent<RigidbodySupport>();
            if (rigidbodySupport != null)
            {
                rigidbodySupport.OnCollisionEnterObservable
                    .Subscribe(report => OnCollisionEnterObserver(report))
                    .AddTo(rigidbodySupport);

                rigidbodySupport.OnCollisionStayObservable
                    .Subscribe(report => OnCollisionStayObserver(report))
                    .AddTo(rigidbodySupport);

                rigidbodySupport.OnCollisionExitObservable
                    .Subscribe(report => OnCollisionExitObserver(report))
                    .AddTo(rigidbodySupport);
            }

            IsReturn = true;
            ResetObject();
        }

        public void Setup(EnemyInfo info, EnemySEClip seClip, EnemyGameUI prefab, Action<int, int, bool> onHitAction)
        {
            EnemyInfo = EnemyInfo.CreateCopy(info);
            enemySE = seClip;
            hitAction = onHitAction;

            if (prefab != null)
            {
                enemyGameUI = Instantiate(prefab, (gameUIRoot != null ? gameUIRoot : transform));
                enemyGameUI.gameObject.SetActive(true);
                enemyGameUI.name = prefab.name;
                enemyGameUI.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

                enemyGameUI.Setup(uiCanvasCamera, Point);
            }

            ResetObject();
        }

        private void SetColliders(bool enabled, float scale = 1f)
        {
            foreach (var collider in colliderSupports)
            {
                collider.Enabled = enabled;

                if (enabled)
                    collider.transform.localScale = Vector3.one * scale;
            }

            if (rigidbodySupport != null)
                rigidbodySupport.Enabled = true;
        }

        private void SetVisible(bool visible)
        {
            foreach (var renderer in modelRenderers)
            {
                try
                {
                    renderer.enabled = visible;
                }
                catch (Exception ex)
                {
                    Log(DebugerLogType.Info, "SetVisible", $"{ex.Message}");
                }
            }
        }

        public void Return()
        {
            if (IsBreak)
                return;

            OnReturn();
        }

        protected void OnReturn()
        {
            if (IsReturn)
                return;

            IsReturn = true;
            GameHandler.ReturnEnemy(this);
        }

        public void Spawn(EmenyAction action, float duration, float runWait = 0f)
        {
            if (action == null || duration <= 0)
            {
                OnReturn();
                return;
            }

            enemyAction = action;

            ResetObject();

            IsReturn = false;

            transform.localPosition = enemyAction.BeginPos;
            transform.LookAt(enemyAction.EndPos);

            SetColliders(true);
            SetVisible(true);
            SoundHandler.PlaySE(spawnSEClip);

            if (animator != null)
            {
                animator.enabled = true;
                animator.Play(animatorStatusIdle);
                animator.SetTrigger(triggerSpawn);
            }

            if (enemyShadow != null)
                enemyShadow.enabled = true;

            _onRunWait?.Dispose();
            _onRunWait = null;

            enemyMover.Setup(duration, enemyAction.BeginPos, enemyAction.EndPos);

            if (runWait <= 0)
            {
                enemyMover.Run();
            }
            else
            {
                _onRunWait = Observable.Timer(TimeSpan.FromSeconds(runWait))
                                .Subscribe(_ =>
                                {
                                    enemyMover.Run();
                                })
                                .AddTo(this);
            }
        }

        public void ResetObject()
        {
            _onRunWait?.Dispose();
            _onRunWait = null;

            _onReturnWait?.Dispose();
            _onReturnWait = null;

            _onDissolveTween?.Kill();
            _onDissolveTween = null;

            IsBreak = false;

            if (animator != null)
            {
                animator.ResetTrigger(triggerSpawn);
                animator.ResetTrigger(triggerDeath);
                animator.ResetTrigger(triggerHit);
                animator.enabled = false;
            }

            enemyMover?.Stop();

            dissolveAmount = 1f;

            StopDissolveEffects();

            SetColliders(false);
            SetVisible(false);
        }

        public void UpdateStatus(GameStatus gameStatus, float deltaTime)
        {
            try
            {
                if (gameStatus == GameStatus.Start || gameStatus == GameStatus.Playing)
                {
                    if (lookAt != null)
                        lookAt.LookAt(uiCanvasCamera.transform.position);

                    if (enemyShadow != null)
                        enemyShadow.transform.localPosition = new Vector3(0f, -transform.localPosition.y + 0.001f, 0f);
                }
            }
            catch (Exception ex)
            {
                Log(DebugerLogType.Info, "UpdateStatus", $"{ex.Message}");
            }
        }

        private void OnCollisionEnterObserver(ColliderReport report)
        {
            CheckHit(report);
        }

        private void OnCollisionStayObserver(ColliderReport report)
        {
            CheckHit(report);
        }

        private void OnCollisionExitObserver(ColliderReport report)
        {
            //CheckHit(report);
        }

        private BulletScript GetBulletColliderReport(ColliderReport report)
        {
            if (report.HitCollider != null && report.HitCollider.AnchorComponent != null)
                return report.HitCollider.AnchorComponent.GetComponent<BulletScript>();
            return null;
        }

        private void CheckHit(ColliderReport report)
        {
            if (report.HitCollider == null || IsBreak)
                return;

            _tmpBullet = GetBulletColliderReport(report);

            if (_tmpBullet != null && !_tmpBullet.IsBreak)
            {
                _tmpBullet?.Break(true);

                // TODO: ライフとかないから必ずキルにする
                if (hitAction != null)
                    hitAction(EnemyNo, Point, true);

                SoundHandler.PlaySE(hitSEClip);

                animator?.SetTrigger(triggerHit);

                Break(true);
            }
        }

        private void PlayDissolveEffects()
        {
            if (dissolveEffects == null || dissolveEffects.Length <= 0)
                return;

            foreach (var item in dissolveEffects)
            {
                item?.Play();
            }
        }

        private void StopDissolveEffects()
        {
            if (dissolveEffects == null || dissolveEffects.Length <= 0)
                return;

            foreach (var item in dissolveEffects)
            {
                item?.Stop();
            }
        }

        public void Break(bool effectPlay = false)
        {
            if (IsBreak)
                return;

            IsBreak = true;
            SetColliders(false);

            enemyMover.Break();

            if (effectPlay)
            {
                SoundHandler.PlaySE(breakSEClip);
                enemyGameUI?.DrawPoint(breakReturnWait);
                animator?.SetTrigger(triggerDeath);
            }

            if (enemyShadow != null)
                enemyShadow.enabled = false;

            _onReturnWait?.Dispose();
            _onReturnWait = null;

            _onDissolveTween?.Kill();
            _onDissolveTween = null;

            if (effectPlay && breakReturnWait > 0)
            {
                PlayDissolveEffects();

                dissolveAmount = 1f;
                _onDissolveTween = DOTween.To(() => dissolveAmount, n => dissolveAmount = n, 0f, breakReturnWait).SetEase(Ease.InOutSine);
                _onDissolveTween?.Play();

                _onReturnWait = Observable.Timer(TimeSpan.FromSeconds(breakReturnWait))
                                    .Subscribe(_ =>
                                    {
                                        OnReturn();
                                    })
                                    .AddTo(this);
            }
            else
            {
                OnReturn();
            }
        }
    }

#if UNITY_EDITOR

    [CustomEditor(typeof(EnemyObject))]
    [CanEditMultipleObjects]
    public class EnemyObjectEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var enemy = target as EnemyObject;

            serializedObject.Update();

            EditorGUILayout.LabelField("敵情報");

            EditorGUI.indentLevel = 2;

            if (enemy != null && enemy.EnemyInfo != null && enemy.EnemyInfo.No > 0)
            {
                EditorGUILayout.LabelField($"No. {enemy.EnemyInfo.No}");
                EditorGUILayout.Space(2);
                EditorGUILayout.LabelField($"【名前】:　{enemy.EnemyInfo.Name}");
                EditorGUILayout.LabelField($"【得点】:　{enemy.EnemyInfo.Point}");
            }
            else
            {
                EditorGUILayout.LabelField("【 No Data 】");
            }

            EditorGUI.indentLevel = 0;

            EditorGUILayout.Space(10);

            serializedObject.ApplyModifiedProperties();

            base.OnInspectorGUI();
        }
    }

#endif
}
