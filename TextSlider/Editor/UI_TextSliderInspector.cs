using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(UI_TextSlider))]
[CanEditMultipleObjects]
public class UI_TextSliderInspector : Editor
{
    SerializedProperty slideDirectionProp;
    SerializedProperty firstTmpuProp;
    SerializedProperty secondTmpuProp;
    SerializedProperty resumeOnEnableProp;
    SerializedProperty tmpuBetweenProp;
    SerializedProperty slideSpeedProp;
    SerializedProperty offsetHProp;
    SerializedProperty offsetVProp;
    SerializedProperty useNotSlidableOptionValuesProp;
    SerializedProperty notSlidableAnchor_MinProp;
    SerializedProperty notSlidableAnchor_MaxProp;
    SerializedProperty notSlidablePivotProp;

    GUIContent gc_Direction = new GUIContent("Slide Direction");

    GUIContent gc_TmpuFold = new GUIContent("TextMeshProUGUIs");
    GUIContent gc_FirstTmpu = new GUIContent("Main TMPU");
    GUIContent gc_SecondTmpu = new GUIContent("Sub TMPU");

    GUIContent gc_SlideSettingValue = new GUIContent("Slide Option Values");
    GUIContent gc_AutoResume = new GUIContent("Auto Resume On Enable");
    GUIContent gc_TmpuBetween = new GUIContent("Space Between TMPU");
    GUIContent gc_Speed = new GUIContent("Slide Speed", "Used only in situations where text slides are available.");
    GUIContent gc_Offset = new GUIContent("Axis Offset", "Horizontal Axis: Vertical offset\nVertical Axis: Horizontal offset");

    GUIContent gc_UseNotSlidableOption = new GUIContent("Use Not Slidable Option");
    GUIContent gc_NotSlidableOptionFoldOut = new GUIContent("Not Slidable Options, Used in situations where sliding is not possible");
    GUIContent gc_NotSlidableOptionAnchorFoldOut = new GUIContent("Anchor");
    GUIContent gc_NotSlidableAnchorMin = new GUIContent("Min");
    GUIContent gc_NotSlidableAnchorMax = new GUIContent("Max");
    GUIContent gc_NotSlidablePivot = new GUIContent("Pivot");

    UI_TextSlider _Target;

    private static bool tmpuFoldOut = false;
    private static bool slideSettingValueFoldOut = false;
    private static bool notSlidableOptionFoldOut = false;
    private static bool notSlidableOptionAnchorFoldOut = false;

    private void OnEnable()
    {
        _Target = target as UI_TextSlider;

        slideDirectionProp = serializedObject.FindProperty("currentDirection");
        firstTmpuProp = serializedObject.FindProperty("tmpu00");
        secondTmpuProp = serializedObject.FindProperty("tmpu01");
        resumeOnEnableProp = serializedObject.FindProperty("resumeOnEnable");
        tmpuBetweenProp = serializedObject.FindProperty("space");
        slideSpeedProp = serializedObject.FindProperty("speed");
        offsetHProp = serializedObject.FindProperty("offset_H");
        offsetVProp = serializedObject.FindProperty("offset_V");
        useNotSlidableOptionValuesProp = serializedObject.FindProperty("useNotSlidableOptionValues");
        notSlidableAnchor_MinProp = serializedObject.FindProperty("notSlidableAnchor_Min");
        notSlidableAnchor_MaxProp = serializedObject.FindProperty("notSlidableAnchor_Max");
        notSlidablePivotProp = serializedObject.FindProperty("notSlidablePivot");
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(slideDirectionProp, gc_Direction);

        EditorGUILayout.Space(10f);
        EditorGUILayout.BeginVertical(GUI.skin.GetStyle("HelpBox"));
        EditorGUI.indentLevel += 1;
        tmpuFoldOut = EditorGUILayout.Foldout(tmpuFoldOut, gc_TmpuFold);
        if (tmpuFoldOut)
        {
            EditorGUI.indentLevel += 1;
            EditorGUILayout.PropertyField(firstTmpuProp, gc_FirstTmpu);
            EditorGUILayout.PropertyField(secondTmpuProp, gc_SecondTmpu);
            EditorGUI.indentLevel -= 1;
        }
        EditorGUI.indentLevel -= 1;
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space(2f);
        EditorGUILayout.BeginVertical(GUI.skin.GetStyle("HelpBox"));
        EditorGUI.indentLevel += 1;
        slideSettingValueFoldOut = EditorGUILayout.Foldout(slideSettingValueFoldOut, gc_SlideSettingValue);
        if (slideSettingValueFoldOut)
        {
            EditorGUI.indentLevel += 1;
            EditorGUILayout.PropertyField(resumeOnEnableProp, gc_AutoResume);
            EditorGUILayout.PropertyField(_Target._IsHorizontal ? offsetHProp : offsetVProp, gc_Offset);
            EditorGUILayout.PropertyField(tmpuBetweenProp, gc_TmpuBetween);
            EditorGUILayout.PropertyField(slideSpeedProp, gc_Speed);
            EditorGUI.indentLevel -= 1;
        }
        EditorGUI.indentLevel -= 1;
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space(2f);
        EditorGUILayout.PropertyField(useNotSlidableOptionValuesProp, gc_UseNotSlidableOption);
        if (useNotSlidableOptionValuesProp.boolValue)
        {
            EditorGUILayout.Space(1f);
            EditorGUILayout.BeginVertical(GUI.skin.GetStyle("HelpBox"));
            EditorGUI.indentLevel += 1;
            notSlidableOptionFoldOut = EditorGUILayout.Foldout(notSlidableOptionFoldOut, gc_NotSlidableOptionFoldOut);
            if (notSlidableOptionFoldOut)
            {
                EditorGUI.indentLevel += 1;
                notSlidableOptionAnchorFoldOut = EditorGUILayout.Foldout(notSlidableOptionAnchorFoldOut, gc_NotSlidableOptionAnchorFoldOut);
                if (notSlidableOptionAnchorFoldOut)
                {
                    EditorGUI.indentLevel += 1;
                    var minValue = EditorGUILayout.Vector2Field(gc_NotSlidableAnchorMin, notSlidableAnchor_MinProp.vector2Value);
                    var maxValue = EditorGUILayout.Vector2Field(gc_NotSlidableAnchorMax, notSlidableAnchor_MaxProp.vector2Value);
                    if (minValue != _Target._NotSlidableAnchor_Min) _Target._NotSlidableAnchor_Min = minValue;
                    if (maxValue != _Target._NotSlidableAnchor_Max) _Target._NotSlidableAnchor_Max = maxValue;
                    EditorGUI.indentLevel -= 1;
                    EditorGUILayout.Space(1f);
                }
                EditorGUILayout.PropertyField(notSlidablePivotProp, gc_NotSlidablePivot);
                EditorGUI.indentLevel -= 1;
            }
            EditorGUI.indentLevel -= 1;
            EditorGUILayout.EndVertical();
        }

        serializedObject.ApplyModifiedProperties();
    }
}