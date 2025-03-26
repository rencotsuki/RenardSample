using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UniRx;

namespace SignageHADO.Game
{
    [Serializable]
    public class BulletScript : MonoBehaviourCustom
    {
        [SerializeField] protected BulletModel bulletModel = default;
        [SerializeField] protected BulletShadow bulletShadow = default;
        [SerializeField] protected BulletShotMover bulletMover = null;
        [SerializeField] protected ColliderSupport[] colliderSupports = new ColliderSupport[0];
        [SerializeField] protected float speed = 10.5f;
        [SerializeField] protected float duration = 10f;
        [SerializeField] protected float scalingSpeed = 7f;  //TODO: スピードにより形状が変化させる値
        [SerializeField] protected int perforate = 0;

        public int SerialNo = 0;

        public int Attack { get; protected set; } = 0;

        public float Speed => speed;
        public float Scale => GameHandler.BulletScale;
        public Vector3 BeginPosition { get; protected set; } = Vector3.zero;
        public Vector3 EndPosition { get; protected set; } = Vector3.zero;

        public bool IsReturn { get; protected set; } = false;
        public bool IsHit { get; protected set; } = false;
        public bool IsBreak { get; protected set; } = false;

        protected Vector3 anchorPos => GameHandler.WorldAnchorPos;

        protected float breakWaitTime => bulletModel != null ? bulletModel.BreakWaitTime : 0.5f;

        private IDisposable _onBreakObservable = null;

        private void Start()
        {
            foreach (var collider in colliderSupports)
            {
                collider.Setup(this);
                collider.SetupOwner(true);
            }

            bulletMover.OnUpdateMoveObservable
                .Subscribe(_ =>
                {
                    bulletModel?.UpdateDraw(anchorPos);
                    bulletShadow?.UpdateDraw();
                })
                .AddTo(bulletMover);

            bulletMover.OnEndMoveSubjectObservable
                .Subscribe(_ =>
                {
                    Break(false);
                })
                .AddTo(bulletMover);
        }

        private void SetColliders(bool enabled, float scale = 1f)
        {
            foreach (var collider in colliderSupports)
            {
                collider.Enabled = enabled;

                if (enabled)
                    collider.transform.localScale = Vector3.one * scale;
            }
        }

        public void ResetObject()
        {
            _onBreakObservable?.Dispose();
            _onBreakObservable = null;

            IsBreak = false;

            bulletMover?.Stop();

            bulletModel?.Return();
            bulletShadow?.Return();

            SetColliders(false);
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
            GameHandler.ReturnBullet(this);
        }

        public void Shot(int attack, Vector3 beginPosition, Vector3 direct)
        {
            if (speed <= 0 || duration <= 0)
            {
                OnReturn();
                return;
            }

            ResetObject();
            IsReturn = false;

            IsBreak = false;
            Attack = attack;
            BeginPosition = beginPosition;

            EndPosition = BeginPosition + direct * (speed * duration);

            transform.position = BeginPosition;
            transform.localRotation = Quaternion.LookRotation((EndPosition - BeginPosition).normalized, Vector3.up);

            SetColliders(true, Scale);

            if (bulletMover != null)
            {
                bulletMover.Setup(speed, duration, BeginPosition, EndPosition)
                           .Firing();
            }

            if (bulletModel != null)
            {
                bulletModel.SetDrawParam(Scale, speed > 0f ? (scalingSpeed > 0 ? speed / scalingSpeed : speed) : 1f);
                bulletModel.Appear();
            }

            if (bulletShadow != null)
            {
                bulletShadow.SetDrawParam(Scale, speed > 0f ? (scalingSpeed > 0 ? speed / scalingSpeed : speed) : 1f);
                bulletShadow.Appear();
            }
        }

        public void Break(bool isHit)
        {
            if (IsBreak) return;

            IsBreak = true;

            bulletShadow?.Return();
            bulletMover?.Impact();

            if (isHit)
            {
                bulletModel?.HitBreak();
            }
            else
            {
                bulletModel?.Disappear();
                GameHandler.MissShot();
            }

            _onBreakObservable?.Dispose();
            _onBreakObservable = null;

            if (breakWaitTime <= 0)
            {
                OnReturn();
            }
            else
            {
                _onBreakObservable = Observable.Timer(TimeSpan.FromSeconds(breakWaitTime))
                    .Subscribe(_ => OnReturn())
                    .AddTo(this);
            }
        }
    }
}
