using System;
using System.ComponentModel;
using UnityEngine.Playables;

namespace SignageHADO.Timeline
{
    public interface NavigatorFacial
    {
        void SetValue(float value);
    }

    [Serializable]
    [DisplayName("NavigatorFacial")]
    public class NavigatorFacialPlayableBehaviour : PlayableBehaviourCustom
    {
        protected ExpressionEnum expression = ExpressionEnum.Neutral;
        protected float blendTime = 0f;
        protected float blendOutDuration = 0f; 

        protected SignageManager manager => SignageManager.Singleton;

        public void Settings(ExpressionEnum expression, float blendTime, float blendOutDuration)
        {
            this.expression = expression;
            this.blendTime = blendTime <= 0f ? 0f : blendTime;
            this.blendOutDuration = blendOutDuration;
        }

        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            manager?.SetFacialExpression(expression, blendTime);
        }

        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            // ブレンドされていない時はNeutralに戻す
            if (blendOutDuration < 0)
                manager?.SetFacialExpression(ExpressionEnum.Neutral);
        }
    }
}
