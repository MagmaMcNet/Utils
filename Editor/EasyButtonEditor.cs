
using MagmaMc.Utils;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EasyButton)), CanEditMultipleObjects]
public class EasyButtonEditor: Editor
{
#if MAGMAMC_PERMISSIONMANAGER
    SerializedProperty PermissionManager;
    SerializedProperty AuthorizedPermissions;
#endif

    // Objects
    SerializedProperty EnableObjectLists;
    SerializedProperty EnableList;
    SerializedProperty DisableList;


    // Events
    SerializedProperty EnableEvents;

    SerializedProperty OnEnabledEvents;
    SerializedProperty OnEnabledReceiver;
    SerializedProperty OnDisabledEvents;
    SerializedProperty OnDisabledReceiver;


    SerializedProperty EnabledMat;
    SerializedProperty DisableMat;
    SerializedProperty Networked;
    SerializedProperty SyncLateJoiners;
    SerializedProperty MasterOnly;
    SerializedProperty DefaultValue;

    private static bool FoldOut_AuthorizedPermissions = false;

    private static bool FoldOut_ObjectList_List = false;
    private static bool FoldOut_ObjectList_OnEnabled = false;
    private static bool FoldOut_ObjectList_OnDisabled = false;

    private static bool FoldOut_Events_List = false;
    private static bool FoldOut_Events_OnEnabled = false;
    private static bool FoldOut_Events_OnDisabled = false;

    private void OnEnable()
    {
#if MAGMAMC_PERMISSIONMANAGER 
        PermissionManager = serializedObject.FindProperty("PermissionManager");
        AuthorizedPermissions = serializedObject.FindProperty("AuthorizedPermissions");
#endif

        EnableObjectLists = serializedObject.FindProperty("ObjectsList");
        EnableList = serializedObject.FindProperty("EnableList");
        DisableList = serializedObject.FindProperty("DisableList");

        EnableEvents = serializedObject.FindProperty("Events");
        OnEnabledEvents = serializedObject.FindProperty("OnEnabledEvents");
        OnEnabledReceiver = serializedObject.FindProperty("OnEnabledReceiver");
        OnDisabledEvents = serializedObject.FindProperty("OnDisabledEvents");
        OnDisabledReceiver = serializedObject.FindProperty("OnDisabledReceiver");

        EnabledMat = serializedObject.FindProperty("EnabledMat");
        DisableMat = serializedObject.FindProperty("DisableMat");
        Networked = serializedObject.FindProperty("Networked");
        SyncLateJoiners = serializedObject.FindProperty("SyncLateJoiners");
        MasterOnly = serializedObject.FindProperty("MasterOnly");
        DefaultValue = serializedObject.FindProperty("DefaultValue");

    }
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorUtilities.UpdateStyles();

        GUILayout.BeginHorizontal(EditorUtilities.MainHeader);
        GUILayout.FlexibleSpace();
        GUILayout.Label("EasyButton - V2.0.0", EditorUtilities.MainHeader);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        EditorGUILayout.Space(5);

#if MAGMAMC_PERMISSIONMANAGER
        EditorUtilities.DrawUILine(Color.gray, 2, 8);
        EditorGUILayout.PropertyField(PermissionManager);
        EditorGUILayout.Space(5);
        if (PermissionManager.objectReferenceValue == null)
        {
            serializedObject.ApplyModifiedProperties();
            return;
        }
        EditorUtilities.DrawList(AuthorizedPermissions, "Authorized Permissions", ref FoldOut_AuthorizedPermissions);
        EditorUtilities.DrawUILine(Color.gray, 2, 8);

#endif

        EditorGUILayout.PropertyField(DefaultValue);

        EditorGUILayout.Space(10);
        EditorGUILayout.PropertyField(Networked);
        if (Networked.boolValue)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(SyncLateJoiners);
            EditorGUI.indentLevel--;
        }
        EditorGUILayout.Space(5);
        EditorGUILayout.PropertyField(MasterOnly);

        EditorGUILayout.Space(10);

        bool EnableObjectList = EnableObjectLists.boolValue;
        EditorUtilities.DrawFoldout("Object List", ref FoldOut_ObjectList_List, ref EnableObjectList);
        EnableObjectLists.boolValue = EnableObjectList;
        if (FoldOut_ObjectList_List)
        {
            EditorUtilities.DrawList(EnableList, "OnEnabled", ref FoldOut_ObjectList_OnEnabled);
            EditorUtilities.DrawList(DisableList, "OnDisabled", ref FoldOut_ObjectList_OnDisabled);
        }
        EditorUtilities.EndFoldoutToggle();

        bool enableEvents = EnableEvents.boolValue;
        EditorUtilities.DrawFoldout("Events", ref FoldOut_Events_List, ref enableEvents);
        EnableEvents.boolValue = enableEvents;
        if (FoldOut_Events_List)
        {
            EditorUtilities.DrawEventReceiverArray(
                OnEnabledReceiver, OnEnabledEvents, "OnEnabled", ref FoldOut_Events_OnEnabled);
            EditorUtilities.DrawEventReceiverArray(
                OnDisabledReceiver, OnDisabledEvents, "OnDisabled", ref FoldOut_Events_OnDisabled);
        }
        EditorUtilities.EndFoldoutToggle();

        EditorGUILayout.Space(5);
        EditorGUILayout.PropertyField(EnabledMat);
        EditorGUILayout.PropertyField(DisableMat);

        serializedObject.ApplyModifiedProperties();
    }
}