using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(VStorySetSponsorBranchDataBehaviour))]
public class VStorySetSponsorBranchDataDrawer : PropertyDrawer
{
    GUIContent m_Label = new GUIContent("데이터 세팅만을 위한 트랙입니다.\n" +
        "수동으로 분기 선택지를 열 수 있도록 하십시오.");
    GUIContent m_List = new GUIContent("Sponsor Branch Group");

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        int fieldCount = 1;
        var arrProp = property.FindPropertyRelative("list_OracleSponItem");
        if (arrProp.isExpanded)
        {
            var len = arrProp.arraySize;
            fieldCount += 2 + 3 * len;
        }

        return (fieldCount + 2) * EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing * (fieldCount - 1);
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var prop = property.FindPropertyRelative("list_OracleSponItem");
        var labelStyle = new GUIStyle();
        labelStyle.fontStyle = FontStyle.Bold;
        labelStyle.normal.textColor = Color.yellow;

        Rect rect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        EditorGUI.LabelField(rect, m_Label, labelStyle);

        rect.y += 2f * EditorGUIUtility.singleLineHeight;
        EditorGUI.PropertyField(rect, prop, m_List);
    }
}
