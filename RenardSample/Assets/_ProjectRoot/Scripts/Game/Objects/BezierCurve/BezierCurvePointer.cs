using System;
using UnityEngine;

[Serializable]
public class BezierCurvePointer : BezierCurve
{
    public float Duration { get; private set; } = 0f;

    public BezierCurvePointer(float duration, Vector3 startPoint, Vector3 endPoint)
        : base(startPoint, endPoint)
    {
        Duration = duration;
    }

    public BezierCurvePointer(float duration, Vector3 startPoint, Vector3 endPoint, Vector3 startControlPoint, Vector3 endControlPoint)
        : base(startPoint, endPoint, startControlPoint, endControlPoint)
    {
        Duration = duration;
    }

    public Vector3 GetPoint(float elapsed) => GetPointAtTime(elapsed);

    public Vector3 GetForward(float elapsed)
    {
        if (elapsed + 0.01f >= 1f) return (GetPointAtTime(1f) - GetPointAtTime(1f - 0.01f)).normalized;
        return (GetPointAtTime(elapsed + 0.01f) - GetPointAtTime(elapsed)).normalized;
    }

#if UNITY_EDITOR

    private Vector3 _from = Vector3.zero;
    private Vector3 _to = Vector3.zero;

    public void OnDrawGizmos(int pointerLinePrecision)
    {
        if (pointerLinePrecision <= 0) return;

        _from = StartPoint;
        _to = Vector3.zero;

        for (int index = 0; index < pointerLinePrecision; index++)
        {
            _to = GetPointAtTime((float)index / (float)pointerLinePrecision);
            OnDrawPointerLine(index, _from, _to);
            _from = _to;
        }

        _to = GetPointAtTime(1.0f);
        OnDrawPointerLine(pointerLinePrecision, _from, _to);

        OnDrawPointerControlLine(StartPoint, StartControlPoint);
        OnDrawPointerControlLine(EndPoint, EndControlPoint);
    }

    private void OnDrawPointerLine(int index, Vector3 from, Vector3 to)
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(from, to);
    }

    private void OnDrawPointerControlLine(Vector3 point, Vector3 controlPoint)
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(point, (point + controlPoint));
    }
#endif
}
