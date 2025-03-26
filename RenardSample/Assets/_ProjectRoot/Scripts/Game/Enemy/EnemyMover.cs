using System;
using UnityEngine;
using UniRx;

namespace SignageHADO.Game
{
    [Serializable]
    public class EnemyMover : EnemyTween
    {
        protected BezierCurvePointer _pointer = null;

        private const float minMoveDistance = 0.01f;

        protected float worldScale => GameHandler.WorldScale;

        public Vector3 StartPoint => _pointer != null ? _pointer.StartPoint : Vector3.zero;

        public Vector3 EndPoint => _pointer != null ? _pointer.EndPoint : Vector3.zero;

        public bool IsMoving => isActive;

        private Subject<Unit> onUpdateMoveSubject = new Subject<Unit>();
        public IObservable<Unit> OnUpdateMoveObservable => onUpdateMoveSubject?.AsObservable();

        private Subject<Unit> onEndMoveSubject = new Subject<Unit>();
        public IObservable<Unit> OnEndMoveSubjectObservable => onEndMoveSubject?.AsObservable();

        /// <summary>設定</summary>
        public EnemyMover Setup(float duration, Vector3 launchPosition, Vector3 endPosition)
        {
            transform.localPosition = launchPosition;

            _pointer = null;

            if (duration <= 0)
                return this;

            if (Vector3.Distance(launchPosition, endPosition) <= minMoveDistance)
                return this;

            transform.localRotation = Quaternion.LookRotation((endPosition - launchPosition).normalized, Vector3.up);

            this.duration = duration / worldScale;

            _pointer = new BezierCurvePointer(duration, launchPosition, endPosition);

            return this;
        }

        /// <summary>発進</summary>
        public void Run(float lagTime = 0f)
        {
            if (duration <= lagTime)
            {
                onEndMoveSubject?.OnNext(Unit.Default);
                return;
            }

            Play(lagTime);
        }

        /// <summary>破棄</summary>
        public void Break()
        {
            Stop();
        }

        protected override void OnUpdate()
        {
            if (_pointer != null)
            {
                transform.localPosition = _pointer.GetPoint(elapsedEasing);
                transform.forward = _pointer.GetForward(elapsedEasing);
            }

            onUpdateMoveSubject?.OnNext(Unit.Default);
        }

        protected override void OnEnd()
        {
            onEndMoveSubject?.OnNext(Unit.Default);
        }
    }

#if UNITY_EDITOR

    [UnityEditor.CustomEditor(typeof(EnemyMover))]
    [UnityEditor.CanEditMultipleObjects]
    public class EnemyMoverEditor : UnityEditor.Editor { }

#endif
}
