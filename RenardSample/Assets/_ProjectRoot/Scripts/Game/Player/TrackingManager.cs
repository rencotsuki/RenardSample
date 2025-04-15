using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using SignageHADO;
using SignageHADO.Tracking;

public static class TrackingHandler
{
    private static TrackingManager manager => TrackingManager.Singleton;

    public static bool Active => manager != null ? manager.Active : false;

    public static bool EnabledDetection => manager != null ? manager.EnabledDetection : false;

    public static Vector2 DefaultViewPos => Vector2.one * 0.5f;

    public static int DetectionTargetCount => manager != null ? manager.DetectionTargetCount : 0;

    public static bool IsHeadTracking => manager != null ? manager.IsHeadTracking : false;
    public static Vector3 HeadLocalAngles => manager != null ? manager.HeadLocalAngles : Vector3.zero;

    public static bool IsLeftHandTracking => manager != null ? manager.IsLeftHandTracking : false;
    public static Vector2 LeftHandViewPos => manager != null ? manager.LeftHandViewPos : DefaultViewPos;
    public static HandPoseEnum LeftHandPose => manager != null ? manager.LeftHandPose : HandPoseEnum.Unknown;

    public static bool IsRightHandTracking => manager != null ? manager.IsRightHandTracking : false;
    public static Vector2 RightHandViewPos => manager != null ? manager.RightHandViewPos : DefaultViewPos;
    public static HandPoseEnum RightHandPose => manager != null ? manager.RightHandPose : HandPoseEnum.Unknown;

    private static DeviceRotation deviceRotation => manager != null ? manager.DeviceRotation : null;
    public static bool CameraDeviceGyroActive => deviceRotation != null ? deviceRotation.GyroActive : false;
    public static Vector3 CameraDeviceEulerAngles => deviceRotation != null ? deviceRotation.EulerAngles : Vector3.zero;

    public static void SetGyroActive(bool value) => deviceRotation?.SetGyroActive(value);

    public static void SetEulerAngles(Vector3 value) => deviceRotation?.SetEulerAngles(value);
}

public enum HandPoseEnum : int
{
    Unknown = 0,

    /// <summary>グー</summary>
    Rock,
    /// <summary>チョキ</summary>
    Scissors,
    /// <summary>パー</summary>
    Paper
}

namespace SignageHADO.Tracking
{
    public class TrackingManager : SingletonMonoBehaviourCustom<TrackingManager>
    {
        [SerializeField] private MediapipeRunner mediapipeRunner = null;
        [SerializeField] private DeviceRotation deviceRotation = null;
        [Header("ﾄﾗｯｷﾝｸﾞﾛｽﾄ判定時間[s]")]
        [SerializeField, Range(0f, 120f)] private float lostTrackingTimeout = 0.5f;
        [Header("基準から１とする距離[cm]")]
        [SerializeField] private Vector2 basiseDistanceSize = new Vector2(40f, 70f);
        [Header("基準位置からのOffset[cm]")]
        [SerializeField] private Vector2 landmarkAnchorOffset = new Vector2(0, -15f);
        [Header("ﾌｨﾙﾀｰ設定")]
        [SerializeField] private bool filterHand = true;
        [SerializeField, Range(0f, 1f), Tooltip(EMAFilter.EMAFilterAlphaToolTip)] private float emaFilterAlpha = 0.2f;

        public bool Active => mediapipeRunner != null ? mediapipeRunner.Active : false;

        public bool EnabledDetection => mediapipeRunner != null ? mediapipeRunner.EnabledDetection : false;

        protected bool reverseTracking
        {
            get => mediapipeRunner != null ? mediapipeRunner.ReverseTracking : false;
            set
            {
                if (mediapipeRunner != null)
                    mediapipeRunner.ReverseTracking = value;
            }
        }

        protected bool isMobile { get; private set; } = false;

        #region TrackingStatus

        [Serializable]
        protected class TrackingStatus
        {
            public bool Active { get; private set; } = false;

            private float _nonStatusBegin = 0f;

            public void Reset()
            {
                if (!Active) return;

                Active = false;
                _nonStatusBegin = 0f;
            }

