using System;
using UnityEngine;

namespace SignageHADO
{
    public static class KalmanFilter
    {
        public const string ProcessNoiseToolTip = @"小さくすると滑らか";
        public const string MeasurementNoiseToolTip = @"観測値の重視";
        public const string NoiseMaxRangeToolTip = @"指定値以上は即時反映";
    }

    [SerializeField]
    public class KalmanFilterFloat
    {
        protected float estimate = 0f;                  // 推定値
        public float ErrorCovariance = 0f;              // 誤差共分散

        private readonly float _processNoise = 0f;      // プロセスノイズ
        private readonly float _measurementNoise = 0f;  // 観測ノイズ
        private readonly float _noiseMaxRange = 0f;     // ノイズを観測する最大値(0以下は全て)

        private float _kalmanGain = 0f;
        private readonly bool _isDebugLog = false;

        public KalmanFilterFloat(float processNoise, float measurementNoise, float noiseMaxRange = 0, bool isDebugLog = false)
        {
            _processNoise = processNoise;
            _measurementNoise = measurementNoise;
            _noiseMaxRange = noiseMaxRange;
            _isDebugLog = isDebugLog;
        }

        public KalmanFilterFloat Set(float initialEstimate, float initialError)
        {
            estimate = initialEstimate;
            ErrorCovariance = initialError;
            return this;
        }

        public float Apply(float measurement)
        {
            try
            {
                ErrorCovariance += 1f * _processNoise;

                if (_noiseMaxRange > 0 && Mathf.Abs(estimate - measurement) > _noiseMaxRange)
                {
                    estimate = measurement;
                }
                else
                {
                    _kalmanGain = ErrorCovariance / (ErrorCovariance + _measurementNoise);
                    estimate += _kalmanGain * (measurement - estimate);
                    ErrorCovariance *= (1f - _kalmanGain);
                }
                return estimate;
            }
            catch (Exception ex)
            {
                if (_isDebugLog)
                    Debug.Log($"{typeof(KalmanFilterFloat).Name}::Apply - {ex.Message}");
            }
            return measurement;
        }
    }

    [SerializeField]
    public class KalmanFilterVector3
    {
        protected Vector3 estimate = Vector3.zero;          // 推定値
        public Vector3 ErrorCovariance = Vector3.zero;      // 誤差共分散

        private readonly float _processNoise = 0f;          // プロセスノイズ
        private readonly float _measurementNoise = 0f;      // 観測ノイズ
        private readonly float _noiseMaxRange = 0f;         // ノイズを観測する最大値(0以下は全て)

        private Vector3 _kalmanGain = Vector3.zero;
        private readonly bool _isDebugLog = false;

        public KalmanFilterVector3(float processNoise, float measurementNoise, float noiseMaxRange = 0, bool isDebugLog = false)
        {
            _processNoise = processNoise;
            _measurementNoise = measurementNoise;
            _noiseMaxRange = noiseMaxRange;
            _isDebugLog = isDebugLog;
        }

        public KalmanFilterVector3 Set(Vector3 initialEstimate, Vector3 initialError)
        {
            estimate = initialEstimate;
            ErrorCovariance = initialError;
            return this;
        }

        public Vector3 Apply(Vector3 measurement)
        {
            try
            {
                ErrorCovariance += Vector3.one * _processNoise;

                // x
                if (_noiseMaxRange > 0 && Mathf.Abs(estimate.x - measurement.x) > _noiseMaxRange)
                {
                    estimate.x = measurement.x;
                }
                else
                {
                    _kalmanGain.x = ErrorCovariance.x / (ErrorCovariance.x + _measurementNoise);
                    estimate.x += _kalmanGain.x * (measurement.x - estimate.x);
                    ErrorCovariance.x *= (1f - _kalmanGain.x);
                }

                // y
                if (_noiseMaxRange > 0 && Mathf.Abs(estimate.y - measurement.y) > _noiseMaxRange)
                {
                    estimate.y = measurement.y;
                }
                else
                {
                    _kalmanGain.y = ErrorCovariance.y / (ErrorCovariance.y + _measurementNoise);
                    estimate.y += _kalmanGain.y * (measurement.y - estimate.y);
                    ErrorCovariance.y *= (1f - _kalmanGain.y);
                }

                // z
                if (_noiseMaxRange > 0 && Mathf.Abs(estimate.z - measurement.z) > _noiseMaxRange)
                {
                    estimate.z = measurement.z;
                }
                else
                {
                    _kalmanGain.z = ErrorCovariance.z / (ErrorCovariance.z + _measurementNoise);
                    estimate.z += _kalmanGain.z * (measurement.z - estimate.z);
                    ErrorCovariance.z *= (1f - _kalmanGain.z);
                }

                return estimate;
            }
            catch (Exception ex)
            {
                if (_isDebugLog)
                    Debug.Log($"{typeof(KalmanFilterVector3).Name}::Apply - {ex.Message}");
            }
            return measurement;
        }
    }

