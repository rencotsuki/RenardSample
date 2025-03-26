using System;
using UnityEngine;

[Serializable]
public class CPUObject : MonoBehaviourCustom
{
    [SerializeField] protected CPUModel model = default;
    [SerializeField] protected AudioSource voiceAudioSource = default;

    protected bool visible = false;
    protected int m_texturePackNo { get; private set; } = 0;
    protected int m_voicePackNo { get; private set; } = 0;

    protected bool isLoadTexture = false;
    protected bool isLoadVoice = false;

    public Transform Head => model != null ? model.Head : null;
    public float HeadHeight { get; private set; } = 1.5f;

    protected virtual Transform objectParent => transform;

    protected Transform floorParent => transform;
    protected Vector3 floorPos => floorParent != null ? floorParent.position : Vector3.zero;

    [HideInInspector] private Vector3 _beforePos = Vector3.zero;
    [HideInInspector] private Quaternion _beforeRot = Quaternion.identity;
    [HideInInspector] private Vector2 _moveDistance = Vector2.zero;
    [HideInInspector] private float _jumpValue = 0f;
    [HideInInspector] private Vector3 _rollDistance = Vector3.zero;
    [HideInInspector] private Vector3 _tmpPos = Vector3.zero;

    private void Start()
    {
        OnSetupAudio();
    }

    /// <summary></summary>
    public CPUObject Setup(int texturePackNo, int voicePackNo)
    {
        m_texturePackNo = texturePackNo;
        m_voicePackNo = voicePackNo;

        HeadHeight = Head != null ? transform.InverseTransformPoint(Head.position).y : 1.5f;

        OnSetup();
        OnLoadResources();
        return this;
    }

    protected virtual void OnSetup()
    {
        model?.Setup();
    }

    protected virtual void OnLoadResources()
    {
        isLoadTexture = true;
        isLoadVoice = true;
    }

    /// <summary></summary>
    public CPUObject SetVisible(bool value)
    {
        OnSetVisible(value);
        visible = value;
        return this;
    }

    protected virtual void OnSetVisible(bool value)
    {
        model?.SetVisible(value);
    }

    protected virtual void OnSetupAudio()
    {
        if (voiceAudioSource == null)
            return;

        voiceAudioSource.spatialBlend = 0f; // 2D
    }

    protected void OnPlayVoice(AudioClip clip, float volume = 1f, float lagTime = 0f)
    {
        if (voiceAudioSource == null || clip == null || volume <= 0f)
            return;

        try
        {
            voiceAudioSource.volume = volume;
            voiceAudioSource.time = lagTime;
            voiceAudioSource.loop = false;

            voiceAudioSource.clip = null;
            voiceAudioSource.PlayOneShot(clip);
        }
        catch
        {
            // 何もしない
        }
    }

    protected void OnStopVoice()
    {
        if (voiceAudioSource == null || !voiceAudioSource.isPlaying)
            return;

        voiceAudioSource.Stop();
    }

    /// <summary></summary>
    public CPUObject UpdateMoveing(Vector3 pos, Quaternion rot)
    {
        _moveDistance.x = (_beforePos.x - pos.x);
        _moveDistance.y = (_beforePos.z - pos.z);
        _jumpValue = (_beforePos.y - pos.y);

        _rollDistance.x = (_beforeRot.eulerAngles.x - rot.eulerAngles.x);
        _rollDistance.y = (_beforeRot.eulerAngles.y - rot.eulerAngles.y);
        _rollDistance.z = (_beforeRot.eulerAngles.z - rot.eulerAngles.z);

        _tmpPos = pos;
        _tmpPos.y = 0f;

        transform.position = floorParent.TransformPoint(_tmpPos);
        transform.localRotation = Quaternion.identity;

        OnUpdateMoveing(_moveDistance, _rollDistance, _jumpValue);

        _beforePos = pos;
        _beforeRot = rot;
        return this;
    }

    protected virtual void OnUpdateMoveing(Vector2 moveDistance, Vector3 rollDistance, float jumpValue)
    {
        // 未使用
    }

    /// <summary></summary>
    public void Appear() => OnAppear();

    protected virtual void OnAppear() { }

    /// <summary></summary>
    public void Disappear() => OnDisappear();

    protected virtual void OnDisappear() { }

    /// <summary></summary>
    public void Damage() => OnDamage();

    protected virtual void OnDamage() { }

    /// <summary></summary>
    public void Dead() => OnDead();

    protected virtual void OnDead() { }

    /// <summary></summary>
    public void Repair() => OnRepair();

    protected virtual void OnRepair() { }

    /// <summary></summary>
    public void GameBegin() => OnGameBegin();

    protected virtual void OnGameBegin() { }

    /// <summary></summary>
    public void GameFinish(bool win) => OnGameFinish(win);

    protected virtual void OnGameFinish(bool win) { }
}
