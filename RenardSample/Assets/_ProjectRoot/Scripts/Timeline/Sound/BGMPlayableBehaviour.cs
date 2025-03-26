using System;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Playables;

namespace SignageHADO.Timeline
{
    [Serializable]
    [DisplayName("BGM")]
    public class BGMPlayableBehaviour : PlayableBehaviourCustom
    {
        protected SoundManager manager => SoundManager.Singleton;

        protected AudioClip bgmClip = null;
        protected float bgmVolume = 1f;
        protected bool bgmLoop = false;
        protected bool endStopCall = false;

        public void Settings(AudioClip clip, float volume, bool loop, bool endStopCall)
        {
            this.bgmClip = clip;
            this.bgmVolume = volume;
            this.bgmLoop = loop;
            this.endStopCall = endStopCall;
        }

        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            manager?.PlayBGM(bgmClip, bgmVolume, bgmLoop);
        }

        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            if (endStopCall)
                manager?.StopBGM();
        }
    }
}
