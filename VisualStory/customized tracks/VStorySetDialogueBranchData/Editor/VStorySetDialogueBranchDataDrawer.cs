using Codice.CM.Client.Differences.Graphic;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Debug = COA_DEBUG.Debug;

[CustomPropertyDrawer(typeof(VStorySetDialogueBranchDataBehaviour))]
public class VStorySetDialogueBranchDataDrawer : PropertyDrawer
{
    GUIContent m_Label = new GUIContent("데이터 세팅만을 위한 트랙입니다.\n" +
        "수동으로 분기 선택지를 열 수 있도록 하거나,\n" +
        "브랜치 포인트 마커를 추가해 자동으로 선택지가 열리도록 하십시오.");
    GUIContent m_BranchArrContent = new GUIContent("Timeline Branch Group");

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        int fieldCount = 1;
        var arrProp = property.FindPropertyRelative("list_TimelineBranch");
        int len = arrProp.arraySize;

        if (arrProp.isExpanded)
        {
            fieldCount += 2 + 3 * len;
        }

        var ret = fieldCount * EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing * (fieldCount - 1);
        return ret + EditorGUIUtility.singleLineHeight * 3f;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var arrProp = property.FindPropertyRelative("list_TimelineBranch");
        var labelStyle = new GUIStyle();
        labelStyle.fontStyle = FontStyle.Bold;
        labelStyle.normal.textColor = Color.yellow;

        position = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        EditorGUI.LabelField(position, m_Label, labelStyle);

        position.y += EditorGUIUtility.singleLineHeight * 3f;
        EditorGUI.PropertyField(position, arrProp, m_BranchArrContent);
    }
}