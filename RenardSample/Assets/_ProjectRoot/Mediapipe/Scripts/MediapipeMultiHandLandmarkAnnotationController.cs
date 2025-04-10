/*
 * Mediapipe - HandLandmarkerResultAnnotationControllerの改修
 */

using System;
using System.Collections.Generic;
using UnityEngine;
using Mediapipe.Unity;
using Mediapipe.Tasks.Vision.HandLandmarker;

using NormalizedLandmark = Mediapipe.Tasks.Components.Containers.NormalizedLandmark;

using Renard.Debuger;

namespace SignageHADO.Tracking
{
#pragma warning disable IDE0065
    using Color = UnityEngine.Color;
#pragma warning restore IDE0065

    public class MediapipeMultiHandLandmarkAnnotationController : AnnotationController<MediapipeMultiHandLandmarkListAnnotation>
    {
        [SerializeField] private bool _visualizeZ = false;

        private HandLandmarkerResult _currentTarget = default;

        public bool IsStale => annotation != null ? annotation.isActive : false;

        private const int leftHandIndex = 0;
        public bool ActiveLeftHandLandmark => (_currentTarget.handLandmarks.Count > leftHandIndex && _currentTarget.handLandmarks[leftHandIndex].landmarks.Count > 0);
        public IReadOnlyList<NormalizedLandmark> CurrentLeftHandLandmarkList => _currentTarget.handLandmarks[leftHandIndex].landmarks;

        private const int rightHandIndex = 1;
        public bool ActiveRightHandLandmark => (_currentTarget.handLandmarks.Count > rightHandIndex && _currentTarget.handLandmarks[rightHandIndex].landmarks.Count > 0);
        public IReadOnlyList<NormalizedLandmark> CurrentRightHandLandmarkList => _currentTarget.handLandmarks[rightHandIndex].landmarks;

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

        public void DrawNow(HandLandmarkerResult target)
        {
            target.CloneTo(ref _currentTarget);
            SyncNow();
        }

        public void DrawLater(HandLandmarkerResult target) => UpdateCurrentTarget(target);

        protected void UpdateCurrentTarget(HandLandmarkerResult newTarget)
        {
            if (IsTargetChanged(newTarget, _currentTarget))
            {
                newTarget.CloneTo(ref _currentTarget);
                isStale = true;
            }
        }

        protected override void SyncNow()
        {
            try
            {
                isStale = false;
                annotation?.SetHandedness(_currentTarget.handedness);
                annotation?.Draw(_currentTarget.handLandmarks, _visualizeZ);
            }
            catch (Exception ex)
            {
                Log(DebugerLogType.Info, "SyncNow", $"{ex.Message}");
            }
        }

        public bool GetLeftHandLandmarkPos(out Vector3 handWorldPos)
        {
            handWorldPos = Vector3.zero;

            if (annotation != null)
                return annotation.GetLeftHandLandmarkPos(out handWorldPos);
            return false;
        }

        public bool GetRightHandLandmarkPos(out Vector3 handWorldPos)
        {
            handWorldPos = Vector3.zero;

            if (annotation != null)
                return annotation.GetRightHandLandmarkPos(out handWorldPos);
            return false;
        }
    }
}