            public void Update(float timeoutSeconds, Func<bool> func)
            {
                if (func != null)
                {
                    if (func())
                    {
                        Active = true;
                        _nonStatusBegin = 0f;
                        return;
                    }

                    if (Active && timeoutSeconds > 0)
                    {
                        if (_nonStatusBegin <= 0)
                        {
                            _nonStatusBegin = Time.realtimeSinceStartup;
                            return;
                        }
                        else if (Time.realtimeSinceStartup - _nonStatusBegin < timeoutSeconds)
                        {
                            return;
                        }
                    }
                }

                Reset();
            }
        }

        private TrackingStatus headTrackingStatus = new TrackingStatus();
        public bool IsHeadTracking => Active && headTrackingStatus != null ? headTrackingStatus.Active : false;

        private TrackingStatus leftHandTrackingStatus = new TrackingStatus();
        protected bool isLeftHandTracking => Active && leftHandTrackingStatus != null ? leftHandTrackingStatus.Active : false;
        public bool IsLeftHandTracking => isLeftHandTracking;

        private TrackingStatus rightHandTrackingStatus = new TrackingStatus();
        protected bool isRightHandTracking => Active && rightHandTrackingStatus != null ? rightHandTrackingStatus.Active : false;
        public bool IsRightHandTracking => isRightHandTracking;

        #endregion

        protected Camera trackingCamera => Camera.main;

        public DeviceRotation DeviceRotation => deviceRotation;

        private Vector3 _cameraDeviceEulerAngles = Vector3.zero;
        protected Vector3 cameraDeviceEulerAngles
        {
            get
            {
                if (deviceRotation != null)
                {
                    if (isMobile)
                    {
                        // モバイル端末はPitch以外を使わない
                        _cameraDeviceEulerAngles.x = deviceRotation.Pitch;
                        _cameraDeviceEulerAngles.y = 0f;
                        _cameraDeviceEulerAngles.z = 0f;
                    }
                    else
                    {
                        _cameraDeviceEulerAngles = deviceRotation.EulerAngles;
                    }
                    return _cameraDeviceEulerAngles;
                }
                return Vector3.zero;
            }
        }

        protected static Vector2 defaultViewPos => TrackingHandler.DefaultViewPos;

        public int DetectionTargetCount => mediapipeRunner != null ? mediapipeRunner.DetectionTargetCount : 0;

        public Vector3 HeadLocalAngles => _face.HeadAngles;

        public Vector2 LeftHandViewPos => _leftHand.ViewPos;
        public HandPoseEnum LeftHandPose => _leftHand.Pose;

        public Vector2 RightHandViewPos => _rightHand.ViewPos;
        public HandPoseEnum RightHandPose => _rightHand.Pose;

        [Serializable]
        protected struct FaceTracking
        {
            public Vector3 HeadAngles;
            public float HeadScale;

            public Vector3 OriginWorldPos;
            public Vector3 FilterWorldPos;
            public Quaternion OriginLocalRotation;
            public Vector3 FilterHeadEulerAngles;

            public Vector3 ErrorCovariancePos;
            public Vector3 ErrorCovarianceEulerAngles;

            public void Clear()
            {
                HeadAngles = Vector3.zero;
                HeadScale = 0f;

                OriginWorldPos = Vector3.zero;
                FilterWorldPos = Vector3.zero;
                OriginLocalRotation = Quaternion.identity;
                FilterHeadEulerAngles = Vector3.zero;

                ErrorCovariancePos = Vector3.zero;
                ErrorCovarianceEulerAngles = Vector3.zero;
            }
        }

        [Serializable]
        protected struct HandTracking
        {
            public HandPoseEnum Pose;
            public Vector2 ViewPos;
            public Vector3 OriginWorldPos;
            public Vector3 FilterPos;

            public Vector3 ErrorCovariancePos;

            public void Clear()
            {
                Pose = HandPoseEnum.Unknown;
                ViewPos = defaultViewPos;
                OriginWorldPos = Vector3.zero;
                FilterPos = Vector3.zero;

                ErrorCovariancePos = Vector3.zero;
            }
        }

        private FaceTracking _face = new FaceTracking();
        private HandTracking _leftHand = new HandTracking();
        private HandTracking _rightHand = new HandTracking();

