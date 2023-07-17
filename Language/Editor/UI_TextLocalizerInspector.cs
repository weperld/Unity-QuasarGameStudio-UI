using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(UI_TextLocalizer))]
[CanEditMultipleObjects]
public class UI_TextLocalizerInspector : Editor
{
    SerializedProperty functionTypeProp;
    SerializedProperty keyProp;
    SerializedProperty localTypeProp;
    SerializedProperty stringCaseProp;
    SerializedProperty useUserLangProp;
    SerializedProperty customLangProp;
    SerializedProperty hideSpecificLangProp;
    SerializedProperty hideTargetProp;

    GUIContent gc_FunctionType = new GUIContent("Function Type");
    GUIContent gc_Key = new GUIContent("Localization Key");
    GUIContent gc_Local = new GUIContent("Local Type");
    GUIContent gc_StrintCase = new GUIContent("Choose String Case");
    GUIContent gc_UserLang = new GUIContent("Use User Language");
    GUIContent gc_SelectLang = new GUIContent("Language Select");
    GUIContent gc_HideLang = new GUIContent("Hide Language", "Depending on the setting language option.");
    GUIContent gc_HideTarget = new GUIContent("Hide Target");

    GUIStyle gs_HorizontalLine = new GUIStyle();

    UI_TextLocalizer castedTarget;

    private void OnEnable()
    {
        castedTarget = target as UI_TextLocalizer;

        functionTypeProp = serializedObject.FindProperty("functionType");
        keyProp = serializedObject.FindProperty("key");
        localTypeProp = serializedObject.FindProperty("type");
        stringCaseProp = serializedObject.FindProperty("stringCase");
        useUserLangProp = serializedObject.FindProperty("useUserLanguage");
        customLangProp = serializedObject.FindProperty("customLanguage");
        hideSpecificLangProp = serializedObject.FindProperty("hideSpecificLanguage");
        hideTargetProp = serializedObject.FindProperty("go_HideTarget");

        gs_HorizontalLine.normal.background = EditorGUIUtility.whiteTexture;
        gs_HorizontalLine.margin = new RectOffset(0, 0, 4, 4);
        gs_HorizontalLine.fixedHeight = 10f;
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.PropertyField(functionTypeProp, gc_FunctionType);

        var typeValue = (UI_TextLocalizer.Function)functionTypeProp.enumValueIndex;
        var usingLocal = typeValue != UI_TextLocalizer.Function.HIDE;
        var usingHide = typeValue != UI_TextLocalizer.Function.LOCALIZER;

        if (usingLocal)
        {
            GUILayout.Space(20f);
            EditorGUI.DrawRect(EditorGUILayout.GetControlRect(true, 3f), Color.green);
            EditorGUILayout.LabelField("▩▩▩ Localization Properties ▩▩▩");
            EditorGUILayout.PropertyField(keyProp, gc_Key);
            EditorGUILayout.PropertyField(localTypeProp, gc_Local);
            EditorGUILayout.PropertyField(stringCaseProp, gc_StrintCase);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(useUserLangProp, gc_UserLang);
            if (!useUserLangProp.boolValue)
            {
                castedTarget.customLanguage = (Data.Enum.Language)EditorGUILayout.EnumPopup(castedTarget.customLanguage);
            }
            EditorGUILayout.EndHorizontal();
        }
        if (usingHide)
        {
            GUILayout.Space(20f);
            EditorGUI.DrawRect(EditorGUILayout.GetControlRect(true, 3f), Color.red);
            EditorGUILayout.LabelField("▩▩▩ Hide Properties ▩▩▩");
            EditorGUILayout.PropertyField(hideSpecificLangProp, gc_HideLang);
            EditorGUILayout.PropertyField(hideTargetProp, gc_HideTarget);
        }

        serializedObject.ApplyModifiedProperties();
    }
}