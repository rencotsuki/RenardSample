using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Timeline;
#endif

namespace SignageHADO.Timeline
{
    [TrackClipType(typeof(GameSignalPlayableAsset))]
    public class GameSignalTrack : TrackAssetCustom
    {
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            GameSignalPlayableAsset asset = null;

            foreach (var clip in GetClips())
            {
                asset = clip.asset as GameSignalPlayableAsset;
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

    [CustomTimelineEditor(typeof(GameSignalTrack))]
    public class GameSignalTrackEditor : TrackEditor
    {
        public override TrackDrawOptions GetTrackOptions(TrackAsset track, UnityEngine.Object binding)
        {
            return new TrackDrawOptions()
            {
                errorText = GetErrorText(track, binding, TrackBindingErrors.All),
                minimumHeight = DefaultTrackHeight,
                trackColor = GetTrackColor(track),
                icon = (Texture2D)EditorGUIUtility.Load("d_Texture Icon")
            };
        }
    }

#endif
}
