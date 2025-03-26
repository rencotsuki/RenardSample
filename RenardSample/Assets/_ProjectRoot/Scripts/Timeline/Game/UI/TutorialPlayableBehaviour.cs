using System;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Playables;

namespace SignageHADO.Timeline
{
    [Serializable]
    [DisplayName("Tutorial")]
    public class TutorialPlayableBehaviour : PlayableBehaviourCustom
    {
        protected Sprite spriteTutorial = null;
        protected bool endClose = false;
        protected float fadeIn = 0f;
        protected float fadeOut = 0f;
        protected float blendOutDuration = 0f;

        protected SystemManager manager => SystemManager.Singleton;

        public void Settings(Sprite spriteTutorial, bool endClose, float fadeIn, float fadeOut)
        {
            this.spriteTutorial = spriteTutorial;
            this.endClose = endClose;
            this.fadeIn = fadeIn;
            this.fadeOut = fadeOut;
        }

        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            manager?.ShowTutorial(spriteTutorial, fadeIn);
        }

        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            if (endClose)
                manager?.HideTutorial(fadeOut);
        }
    }
}
