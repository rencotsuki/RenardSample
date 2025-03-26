using System;
using UnityEngine;
using UnityEngine.Audio;

public class SoundHandler
{
    public const float MindB = -80f;
    public const float MaxdB = 20f;

    public const float MinVolume = 0f;
    public const float MaxVolume = 1f;

    private const float convertMinVolume = 0.001f;

    /// <summary>volume→dB変換</summary>
    public static float ConvertVolumeTodB(float volume)
        => volume <= convertMinVolume ? MindB : (volume - MaxVolume) * Mathf.Abs(MindB);

    /// <summary>dB→volume変換</summary>
    public static float ConvertdBToVolume(float dB)
        => dB <= MindB ? MinVolume : MaxVolume + (dB / Mathf.Abs(MindB));

    private static SoundManager manager => SoundManager.Singleton;

    public static AudioMixerGroup BGMMixer => manager != null ? manager.BGMMixer : null;
    public static AudioMixerGroup JingleMixer => manager != null ? manager.JingleMixer : null;
    public static AudioMixerGroup SEMixer => manager != null ? manager.SEMixer : null;
    public static AudioMixerGroup VoiceMixer => manager != null ? manager.VoiceMixer : null;

    /// <summary>Master音量[0-1]</summary>
    public static float MasterVolume
    {
        get => manager != null ? manager.GetVolume("Volume_Master") : MinVolume;
        set => manager?.SetVolume("Volume_Master", value);
    }

    /// <summary>BGM音量[0-1]</summary>
    public static float BGMVolume
    {
        get => manager != null ? manager.GetVolume("Volume_BGM") : MinVolume;
        set => manager?.SetVolume("Volume_BGM", value);
    }

    /// <summary>Jingle音量[0-1]</summary>
    public static float JingleVolume
    {
        get => manager != null ? manager.GetVolume("Volume_Jingle") : MinVolume;
        set => manager?.SetVolume("Volume_Jingle", value);
    }

    /// <summary>SE音量[0-1]</summary>
    public static float SEVolume
    {
        get => manager != null ? manager.GetVolume("Volume_SE") : MinVolume;
        set => manager?.SetVolume("Volume_SE", value);
    }

    /// <summary>Voice音量[0-1]</summary>
    public static float VoiceVolume
    {
        get => manager != null ? manager.GetVolume("Volume_Voice") : MinVolume;
        set => manager?.SetVolume("Volume_Voice", value);
    }

    public static void Stop()
    {
        manager?.StopBGM();
        manager?.StopJingle();
        manager?.StopSE();
    }

    public static void PlaySE(AudioClip clip, float volume = 1f)
        => manager?.PlaySE(clip, volume);

    public static void StopSE()
        => manager?.StopSE();
}

[Serializable]
public class SoundManager : SingletonMonoBehaviourCustom<SoundManager>
{
    [Header("Mixer")]
    [SerializeField] private AudioMixer audioMixer = null;
    [SerializeField] private AudioMixerGroup mixerGroupBGM = null;
    [SerializeField] private AudioMixerGroup mixerGroupJingle = null;
    [SerializeField] private AudioMixerGroup mixerGroupSE = null;
    [SerializeField] private AudioMixerGroup mixerGroupVoice = null;
    [Header("AudioSource")]
    [SerializeField] private AudioSource audioSourceBGM = default;
    [SerializeField] private AudioSource audioSourceJingle = default;
    [SerializeField] private AudioSource audioSourceSE = default;

    [HideInInspector] private float _tempdB = 0f;

    public float GetVolume(string target)
    {
        if (audioMixer != null && audioMixer.GetFloat(target, out _tempdB))
            return SoundHandler.ConvertdBToVolume(_tempdB);
        return SoundHandler.MinVolume;
    }

    public void SetVolume(string target, float volume)
        => audioMixer?.SetFloat(target, SoundHandler.ConvertVolumeTodB(volume));

    public AudioMixer AudioMixer => audioMixer;
    public AudioMixerGroup BGMMixer => mixerGroupBGM;
    public AudioMixerGroup JingleMixer => mixerGroupJingle;
    public AudioMixerGroup SEMixer => mixerGroupSE;
    public AudioMixerGroup VoiceMixer => mixerGroupVoice;

    protected override void Initialized()
    {
        base.Initialized();
    }

    public void PlayBGM(AudioClip clip, float volume = 1f, bool loop = false)
    {
        if (audioSourceBGM == null)
            return;

        if (clip == null)
            return;

        if (audioSourceBGM.clip != null)
        {
            // 同一ファイルでループ再生中のものは処理させない
            if (audioSourceBGM.clip == clip && audioSourceBGM.loop)
                return;
        }

        audioSourceBGM.Stop();
        audioSourceBGM.clip = clip;
        audioSourceBGM.volume = volume;
        audioSourceBGM.loop = loop;
        audioSourceBGM.Play();
    }

    public void StopBGM()
    {
        if (audioSourceBGM != null)
        {
            audioSourceBGM.Stop();
            audioSourceBGM.clip = null;
        }
    }

    public void PlayJingle(AudioClip clip, float volume = 1f)
    {
        if (audioSourceJingle == null)
            return;

        if (clip == null)
            return;

        audioSourceJingle.PlayOneShot(clip, volume);
    }

    public void StopJingle()
    {
        audioSourceJingle?.Stop();
    }

    public void PlaySE(AudioClip clip, float volume = 1f)
    {
        if (audioSourceSE == null)
            return;

        if (clip == null)
            return;

        audioSourceSE.PlayOneShot(clip, volume);
    }

    public void StopSE()
    {
        audioSourceSE?.Stop();
    }
}
