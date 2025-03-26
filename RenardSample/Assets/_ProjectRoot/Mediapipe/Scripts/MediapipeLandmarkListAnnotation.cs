/*
 * Mediapipe - LandmarkListAnnotationの改修
 */

using System;
using System.Collections.Generic;
using UnityEngine;
using Mediapipe;
using Mediapipe.Unity;

namespace SignageHADO.Tracking
{
#pragma warning disable IDE0065
    using Color = UnityEngine.Color;
#pragma warning restore IDE0065

    using Hand = MediapipeLandmark.Hand;

    public sealed class MediapipeLandmarkListAnnotation : HierarchicalAnnotation
    {
        [SerializeField] private MediapipeFaceLandmarkListAnnotation _faceLandmarkListAnnotation = null;
        [SerializeField] private int _headPosLandmarkIndex = MediapipeLandmark.FaceJaw;
        [SerializeField] private HandLandmarkListAnnotation _leftHandLandmarkListAnnotation = null;
        [SerializeField] private HandLandmarkListAnnotation _rightHandLandmarkListAnnotation = null;
        [SerializeField] private Hand _handPosLandmarkIndex = Hand.Wrist;

        private Camera targetCamera => Camera.main;

        public override bool isMirrored
        {
            set
            {
                if (_faceLandmarkListAnnotation != null)
                    _faceLandmarkListAnnotation.isMirrored = value;

                if (_leftHandLandmarkListAnnotation != null)
                    _leftHandLandmarkListAnnotation.isMirrored = value;

                if (_rightHandLandmarkListAnnotation != null)
                    _rightHandLandmarkListAnnotation.isMirrored = value;

                base.isMirrored = value;
            }
        }

        public override RotationAngle rotationAngle
        {
            set
            {
                if (_faceLandmarkListAnnotation != null)
                    _faceLandmarkListAnnotation.rotationAngle = value;

                if (_leftHandLandmarkListAnnotation != null)
                    _leftHandLandmarkListAnnotation.rotationAngle = value;

                if (_rightHandLandmarkListAnnotation != null)
                    _rightHandLandmarkListAnnotation.rotationAngle = value;

                base.rotationAngle = value;
            }
        }

        /// <summary>基準とする瞳間距離[cm]</summary>
        public float BasiseInterpupillary = 6.3f;

        public bool ActiveFaceLandmark => _faceLandmarkListAnnotation != null ? _faceLandmarkListAnnotation.gameObject.activeSelf : false;
        public bool ActiveLeftHandLandmark => _leftHandLandmarkListAnnotation != null ? _leftHandLandmarkListAnnotation.gameObject.activeSelf : false;
        public bool ActiveRightHandLandmark => _rightHandLandmarkListAnnotation != null ? _rightHandLandmarkListAnnotation.gameObject.activeSelf : false;

        [Serializable]
        private struct FaceTracking
        {
            public Vector3 HeadWorldPos;
            public Vector3 NoseTipWorldPos;
            public Vector3 LeftFaceEdgeWorldPos;
            public Vector3 RightFaceEdgeWorldPos;
            public float HeadScale;
        }

        [Serializable]
        private struct HandTracking
        {
            public Vector3 HandWorldPos;
            public Vector3 IndexMpcWorldPos;
            public Vector3 PrinkyMpcWorldPos;
        }

        private FaceTracking face = new FaceTracking();
        private HandTracking leftHand = new HandTracking();
        private HandTracking rightHand = new HandTracking();

        [HideInInspector] private float _tmpPixelDistance = 0f;

        private void Start()
        {
            _leftHandLandmarkListAnnotation?.SetHandedness(HandLandmarkListAnnotation.Hand.Left);
            _rightHandLandmarkListAnnotation?.SetHandedness(HandLandmarkListAnnotation.Hand.Right);
        }

        public void SetFaceLandmarkColor(Color color)
        {
            _faceLandmarkListAnnotation?.SetLandmarkColor(color);
        }

        public void SetFaceLandmarkRadius(float radius)
        {
            _faceLandmarkListAnnotation?.SetLandmarkRadius(radius);
        }

        public void SetHandLandmarkRadius(float radius)
        {
            _leftHandLandmarkListAnnotation?.SetLandmarkRadius(radius);
            _rightHandLandmarkListAnnotation?.SetLandmarkRadius(radius);
        }

        public void SetFaceConnectionColor(Color color)
        {
            _faceLandmarkListAnnotation?.SetConnectionColor(color);
        }

        public void SetFaceConnectionWidth(float width)
        {
            _faceLandmarkListAnnotation?.SetConnectionWidth(width);
        }

        public void SetHandConnectionColor(Color color)
        {
            _leftHandLandmarkListAnnotation?.SetConnectionColor(color);
            _rightHandLandmarkListAnnotation?.SetConnectionColor(color);
        }

        public void SetHandConnectionWidth(float width)
        {
            _leftHandLandmarkListAnnotation?.SetConnectionWidth(width);
            _rightHandLandmarkListAnnotation?.SetConnectionWidth(width);
        }