    [SerializeField]
    public class KalmanFilterQuaternion
    {
        protected Quaternion estimate = Quaternion.identity;        // 推定値
        public Quaternion ErrorCovariance = Quaternion.identity;    // 誤差共分散

        private readonly float _processNoise = 0f;          // プロセスノイズ
        private readonly float _measurementNoise = 0f;      // 観測ノイズ
        private readonly float _noiseMaxRange = 0f;         // ノイズを観測する最大値(0以下は全て)

        private Quaternion _kalmanGain = Quaternion.identity;
        private readonly bool _isDebugLog = false;

        public KalmanFilterQuaternion(float processNoise, float measurementNoise, float noiseMaxRange = 0, bool isDebugLog = false)
        {
            _processNoise = processNoise;
            _measurementNoise = measurementNoise;
            _noiseMaxRange = noiseMaxRange;
            _isDebugLog = isDebugLog;
        }

        public KalmanFilterQuaternion Set(Quaternion initialEstimate, Quaternion initialError)
        {
            estimate = initialEstimate;
            ErrorCovariance = initialError;
            return this;
        }

        public Quaternion Apply(Quaternion measurement)
        {
            try
            {
                ErrorCovariance.x += 1f * _processNoise;
                ErrorCovariance.y += 1f * _processNoise;
                ErrorCovariance.z += 1f * _processNoise;
                ErrorCovariance.w += 1f * _processNoise;

                // x
                if (_noiseMaxRange > 0 && Mathf.Abs(estimate.x - measurement.x) > _noiseMaxRange)
                {
                    estimate.x = measurement.x;
                }
                else
                {
                    _kalmanGain.x = ErrorCovariance.x / (ErrorCovariance.x + _measurementNoise);
                    estimate.x += _kalmanGain.x * (measurement.x - estimate.x);
                    ErrorCovariance.x *= (1f - _kalmanGain.x);
                }

                // y
                if (_noiseMaxRange > 0 && Mathf.Abs(estimate.y - measurement.y) > _noiseMaxRange)
                {
                    estimate.y = measurement.y;
                }
                else
                {
                    _kalmanGain.y = ErrorCovariance.y / (ErrorCovariance.y + _measurementNoise);
                    estimate.y += _kalmanGain.y * (measurement.y - estimate.y);
                    ErrorCovariance.y *= (1f - _kalmanGain.y);
                }

                // z
                if (_noiseMaxRange > 0 && Mathf.Abs(estimate.z - measurement.z) > _noiseMaxRange)
                {
                    estimate.z = measurement.z;
                }
                else
                {
                    _kalmanGain.z = ErrorCovariance.z / (ErrorCovariance.z + _measurementNoise);
                    estimate.z += _kalmanGain.z * (measurement.z - estimate.z);
                    ErrorCovariance.z *= (1f - _kalmanGain.z);
                }

                // w
                if (_noiseMaxRange > 0 && Mathf.Abs(estimate.w - measurement.w) > _noiseMaxRange)
                {
                    estimate.w = measurement.w;
                }
                else
                {
                    _kalmanGain.w = ErrorCovariance.w / (ErrorCovariance.w + _measurementNoise);
                    estimate.w += _kalmanGain.w * (measurement.w - estimate.w);
                    ErrorCovariance.w *= (1f - _kalmanGain.w);
                }

                return estimate;
            }
            catch (Exception ex)
            {
                if (_isDebugLog)
                    Debug.Log($"{typeof(KalmanFilterQuaternion).Name}::Apply - {ex.Message}");
            }
            return measurement;
        }
    }
}