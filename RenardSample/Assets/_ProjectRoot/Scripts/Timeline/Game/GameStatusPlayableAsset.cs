using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
using UnityEditor.Timeline;
#endif

namespace SignageHADO.Timeline
{
    [Serializable]
    public class GameStatusPlayableAsset : PlayableAssetCustom<GameStatusPlayableBehaviour>
    {
        [SerializeField] protected GameStatus gameStatus = GameStatus.None;

        [SerializeField, GameStatusCustom("gameStatus", GameStatus.Playing, GameStatus.End, GameStatus.Result)]
        protected bool finishEsc = false;

        public GameStatus GameStatus => gameStatus;

        protected const float defaultDuration = 5f;
        protected float clipDuration = 0f;

        protected override void OnCreatePlayable(ref GameStatusPlayableBehaviour playableBehaviour, PlayableGraph graph, GameObject go)
        {
            playableBehaviour?.Settings(gameStatus, finishEsc);
        }

        public void SetupClipData(TimelineClip clip)
        {
            if (clip == null)
                return;

            clip.displayName = gameStatus.ToString();

            // 0以下は認めない
            if (clip.duration <= 0)
                clip.duration = defaultDuration;
        }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class GameStatusCustomAttribute : PropertyAttribute
    {
        public string switcherFieldName = string.Empty;
        public GameStatus[] enableIfValueIs = null;

        public GameStatusCustomAttribute(string switcherFieldName, params GameStatus[] enableIfValueIs)
        {
            this.switcherFieldName = switcherFieldName;
            this.enableIfValueIs = enableIfValueIs;
        }
    }

#if UNITY_EDITOR

    [CustomTimelineEditor(typeof(GameStatusPlayableAsset))]
    public class GameStatusPlayableClipEditor : ClipEditor
    {
        public override void OnClipChanged(TimelineClip clip)
        {
            var asset = clip.asset as GameStatusPlayableAsset;
            asset?.SetupClipData(clip);
        }
    }

    [CustomPropertyDrawer(typeof(GameStatusCustomAttribute))]
    public class GameStatusCustomDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var attr = attribute as GameStatusCustomAttribute;

            if (attr != null && !string.IsNullOrEmpty(attr.switcherFieldName))
            {
                if (!GetIsVisible(attr, property))
                    return;
            }

            EditorGUI.BeginProperty(position, label, property);
            EditorGUI.PropertyField(position, property, label, true);
            EditorGUI.EndProperty();
        }

        private bool GetIsVisible(GameStatusCustomAttribute attribute, SerializedProperty property)
        {
            try
            {
                return attribute.enableIfValueIs != null ? attribute.enableIfValueIs.Contains(GetSwitcherPropertyValue(attribute, property)) : false;
            }
            catch (Exception ex)
            {
                Debug.Log($"hoge: {ex}");
            }
            return false;
        }

        private GameStatus GetSwitcherPropertyValue(GameStatusCustomAttribute attribute, SerializedProperty property)
        {
            var propertyNameIndex = property.propertyPath.LastIndexOf(property.name, StringComparison.Ordinal);
            var switcherPropertyName = property.propertyPath.Substring(0, propertyNameIndex) + attribute.switcherFieldName;
            return (GameStatus)property.serializedObject.FindProperty(switcherPropertyName).enumValueFlag;
        }
    }

#endif
}
