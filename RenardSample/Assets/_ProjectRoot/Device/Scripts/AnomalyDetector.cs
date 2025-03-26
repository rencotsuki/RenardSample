using System;
using UnityEngine;

[Serializable]
public class AnomalyDetector : MonoBehaviourCustom
{
    [SerializeField] private float attitudeThreshold = 25f;
    [SerializeField] private float accelerationThreshold = 8f;

    public float CalcuratedValue { get; private set; } = 0f;

    public bool IsAnomaly { get; private set; } = false;

    private float _angle = 0f;
    private float _previousAngle = 0f;
    private float _deltaAngle = 0f;
    private Vector3 _acceleration = Vector3.zero;
    private bool isExceededAttitude => (CalcuratedValue > attitudeThreshold);
    private bool isExceededAcceleration
    {
        get
        {
            return Mathf.Abs(_acceleration.x) > accelerationThreshold ||
                   Mathf.Abs(_acceleration.y) > accelerationThreshold ||
                   Mathf.Abs(_acceleration.z) > accelerationThreshold;
        }
    }

    public void UpdateAnomalySensor(DeviceSensorScript deviceSensor, float deltaTime)
    {
        _acceleration = deviceSensor.Acceleration * CoodinateTransformer.LandscapeRotator(deviceSensor.Orientation);

        _angle = Mathf.Acos(deviceSensor.Attitude.w) * 2;
        _deltaAngle = deltaTime <= 0 ? 0f : (_angle - _previousAngle) / deltaTime;
        CalcuratedValue = Mathf.Abs(_deltaAngle);
        _previousAngle = _angle;

        IsAnomaly = (isExceededAcceleration && isExceededAttitude);
    }
}
