using System;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Playables;

namespace SignageHADO.Timeline
{
    [Serializable]
    [DisplayName("SignageUI")]
    public class SignageInfomationPlayableBehaviour : PlayableBehaviourCustom
    {
        protected Sprite spriteInfo = null;
        protected bool endClose = false;
        protected float fadeIn = 0f;
        protected float fadeOut = 0f;

        protected SystemManager manager => SystemManager.Singleton;

        public void Settings(Sprite spriteInfo, bool endClose, float fadeIn, float fadeOut)
        {
            this.spriteInfo = spriteInfo;
            this.endClose = endClose;
            this.fadeIn = fadeIn;
            this.fadeOut = fadeOut;
        }

        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            manager?.ShowInfomation(spriteInfo, fadeIn);
        }

        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            if (endClose)
                manager?.HideInfomation(fadeOut);
        }
    }
}
