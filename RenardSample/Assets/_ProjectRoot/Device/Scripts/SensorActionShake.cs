using System;
using UnityEngine;

[Serializable]
public class SensorActionShake : MonoBehaviourCustom
{
    [Serializable]
    private struct Param
    {
        public float ShakeAccelerationThreshold;
        public float ShakeAccelerationThresholdBeginner;
        public float LowerAngleX;
        public float HigherAngleX;
        public float CoolTime;
        public float AccelerationDirectionOK;
    }

    [SerializeField] private Param normal = new Param()
    {
        ShakeAccelerationThreshold = -2.5f,
        ShakeAccelerationThresholdBeginner = -1f,
        LowerAngleX = -40f,
        HigherAngleX = 30f,
        CoolTime = 10f / 60f,
        AccelerationDirectionOK = 0.8f
    };

    [SerializeField] private Param witMotion = new Param()
    {
        ShakeAccelerationThreshold = 1.45f,
        ShakeAccelerationThresholdBeginner = 1.45f,
        LowerAngleX = 55f,
        HigherAngleX = 55f,
        CoolTime = 10f / 60f,
        AccelerationDirectionOK = 0.8f
    };

    private const float shakeAngularVelocityThresholdPreFrame = 250.0f;

    private float shakeAccelerationThreshold => _witmotion ? witMotion.ShakeAccelerationThreshold : normal.ShakeAccelerationThreshold;
    private float shakeAccelerationThresholdBeginner => _witmotion ? witMotion.ShakeAccelerationThresholdBeginner : normal.ShakeAccelerationThresholdBeginner;
    private float lowerAngleX => _witmotion ? witMotion.LowerAngleX : normal.LowerAngleX;
    private float higherAngleX => _witmotion ? witMotion.HigherAngleX : normal.HigherAngleX;
    private float coolTime => _witmotion ? witMotion.CoolTime : normal.CoolTime;
    private float accelerationDirectionOK => _witmotion ? witMotion.AccelerationDirectionOK : normal.AccelerationDirectionOK;

    [HideInInspector] private bool _result = false;
    [HideInInspector] private bool _witmotion = false;
    [HideInInspector] private Vector3 _previousFrameAcceleration = Vector3.zero;
    [HideInInspector] private Vector3 _acceleration = Vector3.zero;
    [HideInInspector] private Vector3 _gravity = Vector3.zero;
    [HideInInspector] private float _dsX = 0f;
    [HideInInspector] private float _degreeX = 0f;
    [HideInInspector] private float _coolTimer = 0f;

    [HideInInspector] private bool _isPunch = false;
    [HideInInspector] private bool _armIsDown = false;
    [HideInInspector] private bool _isChop = false;
    [HideInInspector] private float _higherRad = 0f;
    [HideInInspector] private float _lowerRad = 0f;
    [HideInInspector] private float _currentXRad = 0f;
    [HideInInspector] private float _firstXRad = 0f;
    [HideInInspector] private (Vector3, Vector3, Quaternion) _current = default;
    [HideInInspector] private (Vector3, Vector3, Quaternion) _middle = default;
    [HideInInspector] private (Vector3, Vector3, Quaternion) _first = default;

    public SensorActionEnum SensorAction => SensorActionEnum.Shake;

    private void Start()
    {
        IsDebugLog = false;
    }

    private bool CheckShakeAcceleration(float acceleration, float previousAcceleration, bool beginner)
    {
        if (beginner)
        {
            return (acceleration < shakeAccelerationThresholdBeginner || previousAcceleration < shakeAccelerationThresholdBeginner);
        }
        else
        {
            return (acceleration < shakeAccelerationThreshold || previousAcceleration < shakeAccelerationThreshold);
        }
    }

    private bool IsShakeForPunch(DeviceSensorScript deviceSensor, float shakeThreshold)
    {
        var gravityInDevice = Quaternion.Inverse(deviceSensor.Attitude) * Vector3.down;
        var userAcceleration = deviceSensor.Gravity - gravityInDevice;

        return userAcceleration.magnitude > shakeThreshold;
    }

    private bool IsShakeForVerticalChop(float previousAngularVelocityY, float middleAngularVelocityY, float currentAngularVelocityY)
    {
        if (previousAngularVelocityY > 0 && middleAngularVelocityY > 0 && currentAngularVelocityY <= 0 ||
            previousAngularVelocityY < 0 && middleAngularVelocityY < 0 && currentAngularVelocityY >= 0)
        {
            return Mathf.Abs(previousAngularVelocityY + middleAngularVelocityY) > shakeAngularVelocityThresholdPreFrame * 2;
        }
        return false;
    }

    public bool CheckSensorAction(DeviceSensorScript deviceSensor, float deltaTime)
    {
        if (deviceSensor == null)
            return false;

        _result = false;
        _witmotion = deviceSensor.UseWitMotion;

        if (_coolTimer - deltaTime > 0f)
        {
            _coolTimer -= deltaTime;
        }
        else
        {
            if (_witmotion)
            {
                _first = _middle;
                _middle = _current;
                _current = (deviceSensor.Acceleration, deviceSensor.Gravity, deviceSensor.Attitude);

                _higherRad = higherAngleX * Mathf.Deg2Rad;
                _lowerRad = -lowerAngleX * Mathf.Deg2Rad;
                _currentXRad = Mathf.Asin((_current.Item3 * Vector3.forward).y);

                _isPunch = false;

                if (_lowerRad < _currentXRad && _currentXRad < _higherRad)
                    _isPunch = IsShakeForPunch(deviceSensor, shakeAccelerationThresholdBeginner);

                _firstXRad = Mathf.Asin((_first.Item3 * Vector3.forward).y);
                _armIsDown = _firstXRad < 0;

                _isChop = !_armIsDown && IsShakeForVerticalChop(_first.Item1.y, _middle.Item1.y, _current.Item1.y);
                if (_isPunch || _isChop)
                {
                    _result = true;
                    _coolTimer = coolTime;
                }
            }
            else
            {
                _coolTimer = 0f;
                _acceleration = deviceSensor.Acceleration * CoodinateTransformer.LandscapeRotator(deviceSensor.Orientation);
                _gravity = deviceSensor.Gravity * CoodinateTransformer.LandscapeRotator(deviceSensor.Orientation);
                _dsX = _acceleration.x - _previousFrameAcceleration.x;

                if (_dsX > 0)
                {
                    if (CheckShakeAcceleration(_acceleration.x, _previousFrameAcceleration.x, deviceSensor.IsBeginner))
                    {
                        _degreeX = Mathf.Asin(_gravity.x) * Mathf.Rad2Deg;
                        if (_degreeX > lowerAngleX && _degreeX < higherAngleX)
                        {
                            //if (IsOKAccelerationDirection(acceleration))
                            {
                                _result = true;
                                _coolTimer = coolTime;

                                //Log(DebugerLogType.Info, "CheckSensorAction", $"acceleration={_acceleration}, gravity={_gravity}, dsX={_dsX}");
                            }
                        }
                    }
                }

                _previousFrameAcceleration = _acceleration;
            }
        }
        return _result;
    }

    private bool IsOKAccelerationDirection(Vector3 acceleration)
        => (Mathf.Abs(acceleration.z) / Mathf.Abs(acceleration.x) < accelerationDirectionOK);
}
