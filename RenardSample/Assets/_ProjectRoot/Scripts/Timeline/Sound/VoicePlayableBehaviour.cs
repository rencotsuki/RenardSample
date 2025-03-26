using System;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Playables;

namespace SignageHADO.Timeline
{
    [Serializable]
    [DisplayName("Voice")]
    public class VoicePlayableBehaviour : PlayableBehaviourCustom
    {
        protected SignageManager manager => SignageManager.Singleton;

        protected AudioClip voiceClip = null;
        protected float voiceVolume = 1f;

        public void Settings(AudioClip clip, float volume)
        {
            this.voiceClip = clip;
            this.voiceVolume = volume;
        }

        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            manager?.PlayVoice(voiceClip, voiceVolume);
        }
    }
}
