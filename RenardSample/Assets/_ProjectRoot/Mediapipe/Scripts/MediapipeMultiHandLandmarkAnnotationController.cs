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
    using Hand = HandLandmarkListAnnotation.Hand;

#pragma warning disable IDE0065
    using Color = UnityEngine.Color;
#pragma warning restore IDE0065

    public class MediapipeMultiHandLandmarkAnnotationController : AnnotationController<MediapipeMultiHandLandmarkListAnnotation>
    {
        [SerializeField] private bool _visualizeZ = false;

        private HandLandmarkerResult _currentTarget = default;

        public bool IsStale => annotation != null ? annotation.isActive : false;

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
            }
            catch (Exception ex)
            {
                Log(DebugerLogType.Info, "SyncNow", $"{ex.Message}");
            }
        }

        private string GetCategoryName(Hand hand)
        {
            if (hand == Hand.Left) return "Left";
            if (hand == Hand.Right) return "Right";
            return string.Empty;
        }

        private bool GetHandWorldPos(Hand hand, int index, out Vector3 handWorldPos)
        {
            handWorldPos = Vector3.zero;

            if (annotation != null)
            {
                if (hand == Hand.Left)
                    return annotation.GetLeftHandLandmarkPos(index, out handWorldPos);

                if (hand == Hand.Right)
                    return annotation.GetRightHandLandmarkPos(index, out handWorldPos);
            }
            return false;
        }

        private bool GetHandLandmarkList(Hand hand, out Vector3 handWorldPos, out IReadOnlyList<NormalizedLandmark> list)
        {
            handWorldPos = Vector3.zero;
            list = null;

            try
            {
                for (_index = 0; _index < _currentTarget.handedness.Count; _index++)
                {
                    if (_currentTarget.handedness[_index].categories[0].categoryName == GetCategoryName(hand))
                    {
                        if (_currentTarget.handLandmarks.Count > _index)
                        {
                            if (GetHandWorldPos(hand, _index, out handWorldPos))
                            {
                                list = _currentTarget.handLandmarks[_index].landmarks;
                                return true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log(DebugerLogType.Info, "GetHandLandmarkList", $"[{hand}] - {ex.Message}");
            }
            return false;
        }

        public bool GetLeftHandLandmarkPos(out Vector3 handWorldPos, out IReadOnlyList<NormalizedLandmark> landmarkList)
        {
            // ミラー設定で逆になるのに注意！
            return isMirrored ?
                GetHandLandmarkList(Hand.Left, out handWorldPos, out landmarkList) :
                GetHandLandmarkList(Hand.Right, out handWorldPos, out landmarkList);
        }

        public bool GetRightHandLandmarkPos(out Vector3 handWorldPos, out IReadOnlyList<NormalizedLandmark> landmarkList)
        {
            // ミラー設定で逆になるのに注意！
            return isMirrored ?
                GetHandLandmarkList(Hand.Right, out handWorldPos, out landmarkList) :
                GetHandLandmarkList(Hand.Left, out handWorldPos, out landmarkList);
        }
    }
}
