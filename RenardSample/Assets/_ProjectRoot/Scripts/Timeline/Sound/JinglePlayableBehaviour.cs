using System;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Playables;

namespace SignageHADO.Timeline
{
    [Serializable]
    [DisplayName("Jingle")]
    public class JinglePlayableBehaviour : PlayableBehaviourCustom
    {
        protected SoundManager manager => SoundManager.Singleton;

        protected AudioClip jingleClip = null;
        protected float jingleVolume = 1f;

        public void Settings(AudioClip clip, float volume)
        {
            this.jingleClip = clip;
            this.jingleVolume = volume;
        }

        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            manager?.PlayJingle(jingleClip, jingleVolume);
        }
    }
}
