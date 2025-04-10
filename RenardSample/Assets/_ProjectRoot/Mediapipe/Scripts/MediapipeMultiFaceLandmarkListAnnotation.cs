/*
 * Mediapipe - MultiFaceLandmarkListAnnotationの改修
 */

using System;
using System.Collections.Generic;
using UnityEngine;
using Mediapipe;
using Mediapipe.Unity;

using mptcc = Mediapipe.Tasks.Components.Containers;

namespace SignageHADO.Tracking
{
    using FaceLandmarkListWithIrisAnnotation = MediapipeFaceLandmarkListWithIrisAnnotation;
    using FaceLandmarkListAnnotation = MediapipeFaceLandmarkListAnnotation;

#pragma warning disable IDE0065
    using Color = UnityEngine.Color;
#pragma warning restore IDE0065

    public sealed class MediapipeMultiFaceLandmarkListAnnotation : ListAnnotation<FaceLandmarkListWithIrisAnnotation>
    {
        [SerializeField] private Color _faceLandmarkColor = Color.green;
        [SerializeField] private Color _irisLandmarkColor = Color.yellow;
        [SerializeField] private float _faceLandmarkRadius = 10.0f;
        [SerializeField] private float _irisLandmarkRadius = 10.0f;
        [SerializeField] private Color _faceConnectionColor = Color.red;
        [SerializeField] private Color _irisCircleColor = Color.blue;
        [SerializeField, Range(0, 1)] private float _faceConnectionWidth = 1.0f;
        [SerializeField, Range(0, 1)] private float _irisCircleWidth = 1.0f;
        [SerializeField] private int _headPosLandmarkIndex = MediapipeLandmark.FaceJaw;

        /// <summary>基準とする瞳間距離[cm]</summary>
        public float BasiseInterpupillary = 6.3f;

        private FaceLandmarkListAnnotation faceLandmarkList => children != null ? children[0].FaceLandmarkList : null;
        private IrisLandmarkListAnnotation leftIrisLandmarkList => children != null ? children[0].LeftIrisLandmarkList : null;
        private IrisLandmarkListAnnotation rightIrisLandmarkList => children != null ? children[0].RightIrisLandmarkList : null;

        private Camera targetCamera => Camera.main;

        [Serializable]
        private struct FaceTracking
        {
            public Vector3 HeadWorldPos;
            public Vector3 NoseTipWorldPos;
            public Vector3 LeftFaceEdgeWorldPos;
            public Vector3 RightFaceEdgeWorldPos;
            public float HeadScale;
        }

        private FaceTracking face = new FaceTracking();

        [HideInInspector] private float _tmpPixelDistance = 0f;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (!UnityEditor.PrefabUtility.IsPartOfAnyPrefab(this))
            {
                ApplyFaceLandmarkColor(_faceLandmarkColor);
                ApplyIrisLandmarkColor(_irisLandmarkColor);
                ApplyFaceLandmarkRadius(_faceLandmarkRadius);
                ApplyIrisLandmarkRadius(_irisLandmarkRadius);
                ApplyFaceConnectionColor(_faceConnectionColor);
                ApplyIrisCircleColor(_irisCircleColor);
                ApplyFaceConnectionWidth(_faceConnectionWidth);
                ApplyIrisCircleWidth(_irisCircleWidth);
            }
        }
#endif

        public void SetFaceLandmarkRadius(float radius)
        {
            _faceLandmarkRadius = radius;
            ApplyFaceLandmarkRadius(_faceLandmarkRadius);
        }

        public void SetIrisLandmarkRadius(float radius)
        {
            _irisLandmarkRadius = radius;
            ApplyIrisLandmarkRadius(_irisLandmarkRadius);
        }

        public void SetFaceLandmarkColor(Color color)
        {
            _faceLandmarkColor = color;
            ApplyFaceLandmarkColor(_faceLandmarkColor);
        }

        public void SetIrisLandmarkColor(Color color)
        {
            _irisLandmarkColor = color;
            ApplyIrisLandmarkColor(_irisLandmarkColor);
        }

        public void SetFaceConnectionWidth(float width)
        {
            _faceConnectionWidth = width;
            ApplyFaceConnectionWidth(_faceConnectionWidth);
        }

        public void SetFaceConnectionColor(Color color)
        {
            _faceConnectionColor = color;
            ApplyFaceConnectionColor(_faceConnectionColor);
        }

        public void SetIrisCircleWidth(float width)
        {
            _irisCircleWidth = width;
            ApplyIrisCircleWidth(_irisCircleWidth);
        }

        public void SetIrisCircleColor(Color color)
        {
            _irisCircleColor = color;
            ApplyIrisCircleColor(_irisCircleColor);
        }

        public void Draw(IReadOnlyList<NormalizedLandmarkList> targets, bool visualizeZ = false)
        {
            if (ActivateFor(targets))
            {
                CallActionForAll(targets, (annotation, target) =>
                {
                    if (annotation != null) { annotation.Draw(target, visualizeZ); }
                });
            }
        }

