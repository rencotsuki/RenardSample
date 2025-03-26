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
    public class SEPlayableAsset : PlayableAssetCustom<SEPlayableBehaviour>
    {
        [SerializeField] private AudioClip seClip = null;
        [SerializeField, Range(0f, 1f)] private float seVolume = 1f;

        public const float DefaultLength = 3f;
        private string clipName => seClip != null ? seClip.name : "seClip";
        private float clipLength => seClip != null ? seClip.length : DefaultLength;

        protected override void OnCreatePlayable(ref SEPlayableBehaviour playableBehaviour, PlayableGraph graph, GameObject go)
        {
            playableBehaviour?.Settings(seClip, seVolume);
        }

        public void SetupClipData(TimelineClip clip)
        {
            if (clip == null)
                return;

            if (clip.displayName != clipName)
            {
                clip.displayName = clipName;
                clip.duration = clipLength;
            }
        }
    }

#if UNITY_EDITOR

    [CustomTimelineEditor(typeof(SEPlayableAsset))]
    public class SEPlayableClipEditor : ClipEditor
    {
        public override void OnClipChanged(TimelineClip clip)
        {
            var asset = clip.asset as SEPlayableAsset;
            asset?.SetupClipData(clip);
        }
    }

#endif
}
