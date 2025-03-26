using System;
using UnityEngine;

[Serializable]
public class BezierCurve
{
    /// <summary>開始点</summary>
    public Vector3 StartPoint;

    /// <summary>終了点</summary>
    public Vector3 EndPoint;

    /// <summary>開始制御点</summary>
    public Vector3 StartControlPoint = Vector3.zero;

    /// <summary>終了制御点</summary>
    public Vector3 EndControlPoint = Vector3.zero;

    private Vector3 _b0 = Vector3.zero;
    private Vector3 _b1 = Vector3.zero;
    private Vector3 _b2 = Vector3.zero;
    private Vector3 _b3 = Vector3.zero;

    private float _ax = 0f;
    private float _ay = 0f;
    private float _az = 0f;

    private float _bx = 0f;
    private float _by = 0f;
    private float _bz = 0f;

    private float _cx = 0f;
    private float _cy = 0f;
    private float _cz = 0f;

    private bool _isCurve = false;
    private float _time1 = 0f;
    private float _time2 = 0f;
    private Vector3 _lastDerectPosition = Vector3.zero;

    public BezierCurve(Vector3 startPoint, Vector3 endPoint)
    {
        _isCurve = false;

        StartPoint = startPoint;
        EndPoint = endPoint;
        StartControlPoint = Vector3.zero;
        EndControlPoint = Vector3.zero;

        _lastDerectPosition = EndPoint - StartPoint;
    }

    public BezierCurve(Vector3 startPoint, Vector3 endPoint, Vector3 startControlPoint, Vector3 endControlPoint)
    {
        _isCurve = true;

        StartPoint = startPoint;
        EndPoint = endPoint;
        StartControlPoint = startControlPoint;
        EndControlPoint = endControlPoint;

        var pos = CreatePosition(0.9f);
        _lastDerectPosition = EndPoint - pos;
    }

    private void SetConstant()
    {
        _cx = 3f * ((StartPoint.x + StartControlPoint.x) - StartPoint.x);
        _bx = 3f * ((EndPoint.x + EndControlPoint.x) - (StartPoint.x + StartControlPoint.x)) - _cx;
        _ax = EndPoint.x - StartPoint.x - _cx - _bx;

        _cy = 3f * ((StartPoint.y + StartControlPoint.y) - StartPoint.y);
        _by = 3f * ((EndPoint.y + EndControlPoint.y) - (StartPoint.y + StartControlPoint.y)) - _cy;
        _ay = EndPoint.y - StartPoint.y - _cy - _by;

        _cz = 3f * ((StartPoint.z + StartControlPoint.z) - StartPoint.z);
        _bz = 3f * ((EndPoint.z + EndControlPoint.z) - (StartPoint.z + StartControlPoint.z)) - _cz;
        _az = EndPoint.z - StartPoint.z - _cz - _bz;
    }

    private void CheckConstant()
    {
        if (StartPoint != _b0 || StartControlPoint != _b1 || EndControlPoint != _b2 || EndPoint != _b3)
        {
            SetConstant();
            _b0 = StartPoint;
            _b1 = StartControlPoint;
            _b2 = EndControlPoint;
            _b3 = EndPoint;
        }
        else
        {
            _b0 = Vector3.zero;
            _b1 = Vector3.zero;
            _b2 = Vector3.zero;
            _b3 = Vector3.zero;
        }
    }

    private Vector3 CreatePosition(float time)
    {
        CheckConstant();

        _time1 = time * time;
        _time2 = time * time * time;

        return new Vector3(
            _ax * _time2 + _bx * _time1 + _cx * time + StartPoint.x,
            _ay * _time2 + _by * _time1 + _cy * time + StartPoint.y,
            _az * _time2 + _bz * _time1 + _cz * time + StartPoint.z);
    }

    public Vector3 GetPointAtTime(float time)
    {
        if (time > 0f && time < 1.0f)
        {
            return _isCurve ? CreatePosition(time) : StartPoint + _lastDerectPosition * time;
        }
        else if (time == 1.0f)
        {
            return EndPoint;
        }
        else if (time > 1.0f)
        {
            return EndPoint + _lastDerectPosition * (time - 1f);
        }

        return StartPoint;
    }
}
