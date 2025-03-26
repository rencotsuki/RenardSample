using System;
using UnityEngine;

namespace SignageHADO
{
    public static class EMAFilter
    {
        public const string EMAFilterAlphaToolTip = @"遅い:滑らか(0.01~0.1) < 速い:ノイズが増える(0.2~0.5) < そのまま(0.8~1.0)";
    }

    [SerializeField]
    public class EMAFilterFloat
    {
        private readonly float _alpha = 0f;
        private float? _prevValue = null;

        private readonly bool _isDebugLog = false;

        public EMAFilterFloat(float smoothingFactor, bool isDebugLog = false)
        {
            _alpha = smoothingFactor;
            _isDebugLog = isDebugLog;
        }

        public EMAFilterFloat Set(float prevValue)
        {
            _prevValue = prevValue;
            return this;
        }

        public float Apply(float value)
        {
            try
            {
                if (_prevValue != null)
                {
                    _prevValue = _alpha * value + (1 - _alpha) * _prevValue.Value;
                }
                else
                {
                    _prevValue = value;
                }
                return _prevValue.Value;
            }
            catch (Exception ex)
            {
                if (_isDebugLog)
                    Debug.Log($"{typeof(EMAFilterFloat).Name}::Apply - {ex.Message}");
            }
            return value;
        }
    }

    [SerializeField]
    public class EMAFilterVector2
    {
        private readonly Vector2 _alpha = Vector2.zero;
        private Vector2? _prevValue = null;

        private readonly bool _isDebugLog = false;

        public EMAFilterVector2(Vector2 smoothingFactor, bool isDebugLog = false)
        {
            _alpha = smoothingFactor;
            _isDebugLog = isDebugLog;
        }

        public EMAFilterVector2 Set(Vector2 prevValue)
        {
            _prevValue = prevValue;
            return this;
        }

        public Vector2 Apply(Vector2 value)
        {
            try
            {
                if (_prevValue != null)
                {
                    _prevValue = new Vector2(_alpha.x * value.x + (1 - _alpha.x) * _prevValue.Value.x,
                                             _alpha.y * value.y + (1 - _alpha.y) * _prevValue.Value.y);
                }
                else
                {
                    _prevValue = value;
                }
                return _prevValue.Value;
            }
            catch (Exception ex)
            {
                if (_isDebugLog)
                    Debug.Log($"{typeof(EMAFilterVector2).Name}::Apply - {ex.Message}");
            }
            return value;
        }
    }

    [SerializeField]
    public class EMAFilterVector3
    {
        private readonly Vector3 _alpha = Vector3.zero;
        private Vector3? _prevValue = null;

        private readonly bool _isDebugLog = false;

        public EMAFilterVector3(Vector3 smoothingFactor, bool isDebugLog = false)
        {
            _alpha = smoothingFactor;
            _isDebugLog = isDebugLog;
        }

        public EMAFilterVector3 Set(Vector3 prevValue)
        {
            _prevValue = prevValue;
            return this;
        }

        public Vector3 Apply(Vector3 value)
        {
            try
            {
                if (_prevValue != null)
                {
                    _prevValue = new Vector3(_alpha.x * value.x + (1 - _alpha.x) * _prevValue.Value.x,
                                             _alpha.y * value.y + (1 - _alpha.y) * _prevValue.Value.y,
                                             _alpha.z * value.z + (1 - _alpha.z) * _prevValue.Value.z);
                }
                else
                {
                    _prevValue = value;
                }
                return _prevValue.Value;
            }
            catch (Exception ex)
            {
                if (_isDebugLog)
                    Debug.Log($"{typeof(EMAFilterVector3).Name}::Apply - {ex.Message}");
            }
            return value;
        }
    }

    [SerializeField]
    public class EMAFilterQuaternion
    {
        private readonly Quaternion _alpha = Quaternion.identity;
        private Quaternion? _prevValue = null;

        private readonly bool _isDebugLog = false;

        public EMAFilterQuaternion(Quaternion smoothingFactor, bool isDebugLog = false)
        {
            _alpha = smoothingFactor;
            _isDebugLog = isDebugLog;
        }

        public EMAFilterQuaternion Set(Quaternion prevValue)
        {
            _prevValue = prevValue;
            return this;
        }

        public Quaternion Apply(Quaternion value)
        {
            try
            {
                if (_prevValue != null)
                {
                    _prevValue = new Quaternion(_alpha.x * value.x + (1 - _alpha.x) * _prevValue.Value.x,
                                                _alpha.y * value.y + (1 - _alpha.y) * _prevValue.Value.y,
                                                _alpha.z * value.z + (1 - _alpha.z) * _prevValue.Value.z,
                                                _alpha.w * value.w + (1 - _alpha.w) * _prevValue.Value.w);
                }
                else
                {
                    _prevValue = value;
                }
                return _prevValue.Value;
            }
            catch (Exception ex)
            {
                if (_isDebugLog)
                    Debug.Log($"{typeof(EMAFilterQuaternion).Name}::Apply - {ex.Message}");
            }
            return value;
        }
    }
}