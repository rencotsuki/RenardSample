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
    [Serializable]
    public class TransitionPlayableAsset : PlayableAssetCustom<TransitionPlayableBehaviour>
    {
        [SerializeField] protected Color graphicColor = Color.black;
        [SerializeField] protected bool isCustom = false;

        [SerializeField, TransitionCustom("isCustom")]
        protected TransitionParameter customParameter = new TransitionParameter();

        protected const float defaultDuration = 0.5f;
        protected const float defaultFadeDuration = 0.25f;
        protected float minFadeDuration => TransitionPlayableBehaviour.MinFadeDuration;

        protected float fadeIn = 0f;
        protected float fadeOut = 0f;

        protected override void OnCreatePlayable(ref TransitionPlayableBehaviour playableBehaviour, PlayableGraph graph, GameObject go)
        {
            playableBehaviour?.Settings(graphicColor, fadeIn, fadeOut, isCustom, customParameter);
        }

        public void SetupClipData(TimelineClip clip)
        {
            if (clip == null)
                return;

            if (clip.displayName != "Transition")
            {
                clip.displayName = "Transition";
                clip.duration = defaultDuration + defaultFadeDuration * 2;
                clip.easeInDuration = defaultFadeDuration;
                clip.easeOutDuration = defaultFadeDuration;
            }

            if (clip.easeInDuration < minFadeDuration)
                clip.easeInDuration = 0f;

            if (clip.easeOutDuration < minFadeDuration)
                clip.easeOutDuration = 0f;

            fadeIn = (float)clip.easeInDuration;
            fadeOut = (float)clip.easeOutDuration;
        }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class TransitionCustomAttribute : PropertyAttribute
    {
        public string switcherFieldName = string.Empty;

        public TransitionCustomAttribute(string switcherFieldName)
        {
            this.switcherFieldName = switcherFieldName;
        }
    }

#if UNITY_EDITOR

    [CustomTimelineEditor(typeof(TransitionPlayableAsset))]
    public class TransitionPlayableClipEditor : ClipEditor
    {
        public override void OnClipChanged(TimelineClip clip)
        {
            var asset = clip.asset as TransitionPlayableAsset;
            asset?.SetupClipData(clip);
        }
    }

    [CustomPropertyDrawer(typeof(TransitionCustomAttribute))]
    public class TransitionCustomDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var attr = attribute as TransitionCustomAttribute;

            if (attr != null && !string.IsNullOrEmpty(attr.switcherFieldName))
            {
                if (!GetIsVisible(attr, property))
                    return;
            }

            EditorGUI.BeginProperty(position, label, property);
            EditorGUI.PropertyField(position, property, label, true);
            EditorGUI.EndProperty();
        }

        private bool GetIsVisible(TransitionCustomAttribute attribute, SerializedProperty property)
        {
            try
            {
                return GetSwitcherPropertyValue(attribute, property);
            }
            catch (Exception ex)
            {
                Debug.Log($"TransitionCustomDrawer::GetIsVisible - {ex.Message}");
            }
            return false;
        }

        private bool GetSwitcherPropertyValue(TransitionCustomAttribute attribute, SerializedProperty property)
        {
            var propertyNameIndex = property.propertyPath.LastIndexOf(property.name, StringComparison.Ordinal);
            var switcherPropertyName = property.propertyPath.Substring(0, propertyNameIndex) + attribute.switcherFieldName;
            return property.serializedObject.FindProperty(switcherPropertyName).boolValue;
        }
    }

#endif
}
