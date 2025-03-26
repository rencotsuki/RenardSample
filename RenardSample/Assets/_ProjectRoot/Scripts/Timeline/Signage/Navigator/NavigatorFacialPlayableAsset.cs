using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

#if UNITY_EDITOR
using UnityEditor.Timeline;
#endif

namespace SignageHADO.Timeline
{
    [Serializable]
    public class NavigatorFacialPlayableAsset : PlayableAssetCustom<NavigatorFacialPlayableBehaviour>
    {
        [SerializeField] protected ExpressionEnum expression = ExpressionEnum.Neutral;

        protected float blendTime = 0f;
        protected float blendOutDuration = 0f;

        protected override void OnCreatePlayable(ref NavigatorFacialPlayableBehaviour playableBehaviour, PlayableGraph graph, GameObject go)
        {
            playableBehaviour?.Settings(expression, blendTime, blendOutDuration);
        }

        public void SetupClipData(TimelineClip clip)
        {
            if (clip == null)
                return;

            clip.displayName = expression.ToString();

            if (clip.easeOutDuration > 0)
                clip.easeOutDuration = 0;

            blendTime = clip.easeInDuration > 0 ? (float)clip.easeInDuration : (float)clip.blendInDuration;

            blendOutDuration = (float)clip.blendOutDuration;
        }
    }

#if UNITY_EDITOR

    [CustomTimelineEditor(typeof(NavigatorFacialPlayableAsset))]
    public class NavigatorFacialPlayableClipEditor : ClipEditor
    {
        public override void OnClipChanged(TimelineClip clip)
        {
            var asset = clip.asset as NavigatorFacialPlayableAsset;
            asset?.SetupClipData(clip);
        }
    }

#endif
}
