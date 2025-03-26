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
    public class BGMPlayableAsset : PlayableAssetCustom<BGMPlayableBehaviour>
    {
        [SerializeField] private AudioClip bgmClip = null;
        [SerializeField, Range(0f, 1f)] private float bgmVolume = 1f;
        [SerializeField] private bool bgmLoop = false;
        [SerializeField] private bool endStopCall = false;

        public const float DefaultLength = 3f;
        private string clipName => bgmClip != null ? bgmClip.name : "bgmClip";
        private float clipLength => bgmClip != null ? bgmClip.length : DefaultLength;

        protected override void OnCreatePlayable(ref BGMPlayableBehaviour playableBehaviour, PlayableGraph graph, GameObject go)
        {
            playableBehaviour?.Settings(bgmClip, bgmVolume, bgmLoop, endStopCall);
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

    [CustomTimelineEditor(typeof(BGMPlayableAsset))]
    public class BGMPlayableClipEditor : ClipEditor
    {
        public override void OnClipChanged(TimelineClip clip)
        {
            var asset = clip.asset as BGMPlayableAsset;
            asset?.SetupClipData(clip);
        }
    }

#endif
}
