/*
 * Mediapipe - Solutionの改修
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using Mediapipe;
using Mediapipe.Tasks.Vision.ObjectDetector;
using Mediapipe.Unity;
using Mediapipe.Unity.Sample;
using Mediapipe.Unity.Sample.Holistic;

using TextureFramePool = Mediapipe.Unity.Experimental.TextureFramePool;
using TextureFrame = Mediapipe.Unity.Experimental.TextureFrame;
using ObjectDetectionResult = Mediapipe.Tasks.Components.Containers.DetectionResult;
using Tasks = Mediapipe.Tasks;

namespace SignageHADO.Tracking
{
    using Hand = MediapipeLandmark.Hand;

    public class MediapipeSolution : MediapipeLegacySolutionRunner<HolisticTrackingGraph, ObjectDetector>
    {
        [SerializeField] private RectTransform _worldAnnotationArea = null;
        [SerializeField] private MediapipeLandmarkListAnnotationController _annotationController = null;
        [SerializeField] private bool _enabledDetection = true;
        [SerializeField] private MediapipeDetectionResultAnnotationController _detectionAnnotationController = null;
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

        [Header("ObjectDetection設定")]
        [SerializeField] private MediapipeObjectDetectionConfig config = new MediapipeObjectDetectionConfig();
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

                if (_annotationController != null)
                    _annotationController.isMirrored = _isMirrored;

                if (_detectionAnnotationController != null)
                    _detectionAnnotationController.isMirrored = _isMirrored;
            }
        }

        protected bool isMobile
        {
            get
            {
#if !UNITY_EDITOR
                if (Application.platform == RuntimePlatform.IPhonePlayer ||
                    Application.platform == RuntimePlatform.Android)
                {
                    return true;
                }
#endif
                return false;
            }
        }

        public HolisticTrackingGraph.ModelComplexity modelComplexity
        {
            get => graphRunner.modelComplexity;
            set => graphRunner.modelComplexity = value;
        }

        public bool EnabledDetection
        {
            get => _enabledDetection;
            set => _enabledDetection = value;
        }

        public bool smoothLandmarks
        {
            get => graphRunner.smoothLandmarks;
            set => graphRunner.smoothLandmarks = value;
        }

        public bool refineFaceLandmarks
        {
            get => graphRunner.refineFaceLandmarks;
            set => graphRunner.refineFaceLandmarks = value;
        }

        public bool enableSegmentation
        {
            get => graphRunner.enableSegmentation;
            set => graphRunner.enableSegmentation = value;
        }

        public bool smoothSegmentation
        {
            get => graphRunner.smoothSegmentation;
            set => graphRunner.smoothSegmentation = value;
        }

        public float minDetectionConfidence
        {
            get => graphRunner.minDetectionConfidence;
            set => graphRunner.minDetectionConfidence = value;
        }

        public float minTrackingConfidence
        {
            get => graphRunner.minTrackingConfidence;
            set => graphRunner.minTrackingConfidence = value;
        }

        protected int handLandmarkes => MediapipeLandmark.HandLandmarkes;

        public Vector2 TrakincViewSize { get; private set; } = Vector2.zero;

        public int DetectionTargetCount => _detectionAnnotationController != null ? _detectionAnnotationController.TargetCount : 0;

        protected IReadOnlyList<NormalizedLandmark> currentFaceLandmarkList => _annotationController != null ? _annotationController.CurrentFaceLandmarkList : null;
        protected IReadOnlyList<NormalizedLandmark> currentLeftHandLandmarkList => _annotationController != null ? _annotationController.CurrentLeftHandLandmarkList : null;
        protected IReadOnlyList<NormalizedLandmark> currentRightHandLandmarkList => _annotationController != null ? _annotationController.CurrentRightHandLandmarkList : null;

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

        [HideInInspector] private Packet<NormalizedLandmarkList> _packet = null;
        [HideInInspector] private NormalizedLandmarkList _valueLandmarkList = null;

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
            if (_annotationController != null)
            {
                IsMirrored = _annotationController.isMirrored;
                _annotationController.BasiseInterpupillary = basiseInterpupillary;
            }

            _detectionAnnotationController?.SetTargetCategory(detectionTargetCategory);

            _fingerFacePos = new KalmanFilterVector3(processNoiseFacePos, measurementNoiseFacePos, noiseMaxRangeFacePos, IsDebugLog);
            _fingerFaceRot = new KalmanFilterQuaternion(processNoiseFaceRot, measurementNoiseFaceRot, noiseMaxRangeFaceRot, IsDebugLog);

            _fingerHandPos = new KalmanFilterVector3(processNoiseHand, measurementNoiseHand, noiseMaxRangeHand, IsDebugLog);
            _fingerFilter = new KalmanFilterFloat(processNoiseFinger, measurementNoiseFinger, noiseMaxRangeFinger, IsDebugLog);

            if (graphRunner != null)
            {
                if (Application.platform == RuntimePlatform.IPhonePlayer ||
                    Application.platform == RuntimePlatform.Android)
                {
                    graphRunner.modelComplexity = HolisticTrackingGraph.ModelComplexity.Lite;
                }
                else
                {
                    graphRunner.modelComplexity = HolisticTrackingGraph.ModelComplexity.Full;
                }
            }
        }

        protected override IEnumerator Run()
        {
            Log(DebugerLogType.Info, "Run",
                $"Delegate = {config.Delegate}\n\r" +
                $"Model = {config.ModelName}\n\r" +
                $"Running Mode = {config.RunningMode}\n\r" +
                $"Score Threshold = {config.ScoreThreshold}\n\r" +
                $"Max Results = {config.MaxResults}");

            yield return AssetLoader.PrepareAssetAsync(config.ModelPath);

            var options = config.GetObjectDetectorOptions(config.RunningMode == Tasks.Vision.Core.RunningMode.LIVE_STREAM ? OnObjectDetectionsOutput : null);
            taskApi = ObjectDetector.CreateFromOptions(options, GpuManager.GpuResources);

            var graphInitRequest = graphRunner.WaitForInit(runningMode);
            var imageSource = ImageSourceProvider.ImageSource;

            yield return imageSource.Play();

            if (!imageSource.isPrepared)
            {
                Log(DebugerLogType.Info, "Run", "Failed to start ImageSource, exiting...");
                yield break;
            }

            _textureFramePool = new TextureFramePool(imageSource.textureWidth, imageSource.textureHeight, TextureFormat.RGBA32, 10);

            screen.Initialize(imageSource);
            _worldAnnotationArea.localEulerAngles = imageSource.rotation.Reverse().GetEulerAngles();

            yield return graphInitRequest;
            if (graphInitRequest.isError)
            {
                Log(DebugerLogType.Info, "Run", $"{graphInitRequest.error.Message}");
                yield break;
            }

            if (!runningMode.IsSynchronous())
            {
                graphRunner.OnFaceLandmarksOutput += OnFaceLandmarksOutput;
                graphRunner.OnLeftHandLandmarksOutput += OnLeftHandLandmarksOutput;
                graphRunner.OnRightHandLandmarksOutput += OnRightHandLandmarksOutput;
            }

            SetupAnnotationController(_annotationController, imageSource);
            SetupAnnotationControllerTask(_detectionAnnotationController, imageSource);

            graphRunner.StartRun(imageSource);

            AsyncGPUReadbackRequest req = default;
            var waitUntilReqDone = new WaitUntil(() => req.done);

            var canUseGpuImage = graphRunner.configType == GraphRunner.ConfigType.OpenGLES && GpuManager.GpuResources != null;
            using var glContext = canUseGpuImage ? GpuManager.GetGlContext() : null;

            var transformationOptions = imageSource.GetTransformationOptions(IsMirrored);
            var flipHorizontally = transformationOptions.flipHorizontally;
            var flipVertically = transformationOptions.flipVertically;
            var imageProcessingOptions = new Tasks.Vision.Core.ImageProcessingOptions(rotationDegrees: (int)transformationOptions.rotationAngle);

            var resultSub = ObjectDetectionResult.Alloc(Math.Max(options.maxResults ?? 0, 0));
            Image buildGpuimage = null;
            System.Threading.Tasks.Task<HolisticTrackingResult> task = null;
            HolisticTrackingResult resultTask = default;

            var rectScreen = screen.GetComponent<RectTransform>();
            var tryGetTextureFrame = false;
            var tryGetTextureFrameSub = false;
            TextureFrame textureFrame = null;
            TextureFrame textureFrameSub = null;

            while (coroutine != null)
            {
                if (isPaused)
                {
                    yield return new WaitWhile(() => isPaused);
                }

                tryGetTextureFrame = _textureFramePool.TryGetTextureFrame(out textureFrame);
                tryGetTextureFrameSub = EnabledDetection ? _textureFramePool.TryGetTextureFrame(out textureFrameSub) : true;

                if (!tryGetTextureFrame || !tryGetTextureFrameSub)
                {
                    yield return new WaitForEndOfFrame();
                    continue;
                }

                if (canUseGpuImage)
                {
                    yield return new WaitForEndOfFrame();
                    textureFrame.ReadTextureOnGPU(imageSource.GetCurrentTexture(), false, isMobile);

                    if (EnabledDetection)
                    {
                        textureFrameSub.ReadTextureOnGPU(imageSource.GetCurrentTexture(), flipHorizontally, flipVertically);
                        buildGpuimage = textureFrameSub.BuildGpuImage(glContext);
                    }
                }
                else
                {
                    req = textureFrame.ReadTextureAsync(imageSource.GetCurrentTexture(), false, isMobile);
                    yield return waitUntilReqDone;

                    if (req.hasError)
                    {
                        Log(DebugerLogType.Info, "Run", "Failed to read texture from the image source, exiting...");
                        break;
                    }

                    textureFrame.Release();

                    if (EnabledDetection)
                    {
                        req = textureFrameSub.ReadTextureAsync(imageSource.GetCurrentTexture(), flipHorizontally, flipVertically);
                        yield return waitUntilReqDone;

                        if (req.hasError)
                        {
                            Log(DebugerLogType.Info, "Run", "Failed to read texture from the image source, exiting...");
                            break;
                        }

                        buildGpuimage = textureFrameSub.BuildCPUImage();
                        textureFrameSub.Release();
                    }
                }

                graphRunner.AddTextureFrameToInputStream(textureFrame, glContext);

                if (rectScreen != null && TrakincViewSize != rectScreen.sizeDelta)
                    TrakincViewSize = rectScreen.sizeDelta;

                if (runningMode.IsSynchronous())
                {
                    screen.ReadSync(textureFrame);

                    task = graphRunner.WaitNextAsync();
                    yield return new WaitUntil(() => task.IsCompleted);

                    resultTask = task.Result;
                    _annotationController?.UpdateNow(resultTask.faceLandmarks, resultTask.leftHandLandmarks, resultTask.rightHandLandmarks);

                    resultTask.segmentationMask?.Dispose();
                }

                if (EnabledDetection)
                {
                    try
                    {
                        switch (taskApi.runningMode)
                        {
                            case Tasks.Vision.Core.RunningMode.IMAGE:
                                if (taskApi.TryDetect(buildGpuimage, imageProcessingOptions, ref resultSub))
                                {
                                    _detectionAnnotationController?.DrawNow(resultSub);
                                }
                                else
                                {
                                    _detectionAnnotationController?.DrawNow(default);
                                }
                                break;

                            case Tasks.Vision.Core.RunningMode.VIDEO:
                                if (taskApi.TryDetectForVideo(buildGpuimage, GetCurrentTimestampMillisec(), imageProcessingOptions, ref resultSub))
                                {
                                    _detectionAnnotationController?.DrawNow(resultSub);
                                }
                                else
                                {
                                    _detectionAnnotationController?.DrawNow(default);
                                }
                                break;

                            case Tasks.Vision.Core.RunningMode.LIVE_STREAM:
                                taskApi.DetectAsync(buildGpuimage, GetCurrentTimestampMillisec(), imageProcessingOptions);
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"{ex.Message}");
                    }
                }
            }
        }

        private void OnObjectDetectionsOutput(ObjectDetectionResult result, Image image, long timestamp)
        {
            _detectionAnnotationController?.DrawLater(result);
        }

        private void OnFaceLandmarksOutput(object stream, OutputStream<NormalizedLandmarkList>.OutputEventArgs eventArgs)
        {
            try
            {
                _packet = eventArgs.packet;
                _valueLandmarkList = _packet == null ? default : _packet.Get(NormalizedLandmarkList.Parser);
                _annotationController?.UpdateFaceLandmarkListLater(_valueLandmarkList);
            }
            catch (Exception ex)
            {
                Log(DebugerLogType.Info, "OnFaceLandmarksOutput", $"{ex.Message}");
            }
        }

        private void OnLeftHandLandmarksOutput(object stream, OutputStream<NormalizedLandmarkList>.OutputEventArgs eventArgs)
        {
            try
            {
                _packet = eventArgs.packet;
                _valueLandmarkList = _packet == null ? default : _packet.Get(NormalizedLandmarkList.Parser);
                _annotationController?.UpdateLeftHandLandmarkListLater(_valueLandmarkList);
            }
            catch (Exception ex)
            {
                Log(DebugerLogType.Info, "OnLeftHandLandmarksOutput", $"{ex.Message}");
            }
        }

        private void OnRightHandLandmarksOutput(object stream, OutputStream<NormalizedLandmarkList>.OutputEventArgs eventArgs)
        {
            try
            {
                _packet = eventArgs.packet;
                _valueLandmarkList = _packet == null ? default : _packet.Get(NormalizedLandmarkList.Parser);
                _annotationController?.UpdateRightHandLandmarkListLater(_valueLandmarkList);
            }
            catch (Exception ex)
            {
                Log(DebugerLogType.Info, "OnRightHandLandmarksOutput", $"{ex.Message}");
            }
        }

        #region API

        public bool GetFaceTracking(out Vector3 headWorldPos, out Quaternion headRotation, out float headScale)
        {
            headWorldPos = _face.HeadWorldPos;
            headRotation = _face.HeadRotation;
            headScale = _face.HeadScale;

            try
            {
                if (_annotationController.IsStale && _annotationController.ActiveFaceLandmark)
                {
                    if (_annotationController.GetFaceLandmarkPos(out headWorldPos, out headScale, out _faceNoseWorldPos, out _faceLeftEdgeWorldPos, out _faceRightEdgeWorldPos))
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
                if (_annotationController.IsStale && _annotationController.ActiveLeftHandLandmark)
                {
                    if (_annotationController.GetLeftHandLandmarkPos(out handWorldPos))
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

                    _leftHand.Active = GetHandGesture(currentLeftHandLandmarkList, ref _leftHand, false);
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
                if (_annotationController.IsStale && _annotationController.ActiveRightHandLandmark)
                {
                    if (_annotationController.GetRightHandLandmarkPos(out handWorldPos))
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

                    _rightHand.Active = GetHandGesture(currentRightHandLandmarkList, ref _rightHand, true);
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

                trackingHand.WristPos.x = list[(int)Hand.Wrist].X;
                trackingHand.WristPos.y = list[(int)Hand.Wrist].Y;
                trackingHand.WristPos.z = list[(int)Hand.Wrist].Z;

                trackingHand.ThumbCMCPos.x = list[(int)Hand.ThumbCmcIndex].X;
                trackingHand.ThumbCMCPos.y = list[(int)Hand.ThumbCmcIndex].Y;
                trackingHand.ThumbCMCPos.z = list[(int)Hand.ThumbCmcIndex].Z;

                trackingHand.IndexMCPPos.x = list[(int)Hand.IndexFingerMcpIndex].X;
                trackingHand.IndexMCPPos.y = list[(int)Hand.IndexFingerMcpIndex].Y;
                trackingHand.IndexMCPPos.z = list[(int)Hand.IndexFingerMcpIndex].Z;

                trackingHand.PinkyMCPPos.x = list[(int)Hand.PrinkyMcpIndex].X;
                trackingHand.PinkyMCPPos.y = list[(int)Hand.PrinkyMcpIndex].Y;
                trackingHand.PinkyMCPPos.z = list[(int)Hand.PrinkyMcpIndex].Z;

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
                trackingFinger.Mcp.x = list[headIndex].X;
                trackingFinger.Mcp.y = list[headIndex].Y;
                trackingFinger.Mcp.z = list[headIndex].Z;

                trackingFinger.Mcp = TransformToLocal(trackingFinger.Mcp, trackingHand);

                trackingFinger.Pip.x = list[headIndex + 1].X;
                trackingFinger.Pip.y = list[headIndex + 1].Y;
                trackingFinger.Pip.z = list[headIndex + 1].Z;

                trackingFinger.Pip = TransformToLocal(trackingFinger.Pip, trackingHand);

                trackingFinger.Dip.x = list[headIndex + 2].X;
                trackingFinger.Dip.y = list[headIndex + 2].Y;
                trackingFinger.Dip.z = list[headIndex + 2].Z;

                trackingFinger.Dip = TransformToLocal(trackingFinger.Dip, trackingHand);

                trackingFinger.Tip.x = list[headIndex + 3].X;
                trackingFinger.Tip.y = list[headIndex + 3].Y;
                trackingFinger.Tip.z = list[headIndex + 3].Z;

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
