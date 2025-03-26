using System;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Playables;

namespace SignageHADO.Timeline
{
    using Game;

    [Serializable]
    [DisplayName("Result")]
    public class GameResultPlayableBehaviour : PlayableBehaviourCustom
    {
        protected AnimationClip animationClip = null;

        protected GameManager manager => GameManager.Singleton;
        protected SignalResultView view => manager != null ? manager.GameSignalResultView : null;

        public void Settings(AnimationClip animationClip)
        {
            this.animationClip = animationClip;
        }

        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            if (view != null)
            {
                view.SetResultScore(manager.GameScore);
                view.HandlerResult?.CrossFade(animationClip);
            }
        }

        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            view?.CloseResult();
        }
    }
}
