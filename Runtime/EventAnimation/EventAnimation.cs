using UdonSharp;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MagmaMc.Utils
{
#if UNITY_EDITOR
    [CustomEditor(typeof(EventAnimation))]
    public class EventAnimationEditor: Editor
    {
        SerializedProperty EnabledAnimators;
        SerializedProperty DisabledAnimators;
        SerializedProperty EnabledTriggers;
        SerializedProperty DisabledTriggers;
        SerializedProperty DebugMode;

        GUIStyle MainHeader;
        private void OnEnable()
        {

            EnabledAnimators = serializedObject.FindProperty("EnabledAnimators");
            DisabledAnimators = serializedObject.FindProperty("DisabledAnimators");
            EnabledTriggers = serializedObject.FindProperty("EnabledTriggers");
            DisabledTriggers = serializedObject.FindProperty("DisabledTriggers");
            DebugMode = serializedObject.FindProperty("DebugMode");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            MainHeader = new GUIStyle(EditorStyles.toolbar);
            MainHeader.alignment = TextAnchor.MiddleCenter;
            MainHeader.fixedHeight = 32f;
            MainHeader.fontSize = 20;

            GUILayout.BeginHorizontal(MainHeader);
            GUILayout.FlexibleSpace();
            GUILayout.Label("EventAnimation - V1.0.0", MainHeader);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(5);

            if (DebugMode.boolValue)
            {
                EditorGUILayout.PropertyField(EnabledAnimators);
                EditorGUILayout.PropertyField(EnabledTriggers);
                EditorGUILayout.PropertyField(DisabledAnimators);
                EditorGUILayout.PropertyField(DisabledTriggers);
            }
            else
            {
                GUILayout.Space(10);
                EditorGUILayout.LabelField("OnEnabled", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
                for (int i = 0; i < EnabledAnimators.arraySize; i++)
                {
                    SerializedProperty animatorProp = EnabledAnimators.GetArrayElementAtIndex(i);
                    SerializedProperty triggerProp = EnabledTriggers.GetArrayElementAtIndex(i);

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.ObjectField(animatorProp);
                    triggerProp.stringValue = EditorGUILayout.TextField(triggerProp.stringValue);
                    if (GUILayout.Button("-", GUILayout.Width(20)))
                    {
                        EnabledAnimators.DeleteArrayElementAtIndex(i);
                        EnabledTriggers.DeleteArrayElementAtIndex(i);
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUI.indentLevel--;

                if (GUILayout.Button("Add", GUILayout.Width(60)))
                {
                    EnabledAnimators.InsertArrayElementAtIndex(EnabledAnimators.arraySize);
                    EnabledTriggers.InsertArrayElementAtIndex(EnabledTriggers.arraySize);
                }


                GUILayout.Space(10);
                EditorGUILayout.LabelField("OnDisable", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;

                for (int i = 0; i < DisabledAnimators.arraySize; i++)
                {
                    SerializedProperty animatorProp = DisabledAnimators.GetArrayElementAtIndex(i);
                    SerializedProperty triggerProp = DisabledTriggers.GetArrayElementAtIndex(i);

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.ObjectField(animatorProp);
                    triggerProp.stringValue = EditorGUILayout.TextField(triggerProp.stringValue);
                    if (GUILayout.Button("-", GUILayout.Width(20)))
                    {
                        DisabledAnimators.DeleteArrayElementAtIndex(i);
                        DisabledTriggers.DeleteArrayElementAtIndex(i);
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUI.indentLevel--;

                if (GUILayout.Button("Add", GUILayout.Width(60)))
                {
                    DisabledAnimators.InsertArrayElementAtIndex(DisabledAnimators.arraySize);
                    DisabledTriggers.InsertArrayElementAtIndex(DisabledTriggers.arraySize);
                }

            }

            GUILayout.Space(20);
            EditorGUILayout.PropertyField(DebugMode);

            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
    public class EventAnimation: UdonSharpBehaviour
    {
        public Animator[] EnabledAnimators = new Animator[1];
        public Animator[] DisabledAnimators = new Animator[1];
        public string[] EnabledTriggers = new string[1];
        public string[] DisabledTriggers = new string[1];

        public bool DebugMode = false;


        public void Awake()
        {
            if (EnabledAnimators.Length != EnabledTriggers.Length)
            {
                Debug.LogError($"Error EnabledAnimators & EnabledTriggers Are Not Equal Length, Deleting", this.gameObject);
                Destroy(this);
            }
            if (DisabledAnimators.Length != DisabledTriggers.Length)
            {
                Debug.LogError($"Error DisabledAnimators & DisabledTriggers Are Not Equal Length, Deleting", this.gameObject);
                Destroy(this);
            }
        }

        public void CallEnabled()
        {
            for(int i = 0; i < EnabledAnimators.Length; ++i)
            {
                EnabledAnimators[i].SetTrigger(EnabledTriggers[i]);
                if (DebugMode)
                    Debug.Log($"CallEnabled [{i}] -> '{EnabledTriggers[i]}' On Animator '{EnabledAnimators[i].gameObject.name}'");
            }
        }
        public void CallDisabled()
        {
            for (int i = 0; i < DisabledAnimators.Length; ++i)
            {
                DisabledAnimators[i].SetTrigger(DisabledTriggers[i]);
                if (DebugMode)
                    Debug.Log($"CallDisabled [{i}] -> '{DisabledTriggers[i]}' On Animator '{DisabledAnimators[i].gameObject.name}'");
            }
        }
    }
}