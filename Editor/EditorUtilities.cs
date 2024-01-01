using UnityEditor;
using UnityEngine;

namespace MagmaMc.Utils
{
    public static class EditorUtilities
    {
        public static GUIStyle MainHeader;
        public static GUIStyle foldLabelStyle;
        public static GUIStyle toggleStyle;
        public static GUIStyle layoutStyle;
        public static GUIStyle introLayoutStyle;

        public static void UpdateStyles()
        {
            MainHeader = new GUIStyle(EditorStyles.toolbar);
            MainHeader.alignment = TextAnchor.MiddleCenter;
            MainHeader.fixedHeight = 32f;
            MainHeader.fontSize = 20;

            foldLabelStyle = new GUIStyle(EditorStyles.foldout);
            foldLabelStyle.fontSize = 13;
            foldLabelStyle.fontStyle = FontStyle.Bold;
            foldLabelStyle.alignment = TextAnchor.MiddleLeft;
            foldLabelStyle.padding = new RectOffset(18, 2, 3, 3);

            toggleStyle = new GUIStyle(EditorStyles.toggle);
            toggleStyle.alignment = TextAnchor.MiddleRight;
            layoutStyle = new GUIStyle(EditorStyles.helpBox);
            layoutStyle.margin.left = 0;
            layoutStyle.margin.right = 0;
            layoutStyle.padding.left = 0;
            layoutStyle.padding.right = 0;
            introLayoutStyle = new GUIStyle();

        }


        public static void DrawFoldout(string Label, ref bool Enabled)
        {
            EditorGUILayout.BeginVertical(layoutStyle);
            EditorGUI.indentLevel++;

            EditorGUILayout.BeginHorizontal(introLayoutStyle);
            Enabled = EditorGUILayout.Foldout(Enabled, Label, true, foldLabelStyle);
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }
        public static void DrawFoldout(string Label, ref bool Enabled, ref bool InternalEnabled)
        {
            EditorGUILayout.BeginVertical(layoutStyle);
            EditorGUI.indentLevel++;

            EditorGUILayout.BeginHorizontal(introLayoutStyle);
            Enabled = EditorGUILayout.Foldout(Enabled, Label, true, foldLabelStyle);

            GUILayout.FlexibleSpace();
            GUILayout.Space(10);
            EditorGUILayout.LabelField("Enabled", GUILayout.Width(65));
            InternalEnabled = EditorGUILayout.Toggle(InternalEnabled, toggleStyle, GUILayout.Width(20));

            EditorGUILayout.EndHorizontal();
            EditorGUI.BeginDisabledGroup(!InternalEnabled);
        }

        public static void EndFoldoutToggle()
        {
            EditorGUI.EndDisabledGroup();
            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();
        }


        public static void EndFoldout()
        {
            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();
        }
        public static void DrawUILine(Color color = default, int thickness = 1, int padding = 10, int margin = 0)
        {
            color = color != default ? color : Color.grey;
            Rect rect = EditorGUILayout.GetControlRect(false, GUILayout.Height(padding + thickness));
            rect.height = thickness;
            rect.y += padding * 0.5f;

            switch (margin)
            {
                case < 0:
                    rect.x = 0;
                    rect.width = EditorGUIUtility.currentViewWidth;
                    break;

                case > 0:
                    rect.x += margin;
                    rect.width -= margin * 2;
                    break;
            }

            EditorGUI.DrawRect(rect, color);
        }

        public static void DrawEventReceiverArray(
            SerializedProperty PropertyAReceivers, SerializedProperty PropertyAEvents, string LabelA, ref bool Enabled)
        {
            DrawFoldout(LabelA, ref Enabled);

            if (Enabled)
            {
                for (int i = 0; i < PropertyAReceivers.arraySize; i++)
                {
                    SerializedProperty Receiver = PropertyAReceivers.GetArrayElementAtIndex(i);
                    SerializedProperty Trigger = PropertyAEvents.GetArrayElementAtIndex(i);

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.ObjectField(Receiver, GUIContent.none, GUILayout.Width(200));
                    Trigger.stringValue = EditorGUILayout.TextField(Trigger.stringValue);
                    if (GUILayout.Button("-", GUILayout.Width(20)))
                    {
                        PropertyAEvents.DeleteArrayElementAtIndex(i);
                        PropertyAReceivers.DeleteArrayElementAtIndex(i);
                    }
                    EditorGUILayout.EndHorizontal();
                }

                EditorGUI.indentLevel++;
                if (GUILayout.Button("Add"))
                {
                    PropertyAEvents.InsertArrayElementAtIndex(PropertyAEvents.arraySize);
                    PropertyAReceivers.InsertArrayElementAtIndex(PropertyAReceivers.arraySize);
                }
                EditorGUI.indentLevel--;
            }
            EndFoldout();
        }

        public static void DrawList(SerializedProperty List, string LabelA, ref bool Enabled)
        {

            DrawFoldout(LabelA, ref Enabled);

            if (Enabled)
            {
                for (int i = 0; i < List.arraySize; i++)
                {
                    SerializedProperty Item = List.GetArrayElementAtIndex(i);

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PropertyField(Item, GUIContent.none);
                    if (GUILayout.Button("-", GUILayout.Width(20)))
                    {
                        List.DeleteArrayElementAtIndex(i);
                    }
                    EditorGUILayout.EndHorizontal();
                }

                EditorGUI.indentLevel++;
                if (GUILayout.Button("Add"))
                {
                    List.InsertArrayElementAtIndex(List.arraySize);
                }
                EditorGUI.indentLevel--;
            }
            EndFoldout();
        }
    }
}