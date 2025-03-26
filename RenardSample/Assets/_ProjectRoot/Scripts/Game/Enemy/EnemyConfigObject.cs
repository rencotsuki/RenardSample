using System;
using UnityEngine;
using UnityEngine.Playables;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SignageHADO.Game
{
    [Serializable]
    public class EnemyInfo
    {
        public int No = 0;
        public string Name = string.Empty;
        public int Point = 0;

        public static EnemyInfo CreateCopy(EnemyInfo info)
        {
            return new EnemyInfo()
            {
                No = info != null ? info.No : 0,
                Name = info != null ? info.Name : string.Empty,
                Point = info != null ? info.Point : 0
            };
        }
    }

    [Serializable]
    public class EnemySE
    {
        public string SpawnSE = string.Empty;
        public string HitSE = string.Empty;
        public string BreakSE = string.Empty;
    }

    [Serializable]
    public class EnemyData : EnemyInfo
    {
        public int PoolNum = 1;
        public string ObjectName = string.Empty;
        public EnemySE EnemySE = null;
    }

    [Serializable]
    public class EnemyScoreBonus
    {
        public int Count = 0;
        public int Point = 0;
    }

    [Serializable]
    public class EnemyScoreRank
    {
        public GameScoreRank Rank = GameScoreRank.C;
        public int Point = 0;
    }

    [Serializable]
    public class EnemyConfigObject : MonoBehaviourCustom
    {
        public const string FileName = "EnemyConfig";

        [SerializeField] private PlayableAsset[] timelineAssets = default;
        [SerializeField] private EnemyScoreBonus[] comboBonus = null;
        [SerializeField] private EnemyScoreBonus[] killBonus = null;
        [SerializeField] private EnemyScoreRank[] scoreRank = new EnemyScoreRank[]
        {
            new EnemyScoreRank() { Rank = GameScoreRank.S, Point = 0 },
            new EnemyScoreRank() { Rank = GameScoreRank.A, Point = 0 },
            new EnemyScoreRank() { Rank = GameScoreRank.B, Point = 0 },
            new EnemyScoreRank() { Rank = GameScoreRank.C, Point = 0 },
        };
        [SerializeField] private EnemyData[] enemies = null;

        public EnemyScoreBonus[] ComboBonus => comboBonus;

        public EnemyScoreBonus[] KillBonus => killBonus;

        public EnemyScoreRank[] ScoreRank => scoreRank;

        public EnemyData[] EnemyList => enemies;

        public EnemyData GetEnemyData(int enemyNo)
        {
            if (enemyNo > 0)
            {
                foreach (var enemy in enemies)
                {
                    if (enemy.No == enemyNo)
                        return enemy;
                }
            }
            return null;
        }

        public PlayableAsset GetTimelineData(int index)
        {
            if (index >= 0)
            {
                if (timelineAssets.Length > index)
                    return timelineAssets[index];
            }
            return null;
        }
    }

#if UNITY_EDITOR

    [CustomPropertyDrawer(typeof(EnemyData))]
    public class EnemyDataDrawer : PropertyDrawer
    {
        static readonly GUIContent noLabel = new GUIContent("No.");
        static readonly GUIContent nameLabel = new GUIContent("名称");
        static readonly GUIContent pointLabel = new GUIContent("点数");
        static readonly GUIContent poolNumLabel = new GUIContent("ﾌﾟｰﾙ数");
        static readonly GUIContent objectNameLabel = new GUIContent("ｵﾌﾞｼﾞｪｸﾄ名");
        static readonly GUIContent enemySENameLabel = new GUIContent("効果音");

        const float lineSpace = 2f;
        const float nameFieldWidth = 250f;
        const float intFieldWidth = 150f;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return (EditorGUIUtility.singleLineHeight + lineSpace) * 6 + EditorGUI.GetPropertyHeight(property.FindPropertyRelative("EnemySE"), true);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var noProperty = property.FindPropertyRelative("No");
            var nameProperty = property.FindPropertyRelative("Name");
            var pointProperty = property.FindPropertyRelative("Point");
            var poolNumProperty = property.FindPropertyRelative("PoolNum");
            var objectNameProperty = property.FindPropertyRelative("ObjectName");
            var enemySEProperty = property.FindPropertyRelative("EnemySE");

            EditorGUIUtility.labelWidth = 100f;

            label = EditorGUI.BeginProperty(position, label, property);
            label.text = $"No.{noProperty.intValue}: {nameProperty.stringValue}";
            Rect contentPosition = EditorGUI.PrefixLabel(position, label);

            EditorGUI.indentLevel = 0;
            EditorGUIUtility.labelWidth = 80f;

            contentPosition.height = EditorGUIUtility.singleLineHeight;
            contentPosition.x -= EditorGUIUtility.labelWidth;
            contentPosition.y += EditorGUIUtility.singleLineHeight + lineSpace;

            contentPosition.width = intFieldWidth;
            EditorGUI.PropertyField(contentPosition, noProperty, noLabel);
            contentPosition.y += EditorGUIUtility.singleLineHeight + lineSpace;

            contentPosition.width = nameFieldWidth;
            EditorGUI.PropertyField(contentPosition, nameProperty, nameLabel);
            contentPosition.y += EditorGUIUtility.singleLineHeight + lineSpace;

            contentPosition.width = nameFieldWidth;
            EditorGUI.PropertyField(contentPosition, objectNameProperty, objectNameLabel);
            contentPosition.y += EditorGUIUtility.singleLineHeight + lineSpace;

            contentPosition.width = intFieldWidth;
            EditorGUI.PropertyField(contentPosition, pointProperty, pointLabel);
            contentPosition.y += EditorGUIUtility.singleLineHeight + lineSpace;

            contentPosition.width = intFieldWidth;
            EditorGUI.PropertyField(contentPosition, poolNumProperty, poolNumLabel);
            contentPosition.y += EditorGUIUtility.singleLineHeight + lineSpace;

            contentPosition.width = nameFieldWidth;
            EditorGUI.PropertyField(contentPosition, enemySEProperty, enemySENameLabel, true);
            contentPosition.y += EditorGUIUtility.singleLineHeight + lineSpace;

            EditorGUI.EndProperty();
        }
    }

    [CustomPropertyDrawer(typeof(EnemyScoreBonus))]
    public class EnemyScoreBonusDrawer : PropertyDrawer
    {
        static readonly GUIContent countLabel = new GUIContent("条件数");
        static readonly GUIContent pointLabel = new GUIContent("ボーナス点数");

        const float lineSpace = 2f;
        const float nameFieldWidth = 250f;
        const float intFieldWidth = 150f;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return (EditorGUIUtility.singleLineHeight + lineSpace) * 3;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var countProperty = property.FindPropertyRelative("Count");
            var pointProperty = property.FindPropertyRelative("Point");

            EditorGUIUtility.labelWidth = 100f;

            label = EditorGUI.BeginProperty(position, label, property);
            label.text = $"No.{int.Parse(property.propertyPath.Split('[', ']')[1])}";
            Rect contentPosition = EditorGUI.PrefixLabel(position, label);

            EditorGUI.indentLevel = 0;
            EditorGUIUtility.labelWidth = 80f;

            contentPosition.height = EditorGUIUtility.singleLineHeight;
            contentPosition.x -= EditorGUIUtility.labelWidth;
            contentPosition.y += EditorGUIUtility.singleLineHeight + lineSpace;

            contentPosition.width = intFieldWidth;
            EditorGUI.PropertyField(contentPosition, countProperty, countLabel);
            contentPosition.y += EditorGUIUtility.singleLineHeight + lineSpace;

            contentPosition.width = nameFieldWidth;
            EditorGUI.PropertyField(contentPosition, pointProperty, pointLabel);
            contentPosition.y += EditorGUIUtility.singleLineHeight + lineSpace;

            EditorGUI.EndProperty();
        }
    }

    [CustomPropertyDrawer(typeof(EnemyScoreRank))]
    public class EnemyScoreRankDrawer : PropertyDrawer
    {
        static readonly GUIContent rankLabel = new GUIContent("ランク");
        static readonly GUIContent pointLabel = new GUIContent("条件点数");

        const float lineSpace = 2f;
        const float nameFieldWidth = 250f;
        const float intFieldWidth = 150f;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return (EditorGUIUtility.singleLineHeight + lineSpace) * 3;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var rankProperty = property.FindPropertyRelative("Rank");
            var pointProperty = property.FindPropertyRelative("Point");

            EditorGUIUtility.labelWidth = 100f;

            label = EditorGUI.BeginProperty(position, label, property);
            label.text = $"ランク:{(GameScoreRank)rankProperty.enumValueIndex}";
            Rect contentPosition = EditorGUI.PrefixLabel(position, label);

            EditorGUI.indentLevel = 0;
            EditorGUIUtility.labelWidth = 80f;

            contentPosition.height = EditorGUIUtility.singleLineHeight;
            contentPosition.x -= EditorGUIUtility.labelWidth;
            contentPosition.y += EditorGUIUtility.singleLineHeight + lineSpace;

            contentPosition.width = intFieldWidth;
            EditorGUI.PropertyField(contentPosition, rankProperty, rankLabel);
            contentPosition.y += EditorGUIUtility.singleLineHeight + lineSpace;

            contentPosition.width = nameFieldWidth;
            EditorGUI.PropertyField(contentPosition, pointProperty, pointLabel);
            contentPosition.y += EditorGUIUtility.singleLineHeight + lineSpace;

            EditorGUI.EndProperty();
        }
    }

    [CustomEditor(typeof(EnemyConfigObject))]
    public class EnemyConfigAssetEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            using (new EditorGUI.DisabledGroupScope(true))
            {
                EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour((MonoBehaviour)target), typeof(EnemyConfigObject), false);
            }

            EditorGUILayout.PropertyField(serializedObject.FindProperty("timelineAssets"), new GUIContent("Timeline情報"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("comboBonus"), new GUIContent("ｺﾝﾎﾞﾎﾞｰﾅｽ"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("killBonus"), new GUIContent("撃破ﾎﾞｰﾅｽ"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("scoreRank"), new GUIContent("ﾗﾝｸ設定"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("enemies"), new GUIContent("敵情報"));

            serializedObject.ApplyModifiedProperties();
        }
    }

#endif
}
