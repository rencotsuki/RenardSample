using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Timeline;
#endif

namespace SignageHADO.Timeline
{
    [TrackClipType(typeof(SignageInfomationPlayableAsset))]
    [TrackClipType(typeof(SignageNamePlatePlayableAsset))]
    public class SignageUITrack : TrackAssetCustom
    {
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            SignageInfomationPlayableAsset assetInfo = null;
            SignageNamePlatePlayableAsset assetName = null;

            foreach (var clip in GetClips())
            {
                assetInfo = clip.asset as SignageInfomationPlayableAsset;
                assetInfo?.SetupClipData(clip);

                assetName = clip.asset as SignageNamePlatePlayableAsset;
                assetName?.SetupClipData(clip);
            }

            return base.CreateTrackMixer(graph, go, inputCount);
        }

        protected override void OnBeforeTrackSerialize()
        {
            base.OnBeforeTrackSerialize();
        }
    }

#if UNITY_EDITOR

    [CustomTimelineEditor(typeof(SignageUITrack))]
    public class SignageUITrackEditor : TrackEditor
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
