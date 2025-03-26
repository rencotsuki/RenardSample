using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

[Serializable]
public abstract class EnemyTween : MonoBehaviourCustom
{
    [SerializeField] protected AnimationCurve _easing = AnimationCurve.Linear(0f, 1f, 1f, 1f);
    [SerializeField] protected AnimationCurve _orbitX = AnimationCurve.Linear(0f, 0f, 1f, 0f);
    [SerializeField] protected AnimationCurve _orbitY = AnimationCurve.Linear(0f, 0f, 1f, 0f);

    protected bool isActive = false;
    protected float duration = 0f;
    protected float range = 0f;

    private float _elapsed = 0f;
    private CancellationTokenSource _onUpdateElapsedToken = null;
    private float elapsedValue => duration <= 0f ? 0f : 1f / duration;
    protected float elapsed => _elapsed;
    protected float easing => _easing != null ? _easing.Evaluate(_elapsed) : 1.0f;
    private float _elapsedEasing = 0f;
    protected float elapsedEasing => _elapsed * easing < _elapsedEasing ? _elapsedEasing : _elapsedEasing * easing > 1f ? (_elapsedEasing = 1f) : (_elapsedEasing = _elapsed * easing);

    /// <summary>弾道パス作業点(始点)</summary>
    public Vector3 BeginControlDrection
    {
        get
        {
#if UNITY_EDITOR
            SetupBeginControlDrection();
#endif
            return beginControlDrection;
        }
    }
    private Vector3 beginControlDrection = Vector3.zero;

    /// <summary>弾道パス作業点(始点)</summary>
    public Vector3 EndControlDrection
    {
        get
        {
#if UNITY_EDITOR
            SetupEndControlDrection();
#endif
            return endControlDrection;
        }
    }
    private Vector3 endControlDrection = Vector3.zero;

    private void Awake()
    {
        SetupBeginControlDrection();
        SetupEndControlDrection();
    }

    private void SetupBeginControlDrection()
    {
        beginControlDrection = Vector3.zero;

        if (_orbitX == null || _orbitY == null) return;

        if (_orbitX.keys.Length > 1)
            beginControlDrection += Vector3.forward * _orbitX.keys[0].value;

        if (_orbitY.keys.Length > 1)
            beginControlDrection += Vector3.up * _orbitX.keys[0].value;
    }

    private void SetupEndControlDrection()
    {
        beginControlDrection = Vector3.zero;

        if (_orbitX == null || _orbitY == null) return;

        if (_orbitX.keys.Length > 1)
            beginControlDrection += Vector3.forward * _orbitX.keys[_orbitY.keys.Length - 1].value;

        if (_orbitY.keys.Length > 1)
            beginControlDrection += Vector3.up * _orbitX.keys[_orbitY.keys.Length - 1].value;
    }

    protected virtual void OnDestroy()
    {
        OnReset();
    }

    private void OnReset()
    {
        OnDispose();

        isActive = false;
        _elapsed = 0f;
        _elapsedEasing = 0f;
    }

    protected void Play(float lagTime = 0f)
    {
        OnReset();

        if (duration <= 0f)
        {
            OnEnd();
            return;
        }

        _elapsed = elapsedValue * lagTime;
        _onUpdateElapsedToken = new CancellationTokenSource();
        OnUpdateElapsedAsync(_onUpdateElapsedToken.Token).Forget();
    }

    public void Stop()
    {
        OnReset();
    }

    private void OnDispose()
    {
        _onUpdateElapsedToken?.Cancel();
        _onUpdateElapsedToken?.Dispose();
        _onUpdateElapsedToken = null;
    }

    private async UniTask OnUpdateElapsedAsync(CancellationToken token)
    {
        isActive = true;
        OnStart();

        while (_onUpdateElapsedToken != null && _elapsed < 1f)
        {
            _elapsed += elapsedValue * Time.deltaTime;
            if (_elapsed <= 0f) _elapsed = 0f;
            if (_elapsed >= 1f) _elapsed = 1f;

            OnUpdate();

            await UniTask.Yield(PlayerLoopTiming.Update, token);
            token.ThrowIfCancellationRequested();
        }

        isActive = false;
        OnEnd();

        OnReset();
    }

    protected virtual void OnStart() { }
    protected virtual void OnUpdate() { }
    protected virtual void OnEnd() { }
}

#if UNITY_EDITOR

[UnityEditor.CustomEditor(typeof(EnemyTween))]
[UnityEditor.CanEditMultipleObjects]
public class EnemyTweenEditor : UnityEditor.Editor { }

#endif
