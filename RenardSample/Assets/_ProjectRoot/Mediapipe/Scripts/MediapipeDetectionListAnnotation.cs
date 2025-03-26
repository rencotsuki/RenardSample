/*
 * Mediapipe - DetectionListAnnotationの改修
 */

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Mediapipe;

using mptcc = Mediapipe.Tasks.Components.Containers;

namespace SignageHADO.Tracking
{
    public sealed class MediapipeDetectionListAnnotation : MediapipeListAnnotation<MediapipeDetectionAnnotation>
    {
        [SerializeField, Range(0, 1)] private float _lineWidth = 1.0f;
        [SerializeField] private float _keypointRadius = 15.0f;

        public bool Active => gameObject.activeSelf;

        public int TargetCount { get; private set; } = 0;

        private string[] _targetCategory = null;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (!UnityEditor.PrefabUtility.IsPartOfAnyPrefab(this))
            {
                ApplyLineWidth(_lineWidth);
                ApplyKeypointRadius(_keypointRadius);
            }
        }
#endif

        public void SetTargetCategory(string[] category)
        {
            _targetCategory = category;
        }

        public void SetLineWidth(float lineWidth)
        {
            _lineWidth = lineWidth;
            ApplyLineWidth(lineWidth);
        }

        public void SetKeypointRadius(float keypointRadius)
        {
            _keypointRadius = keypointRadius;
            ApplyKeypointRadius(keypointRadius);
        }

        public void Draw(IReadOnlyList<mptcc.Detection> targets, Vector2Int imageSize, float threshold = 0.0f)
        {
            TargetCount = 0;

            if (ActivateFor(targets))
            {
                CallActionForAll(targets, (annotation, target) =>
                {
                    try
                    {
                        if (_targetCategory != null && _targetCategory.Length > 0)
                        {
                            var category = target.categories?.Count > 0 ? (mptcc.Category?)target.categories[0] : null;
                            if (category == null || !_targetCategory.Contains(category?.categoryName))
                            {
                                annotation?.Draw(default, imageSize, threshold);
                                return;
                            }
                        }

                        annotation?.Draw(target, imageSize, threshold);

                        TargetCount++;
                    }
                    catch (Exception ex)
                    {
                        Debug.Log($"{typeof(MediapipeDetectionListAnnotation).Name}::Draw - {ex.Message}");
                    }
                });
            }
        }

        public void Draw(mptcc.DetectionResult target, Vector2Int imageSize, float threshold = 0.0f)
        {
            Draw(target.detections, imageSize, threshold);
        }

        public void Draw(IReadOnlyList<Detection> targets, float threshold = 0.0f)
        {
            TargetCount = 0;

            if (ActivateFor(targets))
            {
                CallActionForAll(targets, (annotation, target) =>
                {
                    try
                    {
                        if (_targetCategory != null && _targetCategory.Length > 0)
                        {
                            var categoryName = target.Label.Count > 0 ? target.Label[0] : null;
                            if (!_targetCategory.Contains(categoryName))
                            {
                                annotation?.Draw(default, threshold);
                                return;
                            }
                        }

                        annotation?.Draw(target, threshold);

                        TargetCount++;
                    }
                    catch (Exception ex)
                    {
                        Debug.Log($"{typeof(MediapipeDetectionListAnnotation).Name}::Draw - {ex.Message}");
                    }
                });
            }
        }

        public void Draw(DetectionList target, float threshold = 0.0f)
        {
            Draw(target?.Detection, threshold);
        }

        protected override MediapipeDetectionAnnotation InstantiateChild(bool isActive = true)
        {
            var annotation = base.InstantiateChild(isActive);
            annotation.SetLineWidth(_lineWidth);
            annotation.SetKeypointRadius(_keypointRadius);
            return annotation;
        }

        private void ApplyLineWidth(float lineWidth)
        {
            foreach (var detection in children)
            {
                if (detection != null) { detection.SetLineWidth(lineWidth); }
            }
        }

        private void ApplyKeypointRadius(float keypointRadius)
        {
            foreach (var detection in children)
            {
                if (detection != null) { detection.SetKeypointRadius(keypointRadius); }
            }
        }
    }
}
