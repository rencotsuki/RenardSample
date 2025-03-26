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
    public class GameResultPlayableAsset : PlayableAssetCustom<GameResultPlayableBehaviour>
    {
        [SerializeField] protected AnimationClip animationClip = null;

        protected const float defaultDuration = 1f;

        protected override void OnCreatePlayable(ref GameResultPlayableBehaviour playableBehaviour, PlayableGraph graph, GameObject go)
        {
            playableBehaviour?.Settings(animationClip);
        }

        public void SetupClipData(TimelineClip clip)
        {
            if (clip == null)
                return;

            clip.displayName = "Result";

            if (animationClip != null)
            {
                if(clip.duration <= animationClip.length)
                    clip.duration = animationClip.length;
            }
            else
            {
                clip.duration = defaultDuration;
            }

            if (clip.easeInDuration > 0f)
                clip.easeInDuration = 0f;

            if (clip.easeOutDuration > 0f)
                clip.easeOutDuration = 0f;

            if (clip.blendInDuration > 0f)
                clip.blendInDuration = -1f;

            if (clip.blendOutDuration > 0f)
                clip.blendOutDuration = -1f;
        }
    }

#if UNITY_EDITOR

    [CustomTimelineEditor(typeof(GameResultPlayableAsset))]
    public class GameResultPlayableClipEditor : ClipEditor
    {
        public override void OnClipChanged(TimelineClip clip)
        {
            var asset = clip.asset as GameResultPlayableAsset;
            asset?.SetupClipData(clip);
        }
    }

#endif
}
