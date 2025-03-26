/*
 * Mediapipe - ObjectDetectionConfigの改修
 */

using System;
using System.ComponentModel;
using UnityEngine;
using Mediapipe.Unity.Sample;
using Mediapipe.Unity.Sample.ObjectDetection;

using Tasks = Mediapipe.Tasks;

namespace SignageHADO.Tracking
{
    [Serializable]
    public class MediapipeObjectDetectionConfig
    {
        public Tasks.Core.BaseOptions.Delegate Delegate { get; set; } =
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN || UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
            Tasks.Core.BaseOptions.Delegate.CPU;
#else
            Tasks.Core.BaseOptions.Delegate.GPU;
#endif

        [SerializeField] public ModelType Model = ModelType.SSDMobileNetV2Float16;

        [SerializeField] public Tasks.Vision.Core.RunningMode RunningMode = Tasks.Vision.Core.RunningMode.LIVE_STREAM;

        [Header("スコア上位の検出結果の最大数(-1は全てを返す)")]
        [SerializeField] public int MaxResults = -1;

        [Header("予測スコアしきい値(値を下回る結果は拒否)")]
        [SerializeField, Range(0f, 1f)] public float ScoreThreshold = 0.5f;

        public string ModelName => Model.GetDescription() ?? Model.ToString();
        public string ModelPath
        {
            get
            {
                switch (Model)
                {
                    case ModelType.EfficientDetLite0Float16:
                        return "efficientdet_lite0_float16.bytes";
                    case ModelType.EfficientDetLite0Float32:
                        return "efficientdet_lite0_float32.bytes";
                    case ModelType.EfficientDetLite0Int8:
                        return "efficientdet_lite0_int8.bytes";
                    case ModelType.EfficientDetLite2Float16:
                        return "efficientdet_lite2_float16.bytes";
                    case ModelType.EfficientDetLite2Float32:
                        return "efficientdet_lite2_float32.bytes";
                    case ModelType.EfficientDetLite2Int8:
                        return "efficientdet_lite2_int8.bytes";
                    case ModelType.SSDMobileNetV2Float16:
                        return "ssd_mobilenet_v2_float16.bytes";
                    case ModelType.SSDMobileNetV2Float32:
                        return "ssd_mobilenet_v2_float32.bytes";
                    default:
                        return null;
                }
            }
        }

        public Tasks.Vision.ObjectDetector.ObjectDetectorOptions GetObjectDetectorOptions(Tasks.Vision.ObjectDetector.ObjectDetectorOptions.ResultCallback resultCallback = null)
        {
            return new Tasks.Vision.ObjectDetector.ObjectDetectorOptions(
                new Tasks.Core.BaseOptions(Delegate, modelAssetPath: ModelPath),
                runningMode: RunningMode,
                maxResults: MaxResults,
                scoreThreshold: ScoreThreshold,
                resultCallback: resultCallback
            );
        }
    }
}
