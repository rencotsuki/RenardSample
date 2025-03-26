using System;
using UnityEngine;
using UniRx;

public class GyroVisualizer : MonoBehaviourCustom
{
    [SerializeField] private GameObject _targetModel = null;
    [SerializeField] private MeshRenderer meshRenderer = null;
    [SerializeField] private int showCount = 10;
    [SerializeField] private int switchOnOffDurationMilliseconds = 300;

    private Material _material = null;
    private IDisposable _switchIntervalDisposable = null;

    private void Awake()
    {
        _material = meshRenderer != null ? new Material(meshRenderer.material) : null;
        if (_material != null)
        {
            _material.CopyPropertiesFromMaterial(meshRenderer.material);
            meshRenderer.material = _material;
        }

        _targetModel?.SetActive(false);
    }

    private void Start()
    {
        OverActionObservable();
    }

    private void OnDestroy()
    {
        OverActionDispose();
    }

    public void UpdateGyro(Quaternion attitude, bool isAnomaly, bool isDraw)
    {
        if (_targetModel != null)
        {
            if (_targetModel.activeSelf != isDraw)
                _targetModel.SetActive(isDraw);

            if (_targetModel.transform.rotation != attitude)
                _targetModel.transform.rotation = attitude;
        }

        if (isAnomaly)
            OverActionObservable();
    }

    private void OverActionDispose()
    {
        _switchIntervalDisposable?.Dispose();
        _switchIntervalDisposable = null;

        _material?.SetFloat("_Slider_ColorChange", 1f);
    }

    private void OverActionObservable()
    {
        if (_switchIntervalDisposable != null)
            return;

        _switchIntervalDisposable =
            Observable.Interval(TimeSpan.FromMilliseconds(switchOnOffDurationMilliseconds))
                        .Take(showCount)
                        .Subscribe(OverActionSubscribe, OverActionErorr, OverActionDispose)
                        .AddTo(this);
    }

    private void OverActionSubscribe(long time)
    {
        Log(DebugerLogType.Info, "OverActionSubscribe", "Over Action !!!");

        _material?.SetFloat("_Slider_ColorChange", 0f);
    }

    private void OverActionErorr(Exception ex)
    {
        OverActionDispose();
    }
}
