using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Timeline;
#endif

namespace SignageHADO.Timeline
{
    [TrackClipType(typeof(TransitionPlayableAsset))]
    public class TransitionTrack : TrackAssetCustom
    {
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            TransitionPlayableAsset asset = null;

            foreach (var clip in GetClips())
            {
                asset = clip.asset as TransitionPlayableAsset;
                asset?.SetupClipData(clip);
            }

            return base.CreateTrackMixer(graph, go, inputCount);
        }

        protected override void OnBeforeTrackSerialize()
        {
            base.OnBeforeTrackSerialize();
        }
    }

#if UNITY_EDITOR

    [CustomTimelineEditor(typeof(TransitionTrack))]
    public class TransitionTrackEditor : TrackEditor
    {
        public override TrackDrawOptions GetTrackOptions(TrackAsset track, UnityEngine.Object binding)
        {
            return new TrackDrawOptions()
            {
                errorText = GetErrorText(track, binding, TrackBindingErrors.All),
                minimumHeight = DefaultTrackHeight,
                trackColor = GetTrackColor(track),
                icon = (Texture2D)EditorGUIUtility.Load("GUILayer Icon")
            };
        }
    }

#endif
}
