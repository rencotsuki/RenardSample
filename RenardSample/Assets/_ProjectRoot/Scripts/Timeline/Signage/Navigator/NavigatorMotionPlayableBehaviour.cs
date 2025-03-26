using System;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Playables;

namespace SignageHADO.Timeline
{
    [Serializable]
    [DisplayName("NavigatorMotion")]
    public class NavigatorMotionPlayableBehaviour : PlayableBehaviourCustom
    {
        protected AnimationClip animationClip = null;
        protected float fadeLength = 0f;
        protected bool resetPosition = false;

        protected TimelineMotionHandler handler => SignageHandler.NavigateAnimator;

        public void Settings(AnimationClip animationClip, float fadeLength, bool resetPosition)
        {
            this.animationClip = animationClip;
            this.fadeLength = fadeLength;
            this.resetPosition = resetPosition;
        }

        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            if (resetPosition)
            {
                handler?.ResetPose(fadeLength, animationClip);
            }
            else
            {
                handler?.CrossFade(animationClip, fadeLength);
            }
        }
    }
}
