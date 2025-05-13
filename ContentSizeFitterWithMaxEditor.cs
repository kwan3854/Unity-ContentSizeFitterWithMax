using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
namespace UnityEditor.UI
{
    [CustomEditor(typeof(ContentSizeFitterWithMax), true)]
    [CanEditMultipleObjects]
    public class ContentSizeFitterWithMaxEditor : ContentSizeFitterEditor
    {
        SerializedProperty m_WidthType;
        SerializedProperty m_MaxWidth;
        SerializedProperty m_HeightType;
        SerializedProperty m_MaxHeight;
        SerializedProperty m_UpdateMode;

        protected override void OnEnable()
        {
            base.OnEnable();
            m_WidthType = serializedObject.FindProperty("m_WidthType");
            m_MaxWidth = serializedObject.FindProperty("m_MaxWidth");
            m_HeightType = serializedObject.FindProperty("m_HeightType");
            m_MaxHeight = serializedObject.FindProperty("m_MaxHeight");
            m_UpdateMode = serializedObject.FindProperty("m_UpdateMode");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.LabelField("Max Width Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(m_WidthType, new GUIContent("Width Type"));
            string widthLabel = "Max Width";
            switch (m_WidthType.enumValueIndex)
            {
                case (int)ContentSizeFitterWithMax.SizeType.Pixel:
                    widthLabel = "Max Width (px)";
                    break;
                case (int)ContentSizeFitterWithMax.SizeType.ScreenPercentage:
                    widthLabel = "Max Width (% of screen)";
                    break;
                case (int)ContentSizeFitterWithMax.SizeType.CanvasPercentage:
                    widthLabel = "Max Width (% of canvas)";
                    break;
            }

            EditorGUILayout.PropertyField(m_MaxWidth, new GUIContent(widthLabel));

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Max Height Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(m_HeightType, new GUIContent("Height Type"));
            string heightLabel = "Max Height";
            switch (m_HeightType.enumValueIndex)
            {
                case (int)ContentSizeFitterWithMax.SizeType.Pixel:
                    heightLabel = "Max Height (px)";
                    break;
                case (int)ContentSizeFitterWithMax.SizeType.ScreenPercentage:
                    heightLabel = "Max Height (% of screen)";
                    break;
                case (int)ContentSizeFitterWithMax.SizeType.CanvasPercentage:
                    heightLabel = "Max Height (% of canvas)";
                    break;
            }

            EditorGUILayout.PropertyField(m_MaxHeight, new GUIContent(heightLabel));

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(m_UpdateMode, new GUIContent("Dynamic Update Mode"));

            EditorGUILayout.HelpBox("In Editor mode, the component always updates continuously for real-time feedback.",
                MessageType.Info);

            if (m_UpdateMode.enumValueIndex == (int)ContentSizeFitterWithMax.UpdateMode.Continuous)
            {
                EditorGUILayout.HelpBox("Continuous update mode may impact performance. Use only when necessary.",
                    MessageType.Warning);
            }

            serializedObject.ApplyModifiedProperties();

            base.OnInspectorGUI();
        }
    }
}
#endif