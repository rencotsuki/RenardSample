/*
 * Mediapipe - DetectionAnnotationの改修
 */

using System;
using UnityEngine;
using UnityEngine.Serialization;
using Mediapipe;
using Mediapipe.Unity;
using Mediapipe.Unity.CoordinateSystem;

using mptcc = Mediapipe.Tasks.Components.Containers;

namespace SignageHADO.Tracking
{
#pragma warning disable IDE0065
    using Color = UnityEngine.Color;
#pragma warning restore IDE0065

    public sealed class MediapipeDetectionAnnotation : HierarchicalAnnotation
    {
        [FormerlySerializedAs("_locationDataAnnotation")]
        [SerializeField] private RectangleAnnotation _boundingBoxAnnotation = null;
        [FormerlySerializedAs("_relativeKeypointsAnnotation")]
        [SerializeField] private PointListAnnotation _keypointsAnnotation = null;
        [SerializeField] private LabelAnnotation _labelAnnotation = null;

        [HideInInspector] private mptcc.Category? _category = null;
        [HideInInspector] private float _score = 0f;
        [HideInInspector] private Color _color = Color.clear;
        [HideInInspector] private Vector3[] _rectVertices = null;
        [HideInInspector] private float _width = 0f;
        [HideInInspector] private float _height = 0f;
        [HideInInspector] private float _maxWidth = 0f;
        [HideInInspector] private float _maxHeight = 0f;
        [HideInInspector] private string _labelText = string.Empty;
        [HideInInspector] private int _vertexId = 0;
        [HideInInspector] private bool _isInverted = false;

        [HideInInspector] private float _tmpThreshold = 0f;
        [HideInInspector] private float _tmpColorH = 0f;
        [HideInInspector] private Color _tmpHSVToRGB = Color.clear;

        public override bool isMirrored
        {
            set
            {
                if (_boundingBoxAnnotation != null)
                    _boundingBoxAnnotation.isMirrored = value;

                if (_keypointsAnnotation != null)
                    _keypointsAnnotation.isMirrored = value;

                if (_labelAnnotation != null)
                    _labelAnnotation.isMirrored = value;

                base.isMirrored = value;
            }
        }

        public override RotationAngle rotationAngle
        {
            set
            {
                if (_boundingBoxAnnotation != null)
                    _boundingBoxAnnotation.rotationAngle = value;

                if (_keypointsAnnotation != null)
                    _keypointsAnnotation.rotationAngle = value;

                if (_labelAnnotation != null)
                    _labelAnnotation.rotationAngle = value;

                base.rotationAngle = value;
            }
        }

        public void SetLineWidth(float lineWidth)
        {
            _boundingBoxAnnotation?.SetLineWidth(lineWidth);
        }

        public void SetKeypointRadius(float radius)
        {
            _keypointsAnnotation?.SetRadius(radius);
        }

        public void Draw(Detection target, float threshold = 0.0f)
        {
            try
            {
                if (ActivateFor(target))
                {
                    _score = target.Score.Count > 0 ? target.Score[0] : 1.0f;
                    _color = GetColor(_score, Mathf.Clamp(threshold, 0.0f, 1.0f));

                    _rectVertices = GetScreenRect().GetRectVertices(target.LocationData.RelativeBoundingBox, rotationAngle, isMirrored);
                    _boundingBoxAnnotation.SetColor(GetColor(_score, Mathf.Clamp(threshold, 0.0f, 1.0f)));
                    _boundingBoxAnnotation.Draw(_rectVertices);

                    _width = _rectVertices[2].x - _rectVertices[0].x;
                    _height = _rectVertices[2].y - _rectVertices[0].y;
                    _labelText = target.Label.Count > 0 ? target.Label[0] : null;
                    _vertexId = (((int)rotationAngle / 90) + 1) % 4;
                    _isInverted = ImageCoordinate.IsInverted(rotationAngle);
                    (_maxWidth, _maxHeight) = _isInverted ? (_height, _width) : (_width, _height);
                    _labelAnnotation.Draw(_labelText, _rectVertices[_vertexId], _color, _maxWidth, _maxHeight);

                    _keypointsAnnotation.Draw(target.LocationData.RelativeKeypoints);
                }
            }
            catch (Exception ex)
            {
                Debug.Log($"{typeof(MediapipeDetectionAnnotation).Name}::Draw - {ex.Message}");
            }
        }

        public void Draw(mptcc.Detection target, Vector2Int imageSize, float threshold = 0.0f)
        {
            try
            {
                if (ActivateFor(target))
                {
                    _category = target.categories?.Count > 0 ? (mptcc.Category?)target.categories[0] : null;
                    _score = _category != null ? _category.Value.score : 1.0f;
                    _color = GetColor(_score, Mathf.Clamp(threshold, 0.0f, 1.0f));

                    _rectVertices = GetScreenRect().GetRectVertices(target.boundingBox, imageSize, rotationAngle, isMirrored);
                    _boundingBoxAnnotation.SetColor(GetColor(_score, Mathf.Clamp(threshold, 0.0f, 1.0f)));
                    _boundingBoxAnnotation.Draw(_rectVertices);

                    _width = _rectVertices[2].x - _rectVertices[0].x;
                    _height = _rectVertices[2].y - _rectVertices[0].y;
                    _labelText = _category?.categoryName;
                    _vertexId = (((int)rotationAngle / 90) + 1) % 4;
                    _isInverted = ImageCoordinate.IsInverted(rotationAngle);
                    (_maxWidth, _maxHeight) = _isInverted ? (_height, _width) : (_width, _height);
                    _labelAnnotation.Draw(_labelText, _rectVertices[_vertexId], _color, _maxWidth, _maxHeight);

                    _keypointsAnnotation.Draw(target.keypoints);
                }
            }
            catch (Exception ex)
            {
                Debug.Log($"{typeof(MediapipeDetectionAnnotation).Name}::Draw - {ex.Message}");
            }
        }

        private Color GetColor(float score, float threshold)
        {
            _tmpThreshold = (score - threshold) / (1 - threshold);
            _tmpColorH = Mathf.Lerp(90, 0, _tmpThreshold) / 360;
            _tmpHSVToRGB = Color.HSVToRGB(_tmpColorH, 1, 1);

            if (_tmpThreshold < 0)
                _tmpHSVToRGB.a = 0.5f;

            return _tmpHSVToRGB;
        }
    }
}
