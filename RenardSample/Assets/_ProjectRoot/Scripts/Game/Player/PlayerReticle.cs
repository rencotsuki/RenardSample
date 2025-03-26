using System;
using UnityEngine;

namespace SignageHADO.Game
{
    public class PlayerReticle : MonoBehaviourCustom
    {
        [Header("Ray設定")]
        [SerializeField] private LayerMask rayTargets = default;

        private Camera viewCamera => GameHandler.ViewCamera;

        protected float rayRadius = 0.52f;
        protected float maxRayLength = 20f;
        protected bool isHitRay = false;
        protected Vector3 rayWorldPos = Vector3.zero;
        protected Vector2 rayScreenPos = Vector2.zero;

        [HideInInspector] private Vector3 _rayDirection = Vector3.zero;
        [HideInInspector] private float _rayHitDistance = 0f;
        [HideInInspector] private RaycastHit _hitRay = default;
        [HideInInspector] private RaycastHit[] _hitRayAll = null;
        [HideInInspector] private ColliderSupport _collider = null;
        [HideInInspector] private ColliderSupport _hitObject = null;

        private void Start()
        {
            ClearReticle();
        }

        public void Set(float rayRadius, float maxRayLength)
        {
            this.rayRadius = rayRadius;
            this.maxRayLength = maxRayLength;
        }

        public void ClearReticle()
        {
            isHitRay = false;
            rayWorldPos = Vector3.zero;
            rayScreenPos = Vector2.zero;
        }

        public bool GetReticleTargetPosition(Transform slot, Vector2 reticleViewPos, out Vector3 worldPos)
        {
            worldPos = rayWorldPos;

            if (viewCamera == null || slot == null)
                return false;

            isHitRay = RayAction(viewCamera.ViewportPointToRay(reticleViewPos), slot.position, out _rayDirection, out _rayHitDistance);

            rayWorldPos = worldPos = slot.position + _rayDirection * _rayHitDistance;
            return isHitRay;
        }

        private bool RayAction(Ray ray, Vector3 slotPos, out Vector3 rayDirection, out float rayHitDistance)
        {
            _hitRayAll = Physics.SphereCastAll(ray, rayRadius, maxRayLength, rayTargets);

            _hitRay = default;
            _hitObject = null;

            if (_hitRayAll != null && _hitRayAll.Length > 0)
            {
                foreach (var item in _hitRayAll)
                {
                    if (item.collider == null)
                        continue;

                    _collider = item.collider.GetComponent<ColliderSupport>();
                    if (_collider == null && !_collider.Enabled)
                        continue;

                    if (_hitRay.distance == 0 || _hitRay.distance > item.distance)
                    {
                        _hitRay = item;

                        if (_hitObject == null || !_hitObject.IsEnemy)
                            _hitObject = _collider;
                    }
                }
            }

            if (_hitObject != null && _hitObject.IsEnemy)
            {
                rayHitDistance = Vector3.Distance(slotPos, _hitObject.transform.position);
                rayDirection = ((ray.origin + ray.direction * rayHitDistance) - slotPos).normalized;
                return true;
            }
            else
            {
                rayHitDistance = maxRayLength;
                rayDirection = ((ray.origin + ray.direction * maxRayLength) - slotPos).normalized;
                return false;
            }
        }
    }
}
