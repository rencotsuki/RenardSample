using System;
using UnityEngine;

namespace SignageHADO.UI
{
    [Serializable]
    [DisallowMultipleComponent]
#if UNITY_EDITOR
    [ExecuteInEditMode]
#endif
    public class UI3DFrame : MonoBehaviourCustom
    {
        [SerializeField] private Camera targetCam = default;
        [SerializeField] private Transform _root = null;
        protected Transform root => _root != null ? _root : transform;

        public float CameraDistance = 0.5f;
        public Vector2 TargetPixel = new Vector2(1920, 1080);
        public int PixelsPerUnit = 100;

        private Vector3 camPosition => targetCam != null ? targetCam.transform.position : Vector3.zero;
        private Vector3 camForward => targetCam != null ? targetCam.transform.forward : Vector3.forward;
        private Vector3 camUp => targetCam != null ? targetCam.transform.up : Vector3.up;
        private Rect camRect => targetCam != null ? targetCam.rect : new Rect(0f, 0f, 1f, 1f);
        private float nearClipPlane => targetCam != null ? targetCam.nearClipPlane : 0.1f;
        private float fieldOfView => targetCam != null ? targetCam.fieldOfView : 60f;
        private Vector2 scaleSize => new Vector2(PixelsPerUnit <= 0 ? 0f : TargetPixel.x / PixelsPerUnit,
                                                 PixelsPerUnit <= 0 ? 0f : TargetPixel.y / PixelsPerUnit);
        private float adjustedPixelsScale => (TargetPixel.x <= 0 || TargetPixel.y <= 0) ? 1f : TargetPixel.x / TargetPixel.y;

        private float _resizeScele = 0f;
        private float _obliqueSideLength = 0f;
        public Vector3 ResizeScele => OnUpdateResize(nearClipPlane, fieldOfView, camRect);

        public void SetCamera(Camera cam)
        {
            targetCam = cam;
        }

        public void SetParameter(float distance, Vector2 pixel, int perUnit)
        {
            CameraDistance = distance;
            TargetPixel = pixel;
            PixelsPerUnit = perUnit;
        }

        public void SetRoot(Transform root)
        {
            _root = root;
        }

        private void Update()
        {
            OnUpdateScale();
        }

        private void OnUpdateScale()
        {
            if (root == null)
                return;

            root.SetPositionAndRotation(camPosition + (camForward * (nearClipPlane + CameraDistance)),
                                        Quaternion.LookRotation((root.position - camPosition), camUp));
            root.localScale = OnUpdateResize((nearClipPlane + CameraDistance), fieldOfView, camRect);
        }

        private float GetResizeScele(float distance, float fieldOfView)
        {
            _obliqueSideLength = distance * Mathf.Tan((fieldOfView * 0.5f) * Mathf.Deg2Rad);
            return _obliqueSideLength <= 0 ? 1f : (scaleSize.x <= 0 ? 0f : (_obliqueSideLength * 2f) / scaleSize.x) * adjustedPixelsScale;
        }

        private Vector3 OnUpdateResize(float distance, float fieldOfView, Rect viewportRect)
        {
            _resizeScele = GetResizeScele(distance, fieldOfView);
            return new Vector3(viewportRect.height <= 0 ? _resizeScele : _resizeScele / viewportRect.height,
                               viewportRect.width <= 0 ? _resizeScele : _resizeScele / viewportRect.width,
                               viewportRect.height <= 0 ? _resizeScele : _resizeScele / viewportRect.height);
        }

#if UNITY_EDITOR

        [SerializeField] private bool _drawGizmos = true;

        private Vector3 _drawScaleSize = Vector3.one;
        private Vector3[] _cubePoint = new Vector3[0];

        private void OnDrawGizmos()
        {
            if (!_drawGizmos || root == null)
                return;

            _drawScaleSize = OnUpdateResize((nearClipPlane + CameraDistance), fieldOfView, camRect);

            Gizmos.color = Application.isPlaying ? Color.yellow : Color.red;
            DrawWireCubeCustom(root.position, root.right, root.up, new Vector2(scaleSize.x * _drawScaleSize.x, scaleSize.y * _drawScaleSize.y));
        }

        private Vector3[] DrawWireCubeCustom(Vector3 center, Vector3 right, Vector3 up, Vector3 size)
        {
            _cubePoint = new Vector3[]
                {
                    center - (right * (size.x * 0.5f)) + (up * (size.y * 0.5f)),    // 左上
                    center + (right * (size.x * 0.5f)) + (up * (size.y * 0.5f)),    // 右上
                    center + (right * (size.x * 0.5f)) - (up * (size.y * 0.5f)),    // 右下
                    center - (right * (size.x * 0.5f)) - (up * (size.y * 0.5f))     // 左下
                };

            Gizmos.DrawLine(_cubePoint[0], _cubePoint[1]);    // 上
            Gizmos.DrawLine(_cubePoint[1], _cubePoint[2]);    // 右
            Gizmos.DrawLine(_cubePoint[2], _cubePoint[3]);    // 下
            Gizmos.DrawLine(_cubePoint[3], _cubePoint[0]);    // 左

            return _cubePoint;
        }

#endif
    }
}