        [HideInInspector] private Quaternion _cameraDeviceRotation = Quaternion.identity;
        [HideInInspector] private Vector3 _cvHeadWorldPoint = Vector3.zero;
        [HideInInspector] private Vector3 _cvHandWorldPoint = Vector3.zero;
        [HideInInspector] private Quaternion _tmpCameraDiffRotation = Quaternion.identity;
        [HideInInspector] private Vector3 _tmpCameraScreenPoint = Vector3.zero;
        [HideInInspector] private Vector3 _tmpLandmarkAnchorPos = Vector3.zero;
        [HideInInspector] private float _tmpHeadScreenScale = 0f;
        [HideInInspector] private Vector2 _tmpHandViewPos = Vector2.zero;

        [HideInInspector] private EMAFilterVector2 _emaFilterHandLeft = null;
        [HideInInspector] private EMAFilterVector2 _emaFilterHandRight = null;

        private CancellationTokenSource _onUpdateToken = null;

        protected override void Initialized()
        {
            base.Initialized();
            IsDebugLog = false;

#if UNITY_EDITOR
            isMobile = false;
#else
            isMobile = (Application.platform == RuntimePlatform.IPhonePlayer ||
                        Application.platform == RuntimePlatform.Android);
#endif

            _emaFilterHandLeft = new EMAFilterVector2(Vector2.one *  emaFilterAlpha);
            _emaFilterHandRight = new EMAFilterVector2(Vector2.one * emaFilterAlpha);
        }

        private void Start()
        {
            OnDisposeUpdate();
            _onUpdateToken = new CancellationTokenSource();
            OnUpdateAsync(_onUpdateToken.Token).Forget();
        }

        protected override void OnDestroy()
        {
            OnDisposeUpdate();
            base.OnDestroy();
        }

        private void OnDisposeUpdate()
        {
            _onUpdateToken?.Dispose();
            _onUpdateToken = null;
        }

        private async UniTask OnUpdateAsync(CancellationToken token)
        {
            // モバイル端末の背面カメラの逆向き対応
            reverseTracking = isMobile;

            ResetTracking();

            while (_onUpdateToken != null)
            {
                if (Active)
                {
                    UpdateAnnotationController();
                }

                await UniTask.Yield(PlayerLoopTiming.Update, token);
                token.ThrowIfCancellationRequested();
            }
        }

        public void ResetTracking()
        {
            headTrackingStatus?.Reset();
            leftHandTrackingStatus?.Reset();
            rightHandTrackingStatus?.Reset();

            _face.Clear();
            _leftHand.Clear();
            _rightHand.Clear();
        }

        private void UpdateAnnotationController()
        {
            if (mediapipeRunner != null)
            {
                headTrackingStatus.Update(lostTrackingTimeout,
                    () => mediapipeRunner.GetFaceTracking(out _face.OriginWorldPos, out _face.OriginLocalRotation, out _face.HeadScale));

                leftHandTrackingStatus.Update(lostTrackingTimeout,
                    () => mediapipeRunner.GetLeftHandGesture(out _leftHand.OriginWorldPos, out _leftHand.Pose));

                rightHandTrackingStatus.Update(lostTrackingTimeout,
                    () => mediapipeRunner.GetRightHandGesture(out _rightHand.OriginWorldPos, out _rightHand.Pose));
            }
            else
            {
                headTrackingStatus.Reset();
                leftHandTrackingStatus.Reset();
                rightHandTrackingStatus.Reset();
            }

            // カメラの傾きを考慮する
            _cameraDeviceRotation.eulerAngles = cameraDeviceEulerAngles;

            UpdateHeadTracking();
            UpdateHandTracking(_face.OriginWorldPos, 1f - _face.HeadScale);
        }

