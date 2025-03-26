using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

[Serializable]
public class CPUPanelObject : CPUObject
{
    [HideInInspector] private CPUPanelModel _panelModel = default;
    [HideInInspector] private Texture2D _idle = null;
    [HideInInspector] private Texture2D _fire = null;
    [HideInInspector] private Texture2D _hit = null;
    [HideInInspector] private Texture2D _die = null;
    [HideInInspector] private Texture2D _win = null;

    [HideInInspector] private AudioClip _voiceClipAppear = null;
    [HideInInspector] private AudioClip _voiceClipDisappear = null;
    [HideInInspector] private AudioClip _voiceClipGameBegin = null;
    [HideInInspector] private AudioClip _voiceClipGameFinish = null;
    [HideInInspector] private AudioClip _voiceClipAttack = null;
    [HideInInspector] private AudioClip _voiceClipDefense = null;
    [HideInInspector] private AudioClip _voiceClipDamage = null;
    [HideInInspector] private AudioClip _voiceClipDeath = null;
    [HideInInspector] private AudioClip _voiceClipRepair = null;
    [HideInInspector] private AudioClip _voiceClipPointGet = null;
    [HideInInspector] private AudioClip _voiceClipWin = null;

    private CancellationTokenSource _loadTexturesToken = null;
    private CancellationTokenSource _loadVoicesToken = null;

    protected override void OnSetup()
    {
        //var texturePack = LocalDB.GetTexturePackData(m_texturePackNo);
        model?.Setup(string.Empty);

        _panelModel = model.GetComponent<CPUPanelModel>();
    }

    private void OnDestroy()
    {
        OnDisposeLoadTextures();
        OnDisposeLoadVoices();
    }

    protected override void OnLoadResources()
    {
        OnDisposeLoadTextures();
        _loadTexturesToken = new CancellationTokenSource();
        OnLoadTextureAsync(_loadTexturesToken.Token).Forget();

        OnDisposeLoadVoices();
        _loadVoicesToken = new CancellationTokenSource();
        OnLoadVoicesAsync(_loadVoicesToken.Token).Forget();
    }

    private void OnDisposeLoadTextures()
    {
        _loadTexturesToken?.Cancel();
        _loadTexturesToken?.Dispose();
        _loadTexturesToken = null;
    }

    private async UniTask OnLoadTextureAsync(CancellationToken token)
    {
        isLoadTexture = false;

        _idle = null;
        _fire = null;
        _hit = null;
        _die = null;
        _win = null;

        if (m_texturePackNo <= 0)
        {
            isLoadTexture = true;
            return;
        }

        //try
        //{
        //    await UniTask.WaitWhile(() => !TexturePackManager.Exists, PlayerLoopTiming.Update, token);
        //    token.ThrowIfCancellationRequested();

        //    _idle = await TexturePackManager.ReadCPUTextureAsync(token, m_texturePackNo, Paking.TextureType.Idle);
        //    _fire = await TexturePackManager.ReadCPUTextureAsync(token, m_texturePackNo, Paking.TextureType.Fire);
        //    _hit = await TexturePackManager.ReadCPUTextureAsync(token, m_texturePackNo, Paking.TextureType.Hit);
        //    _die = await TexturePackManager.ReadCPUTextureAsync(token, m_texturePackNo, Paking.TextureType.Die);
        //    _win = await TexturePackManager.ReadCPUTextureAsync(token, m_texturePackNo, Paking.TextureType.Win);

        //    _panelModel?.SetTextures(_idle, _fire, _hit, _die, _win);

        //    isLoadTexture = true;
        //}
        //catch (Exception ex)
        //{
        //    Log(DebugerLogType.Info, "OnLoadTextureAsync", $"{ex.Message}");
        //}
    }

    private void OnDisposeLoadVoices()
    {
        _loadVoicesToken?.Cancel();
        _loadVoicesToken?.Dispose();
        _loadVoicesToken = null;
    }

    private async UniTask OnLoadVoicesAsync(CancellationToken token)
    {
        isLoadVoice = false;

        _voiceClipAppear = null;
        _voiceClipDisappear = null;
        _voiceClipGameBegin = null;
        _voiceClipGameFinish = null;
        _voiceClipAttack = null;
        _voiceClipDefense = null;
        _voiceClipDamage = null;
        _voiceClipDeath = null;
        _voiceClipRepair = null;
        _voiceClipPointGet = null;
        _voiceClipWin = null;

        if (m_voicePackNo <= 0)
        {
            isLoadVoice = true;
            return;
        }

        //try
        //{
        //    await UniTask.WaitWhile(() => !SoundPackManager.Exists, PlayerLoopTiming.Update, token);
        //    token.ThrowIfCancellationRequested();

        //    _voiceClipAppear = await SoundPackManager.ReadVoiceAsync(token, m_voicePackNo, Paking.VoiceType.Appear);
        //    _voiceClipDisappear = await SoundPackManager.ReadVoiceAsync(token, m_voicePackNo, Paking.VoiceType.Disappear);
        //    _voiceClipGameBegin = await SoundPackManager.ReadVoiceAsync(token, m_voicePackNo, Paking.VoiceType.GameBegin);
        //    _voiceClipGameFinish = await SoundPackManager.ReadVoiceAsync(token, m_voicePackNo, Paking.VoiceType.GameFinish);
        //    _voiceClipAttack = await SoundPackManager.ReadVoiceAsync(token, m_voicePackNo, Paking.VoiceType.Attack);
        //    _voiceClipDefense = await SoundPackManager.ReadVoiceAsync(token, m_voicePackNo, Paking.VoiceType.Defense);
        //    _voiceClipDamage = await SoundPackManager.ReadVoiceAsync(token, m_voicePackNo, Paking.VoiceType.Damage);
        //    _voiceClipDeath = await SoundPackManager.ReadVoiceAsync(token, m_voicePackNo, Paking.VoiceType.Death);
        //    _voiceClipRepair = await SoundPackManager.ReadVoiceAsync(token, m_voicePackNo, Paking.VoiceType.Repair);
        //    _voiceClipPointGet = await SoundPackManager.ReadVoiceAsync(token, m_voicePackNo, Paking.VoiceType.PointGet);
        //    _voiceClipWin = await SoundPackManager.ReadVoiceAsync(token, m_voicePackNo, Paking.VoiceType.Win);

        //    isLoadVoice = true;
        //}
        //catch (Exception ex)
        //{
        //    Log(DebugerLogType.Info, "OnLoadVoicesAsync", $"{ex.Message}");
        //}
    }

    protected override void OnAppear()
    {
        _panelModel?.Appear();
    }

    protected override void OnDisappear()
    {
        _panelModel?.Disappear();
    }

    protected override void OnDamage()
    {
        _panelModel?.Damage();

        if (visible)
            OnPlayVoice(_voiceClipDamage);
    }

    protected override void OnDead()
    {
        _panelModel?.Death();

        if (visible)
            OnPlayVoice(_voiceClipDeath);
    }

    protected override void OnRepair()
    {
        _panelModel?.Repair();

        if (visible)
            OnPlayVoice(_voiceClipRepair);
    }

    protected override void OnGameBegin()
    {
        _panelModel?.GameBegin();

        if (visible)
            OnPlayVoice(_voiceClipGameBegin);
    }

    protected override void OnGameFinish(bool win)
    {
        _panelModel?.GameFinish(win);

        if (visible)
            OnPlayVoice(_voiceClipGameFinish);
    }
}
