using System;
using System.ComponentModel;
using UnityEngine.Playables;

namespace SignageHADO.Timeline
{
    [Serializable]
    [DisplayName("NavigatorLines")]
    public class NavigatorLinesPlayableBehaviour : PlayableBehaviourCustom
    {
        protected string lines = string.Empty;

        protected SystemManager manager => SystemManager.Singleton;

        public void Settings(string lines)
        {
            this.lines = lines;
        }

        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            manager?.ShowLines(lines);
        }

        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            manager?.HideLines();
        }
    }
}
