/*
 * Mediapipe - HandLandmarkDetectionConfigとObjectDetectionConfigの合成改修
 */

using System;
using UnityEngine;
using Mediapipe.Tasks.Vision.FaceLandmarker;
using Mediapipe.Tasks.Vision.HandLandmarker;
using Mediapipe.Tasks.Vision.ObjectDetector;
using Mediapipe.Unity.Sample;

using Tasks = Mediapipe.Tasks;
using FaceModelType = Mediapipe.Unity.Sample.FaceDetection.ModelType;
using ObjectModelType = Mediapipe.Unity.Sample.ObjectDetection.ModelType;

namespace SignageHADO.Tracking
{
    [Serializable]
    public class MediapipeConfig
    {
        public Tasks.Core.BaseOptions.Delegate Delegate { get; set; } =
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN || UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
            Tasks.Core.BaseOptions.Delegate.CPU;
#else
            Tasks.Core.BaseOptions.Delegate.GPU;
#endif

        [SerializeField] public Tasks.Vision.Core.RunningMode RunningMode = Tasks.Vision.Core.RunningMode.LIVE_STREAM;

        [SerializeField] public int NumFaces = 1;

        [SerializeField] public float MinFaceDetectionConfidence = 0.5f;

        [SerializeField] public float MinFacePresenceConfidence = 0.5f;

        [SerializeField] public bool OutputFaceBlendshapes = true;

        [SerializeField] public FaceModelType FaceModel = FaceModelType.BlazeFaceShortRange;

        [SerializeField] public int NumHands = 2;

        [SerializeField] public float MinHandDetectionConfidence = 0.5f;

        [SerializeField] public float MinHandPresenceConfidence = 0.5f;

        [SerializeField] public float MinTrackingConfidence = 0.5f;

        [SerializeField] public ObjectModelType ObjectModel = ObjectModelType.SSDMobileNetV2Float16;

        [Header("スコア上位の検出結果の最大数(-1は全てを返す)")]
        [SerializeField] public int MaxResults = -1;

        [Header("予測スコアしきい値(値を下回る結果は拒否)")]
        [SerializeField, Range(0f, 1f)] public float ScoreThreshold = 0.5f;

        public string FaceModelName => FaceModel.GetDescription() ?? FaceModel.ToString();

        public string FaceModelPath => OutputFaceBlendshapes ? "face_landmarker_v2_with_blendshapes.bytes" : "face_landmarker_v2.bytes";

        public string HandModelPath => "hand_landmarker.bytes";

        public string ObjectModelName => ObjectModel.GetDescription() ?? ObjectModel.ToString();

        public string ObjectModelPath
        {
            get
            {
                switch (ObjectModel)
                {
                    case ObjectModelType.EfficientDetLite0Float16:
                        return "efficientdet_lite0_float16.bytes";
                    case ObjectModelType.EfficientDetLite0Float32:
                        return "efficientdet_lite0_float32.bytes";
                    case ObjectModelType.EfficientDetLite0Int8:
                        return "efficientdet_lite0_int8.bytes";
                    case ObjectModelType.EfficientDetLite2Float16:
                        return "efficientdet_lite2_float16.bytes";
                    case ObjectModelType.EfficientDetLite2Float32:
                        return "efficientdet_lite2_float32.bytes";
                    case ObjectModelType.EfficientDetLite2Int8:
                        return "efficientdet_lite2_int8.bytes";
                    case ObjectModelType.SSDMobileNetV2Float16:
                        return "ssd_mobilenet_v2_float16.bytes";
                    case ObjectModelType.SSDMobileNetV2Float32:
                        return "ssd_mobilenet_v2_float32.bytes";
                    default:
                        return null;
                }
            }
        }

        public FaceLandmarkerOptions GetFaceLandmarkerOptions(FaceLandmarkerOptions.ResultCallback resultCallback = null)
        {
            return new FaceLandmarkerOptions(
                new Tasks.Core.BaseOptions(Delegate, modelAssetPath: FaceModelPath),
                runningMode: RunningMode,
                numFaces: NumFaces,
                minFaceDetectionConfidence: MinFaceDetectionConfidence,
                minFacePresenceConfidence: MinFacePresenceConfidence,
                minTrackingConfidence: MinTrackingConfidence,
                resultCallback: resultCallback
            );
        }

        public HandLandmarkerOptions GetHandLandmarkerOptions(HandLandmarkerOptions.ResultCallback resultCallback = null)
        {
            return new HandLandmarkerOptions(
                new Tasks.Core.BaseOptions(Delegate, modelAssetPath: HandModelPath),
                runningMode: RunningMode,
                numHands: NumHands,
                minHandDetectionConfidence: MinHandDetectionConfidence,
                minHandPresenceConfidence: MinHandPresenceConfidence,
                minTrackingConfidence: MinTrackingConfidence,
                resultCallback: resultCallback
            );
        }

        public ObjectDetectorOptions GetObjectDetectorOptions(ObjectDetectorOptions.ResultCallback resultCallback = null)
        {
            return new ObjectDetectorOptions(
                new Tasks.Core.BaseOptions(Delegate, modelAssetPath: ObjectModelPath),
                runningMode: RunningMode,
                maxResults: MaxResults,
                scoreThreshold: ScoreThreshold,
                resultCallback: resultCallback
            );
        }
    }
}
