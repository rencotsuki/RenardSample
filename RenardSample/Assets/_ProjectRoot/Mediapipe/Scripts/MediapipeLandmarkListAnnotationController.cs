/*
 * Mediapipe - LandmarkListAnnotationControllerの改修
 */

using System;
using System.Collections.Generic;
using UnityEngine;
using Mediapipe;
using Mediapipe.Unity;
using Renard.Debuger;

namespace SignageHADO.Tracking
{
    public class MediapipeLandmarkListAnnotationController : AnnotationController<MediapipeLandmarkListAnnotation>
    {
        [SerializeField] private bool _visualizeZ = false;

        public bool IsStale => isStale;

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

        public bool ActiveFaceLandmark => annotation != null ? annotation.ActiveFaceLandmark : false;

        private IReadOnlyList<NormalizedLandmark> _currentFaceLandmarkList = null;
        public IReadOnlyList<NormalizedLandmark> CurrentFaceLandmarkList => _currentFaceLandmarkList;

        public bool ActiveLeftHandLandmark => annotation != null ? annotation.ActiveLeftHandLandmark : false;

        private IReadOnlyList<NormalizedLandmark> _currentLeftHandLandmarkList = null;
        public IReadOnlyList<NormalizedLandmark> CurrentLeftHandLandmarkList => _currentLeftHandLandmarkList;

        public bool ActiveRightHandLandmark => annotation != null ? annotation.ActiveRightHandLandmark : false;

        private IReadOnlyList<NormalizedLandmark> _currentRightHandLandmarkList = null;
        public IReadOnlyList<NormalizedLandmark> CurrentRightHandLandmarkList => _currentRightHandLandmarkList;

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

        public void Settings(bool mirror) => Settings(mirror, rotationAngle);
        public void Settings(RotationAngle angle) => Settings(isMirrored, angle);
        public void Settings(bool mirror, RotationAngle angle)
        {
            isMirrored = mirror;
            rotationAngle = angle;
        }

        public void UpdateNow(IReadOnlyList<NormalizedLandmark> faceLandmarkList,
                              IReadOnlyList<NormalizedLandmark> leftHandLandmarkList,
                              IReadOnlyList<NormalizedLandmark> rightHandLandmarkList)
        {
            _currentFaceLandmarkList = faceLandmarkList;
            _currentLeftHandLandmarkList = leftHandLandmarkList;
            _currentRightHandLandmarkList = rightHandLandmarkList;

            SyncNow();
        }

        public void UpdateNow(NormalizedLandmarkList faceLandmarkList,
                              NormalizedLandmarkList leftHandLandmarkList,
                              NormalizedLandmarkList rightHandLandmarkList)
        {
            UpdateNow(faceLandmarkList?.Landmark,
                      leftHandLandmarkList?.Landmark,
                      rightHandLandmarkList?.Landmark);
        }

        public void UpdateFaceLandmarkListLater(IReadOnlyList<NormalizedLandmark> faceLandmarkList)
        {
            UpdateCurrentTarget(faceLandmarkList, ref _currentFaceLandmarkList);
        }

        public void UpdateFaceLandmarkListLater(NormalizedLandmarkList faceLandmarkList)
        {
            UpdateFaceLandmarkListLater(faceLandmarkList?.Landmark);
        }

        public void UpdateLeftHandLandmarkListLater(IReadOnlyList<NormalizedLandmark> leftHandLandmarkList)
        {
            UpdateCurrentTarget(leftHandLandmarkList, ref _currentLeftHandLandmarkList);
        }

        public void UpdateLeftHandLandmarkListLater(NormalizedLandmarkList leftHandLandmarkList)
        {
            UpdateLeftHandLandmarkListLater(leftHandLandmarkList?.Landmark);
        }

        public void UpdateRightHandLandmarkListLater(IReadOnlyList<NormalizedLandmark> rightHandLandmarkList)
        {
            UpdateCurrentTarget(rightHandLandmarkList, ref _currentRightHandLandmarkList);
        }

        public void UpdateRightHandLandmarkListLater(NormalizedLandmarkList rightHandLandmarkList)
        {
            UpdateRightHandLandmarkListLater(rightHandLandmarkList?.Landmark);
        }

        protected override void SyncNow()
        {
            isStale = false;

            try
            {
                annotation.Draw(_currentFaceLandmarkList,
                                _currentLeftHandLandmarkList,
                                _currentRightHandLandmarkList,
                                _visualizeZ);
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