        public void Draw(IReadOnlyList<mptcc.NormalizedLandmarks> targets, bool visualizeZ = false)
        {
            if (ActivateFor(targets))
            {
                CallActionForAll(targets, (annotation, target) =>
                {
                    if (annotation != null) { annotation.Draw(target, visualizeZ); }
                });
            }
        }

        protected override FaceLandmarkListWithIrisAnnotation InstantiateChild(bool isActive = true)
        {
            var annotation = base.InstantiateChild(isActive);
            annotation.SetFaceLandmarkRadius(_faceLandmarkRadius);
            annotation.SetIrisLandmarkRadius(_irisLandmarkRadius);
            annotation.SetFaceLandmarkColor(_faceLandmarkColor);
            annotation.SetIrisLandmarkColor(_irisLandmarkColor);
            annotation.SetFaceConnectionWidth(_faceConnectionWidth);
            annotation.SetFaceConnectionColor(_faceConnectionColor);
            annotation.SetIrisCircleWidth(_irisCircleWidth);
            annotation.SetIrisCircleColor(_irisCircleColor);
            return annotation;
        }

        private void ApplyFaceLandmarkRadius(float radius)
        {
            foreach (var faceLandmarkList in children)
            {
                if (faceLandmarkList != null) { faceLandmarkList.SetFaceLandmarkRadius(radius); }
            }
        }

        private void ApplyIrisLandmarkRadius(float radius)
        {
            foreach (var faceLandmarkList in children)
            {
                if (faceLandmarkList != null) { faceLandmarkList.SetIrisLandmarkRadius(radius); }
            }
        }

        private void ApplyFaceLandmarkColor(Color color)
        {
            foreach (var faceLandmarkList in children)
            {
                if (faceLandmarkList != null) { faceLandmarkList.SetFaceLandmarkColor(color); }
            }
        }

        private void ApplyIrisLandmarkColor(Color color)
        {
            foreach (var faceLandmarkList in children)
            {
                if (faceLandmarkList != null) { faceLandmarkList.SetIrisLandmarkColor(color); }
            }
        }

        private void ApplyFaceConnectionWidth(float width)
        {
            foreach (var faceLandmarkList in children)
            {
                if (faceLandmarkList != null) { faceLandmarkList.SetFaceConnectionWidth(width); }
            }
        }

        private void ApplyFaceConnectionColor(Color color)
        {
            foreach (var faceLandmarkList in children)
            {
                if (faceLandmarkList != null) { faceLandmarkList.SetFaceConnectionColor(color); }
            }
        }

        private void ApplyIrisCircleWidth(float width)
        {
            foreach (var faceLandmarkList in children)
            {
                if (faceLandmarkList != null) { faceLandmarkList.SetIrisCircleWidth(width); }
            }
        }

        private void ApplyIrisCircleColor(Color color)
        {
            foreach (var faceLandmarkList in children)
            {
                if (faceLandmarkList != null) { faceLandmarkList.SetIrisCircleColor(color); }
            }
        }

        private float GetPixelDistance(Vector3 from, Vector3 to)
        {
            return targetCamera != null ?
                Vector2.Distance(targetCamera.WorldToScreenPoint(from), targetCamera.WorldToScreenPoint(to)) :
                0f;
        }

        public bool GetFaceLandmarkPos(out Vector3 headWorldPos, out float headScale, out Vector3 noseTipWorldPos, out Vector3 leftFaceEdgeWorldPos, out Vector3 rightFaceEdgeWorldPos)
        {
            headWorldPos = face.HeadWorldPos;
            headScale = face.HeadScale;
            noseTipWorldPos = face.NoseTipWorldPos;
            leftFaceEdgeWorldPos = face.LeftFaceEdgeWorldPos;
            rightFaceEdgeWorldPos = face.RightFaceEdgeWorldPos;

            try
            {
                face.HeadWorldPos = faceLandmarkList[_headPosLandmarkIndex].transform.position;
                headWorldPos = face.HeadWorldPos;

                face.NoseTipWorldPos = faceLandmarkList[MediapipeLandmark.FaceNoseTip].transform.position;
                noseTipWorldPos = face.NoseTipWorldPos;

                face.LeftFaceEdgeWorldPos = faceLandmarkList[MediapipeLandmark.FaceLeftEdgeIndex].transform.position;
                leftFaceEdgeWorldPos = face.LeftFaceEdgeWorldPos;

                face.RightFaceEdgeWorldPos = faceLandmarkList[MediapipeLandmark.FaceRightEdgeIndex].transform.position;
                rightFaceEdgeWorldPos = face.RightFaceEdgeWorldPos;

                // 左目(33)と右目(263)のピクセル距離
                _tmpPixelDistance = GetPixelDistance(faceLandmarkList[MediapipeLandmark.FaceLeftEye].transform.position,
                                                     faceLandmarkList[MediapipeLandmark.FaceRightEye].transform.position);

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
    }
}
