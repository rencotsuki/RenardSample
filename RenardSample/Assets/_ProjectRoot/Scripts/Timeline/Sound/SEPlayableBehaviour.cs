using System;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Playables;

namespace SignageHADO.Timeline
{
    [Serializable]
    [DisplayName("SE")]
    public class SEPlayableBehaviour : PlayableBehaviourCustom
    {
        protected SoundManager manager => SoundManager.Singleton;

        protected AudioClip seClip = null;
        protected float seVolume = 1f;

        public void Settings(AudioClip clip, float volume)
        {
            this.seClip = clip;
            this.seVolume = volume;
        }

        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            manager?.PlaySE(seClip, seVolume);
        }
    }
}
