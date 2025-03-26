using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Timeline;
#endif

namespace SignageHADO.Timeline
{
    [TrackClipType(typeof(SEPlayableAsset))]
    public class SETrack : TrackAssetCustom
    {
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            SEPlayableAsset asset = null;

            foreach (var clip in GetClips())
            {
                asset = clip.asset as SEPlayableAsset;
                asset?.SetupClipData(clip);
            }

            return base.CreateTrackMixer(graph, go, inputCount);
        }
    }

#if UNITY_EDITOR

    [CustomTimelineEditor(typeof(SETrack))]
    public class SETrackEditor : TrackEditor
    {
        public override TrackDrawOptions GetTrackOptions(TrackAsset track, UnityEngine.Object binding)
        {
            return new TrackDrawOptions()
            {
                errorText = GetErrorText(track, binding, TrackBindingErrors.All),
                minimumHeight = DefaultTrackHeight,
                trackColor = GetTrackColor(track),
                icon = (Texture2D)EditorGUIUtility.Load("AudioClip Icon")
            };
        }
    }

#endif
}
