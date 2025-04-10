/*
 * Mediapipe - XXXLandmarkerRunnerの改修
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using Mediapipe;
using Mediapipe.Tasks.Vision.FaceLandmarker;
using Mediapipe.Tasks.Vision.HandLandmarker;
using Mediapipe.Tasks.Vision.ObjectDetector;
using Mediapipe.Unity;
using Mediapipe.Unity.Sample;

using TextureFramePool = Mediapipe.Unity.Experimental.TextureFramePool;
using NormalizedLandmark = Mediapipe.Tasks.Components.Containers.NormalizedLandmark;
using ObjectDetectionResult = Mediapipe.Tasks.Components.Containers.DetectionResult;
using Tasks = Mediapipe.Tasks;

namespace SignageHADO.Tracking
{
    using Hand = MediapipeLandmark.Hand;

    public class MediapipeLandmarkerRunner : MediapipeVisionTaskApiRunner
    {
        [SerializeField] private RectTransform _worldAnnotationArea = null;
        [SerializeField] private MediapipeMultiFaceLandmarkAnnotationController _faceLandmarkListAnnotation = null;
        [SerializeField] private MediapipeMultiHandLandmarkAnnotationController _handLandmarkListAnnotation = null;
        [SerializeField] private bool _enabledDetection = true;
        [SerializeField] private MediapipeDetectionResultAnnotationController _detectionAnnotation = null;
        [Header("Landmarkフィルター設定")]
        [Header("-- face --")]
        [SerializeField] private bool filterFacePos = true;
        [SerializeField, Tooltip(KalmanFilter.ProcessNoiseToolTip)] private float processNoiseFacePos = 1e-3f;
        [SerializeField, Tooltip(KalmanFilter.MeasurementNoiseToolTip)] private float measurementNoiseFacePos = 1e-1f;
        [SerializeField, Tooltip(KalmanFilter.NoiseMaxRangeToolTip)] private float noiseMaxRangeFacePos = 3;
        [SerializeField] private bool filterFaceRot = true;
        [SerializeField, Tooltip(KalmanFilter.ProcessNoiseToolTip)] private float processNoiseFaceRot = 1e-3f;
        [SerializeField, Tooltip(KalmanFilter.MeasurementNoiseToolTip)] private float measurementNoiseFaceRot = 1e-1f;
        [SerializeField, Tooltip(KalmanFilter.NoiseMaxRangeToolTip)] private float noiseMaxRangeFaceRot = 5f;

        [Header("-- hand --")]
        [SerializeField] private bool filterHand = true;
        [SerializeField, Tooltip(KalmanFilter.ProcessNoiseToolTip)] private float processNoiseHand = 1e-3f;
        [SerializeField, Tooltip(KalmanFilter.MeasurementNoiseToolTip)] private float measurementNoiseHand = 1e-1f;
        [SerializeField, Tooltip(KalmanFilter.NoiseMaxRangeToolTip)] private float noiseMaxRangeHand = 1.5f;

        [Header("-- finger --")]
        [SerializeField] private bool filterFinger = true;
        [SerializeField, Tooltip(KalmanFilter.ProcessNoiseToolTip)] private float processNoiseFinger = 1e-4f;
        [SerializeField, Tooltip(KalmanFilter.MeasurementNoiseToolTip)] private float measurementNoiseFinger = 1e-2f;
        [SerializeField, Tooltip(KalmanFilter.NoiseMaxRangeToolTip)] private float noiseMaxRangeFinger = 1.5f;

        [Header("設定")]
        [SerializeField] private MediapipeConfig config = new MediapipeConfig();
        [SerializeField] private string[] detectionTargetCategory = new string[] { "person" };
        [Header("基準とする瞳間距離[cm]")]
        [SerializeField] private float basiseInterpupillary = 6.3f;

        private TextureFramePool _textureFramePool = null;

        private bool _isMirrored = false;
        public bool IsMirrored
        {
            get => _isMirrored;
            set
            {
                _isMirrored = value;

                if (_faceLandmarkListAnnotation != null)
                    _faceLandmarkListAnnotation.isMirrored = _isMirrored;

                if (_handLandmarkListAnnotation != null)
                    _handLandmarkListAnnotation.isMirrored = _isMirrored;

                if (_detectionAnnotation != null)
                    _detectionAnnotation.isMirrored = _isMirrored;
            }
        }

        public bool EnabledDetection
        {
            get => _enabledDetection;
            set => _enabledDetection = value;
        }

        protected int handLandmarkes => MediapipeLandmark.HandLandmarkes;

        public Vector2 TrakincViewSize { get; private set; } = Vector2.zero;

        public int DetectionTargetCount => _detectionAnnotation != null ? _detectionAnnotation.TargetCount : 0;

        protected RotationAngle rotationAngle
        {
            set
            {
                if (_faceLandmarkListAnnotation != null)
                    _faceLandmarkListAnnotation.rotationAngle = value;

                if (_handLandmarkListAnnotation != null)
                    _handLandmarkListAnnotation.rotationAngle = value;

                if (_detectionAnnotation != null)
                    _detectionAnnotation.rotationAngle = value;
            }
        }

        [Serializable]
        protected class TrackingFace
        {
            public bool Active = false;

            public Vector3 HeadWorldPos = Vector3.zero;
            public float HeadScale = 0f;

            public Quaternion HeadRotation = Quaternion.identity;

            public Vector3 ErrorCovariancePos = Vector3.one * 100f;
            public Quaternion ErrorCovarianceRot = new Quaternion(1f, 1f, 1f, 1f);
        }

        [Serializable]
        protected class TrackingFinger
        {
            public bool Active = false;

            public Vector3 Mcp = Vector3.zero;
            public Vector3 Pip = Vector3.zero;
            public Vector3 Dip = Vector3.zero;
            public Vector3 Tip = Vector3.zero;

            public float FirstJoint = 0f;   // Tip - Dip - Pip の角度
            public float SecondJoint = 0f;  // Dip - Pip - Mcp の角度
            public float ThirdJoint = 0f;   // Pip - Mcp - Wrist の角度

            public float ErrorCovarianceFirst = 360f;
            public float ErrorCovarianceSecond = 360f;
            public float ErrorCovarianceThird = 360f;
        }

        [Serializable]
        protected class TrackingHand
        {
            public bool Active = false;

            public Vector3 HandWorldPos = Vector3.zero;
            public HandPoseEnum Pose = HandPoseEnum.Unknown;

            public Vector3 ErrorCovariancePos = Vector3.one * 100f;

            #region Finger

            public Vector3 WristPos = Vector3.zero;
            public Vector3 ThumbCMCPos = Vector3.zero;
            public Vector3 IndexMCPPos = Vector3.zero;
            public Vector3 PinkyMCPPos = Vector3.zero;

            public Vector3 HandBasisX = Vector3.zero;
            public Vector3 HandBasisY = Vector3.zero;
            public Vector3 HandBasisZ = Vector3.zero;

            public bool IsOpenThumb = false;
            public TrackingFinger Thumb = new TrackingFinger();

            public bool IsOpenIndexFinger = false;
            public TrackingFinger IndexFinger = new TrackingFinger();

            public bool IsOpenMiddleFinger = false;
            public TrackingFinger MiddleFinger = new TrackingFinger();

            public bool IsOpenRingFinger = false;
            public TrackingFinger RingFinger = new TrackingFinger();

            public bool IsOpenPrinky = false;
            public TrackingFinger Prinky = new TrackingFinger();

            #endregion
        }

        [HideInInspector] private KalmanFilterVector3 _fingerFacePos = null;
        [HideInInspector] private KalmanFilterQuaternion _fingerFaceRot = null;
        [HideInInspector] private TrackingFace _face = new TrackingFace();
        [HideInInspector] private Vector3 _faceNoseWorldPos = Vector3.zero;
        [HideInInspector] private Vector3 _faceLeftEdgeWorldPos = Vector3.zero;
        [HideInInspector] private Vector3 _faceRightEdgeWorldPos = Vector3.zero;
        [HideInInspector] private Vector3 _tmpFaceVec = Vector3.zero;
        [HideInInspector] private Vector3 _tmpForward = Vector3.zero;
        [HideInInspector] private Quaternion _tmpFaceRot = Quaternion.identity;
        [HideInInspector] private Quaternion _tmpFaceRot2 = Quaternion.identity;

        [HideInInspector] private KalmanFilterVector3 _fingerHandPos = null;
        [HideInInspector] private KalmanFilterFloat _fingerFilter = null;
        [HideInInspector] private TrackingHand _leftHand = new TrackingHand();
        [HideInInspector] private TrackingHand _rightHand = new TrackingHand();
        [HideInInspector] private Vector3 _tmpFingerLocalPos = Vector3.zero;

        private void Awake()
        {
            if (_faceLandmarkListAnnotation != null)
            {
                IsMirrored = _faceLandmarkListAnnotation.isMirrored;
                _faceLandmarkListAnnotation.BasiseInterpupillary = basiseInterpupillary;
            }

            _detectionAnnotation?.SetTargetCategory(detectionTargetCategory);

            _fingerFacePos = new KalmanFilterVector3(processNoiseFacePos, measurementNoiseFacePos, noiseMaxRangeFacePos, IsDebugLog);
            _fingerFaceRot = new KalmanFilterQuaternion(processNoiseFaceRot, measurementNoiseFaceRot, noiseMaxRangeFaceRot, IsDebugLog);

            _fingerHandPos = new KalmanFilterVector3(processNoiseHand, measurementNoiseHand, noiseMaxRangeHand, IsDebugLog);
            _fingerFilter = new KalmanFilterFloat(processNoiseFinger, measurementNoiseFinger, noiseMaxRangeFinger, IsDebugLog);
        }

        public override void Stop()
        {
            base.Stop();
            _textureFramePool?.Dispose();
            _textureFramePool = null;
        }

        protected override IEnumerator Run()
        {
            Log(DebugerLogType.Info, "Run",
                $"Delegate = {config.Delegate}\n\r" +
                $"Running Mode = {config.RunningMode}\n\r" +
                $"NumFaces = {config.NumFaces}\n\r" +
                $"MinFaceDetectionConfidence = {config.MinFaceDetectionConfidence}\n\r" +
                $"MinFacePresenceConfidence = {config.MinFacePresenceConfidence}\n\r" +
                $"NumHands = {config.NumHands}\n\r" +
                $"MinHandDetectionConfidence = {config.MinHandDetectionConfidence}\n\r" +
                $"MinHandPresenceConfidence = {config.MinHandPresenceConfidence}\n\r" +
                $"MinTrackingConfidence = {config.MinTrackingConfidence}\n\r" +
                $"Max Results = {config.MaxResults}");

            yield return AssetLoader.PrepareAssetAsync(config.FaceModelPath);
            yield return AssetLoader.PrepareAssetAsync(config.HandModelPath);
            yield return AssetLoader.PrepareAssetAsync(config.ObjectModelPath);

            var runningMode = config.RunningMode;

            var optionsFace = config.GetFaceLandmarkerOptions(runningMode == Tasks.Vision.Core.RunningMode.LIVE_STREAM ? OnFaceLandmarkDetectionOutput : null);
            faceLandmarker = FaceLandmarker.CreateFromOptions(optionsFace, GpuManager.GpuResources);

            var optionsHand = config.GetHandLandmarkerOptions(runningMode == Tasks.Vision.Core.RunningMode.LIVE_STREAM ? OnHandLandmarkDetectionOutput : null);
            handLandmarker = HandLandmarker.CreateFromOptions(optionsHand, GpuManager.GpuResources);

            var optionsObject = config.GetObjectDetectorOptions(runningMode == Tasks.Vision.Core.RunningMode.LIVE_STREAM ? OnObjectDetectionsOutput : null);
            objectDetector = ObjectDetector.CreateFromOptions(optionsObject, GpuManager.GpuResources);

            var imageSource = ImageSourceProvider.ImageSource;

            yield return imageSource.Play();

            if (!imageSource.isPrepared)
            {
                Debug.LogError("Failed to start ImageSource, exiting...");
                yield break;
            }

            _textureFramePool = new TextureFramePool(imageSource.textureWidth, imageSource.textureHeight, TextureFormat.RGBA32, 10);

            screen.Initialize(imageSource);

            SetupAnnotationController(_faceLandmarkListAnnotation, imageSource);
            SetupAnnotationController(_handLandmarkListAnnotation, imageSource);
            SetupAnnotationController(_detectionAnnotation, imageSource);

            var transformationOptions = imageSource.GetTransformationOptions();
            var flipHorizontally = transformationOptions.flipHorizontally;
            var flipVertically = transformationOptions.flipVertically;
            var imageProcessingOptions = new Tasks.Vision.Core.ImageProcessingOptions(rotationDegrees: (int)transformationOptions.rotationAngle);

            AsyncGPUReadbackRequest req = default;
            var waitUntilReqDone = new WaitUntil(() => req.done);
            var resultFace = FaceLandmarkerResult.Alloc(optionsFace.numFaces);
            var resultHand = HandLandmarkerResult.Alloc(optionsHand.numHands);
            var resultDetection = ObjectDetectionResult.Alloc(Math.Max(optionsObject.maxResults ?? 0, 0));

            var canUseGpuImage = optionsHand.baseOptions.delegateCase == Tasks.Core.BaseOptions.Delegate.GPU &&
              SystemInfo.graphicsDeviceType == GraphicsDeviceType.OpenGLES3 &&
              GpuManager.GpuResources != null;
            using var glContext = canUseGpuImage ? GpuManager.GetGlContext() : null;

            Image imageFace ,imageHand, imageDetection = null;

            while (true)
            {
                if (isPaused)
                {
                    yield return new WaitWhile(() => isPaused);
                }

                if (!_textureFramePool.TryGetTextureFrame(out var textureFrame))
                {
                    yield return new WaitForEndOfFrame();
                    continue;
                }

                imageFace = imageHand = imageDetection = null;
                if (canUseGpuImage)
                {
                    yield return new WaitForEndOfFrame();
                    textureFrame.ReadTextureOnGPU(imageSource.GetCurrentTexture(), flipHorizontally, flipVertically);
                    imageFace = textureFrame.BuildGpuImage(glContext);
                    imageHand = textureFrame.BuildGpuImage(glContext);
                    imageDetection = textureFrame.BuildGpuImage(glContext);
                }
                else
                {
                    req = textureFrame.ReadTextureAsync(imageSource.GetCurrentTexture(), flipHorizontally, flipVertically);
                    yield return waitUntilReqDone;

                    if (req.hasError)
                    {
                        Debug.LogError($"Failed to read texture from the image source, exiting...");
                        break;
                    }
                    imageFace = textureFrame.BuildCPUImage();
                    imageHand = textureFrame.BuildCPUImage();
                    imageDetection = textureFrame.BuildCPUImage();
                    textureFrame.Release();
                }

                switch (runningMode)
                {
                    case Tasks.Vision.Core.RunningMode.IMAGE:
                        {
                            if (faceLandmarker.TryDetect(imageFace, imageProcessingOptions, ref resultFace))
                            {
                                _faceLandmarkListAnnotation.DrawNow(resultFace);
                            }
                            else
                            {
                                _faceLandmarkListAnnotation.DrawNow(default);
                            }

                            if (handLandmarker.TryDetect(imageHand, imageProcessingOptions, ref resultHand))
                            {
                                _handLandmarkListAnnotation.DrawNow(resultHand);
                            }
                            else
                            {
                                _handLandmarkListAnnotation.DrawNow(default);
                            }

                            if (EnabledDetection)
                            {
                                if (objectDetector.TryDetect(imageDetection, imageProcessingOptions, ref resultDetection))
                                {
                                    _detectionAnnotation?.DrawNow(resultDetection);
                                }
                                else
                                {
                                    _detectionAnnotation?.DrawNow(default);

                                }
                            }
                        }
                        break;

                    case Tasks.Vision.Core.RunningMode.VIDEO:
                        {
                            if (faceLandmarker.TryDetectForVideo(imageFace, GetCurrentTimestampMillisec(), imageProcessingOptions, ref resultFace))
                            {
                                _faceLandmarkListAnnotation.DrawNow(resultFace);
                            }
                            else
                            {
                                _faceLandmarkListAnnotation.DrawNow(default);
                            }

                            if (handLandmarker.TryDetectForVideo(imageHand, GetCurrentTimestampMillisec(), imageProcessingOptions, ref resultHand))
                            {
                                _handLandmarkListAnnotation.DrawNow(resultHand);
                            }
                            else
                            {
                                _handLandmarkListAnnotation.DrawNow(default);
                            }

                            if (EnabledDetection)
                            {
                                if (objectDetector.TryDetectForVideo(imageDetection, GetCurrentTimestampMillisec(), imageProcessingOptions, ref resultDetection))
                                {
                                    _detectionAnnotation?.DrawNow(resultDetection);
                                }
                                else
                                {
                                    _detectionAnnotation?.DrawNow(default);
                                }
                            }
                        }
                        break;

                    case Tasks.Vision.Core.RunningMode.LIVE_STREAM:
                        {
                            faceLandmarker.DetectAsync(imageFace, GetCurrentTimestampMillisec(), imageProcessingOptions);

                            handLandmarker.DetectAsync(imageHand, GetCurrentTimestampMillisec(), imageProcessingOptions);

                            if (EnabledDetection)
                                objectDetector.DetectAsync(imageDetection, GetCurrentTimestampMillisec(), imageProcessingOptions);
                        }
                        break;
                }
            }
        }

        private void OnFaceLandmarkDetectionOutput(FaceLandmarkerResult result, Image image, long timestamp)
        {
            _faceLandmarkListAnnotation.DrawLater(result);
        }

        private void OnHandLandmarkDetectionOutput(HandLandmarkerResult result, Image image, long timestamp)
        {
            _handLandmarkListAnnotation.DrawLater(result);
        }

        private void OnObjectDetectionsOutput(ObjectDetectionResult result, Image image, long timestamp)
        {
            _detectionAnnotation?.DrawLater(result);
        }

        #region API

        public bool GetFaceTracking(out Vector3 headWorldPos, out Quaternion headRotation, out float headScale)
        {
            headWorldPos = _face.HeadWorldPos;
            headRotation = _face.HeadRotation;
            headScale = _face.HeadScale;

            try
            {
                if (_faceLandmarkListAnnotation.IsStale && _faceLandmarkListAnnotation.ActiveFaceLandmark)
                {
                    if (_faceLandmarkListAnnotation.GetFaceLandmarkPos(out headWorldPos, out headScale, out _faceNoseWorldPos, out _faceLeftEdgeWorldPos, out _faceRightEdgeWorldPos))
                    {
                        if (filterFacePos)
                        {
                            headWorldPos = _fingerFacePos
                                            .Set(_face.HeadWorldPos, _face.ErrorCovariancePos)
                                            .Apply(headWorldPos);

                            _face.HeadWorldPos = headWorldPos;
                            _face.ErrorCovariancePos = _fingerFacePos.ErrorCovariance;
                        }
                        else
                        {
                            _face.HeadWorldPos = headWorldPos;
                        }

                        _face.HeadScale = headScale;

                        if (GetHeadRotation(_faceNoseWorldPos, _faceLeftEdgeWorldPos, _faceRightEdgeWorldPos, out headRotation))
                        {
                            if (filterFaceRot)
                            {
                                headRotation = _fingerFaceRot
                                                    .Set(_face.HeadRotation, _face.ErrorCovarianceRot)
                                                    .Apply(headRotation);

                                _face.HeadRotation = headRotation;
                                _face.ErrorCovarianceRot = _fingerFaceRot.ErrorCovariance;
                            }
                            else
                            {
                                _face.HeadRotation = headRotation;
                            }
                            return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log(DebugerLogType.Info, $"GetFaceTracking", $"{ex.Message}");
            }
            return false;
        }

        protected bool GetHeadRotation(Vector3 noiseTipWorldPos, Vector3 leftFaceEdgeWorldPos, Vector3 rightFaceEdgeWorldPos, out Quaternion headRot)
        {
            headRot = Quaternion.identity;

            try
            {
                _tmpFaceVec = noiseTipWorldPos - (leftFaceEdgeWorldPos + rightFaceEdgeWorldPos) / 2;
                _tmpFaceRot = _tmpFaceVec != Vector3.zero ? Quaternion.LookRotation(_tmpFaceVec, Vector3.up) : Quaternion.identity;

                _tmpForward = rightFaceEdgeWorldPos - leftFaceEdgeWorldPos;
                _tmpFaceRot2 = _tmpForward != Vector3.zero ? Quaternion.LookRotation(_tmpForward, Vector3.up) : Quaternion.identity;

                headRot = Quaternion.Euler(_tmpFaceRot.eulerAngles.x, _tmpFaceRot.eulerAngles.y, _tmpFaceRot2.eulerAngles.x);
                return true;
            }
            catch (Exception ex)
            {
                Log(DebugerLogType.Info, $"GetHeadRotation", $"{ex.Message}");
            }
            return false;
        }

        public bool GetLeftHandGesture(out Vector3 handWorldPos, out HandPoseEnum pose)
        {
            handWorldPos = _leftHand.HandWorldPos;
            pose = _leftHand.Pose;

            try
            {
                if (_handLandmarkListAnnotation.IsStale && _handLandmarkListAnnotation.ActiveLeftHandLandmark)
                {
                    if (_handLandmarkListAnnotation.GetLeftHandLandmarkPos(out handWorldPos))
                    {
                        if (filterHand)
                        {
                            handWorldPos = _fingerHandPos
                                            .Set(_leftHand.HandWorldPos, _leftHand.ErrorCovariancePos)
                                            .Apply(handWorldPos);

                            _leftHand.HandWorldPos = handWorldPos;
                            _leftHand.ErrorCovariancePos = _fingerHandPos.ErrorCovariance;
                        }
                        else
                        {
                            _leftHand.HandWorldPos = handWorldPos;
                        }
                    }

                    _leftHand.Active = GetHandGesture(_handLandmarkListAnnotation.CurrentLeftHandLandmarkList, ref _leftHand, false);
                    return _leftHand.Active;
                }
            }
            catch (Exception ex)
            {
                Log(DebugerLogType.Info, $"GetLeftHandGesture", $"{ex.Message}");
            }

            _leftHand.Active = false;
            return false;
        }

        public bool GetRightHandGesture(out Vector3 handWorldPos, out HandPoseEnum pose)
        {
            handWorldPos = _rightHand.HandWorldPos;
            pose = _rightHand.Pose;

            try
            {
                if (_handLandmarkListAnnotation.IsStale && _handLandmarkListAnnotation.ActiveRightHandLandmark)
                {
                    if (_handLandmarkListAnnotation.GetRightHandLandmarkPos(out handWorldPos))
                    {
                        if (filterHand)
                        {
                            handWorldPos = _fingerHandPos
                                            .Set(_rightHand.HandWorldPos, _rightHand.ErrorCovariancePos)
                                            .Apply(handWorldPos);

                            _rightHand.HandWorldPos = handWorldPos;
                            _rightHand.ErrorCovariancePos = _fingerHandPos.ErrorCovariance;
                        }
                        else
                        {
                            _rightHand.HandWorldPos = handWorldPos;
                        }
                    }

                    _rightHand.Active = GetHandGesture(_handLandmarkListAnnotation.CurrentRightHandLandmarkList, ref _rightHand, true);
                    return _rightHand.Active;
                }
            }
            catch (Exception ex)
            {
                Log(DebugerLogType.Info, $"GetRightHandGesture", $"{ex.Message}");
            }

            _rightHand.Active = false;
            return false;
        }

        protected bool GetHandGesture(IReadOnlyList<NormalizedLandmark> list, ref TrackingHand trackingHand, bool right)
        {
            try
            {
                if (list == null || list.Count < handLandmarkes)
                    throw new Exception("not hand landmarkes.");

                trackingHand.WristPos.x = list[(int)Hand.Wrist].x;
                trackingHand.WristPos.y = list[(int)Hand.Wrist].y;
                trackingHand.WristPos.z = list[(int)Hand.Wrist].z;

                trackingHand.ThumbCMCPos.x = list[(int)Hand.ThumbCmcIndex].x;
                trackingHand.ThumbCMCPos.y = list[(int)Hand.ThumbCmcIndex].y;
                trackingHand.ThumbCMCPos.z = list[(int)Hand.ThumbCmcIndex].z;

                trackingHand.IndexMCPPos.x = list[(int)Hand.IndexFingerMcpIndex].x;
                trackingHand.IndexMCPPos.y = list[(int)Hand.IndexFingerMcpIndex].y;
                trackingHand.IndexMCPPos.z = list[(int)Hand.IndexFingerMcpIndex].z;

                trackingHand.PinkyMCPPos.x = list[(int)Hand.PrinkyMcpIndex].x;
                trackingHand.PinkyMCPPos.y = list[(int)Hand.PrinkyMcpIndex].y;
                trackingHand.PinkyMCPPos.z = list[(int)Hand.PrinkyMcpIndex].z;

                GetHandBasis(ref trackingHand);

                // TODO: 親指が画角によって上手く認識されないことがあるので使わない
                //CheckedBendFinger((int)Hand.ThumbCmcIndex, list, trackingHand, ref trackingHand.Thumb);
                //if (right)
                //{
                //    trackingHand.IsOpenThumb = trackingHand.Thumb.Active ?
                //                                    (trackingHand.Thumb.Dip.x > trackingHand.Thumb.Pip.x && trackingHand.Thumb.Tip.x > trackingHand.Thumb.Pip.x) :
                //                                    false;
                //}
                //else
                //{
                //    trackingHand.IsOpenThumb = trackingHand.Thumb.Active ?
                //                                    (trackingHand.Thumb.Dip.x < trackingHand.Thumb.Pip.x && trackingHand.Thumb.Tip.x < trackingHand.Thumb.Pip.x) :
                //                                    false;
                //}

                CheckedBendFinger((int)Hand.IndexFingerMcpIndex, list, trackingHand, ref trackingHand.IndexFinger);
                trackingHand.IsOpenIndexFinger = trackingHand.IndexFinger.Active ?
                                                    (trackingHand.IndexFinger.Dip.y > trackingHand.IndexFinger.Pip.y && trackingHand.IndexFinger.Tip.y > trackingHand.IndexFinger.Pip.y) :
                                                    false;

                CheckedBendFinger((int)Hand.MiddleFingerMcpIndex, list, trackingHand, ref trackingHand.MiddleFinger);
                trackingHand.IsOpenMiddleFinger = trackingHand.MiddleFinger.Active ?
                                                    (trackingHand.MiddleFinger.Dip.y > trackingHand.MiddleFinger.Pip.y && trackingHand.MiddleFinger.Tip.y > trackingHand.MiddleFinger.Pip.y) :
                                                    false;

                CheckedBendFinger((int)Hand.RingFingerMcpIndex, list, trackingHand, ref trackingHand.RingFinger);
                trackingHand.IsOpenRingFinger = trackingHand.RingFinger.Active ?
                                                    (trackingHand.RingFinger.Dip.y > trackingHand.RingFinger.Pip.y && trackingHand.RingFinger.Tip.y > trackingHand.RingFinger.Pip.y) :
                                                    false;

                CheckedBendFinger((int)Hand.PrinkyMcpIndex, list, trackingHand, ref trackingHand.Prinky);
                trackingHand.IsOpenPrinky = trackingHand.Prinky.Active ?
                                                (trackingHand.Prinky.Dip.y > trackingHand.Prinky.Pip.y && trackingHand.Prinky.Tip.y > trackingHand.Prinky.Pip.y) :
                                                false;

                //-- ポーズ判定

                //グー
                //if (!trackingHand.IsOpenThumb && !trackingHand.IsOpenIndexFinger && !trackingHand.IsOpenMiddleFinger && !trackingHand.IsOpenRingFinger && !trackingHand.IsOpenPrinky)
                if (!trackingHand.IsOpenIndexFinger && !trackingHand.IsOpenMiddleFinger && !trackingHand.IsOpenRingFinger && !trackingHand.IsOpenPrinky)
                {
                    trackingHand.Pose = HandPoseEnum.Rock;
                }
                //チョキ
                //else if (!trackingHand.IsOpenThumb && trackingHand.IsOpenIndexFinger && trackingHand.IsOpenMiddleFinger && !trackingHand.IsOpenRingFinger && !trackingHand.IsOpenPrinky)
                else if (trackingHand.IsOpenIndexFinger && trackingHand.IsOpenMiddleFinger && !trackingHand.IsOpenRingFinger && !trackingHand.IsOpenPrinky)
                {
                    trackingHand.Pose = HandPoseEnum.Scissors;
                }
                // パー
                //else if (trackingHand.IsOpenThumb && trackingHand.IsOpenIndexFinger && trackingHand.IsOpenMiddleFinger && trackingHand.IsOpenRingFinger && trackingHand.IsOpenPrinky)
                else if (trackingHand.IsOpenIndexFinger && trackingHand.IsOpenMiddleFinger && trackingHand.IsOpenRingFinger && trackingHand.IsOpenPrinky)
                {
                    trackingHand.Pose = HandPoseEnum.Paper;
                }
                // 該当なし
                else
                {
                    trackingHand.Pose = HandPoseEnum.Unknown;
                }

                return true;
            }
            catch (Exception ex)
            {
                Log(DebugerLogType.Info, $"GetHandGesture", $"{ex.Message}");
            }
            return false;
        }

        protected void GetHandBasis(ref TrackingHand trackingHand)
        {
            trackingHand.HandBasisX = (trackingHand.PinkyMCPPos - trackingHand.ThumbCMCPos).normalized;
            trackingHand.HandBasisZ = Vector3.Cross(trackingHand.HandBasisX, trackingHand.IndexMCPPos - trackingHand.WristPos).normalized;
            trackingHand.HandBasisY = Vector3.Cross(trackingHand.HandBasisZ, trackingHand.HandBasisX).normalized;
        }

        protected void CheckedBendFinger(int headIndex, IReadOnlyList<NormalizedLandmark> list, TrackingHand trackingHand, ref TrackingFinger trackingFinger)
        {
            try
            {
                trackingFinger.Mcp.x = list[headIndex].x;
                trackingFinger.Mcp.y = list[headIndex].y;
                trackingFinger.Mcp.z = list[headIndex].z;

                trackingFinger.Mcp = TransformToLocal(trackingFinger.Mcp, trackingHand);

                trackingFinger.Pip.x = list[headIndex + 1].x;
                trackingFinger.Pip.y = list[headIndex + 1].y;
                trackingFinger.Pip.z = list[headIndex + 1].z;

                trackingFinger.Pip = TransformToLocal(trackingFinger.Pip, trackingHand);

                trackingFinger.Dip.x = list[headIndex + 2].x;
                trackingFinger.Dip.y = list[headIndex + 2].y;
                trackingFinger.Dip.z = list[headIndex + 2].z;

                trackingFinger.Dip = TransformToLocal(trackingFinger.Dip, trackingHand);

                trackingFinger.Tip.x = list[headIndex + 3].x;
                trackingFinger.Tip.y = list[headIndex + 3].y;
                trackingFinger.Tip.z = list[headIndex + 3].z;

                trackingFinger.Tip = TransformToLocal(trackingFinger.Tip, trackingHand);

                if (filterFinger)
                {
                    trackingFinger.ThirdJoint = _fingerFilter
                        .Set(trackingFinger.ThirdJoint, trackingFinger.ErrorCovarianceThird)
                        .Apply(Mathf.Repeat(GetFingerSignedAngle(trackingFinger.Pip, Vector3.zero, trackingFinger.Mcp, (Vector3.zero - trackingFinger.Mcp).normalized), 360f));

                    trackingFinger.ErrorCovarianceThird = _fingerFilter.ErrorCovariance;

                    trackingFinger.SecondJoint = _fingerFilter
                        .Set(trackingFinger.SecondJoint, trackingFinger.ErrorCovarianceSecond)
                        .Apply(Mathf.Repeat(GetFingerSignedAngle(trackingFinger.Dip, trackingFinger.Mcp, trackingFinger.Pip, (trackingFinger.Mcp - trackingFinger.Pip).normalized), 360f));

                    trackingFinger.ErrorCovarianceSecond = _fingerFilter.ErrorCovariance;

                    trackingFinger.FirstJoint = _fingerFilter
                        .Set(trackingFinger.FirstJoint, trackingFinger.ErrorCovarianceFirst)
                        .Apply(Mathf.Repeat(GetFingerSignedAngle(trackingFinger.Tip, trackingFinger.Pip, trackingFinger.Dip, (trackingFinger.Pip - trackingFinger.Dip).normalized), 360f));

                    trackingFinger.ErrorCovarianceFirst = _fingerFilter.ErrorCovariance;
                }
                else
                {
                    trackingFinger.ThirdJoint = Mathf.Repeat(GetFingerSignedAngle(trackingFinger.Pip, Vector3.zero, trackingFinger.Mcp, (Vector3.zero - trackingFinger.Mcp).normalized), 360f);
                    trackingFinger.SecondJoint = Mathf.Repeat(GetFingerSignedAngle(trackingFinger.Dip, trackingFinger.Mcp, trackingFinger.Pip, (trackingFinger.Mcp - trackingFinger.Pip).normalized), 360f);
                    trackingFinger.FirstJoint = Mathf.Repeat(GetFingerSignedAngle(trackingFinger.Tip, trackingFinger.Pip, trackingFinger.Dip, (trackingFinger.Pip - trackingFinger.Dip).normalized), 360f);
                }

                trackingFinger.Active = true;
            }
            catch (Exception ex)
            {
                Log(DebugerLogType.Info, "CheckedBendFinger", $"{ex.Message}");
                trackingFinger.Active = false;
            }
        }

        protected Vector3 TransformToLocal(Vector3 point, TrackingHand trackingHand)
        {
            _tmpFingerLocalPos = point - trackingHand.WristPos;

            return new Vector3(Vector3.Dot(_tmpFingerLocalPos, trackingHand.HandBasisX),
                               Vector3.Dot(_tmpFingerLocalPos, trackingHand.HandBasisY),
                               Vector3.Dot(_tmpFingerLocalPos, trackingHand.HandBasisZ));
        }

        protected float GetFingerSignedAngle(Vector3 target, Vector3 anchor, Vector3 middle, Vector3 referenceAxis)
        {
            try
            {
                return Vector3.SignedAngle((middle - anchor), (target - middle), referenceAxis);
            }
            catch (Exception ex)
            {
                Log(DebugerLogType.Info, "GetFingerSignedAngle", $"{ex.Message}");
            }
            return 0f;
        }

        #endregion
    }
}
