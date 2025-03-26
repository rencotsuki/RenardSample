using System;
using System.ComponentModel;
using UnityEngine.Playables;

namespace SignageHADO.Timeline
{
    [Serializable]
    [DisplayName("BGM")]
    public class BGMStopPlayableBehaviour : PlayableBehaviourCustom
    {
        protected SoundManager manager => SoundManager.Singleton;

        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            manager?.StopBGM();
        }
    }
}
