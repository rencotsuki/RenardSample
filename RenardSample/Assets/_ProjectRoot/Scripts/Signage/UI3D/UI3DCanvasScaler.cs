using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SignageHADO.UI
{
    public class UI3DCanvasScaler : CanvasScaler
    {
        [SerializeField] protected ScaleMode m_Mode = ScaleMode.ScaleWithScreenSize;
        public ScaleMode Mode
        {
            get => m_Mode;
            set => uiScaleMode = m_Mode = value;
        }

        [SerializeField] protected Vector2 m_ResolutionSize = new Vector2(1920f, 1080f);
        public Vector2 ResolutionSize => m_ResolutionSize;

        [SerializeField] protected float m_PixelsPerUnit = 100;
        public float PixelsPerUnit => m_PixelsPerUnit;

        [SerializeField] protected ScreenMatchMode m_MatchMode = ScreenMatchMode.Expand;
        public ScreenMatchMode MatchMode => m_MatchMode;

        protected override void Awake()
        {
            uiScaleMode = m_Mode;

            if (uiScaleMode == ScaleMode.ScaleWithScreenSize)
            {
                Set(m_ResolutionSize, m_PixelsPerUnit, m_MatchMode);
            }
            else
            {
                referenceResolution = m_ResolutionSize;
                referencePixelsPerUnit = m_PixelsPerUnit;
            }

            base.Awake();
        }

        public void Set(Vector2 resolutionSize, float pixelsPerUnit, ScreenMatchMode matchMode)
        {
            Mode = ScaleMode.ScaleWithScreenSize;

            screenMatchMode = m_MatchMode = matchMode;
            referenceResolution = m_ResolutionSize = resolutionSize;
            referencePixelsPerUnit = m_PixelsPerUnit = pixelsPerUnit;
        }

#if UNITY_EDITOR

        [CustomEditor(typeof(UI3DCanvasScaler))]
        public class CharacterEditor : Editor
        {
            private UI3DCanvasScaler canvasScaler = null;

            private SerializedProperty m_UIScaleMode = null;

            private SerializedProperty m_ScaleFactor = null;
            private SerializedProperty m_PhysicalUnit = null;
            private SerializedProperty m_FallbackScreenDPI = null;
            private SerializedProperty m_DefaultSpriteDPI = null;
            private SerializedProperty m_ReferencePixelsPerUnit = null;

            private SerializedProperty m_ResolutionSize = null;
            private SerializedProperty m_PixelsPerUnit = null;
            private SerializedProperty m_MatchMode = null;

            private void OnEnable()
            {
                canvasScaler = (UI3DCanvasScaler)target;

                m_UIScaleMode = serializedObject.FindProperty("m_Mode");

                m_ScaleFactor = serializedObject.FindProperty("m_ScaleFactor");
                m_PhysicalUnit = serializedObject.FindProperty("m_PhysicalUnit");
                m_FallbackScreenDPI = serializedObject.FindProperty("m_FallbackScreenDPI");
                m_DefaultSpriteDPI = serializedObject.FindProperty("m_DefaultSpriteDPI");
                m_ReferencePixelsPerUnit = serializedObject.FindProperty("m_ReferencePixelsPerUnit");

                m_ResolutionSize = serializedObject.FindProperty("m_ResolutionSize");
                m_PixelsPerUnit = serializedObject.FindProperty("m_PixelsPerUnit");
                m_MatchMode = serializedObject.FindProperty("m_MatchMode");
            }

            public override void OnInspectorGUI()
            {
                // シリアライズオブジェクトの更新
                serializedObject.Update();

                EditorGUILayout.BeginVertical();

                if (canvasScaler != null)
                {
                    if (m_UIScaleMode != null) EditorGUILayout.PropertyField(m_UIScaleMode);

                    if (canvasScaler.uiScaleMode != canvasScaler.Mode)
                        canvasScaler.uiScaleMode = canvasScaler.Mode;

                    if (canvasScaler.uiScaleMode == ScaleMode.ConstantPixelSize)
                    {
                        if (m_ScaleFactor != null) EditorGUILayout.PropertyField(m_ScaleFactor);
                        if (m_ReferencePixelsPerUnit != null) EditorGUILayout.PropertyField(m_ReferencePixelsPerUnit);
                    }

                    if (canvasScaler.uiScaleMode == ScaleMode.ConstantPhysicalSize)
                    {
                        if (m_PhysicalUnit != null) EditorGUILayout.PropertyField(m_PhysicalUnit);
                        if (m_FallbackScreenDPI != null) EditorGUILayout.PropertyField(m_FallbackScreenDPI);
                        if (m_DefaultSpriteDPI != null) EditorGUILayout.PropertyField(m_DefaultSpriteDPI);
                        if (m_ReferencePixelsPerUnit != null) EditorGUILayout.PropertyField(m_ReferencePixelsPerUnit);
                    }

                    if (canvasScaler.uiScaleMode == ScaleMode.ScaleWithScreenSize)
                    {
                        if (m_ResolutionSize != null) EditorGUILayout.PropertyField(m_ResolutionSize);
                        if (m_PixelsPerUnit != null) EditorGUILayout.PropertyField(m_PixelsPerUnit);
                        if (m_MatchMode != null) EditorGUILayout.PropertyField(m_MatchMode);
                    }
                }

                EditorGUILayout.EndVertical();

                serializedObject.ApplyModifiedProperties();
            }
        }

#endif
    }
}