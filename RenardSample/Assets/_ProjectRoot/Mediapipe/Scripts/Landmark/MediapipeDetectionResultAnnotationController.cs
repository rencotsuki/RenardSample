/*
 * Mediapipe - DetectionResultAnnotationControllerの改修
 */

using System;
using UnityEngine;
using Mediapipe.Unity;
using Renard.Debuger;

using mptcc = Mediapipe.Tasks.Components.Containers;

namespace SignageHADO.Tracking
{
    public class MediapipeDetectionResultAnnotationController : AnnotationController<MediapipeDetectionListAnnotation>
    {
        [SerializeField, Range(0, 1)] private float _threshold = 0.0f;

        protected bool active => annotation != null ? annotation.Active : false;

        public int TargetCount => active && annotation != null ? annotation.TargetCount : 0;

        private mptcc.DetectionResult _currentTarget = default;

        public bool IsDebugLog = false;

        protected void Log(DebugerLogType logType, string methodName, string message)
        {
            if (!IsDebugLog)
            {
                if (logType == DebugerLogType.Info)
                    return;
            }

            DebugLogger.Log(this.GetType(), logType, methodName, message);
        }

        public void SetTargetCategory(string[] category)
            => annotation?.SetTargetCategory(category);

        public void DrawNow(mptcc.DetectionResult target)
        {
            _currentTarget = target;
            SyncNow();
        }

        public void DrawLater(mptcc.DetectionResult target)
            => UpdateCurrentTarget(target, ref _currentTarget);

        protected override void SyncNow()
        {
            try
            {
                isStale = false;
                annotation?.Draw(_currentTarget, imageSize, _threshold);
            }
            catch (Exception ex)
            {
                Log(DebugerLogType.Info, "SyncNow", $"{ex.Message}");
            }
        }
    }
}
