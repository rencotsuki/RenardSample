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
    public class VoicePlayableAsset : PlayableAssetCustom<VoicePlayableBehaviour>
    {
        [SerializeField] private AudioClip voiceClip = null;
        [SerializeField, Range(0f, 1f)] private float voiceVolume = 1f;

        protected override void OnCreatePlayable(ref VoicePlayableBehaviour playableBehaviour, PlayableGraph graph, GameObject go)
        {
            playableBehaviour?.Settings(voiceClip, voiceVolume);
        }

        public void SetupClipData(TimelineClip clip)
        {
            if (clip == null)
                return;

            clip.displayName = voiceClip != null ? voiceClip.name : "voiceClip";
            clip.duration = voiceClip != null ? voiceClip.length : 1f;
        }
    }

#if UNITY_EDITOR

    [CustomTimelineEditor(typeof(VoicePlayableAsset))]
    public class VoicePlayableClipEditor : ClipEditor
    {
        public override void OnClipChanged(TimelineClip clip)
        {
            var asset = clip.asset as VoicePlayableAsset;
            asset?.SetupClipData(clip);
        }
    }

#endif
}
