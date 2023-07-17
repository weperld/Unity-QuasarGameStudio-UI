using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Debug = COA_DEBUG.Debug;

[CustomPropertyDrawer(typeof(TimelineParam.TimelineBranch))]
public class VStorySetDialogueBranchData_ElementDrawer : PropertyDrawer
{
    GUIContent m_TimelineEnumContent = new GUIContent("Timeline Table EnumID");
    GUIContent m_ShowingIndexContent = new GUIContent("Showing Dialogue Data Index", "The First Index is 0. Using Dialogue Colomn in Visual_Story_Timeline_Table");

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        int fieldCount = 3;
        return fieldCount * EditorGUIUtility.singleLineHeight;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var enumProp = property.FindPropertyRelative("timelineEnumId");
        var indexProp = property.FindPropertyRelative("showingIndex");

        position = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        EditorGUI.PropertyField(position, enumProp, m_TimelineEnumContent);

        position.y += EditorGUIUtility.singleLineHeight;
        EditorGUI.PropertyField(position, indexProp, m_ShowingIndexContent);

        position.y += EditorGUIUtility.singleLineHeight;
        EditorGUI.LabelField(position, "===============================================================================================================");
    }
}