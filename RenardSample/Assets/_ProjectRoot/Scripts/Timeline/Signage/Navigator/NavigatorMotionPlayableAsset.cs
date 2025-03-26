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
    public class NavigatorMotionPlayableAsset : PlayableAssetCustom<NavigatorMotionPlayableBehaviour>
    {
        [SerializeField] protected AnimationClip animationClip = null;
        [SerializeField] protected bool resetPosition = false;

        private float _fadeLength = 0f;

        protected override void OnCreatePlayable(ref NavigatorMotionPlayableBehaviour playableBehaviour, PlayableGraph graph, GameObject go)
        {
            playableBehaviour?.Settings(animationClip, _fadeLength, resetPosition);
        }

        public void SetupClipData(TimelineClip clip)
        {
            if (clip == null)
                return;

            if (animationClip != null)
            {
                clip.displayName = animationClip.name;

                // ClipLengthに合わせる（Loopingは除く）
                if (!animationClip.isLooping && clip.duration != animationClip.length)
                    clip.duration = animationClip.length;
            }
            else
            {
                clip.displayName ="Not Clip";
            }

            _fadeLength = (float)clip.easeInDuration;
        }
    }

#if UNITY_EDITOR

    [CustomTimelineEditor(typeof(NavigatorMotionPlayableAsset))]
    public class NavigatorMotionPlayableClipEditor : ClipEditor
    {
        public override void OnClipChanged(TimelineClip clip)
        {
            var asset = clip.asset as NavigatorMotionPlayableAsset;
            asset?.SetupClipData(clip);
        }
    }

#endif
}