        public void Draw(IReadOnlyList<NormalizedLandmark> faceLandmarks,
                         IReadOnlyList<NormalizedLandmark> leftHandLandmarks,
                         IReadOnlyList<NormalizedLandmark> rightHandLandmarks,
                         bool visualizeZ = false)
        {
            var mask = PoseLandmarkListAnnotation.BodyParts.All;
            if (faceLandmarks != null)
            {
                mask ^= PoseLandmarkListAnnotation.BodyParts.Face;
            }
            if (leftHandLandmarks != null)
            {
                mask ^= PoseLandmarkListAnnotation.BodyParts.LeftHand;
            }
            if (rightHandLandmarks != null)
            {
                mask ^= PoseLandmarkListAnnotation.BodyParts.RightHand;
            }

            _faceLandmarkListAnnotation.Draw(faceLandmarks, visualizeZ);
            _leftHandLandmarkListAnnotation.Draw(leftHandLandmarks, visualizeZ);
            _rightHandLandmarkListAnnotation.Draw(rightHandLandmarks, visualizeZ);
        }

        public void Draw(NormalizedLandmarkList faceLandmarks,
                         NormalizedLandmarkList leftHandLandmarks,
                         NormalizedLandmarkList rightHandLandmarks,
                         bool visualizeZ = false)
        {
            Draw(faceLandmarks?.Landmark,
                 leftHandLandmarks?.Landmark,
                 rightHandLandmarks?.Landmark,
                 visualizeZ);
        }

        private float GetPixelDistance(Vector3 from, Vector3 to)
        {
            return targetCamera != null ?
                Vector2.Distance(targetCamera.WorldToScreenPoint(from), targetCamera.WorldToScreenPoint(to)) :
                0f;
        }

        public bool GetFaceLandmarkPos(out Vector3 headWorldPos, out float headScale, out Vector3 noseTipWorldPos, out Vector3 leftFaceEdgeWorldPos, out Vector3 rightFaceEdgeWorldPos)
            => GetFaceLandmarkPos(ref _faceLandmarkListAnnotation, ref face, out headWorldPos, out headScale, out noseTipWorldPos, out leftFaceEdgeWorldPos, out rightFaceEdgeWorldPos);

        private bool GetFaceLandmarkPos(ref MediapipeFaceLandmarkListAnnotation faseLandmarkList, ref FaceTracking face, out Vector3 headWorldPos, out float headScale, out Vector3 noseTipWorldPos, out Vector3 leftFaceEdgeWorldPos, out Vector3 rightFaceEdgeWorldPos)
        {
            headWorldPos = face.HeadWorldPos;
            headScale = face.HeadScale;
            noseTipWorldPos = face.NoseTipWorldPos;
            leftFaceEdgeWorldPos = face.LeftFaceEdgeWorldPos;
            rightFaceEdgeWorldPos = face.RightFaceEdgeWorldPos;

            try
            {
                face.HeadWorldPos = faseLandmarkList[_headPosLandmarkIndex].transform.position;
                headWorldPos = face.HeadWorldPos;

                face.NoseTipWorldPos = faseLandmarkList[MediapipeLandmark.FaceNoseTip].transform.position;
                noseTipWorldPos = face.NoseTipWorldPos;

                face.LeftFaceEdgeWorldPos = faseLandmarkList[MediapipeLandmark.FaceLeftEdgeIndex].transform.position;
                leftFaceEdgeWorldPos = face.LeftFaceEdgeWorldPos;

                face.RightFaceEdgeWorldPos = faseLandmarkList[MediapipeLandmark.FaceRightEdgeIndex].transform.position;
                rightFaceEdgeWorldPos = face.RightFaceEdgeWorldPos;

                // 左目(33)と右目(263)のピクセル距離
                _tmpPixelDistance = GetPixelDistance(faseLandmarkList[MediapipeLandmark.FaceLeftEye].transform.position,
                                                     faseLandmarkList[MediapipeLandmark.FaceRightEye].transform.position);

                // スケールを計算
                face.HeadScale = (BasiseInterpupillary <= 0 || _tmpPixelDistance <= 0) ? 1f : BasiseInterpupillary / _tmpPixelDistance;
                headScale = face.HeadScale;

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool GetLeftHandLandmarkPos(out Vector3 handWorldPos)
            => GetHandLandmarkPos(ref _leftHandLandmarkListAnnotation, ref leftHand, out handWorldPos);

        public bool GetRightHandLandmarkPos(out Vector3 handWorldPos)
            => GetHandLandmarkPos(ref _rightHandLandmarkListAnnotation, ref rightHand, out handWorldPos);

        private bool GetHandLandmarkPos(ref HandLandmarkListAnnotation handLandmarkList, ref HandTracking hand, out Vector3 handWorldPos)
        {
            handWorldPos = hand.HandWorldPos;

            try
            {
                handWorldPos = handLandmarkList[(int)_handPosLandmarkIndex].transform.position;
                hand.HandWorldPos = handWorldPos;

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
