using System;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Playables;

namespace SignageHADO.Timeline
{
    using Game;

    [Serializable]
    [DisplayName("Signal")]
    public class GameSignalPlayableBehaviour : PlayableBehaviourCustom
    {
        protected AnimationClip animationClip = null;
        protected float fadeLength = 0f;
        protected bool endClose = false;

        protected GameManager manager => GameManager.Singleton;
        protected SignalResultView view => manager != null ? manager.GameSignalResultView : null;

        public void Settings(AnimationClip animationClip, float fadeLength, bool endClose)
        {
            this.animationClip = animationClip;
            this.fadeLength = fadeLength;
            this.endClose = endClose;
        }

        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            if (view != null)
                view.HandlerSignal?.CrossFade(animationClip);
        }

        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            if (endClose)
                view?.CloseSignal();
        }
    }
}
