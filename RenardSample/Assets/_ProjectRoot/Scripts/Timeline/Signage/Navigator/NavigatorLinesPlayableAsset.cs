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
    public class NavigatorLinesPlayableAsset : PlayableAssetCustom<NavigatorLinesPlayableBehaviour>
    {
        [SerializeField, TextArea] protected string lines = string.Empty;

        protected override void OnCreatePlayable(ref NavigatorLinesPlayableBehaviour playableBehaviour, PlayableGraph graph, GameObject go)
        {
            playableBehaviour?.Settings(lines);
        }

        public void SetupClipData(TimelineClip clip)
        {
            if (clip == null)
                return;

            clip.displayName = string.IsNullOrEmpty(lines) ? "---" : lines.Substring(0, lines.Length < 5 ? lines.Length : 5);

            // ChangeLinesTime未満は認めない
            if (clip.duration < SignageView.ChangeLinesTime)
                clip.duration = SignageView.ChangeLinesTime;

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

    [CustomTimelineEditor(typeof(NavigatorLinesPlayableAsset))]
    public class NavigatorLinesPlayableClipEditor : ClipEditor
    {
        public override void OnClipChanged(TimelineClip clip)
        {
            var asset = clip.asset as NavigatorLinesPlayableAsset;
            asset?.SetupClipData(clip);
        }
    }

#endif
}
