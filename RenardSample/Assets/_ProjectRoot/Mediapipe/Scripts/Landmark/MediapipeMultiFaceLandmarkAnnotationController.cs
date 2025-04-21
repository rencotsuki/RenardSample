/*
 * Mediapipe - FaceLandmarkerResultAnnotationControllerの改修
 */

using System;
using System.Collections.Generic;
using UnityEngine;
using Mediapipe.Unity;
using Mediapipe.Tasks.Vision.FaceLandmarker;

using NormalizedLandmark = Mediapipe.Tasks.Components.Containers.NormalizedLandmark;

using Renard.Debuger;

namespace SignageHADO.Tracking
{
#pragma warning disable IDE0065
    using Color = UnityEngine.Color;
#pragma warning restore IDE0065

    public class MediapipeMultiFaceLandmarkAnnotationController : AnnotationController<MediapipeMultiFaceLandmarkListAnnotation>
    {
        [SerializeField] private bool _visualizeZ = false;

        private FaceLandmarkerResult _currentTarget = default;

        public bool IsStale => annotation != null ? annotation.isActive : false;

        /// <summary>基準とする瞳間距離[cm]</summary>
        public float BasiseInterpupillary
        {
            get => annotation != null ? annotation.BasiseInterpupillary : 0f;
            set
            {
                if (annotation != null)
                    annotation.BasiseInterpupillary = value;
            }
        }

        public bool ActiveFaceLandmark => (_currentTarget.faceLandmarks[0].landmarks.Count > 0);

        public IReadOnlyList<NormalizedLandmark> CurrentFaceLandmarkList => _currentTarget.faceLandmarks[0].landmarks;

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

        public void DrawNow(FaceLandmarkerResult target)
        {
            target.CloneTo(ref _currentTarget);
            SyncNow();
        }

        public void DrawLater(FaceLandmarkerResult target) => UpdateCurrentTarget(target);

        protected void UpdateCurrentTarget(FaceLandmarkerResult newTarget)
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
                annotation?.Draw(_currentTarget.faceLandmarks, _visualizeZ);
            }
            catch (Exception ex)
            {
                Log(DebugerLogType.Info, "SyncNow", $"{ex.Message}");
            }
        }

        public bool GetFaceLandmarkPos(out Vector3 headWorldPos, out float headScale, out Vector3 noiseTipWorldPos, out Vector3 leftFaceEdgeWorldPos, out Vector3 rightFaceEdgeWorldPos)
        {
            headWorldPos = Vector3.zero;
            headScale = 0f;
            noiseTipWorldPos = Vector3.zero;
            leftFaceEdgeWorldPos = Vector3.zero;
            rightFaceEdgeWorldPos = Vector3.zero;

            if (annotation != null)
                return annotation.GetFaceLandmarkPos(out headWorldPos, out headScale, out noiseTipWorldPos, out leftFaceEdgeWorldPos, out rightFaceEdgeWorldPos);
            return false;
        }
    }
}
