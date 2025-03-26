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
    public class SignageNamePlatePlayableAsset : PlayableAssetCustom<SignageNamePlatePlayableBehaviour>
    {
        [SerializeField] protected Sprite spritePlate = null;
        [SerializeField] protected Sprite spriteName = null;
        [SerializeField] protected Sprite spriteNameRole = null;

        private float _showAnima = 0f;
        private float _hideAnima = 0f;

        protected const float defaultDuration = 1f;

        protected override void OnCreatePlayable(ref SignageNamePlatePlayableBehaviour playableBehaviour, PlayableGraph graph, GameObject go)
        {
            playableBehaviour?.Settings(spritePlate, spriteName, spriteNameRole, _showAnima, _hideAnima);
        }

        public void SetupClipData(TimelineClip clip)
        {
            if (clip == null)
                return;

            clip.displayName = "NamePlate";

            // 0以下は認めない
            if (clip.duration <= 0)
                clip.duration = defaultDuration;

            _showAnima = clip.blendInDuration > 0 ? (float)clip.blendInDuration : (float)clip.easeInDuration;
            _hideAnima = (float)clip.easeOutDuration;
        }
    }

#if UNITY_EDITOR

    [CustomTimelineEditor(typeof(SignageNamePlatePlayableAsset))]
    public class SignageNamePlatePlayableClipEditor : ClipEditor
    {
        public override void OnClipChanged(TimelineClip clip)
        {
            var asset = clip.asset as SignageNamePlatePlayableAsset;
            asset?.SetupClipData(clip);
        }
    }

#endif
}
