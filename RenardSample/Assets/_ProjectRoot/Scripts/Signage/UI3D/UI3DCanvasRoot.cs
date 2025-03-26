using UnityEngine;

namespace SignageHADO.UI
{
    [DisallowMultipleComponent]
    public sealed class UI3DCanvasRoot : MonoBehaviourCustom
    {
        [SerializeField] private bool _active = false;
        [SerializeField] private Canvas _rootCanvas = default;
        [SerializeField] private float _cameraDistance = 0.5f;

        private Camera _targetCam = null;
        private int _sortingOrder = 0;
        private UI3DCanvasScaler _canvasScaler = null;
        private float pixelsPerUnit => _canvasScaler != null ? _canvasScaler.PixelsPerUnit : 100f;
        private Vector2 targetPixel => _canvasScaler != null ? _canvasScaler.ResolutionSize :  new Vector2(1920f, 1080f);

        private Vector2 _scaleSize
        {
            get
            {
                return new Vector2(pixelsPerUnit <= 0 ? 0f : targetPixel.x / pixelsPerUnit,
                                   pixelsPerUnit <= 0 ? 0f : targetPixel.y / pixelsPerUnit);
            }
        }

        private float _adjustedPixelsScale
        {
            get
            {
                return (targetPixel.x <= 0 || targetPixel.y <= 0) ? 1f : (float)targetPixel.x / (float)targetPixel.y;
            }
        }

        [HideInInspector] private RenderMode _backupRenderMode = RenderMode.ScreenSpaceOverlay;
        [HideInInspector] private Camera _backupWorldCamera = null;
        [HideInInspector] private int _backupSortingOrder = 0;
        [HideInInspector] private Vector3 _backupPosition = Vector3.zero;
        [HideInInspector] private Quaternion _backupRotation = Quaternion.identity;
        [HideInInspector] private Vector3 _backupLocalScale = Vector3.zero;

        private void Awake()
        {
            if (_rootCanvas != null)
            {
                _backupRenderMode = _rootCanvas.renderMode;
                _backupWorldCamera = _rootCanvas.worldCamera;
                _backupSortingOrder = _rootCanvas.sortingOrder;
                _backupPosition = _rootCanvas.transform.position;
                _backupRotation = _rootCanvas.transform.rotation;
                _backupLocalScale = _rootCanvas.transform.localScale;
            }
        }

        private void Start()
        {
            _canvasScaler = _rootCanvas?.GetComponentInChildren<UI3DCanvasScaler>();
        }

        private void ResetBackup()
        {
            if (_rootCanvas == null)
                return;

            _rootCanvas.renderMode = _backupRenderMode;
            _rootCanvas.worldCamera = _backupWorldCamera;
            _rootCanvas.sortingOrder = _backupSortingOrder;

            _rootCanvas.transform.position = _backupPosition;
            _rootCanvas.transform.rotation = _backupRotation;
            _rootCanvas.transform.localScale = _backupLocalScale;
        }

        public UI3DCanvasRoot SetActive(bool value)
        {
            _active = value;
            if (!_active) ResetBackup();
            return this;
        }

        public UI3DCanvasRoot SetCamera(Camera cam) => SetCamera(cam, _backupSortingOrder);
        public UI3DCanvasRoot SetCamera(Camera cam, int sortingOrder)
        {
            _targetCam = cam;
            _sortingOrder = sortingOrder;
            return this;
        }

        private void Update() => OnUpdate();

        private void OnUpdate()
        {
            if (_targetCam == null || _rootCanvas == null)
                return;

            if (!_active)
                return;

            if (_rootCanvas.renderMode != RenderMode.WorldSpace)
            {
                _rootCanvas.renderMode = RenderMode.WorldSpace;
                _rootCanvas.worldCamera = _targetCam;
                _rootCanvas.sortingOrder = _sortingOrder;
            }

            _rootCanvas.transform.position = _targetCam.transform.position + (_targetCam.transform.forward * (_targetCam.nearClipPlane + _cameraDistance));
            _rootCanvas.transform.rotation = Quaternion.LookRotation((_rootCanvas.transform.position - _targetCam.transform.position), _targetCam.transform.up);
            _rootCanvas.transform.localScale = OnUpdateResize((_targetCam.nearClipPlane + _cameraDistance), _targetCam.fieldOfView, _targetCam.rect);
        }

        private float GetResizeScele(float distance, float fieldOfView)
        {
            var obliqueSideLength = distance * Mathf.Tan((fieldOfView * 0.5f) * Mathf.Deg2Rad);
            return obliqueSideLength <= 0 ? 1f : ((obliqueSideLength * 2f) / _scaleSize.x) * _adjustedPixelsScale;
        }

        private Vector3 OnUpdateResize(float distance, float fieldOfView, Rect viewportRect)
        {
            var resizeScele = GetResizeScele(distance, fieldOfView);
            return new Vector3(viewportRect.height <= 0 ? resizeScele : resizeScele / viewportRect.height,
                               viewportRect.width <= 0 ? resizeScele : resizeScele / viewportRect.width,
                               viewportRect.height <= 0 ? resizeScele : resizeScele / viewportRect.height);
        }

#if UNITY_EDITOR

        [SerializeField] private bool drawGizmos = false;

        private void OnValidate() => OnUpdate();

        private void OnDrawGizmos()
        {
            if (_rootCanvas == null || !drawGizmos)
                return;

            if (_canvasScaler == null)
                _canvasScaler = _rootCanvas?.GetComponentInChildren<UI3DCanvasScaler>();

            if (_targetCam != null)
            {
                _rootCanvas.transform.localScale = OnUpdateResize((_targetCam.nearClipPlane + _cameraDistance), _targetCam.fieldOfView, new Rect(0f, 0f, 1f, 1f));
            }
            else
            {
                _rootCanvas.transform.localScale = Vector3.one;
            }

            Gizmos.color = Application.isPlaying ? Color.yellow : Color.red;

            DrawWireCubeCustom(
                _rootCanvas.transform.position,
                _rootCanvas.transform.right,
                _rootCanvas.transform.up,
                new Vector2(_scaleSize.x * _rootCanvas.transform.localScale.x, _scaleSize.y * _rootCanvas.transform.localScale.y));
        }

        private Vector3[] DrawWireCubeCustom(Vector3 center, Vector3 right, Vector3 up, Vector3 size)
        {
            var cubePoint = new Vector3[]
                {
                    center - (right * (size.x * 0.5f)) + (up * (size.y * 0.5f)),    // 左上
                    center + (right * (size.x * 0.5f)) + (up * (size.y * 0.5f)),    // 右上
                    center + (right * (size.x * 0.5f)) - (up * (size.y * 0.5f)),    // 右下
                    center - (right * (size.x * 0.5f)) - (up * (size.y * 0.5f))     // 左下
                };

            Gizmos.DrawLine(cubePoint[0], cubePoint[1]);    // 上
            Gizmos.DrawLine(cubePoint[1], cubePoint[2]);    // 右
            Gizmos.DrawLine(cubePoint[2], cubePoint[3]);    // 下
            Gizmos.DrawLine(cubePoint[3], cubePoint[0]);    // 左

            return cubePoint;
        }

#endif
    }
}
