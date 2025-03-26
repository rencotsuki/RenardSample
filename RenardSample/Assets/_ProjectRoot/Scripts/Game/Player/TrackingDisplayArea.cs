using System;
using UnityEngine;

namespace SignageHADO
{
    [RequireComponent(typeof(RectTransform))]
    public class TrackingDisplayArea : MonoBehaviourCustom
    {
        protected Vector2 displaySize = new Vector2(1080f, 1920f);
        protected Vector3 displayRollOffset = Vector3.zero;

        private RectTransform _rectTransform = null;
        private RectTransform _parentRectTransform = null;
        private BoxCollider _colliderAria = null;

        public RectTransform TrackingRectTransform => _rectTransform;

        [HideInInspector] private float _distance = 0f;
        [HideInInspector] private Rect _rect = default;
        [HideInInspector] private Vector3 _rotatedTopLeftRel = Vector3.zero;
        [HideInInspector] private Vector3 _rotatedTopRightRel = Vector3.zero;
        [HideInInspector] private float _maxHeight = 0f;
        [HideInInspector] private float _ratio = 0f;
        [HideInInspector] private RaycastHit _hitRay = default;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _parentRectTransform = transform.parent.GetComponentInParent<RectTransform>(true);

            _colliderAria = gameObject.AddComponent<BoxCollider>();
            if (_colliderAria != null)
            {
                _colliderAria.isTrigger = true;
                _colliderAria.size = Vector3.zero;
            }
        }

        public void SetDisplayInfo(Vector2 size, Vector3 rollOffset)
        {
            displaySize = size;
            displayRollOffset = rollOffset;

            _rect = default;

            if (_rectTransform != null)
                _rectTransform.sizeDelta = Vector3.zero;
        }

        private void LateUpdate()
        {
            if (_rectTransform == null || _parentRectTransform == null)
                return;

            if (_parentRectTransform.rect != _rect || _rectTransform.sizeDelta == Vector2.zero)
            {
                if (displaySize.x > 0 && displaySize.y > 0)
                {
                    _rect = _parentRectTransform.rect;

                    _rectTransform.sizeDelta = displaySize;

                    _rotatedTopLeftRel = _parentRectTransform.localRotation * new Vector2(_rect.xMin - _rect.center.x, _rect.yMin - _rect.center.y);
                    _rotatedTopRightRel = _parentRectTransform.localRotation * new Vector2(_rect.xMax - _rect.center.x, _rect.yMin - _rect.center.y);
                    _maxHeight = Mathf.Max(Mathf.Abs(_rotatedTopLeftRel.y), Mathf.Abs(_rotatedTopRightRel.y)) * 2;

                    _ratio = _maxHeight / displaySize.y;

                    _rectTransform.offsetMin *= _ratio;
                    _rectTransform.offsetMax *= _ratio;

                    _distance = Vector3.Distance(_parentRectTransform.position, _rectTransform.position);

                    if (_colliderAria != null)
                        _colliderAria.size = new Vector3(_rectTransform.sizeDelta.x, _rectTransform.sizeDelta.y, 0.1f);
                }
            }

            if (_rectTransform.localEulerAngles != displayRollOffset)
                _rectTransform.localEulerAngles = displayRollOffset;
        }

        public bool CopyScreenPoint(Vector3 originPos, out Vector3 planeLocalPos)
        {
            planeLocalPos = Vector3.zero;

            try
            {
                if (!Physics.Raycast(originPos, Vector3.forward, out _hitRay, (_distance * 1.5f), (1 << gameObject.layer)))
                    throw new Exception("not hit.");

                planeLocalPos = transform.InverseTransformPoint(_hitRay.point);
                return true;
            }
            catch (Exception ex)
            {
                Log(DebugerLogType.Info, "CopyScreenPoint", $"{ex.Message}");
            }
            return false;
        }
    }
}
