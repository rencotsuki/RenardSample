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
    public class GameSignalPlayableAsset : PlayableAssetCustom<GameSignalPlayableBehaviour>
    {
        [SerializeField] protected AnimationClip animationClip = null;
        [SerializeField] protected bool endClose = false;

        protected const float defaultDuration = 1f;

        private float _fadeLength = 0f;

        protected override void OnCreatePlayable(ref GameSignalPlayableBehaviour playableBehaviour, PlayableGraph graph, GameObject go)
        {
            playableBehaviour?.Settings(animationClip, _fadeLength, endClose);
        }

        public void SetupClipData(TimelineClip clip)
        {
            if (clip == null)
                return;

            clip.displayName = animationClip != null ? animationClip.name.ToString() : "no clip.";

            if (animationClip != null)
            {
                if (clip.duration <= animationClip.length)
                    clip.duration = animationClip.length;
            }
            else
            {
                clip.duration = defaultDuration;
            }

            _fadeLength = (float)clip.easeInDuration;
        }
    }

#if UNITY_EDITOR

    [CustomTimelineEditor(typeof(GameSignalPlayableAsset))]
    public class GameSignalPlayableClipEditor : ClipEditor
    {
        public override void OnClipChanged(TimelineClip clip)
        {
            var asset = clip.asset as GameSignalPlayableAsset;
            asset?.SetupClipData(clip);
        }
    }

#endif
}
