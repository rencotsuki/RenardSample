using System;
using UnityEngine.Timeline;

#if UNITY_EDITOR
using UnityEditor.Timeline;
#endif

namespace SignageHADO.Timeline
{
    [Serializable]
    public class BGMStopPlayableAsset : PlayableAssetCustom<BGMStopPlayableBehaviour>
    {
        public void SetupClipData(TimelineClip clip)
        {
            if (clip == null)
                return;

            clip.displayName = "BGMStop";
        }
    }

#if UNITY_EDITOR

    [CustomTimelineEditor(typeof(BGMStopPlayableAsset))]
    public class BGMStopPlayableClipEditor : ClipEditor
    {
        public override void OnClipChanged(TimelineClip clip)
        {
            var asset = clip.asset as BGMStopPlayableAsset;
            asset?.SetupClipData(clip);
        }
    }

#endif
}