        private void UpdateHeadTracking()
        {
            if (IsHeadTracking)
            {
                _face.HeadAngles.x = Mathf.FloorToInt(_face.OriginLocalRotation.eulerAngles.x);
                _face.HeadAngles.x = _face.HeadAngles.x < 180 ? _face.HeadAngles.x : _face.HeadAngles.x - 360;

                _face.HeadAngles.y = Mathf.FloorToInt(_face.OriginLocalRotation.eulerAngles.y) - 180;

                _face.HeadAngles.z = Mathf.FloorToInt(_face.OriginLocalRotation.eulerAngles.z);
                _face.HeadAngles.z = _face.HeadAngles.z < 180 ? _face.HeadAngles.z : _face.HeadAngles.z - 360;
            }
            else
            {
                _face.HeadAngles = Vector3.zero;
                _face.HeadScale = 0f;
            }

            // モバイル端末の背面カメラの逆向き対応
            if (reverseTracking)
            {
                _face.HeadAngles.x = _face.HeadAngles.x * 1.2f;
                _face.HeadAngles.y = -_face.HeadAngles.y;
            }
        }

        private Vector2 GetHandViewPosition(Vector3 headWorldPos, float headScreenScale, Vector3 worldPos)
        {
            _tmpHandViewPos = defaultViewPos;

            try
            {
                if (trackingCamera != null)
                {
                    if (_cameraDeviceRotation == Quaternion.identity)
                    {
                        _cvHeadWorldPoint = headWorldPos;
                        _cvHandWorldPoint = worldPos;
                    }
                    else
                    {
                        _tmpCameraDiffRotation = Quaternion.Inverse(Quaternion.identity) * _cameraDeviceRotation;

                        _tmpCameraScreenPoint = _tmpCameraDiffRotation * trackingCamera.WorldToScreenPoint(headWorldPos);
                        _cvHeadWorldPoint = trackingCamera.ScreenToWorldPoint(Quaternion.Inverse(_tmpCameraDiffRotation) * _tmpCameraScreenPoint);

                        _tmpCameraScreenPoint = _tmpCameraDiffRotation * trackingCamera.WorldToScreenPoint(worldPos);
                        _cvHandWorldPoint = trackingCamera.ScreenToWorldPoint(Quaternion.Inverse(_tmpCameraDiffRotation) * _tmpCameraScreenPoint);
                    }

                    _tmpHandViewPos.x = basiseDistanceSize.x > 0 ? (_cvHandWorldPoint.x - _cvHeadWorldPoint.x) / (basiseDistanceSize.x * headScreenScale) : 0f;
                    _tmpHandViewPos.y = basiseDistanceSize.y > 0 ? (_cvHandWorldPoint.y - _cvHeadWorldPoint.y) / (basiseDistanceSize.y * headScreenScale) : 0f;
                }
            }
            catch (Exception ex)
            {
                Log(DebugerLogType.Info, "GetHandViewPosition", $"{ex.Message}");
            }
            return _tmpHandViewPos;
        }

        private void UpdateHandTracking(Vector3 headWorldPos, float headScale)
        {
            _tmpLandmarkAnchorPos = headWorldPos + new Vector3(landmarkAnchorOffset.x, landmarkAnchorOffset.y, 0f) * (1f - Mathf.Clamp01(1 - headScale));
            _tmpHeadScreenScale = 1f + Mathf.Clamp01(1 - headScale);

            if (isLeftHandTracking)
            {
                _leftHand.ViewPos = GetHandViewPosition(_tmpLandmarkAnchorPos, _tmpHeadScreenScale, _leftHand.OriginWorldPos);
            }
            else
            {
                _leftHand.ViewPos = defaultViewPos;
                _leftHand.Pose = HandPoseEnum.Unknown;
            }

            if (filterHand)
                _leftHand.ViewPos = _emaFilterHandLeft.Apply(_leftHand.ViewPos);

            if (isRightHandTracking)
            {
                _rightHand.ViewPos = GetHandViewPosition(_tmpLandmarkAnchorPos, _tmpHeadScreenScale, _rightHand.OriginWorldPos);
            }
            else
            {
                _rightHand.ViewPos = defaultViewPos;
                _rightHand.Pose = HandPoseEnum.Unknown;
            }

            if (filterHand)
                _rightHand.ViewPos = _emaFilterHandRight.Apply(_rightHand.ViewPos);

            // モバイル端末の背面カメラの逆向き対応
            if (reverseTracking)
            {
                _leftHand.ViewPos.x = -_leftHand.ViewPos.x;
                _rightHand.ViewPos.x = -_rightHand.ViewPos.x;
            }
        }
    }
}