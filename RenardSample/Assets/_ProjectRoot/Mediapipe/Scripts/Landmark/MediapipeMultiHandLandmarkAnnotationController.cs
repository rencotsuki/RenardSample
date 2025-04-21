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
    using Hand = MediapipeLandmark.Hand;

#pragma warning disable IDE0065
    using Color = UnityEngine.Color;
#pragma warning restore IDE0065

    public class MediapipeMultiHandLandmarkAnnotationController : AnnotationController<MediapipeMultiHandLandmarkListAnnotation>
    {
        [SerializeField] private bool _visualizeZ = false;
        [SerializeField] private Hand _handPosLandmarkIndex = Hand.Wrist;

        private HandLandmarkerResult _currentTarget = default;

        public bool IsStale => annotation != null ? annotation.isActive : false;

        [Serializable]
        private struct HandTracking
        {
            public string CategoryName;
            public int Index;
            public Vector3 HandWorldPos;
            public IReadOnlyList<NormalizedLandmark> List;
        }

        private HandTracking leftHand = new HandTracking();
        private HandTracking rightHand = new HandTracking();

        [HideInInspector] private int _index = 0;

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

                // カメラ画像を判定するので実手と向きが反転することに注意！

                leftHand.CategoryName = isMirrored ? "Left" : "Right";
                rightHand.CategoryName = isMirrored ? "Right" : "Left";
                leftHand.Index = -1;
                rightHand.Index = -1;

                for (_index = 0; _index < _currentTarget.handedness.Count; _index++)
                {
                    if (_currentTarget.handedness[_index].categories[0].categoryName == leftHand.CategoryName)
                        leftHand.Index = _index;

                    if (_currentTarget.handedness[_index].categories[0].categoryName == rightHand.CategoryName)
                        rightHand.Index = _index;
                }
            }
            catch (Exception ex)
            {
                Log(DebugerLogType.Info, "SyncNow", $"{ex.Message}");
            }
        }

        private bool GetHandLandmarkList(ref HandTracking hand, out Vector3 handWorldPos, out IReadOnlyList<NormalizedLandmark> list)
        {
            handWorldPos = hand.HandWorldPos;
            list = hand.List;

            try
            {
                if (hand.Index >= 0)
                {
                    if (annotation.GetHandLandmarkPos(hand.Index, (int)_handPosLandmarkIndex, out handWorldPos))
                        hand.HandWorldPos = handWorldPos;

                    if (_currentTarget.handLandmarks.Count > hand.Index)
                    {
                        list = _currentTarget.handLandmarks[hand.Index].landmarks;
                        hand.List = list;
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                Log(DebugerLogType.Info, "GetHandLandmarkList", $"[{hand.Index}] - {ex.Message}");
                handWorldPos = hand.HandWorldPos;
                list = hand.List;
            }
            return false;
        }

        public bool GetLeftHandLandmarkPos(bool reverse, out Vector3 handWorldPos, out IReadOnlyList<NormalizedLandmark> landmarkList)
        {
            return reverse ?
                GetHandLandmarkList(ref rightHand, out handWorldPos, out landmarkList) :
                GetHandLandmarkList(ref leftHand, out handWorldPos, out landmarkList);
        }

        public bool GetRightHandLandmarkPos(bool reverse, out Vector3 handWorldPos, out IReadOnlyList<NormalizedLandmark> landmarkList)
        {
            return reverse ?
                 GetHandLandmarkList(ref leftHand, out handWorldPos, out landmarkList) :
                 GetHandLandmarkList(ref rightHand, out handWorldPos, out landmarkList);
        }
    }
}
