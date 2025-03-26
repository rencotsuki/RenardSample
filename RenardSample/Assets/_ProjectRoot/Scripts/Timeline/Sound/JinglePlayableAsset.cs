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
    public class JinglePlayableAsset : PlayableAssetCustom<JinglePlayableBehaviour>
    {
        [SerializeField] private AudioClip jingleClip = null;
        [SerializeField, Range(0f, 1f)] private float jingleVolume = 1f;

        public const float DefaultLength = 3f;
        private string clipName => jingleClip != null ? jingleClip.name : "jingleClip";
        private float clipLength => jingleClip != null ? jingleClip.length : DefaultLength;

        protected override void OnCreatePlayable(ref JinglePlayableBehaviour playableBehaviour, PlayableGraph graph, GameObject go)
        {
            playableBehaviour?.Settings(jingleClip, jingleVolume);
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

    [CustomTimelineEditor(typeof(JinglePlayableAsset))]
    public class JinglePlayableClipEditor : ClipEditor
    {
        public override void OnClipChanged(TimelineClip clip)
        {
            var asset = clip.asset as JinglePlayableAsset;
            asset?.SetupClipData(clip);
        }
    }

#endif
}
