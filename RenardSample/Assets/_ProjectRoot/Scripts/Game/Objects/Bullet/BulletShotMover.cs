using System;
using UnityEngine;
using UniRx;

namespace SignageHADO.Game
{
    [Serializable]
    public class BulletShotMover : BulletTween
    {
        protected BezierCurvePointer _pointer = null;

        protected float worldScale => GameHandler.WorldScale;

        public Vector3 StartPoint => _pointer != null ? _pointer.StartPoint : Vector3.zero;

        public Vector3 EndPoint => _pointer != null ? _pointer.EndPoint : Vector3.zero;

        public bool IsMoving => isActive;

        private Subject<Unit> onUpdateMoveSubject = new Subject<Unit>();
        public IObservable<Unit> OnUpdateMoveObservable => onUpdateMoveSubject?.AsObservable();

        private Subject<Unit> onEndMoveSubject = new Subject<Unit>();
        public IObservable<Unit> OnEndMoveSubjectObservable => onEndMoveSubject?.AsObservable();

        /// <summary>設定</summary>
        public BulletShotMover Setup(float speed, float duration, Vector3 launchPosition, Vector3 endPosition)
        {
            transform.position = launchPosition;
            transform.localRotation = Quaternion.LookRotation((endPosition - launchPosition).normalized, Vector3.up);

            if (speed <= 0 || duration <= 0) return this;

            this.duration = duration / worldScale;
            range = (duration * speed) / worldScale;

            _pointer = new BezierCurvePointer(duration, launchPosition, endPosition);

            return this;
        }

        /// <summary>発射</summary>
        public void Firing(float lagTime = 0f)
        {
            if (_pointer == null)
            {
                onEndMoveSubject?.OnNext(Unit.Default);
                return;
            }

            Play(lagTime);
        }

        /// <summary>着弾</summary>
        public void Impact()
        {
            Stop();
        }

        protected override void OnUpdate()
        {
            transform.position = _pointer.GetPoint(elapsedEasing);
            transform.forward = _pointer.GetForward(elapsedEasing);

            onUpdateMoveSubject?.OnNext(Unit.Default);
        }

        protected override void OnEnd()
        {
            onEndMoveSubject?.OnNext(Unit.Default);
        }
    }

#if UNITY_EDITOR

    [UnityEditor.CustomEditor(typeof(BulletShotMover))]
    [UnityEditor.CanEditMultipleObjects]
    public class BulletShotMoverEditor : UnityEditor.Editor { }

#endif
}
