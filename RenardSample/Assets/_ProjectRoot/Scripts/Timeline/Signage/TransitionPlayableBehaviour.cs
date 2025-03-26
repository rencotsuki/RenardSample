using System;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Playables;

namespace SignageHADO.Timeline
{
    [Serializable]
    [DisplayName("Transition")]
    public class TransitionPlayableBehaviour : PlayableBehaviourCustom
    {
        protected Color graphicColor = Color.black;
        protected float fadeIn = 0f;
        protected float fadeOut = 0f;
        protected bool isCustom = false;
        protected TransitionParameter customParameter = null;

        public const float MinFadeDuration = 0.05f;

        protected SystemManager manager => SystemManager.Singleton;

        public void Settings(Color graphicColor, float fadeIn, float fadeOut, bool isCustom, TransitionParameter customParameter)
        {
            this.graphicColor = graphicColor;
            this.fadeIn = fadeIn < MinFadeDuration ? 0f : fadeIn;
            this.fadeOut = fadeOut < MinFadeDuration ? 0f : fadeOut;
            this.isCustom = isCustom;
            this.customParameter = customParameter;
        }

        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            if (fadeIn >= MinFadeDuration)
            {
                if (isCustom)
                {
                    manager?.ScreenTransition(graphicColor, customParameter, fadeIn);
                }
                else
                {
                    manager?.ScreenTransitionFadeIn(graphicColor, fadeIn);
                }
            }
        }

        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            if (fadeOut >= MinFadeDuration)
                manager?.ScreenTransitionFadeOut(graphicColor, fadeOut);
        }
    }
}
