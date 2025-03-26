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
    public class SignageInfomationPlayableAsset : PlayableAssetCustom<SignageInfomationPlayableBehaviour>
    {
        [SerializeField] protected Sprite spriteInfo = null;

        protected const float defaultDuration = 1f;

        private bool _endClose = false;
        private float _fadeIn = 0f;
        private float _fadeOut = 0f;

        protected override void OnCreatePlayable(ref SignageInfomationPlayableBehaviour playableBehaviour, PlayableGraph graph, GameObject go)
        {
            playableBehaviour?.Settings(spriteInfo, _endClose, _fadeIn, _fadeOut);
        }

        public void SetupClipData(TimelineClip clip)
        {
            if (clip == null)
                return;

            clip.displayName = spriteInfo != null ? spriteInfo.name : "not image";

            // 0以下は認めない
            if (clip.duration <= 0)
                clip.duration = defaultDuration;

            _fadeIn = clip.blendInDuration > 0 ? (float)clip.blendInDuration : (float)clip.easeInDuration;
            _fadeOut = (float)clip.easeOutDuration;

            // ブレンドされていない時は次がないので消す
            if (clip.blendOutDuration <= 0)
                _endClose = true;
        }
    }

#if UNITY_EDITOR

    [CustomTimelineEditor(typeof(SignageInfomationPlayableAsset))]
    public class SignageUIPlayableClipEditor : ClipEditor
    {
        public override void OnClipChanged(TimelineClip clip)
        {
            var asset = clip.asset as SignageInfomationPlayableAsset;
            asset?.SetupClipData(clip);
        }
    }

#endif
}
