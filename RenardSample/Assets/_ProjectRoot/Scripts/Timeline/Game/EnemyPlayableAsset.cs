using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Timeline;
#endif

namespace SignageHADO.Timeline
{
    using Game;

    [Serializable]
    public class EnemyPlayableAsset : PlayableAssetCustom<EnemyPlayableBehaviour>
    {
        [SerializeField] protected int emenyNo = 0;
        [SerializeField] protected EmenyAction emenyAction = null;

        protected const float defaultDuration = 5f;

        private float _duration = 0f;
        private float _runWait = 0f;

        protected override void OnCreatePlayable(ref EnemyPlayableBehaviour playableBehaviour, PlayableGraph graph, GameObject go)
        {
            playableBehaviour?.Settings(emenyNo, emenyAction, _duration, _runWait);
        }

        public void SetupClipData(TimelineClip clip)
        {
            if (clip == null)
                return;

            clip.displayName = (emenyNo > 0 ? $"enemy{emenyNo}" : "---");

            // 0以下は認めない
            if (clip.duration <= 0)
                clip.duration = defaultDuration;

            _duration = (float)clip.duration;
            _runWait = (float)clip.easeInDuration;
        }
    }

#if UNITY_EDITOR

    [CustomTimelineEditor(typeof(EnemyPlayableAsset))]
    public class EnemyPlayableClipEditor : ClipEditor
    {
        public override void OnClipChanged(TimelineClip clip)
        {
            var asset = clip.asset as EnemyPlayableAsset;
            asset?.SetupClipData(clip);
        }
    }

    [CustomEditor(typeof(EnemyPlayableAsset))]
    [CanEditMultipleObjects]
    public class EnemyPlayableAssetEditor : Editor
    {
        // 複数選択編集ができるように
    }

#endif
}
