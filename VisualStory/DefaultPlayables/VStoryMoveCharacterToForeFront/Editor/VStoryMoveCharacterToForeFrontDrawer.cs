using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(VStoryMoveCharacterToForeFrontBehaviour))]
public class VStoryMoveCharacterToForeFrontDrawer : PropertyDrawer
{
    GUIContent m_LabelContent = new GUIContent("맨 마지막 인덱스의 캐릭터가 맨 앞에 보임.");
    GUIContent m_ListContent = new GUIContent("Fore Front Target Index");

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        property = property.FindPropertyRelative("setter");
        var listProp = property.FindPropertyRelative("list_Target");
        int fieldCount = 2;
        if (listProp.isExpanded)
        {
            var count = listProp.arraySize;
            fieldCount += 2 + count;
        }

        return fieldCount * EditorGUIUtility.singleLineHeight + (fieldCount - 1) * EditorGUIUtility.standardVerticalSpacing;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        property = property.FindPropertyRelative("setter");
        var listProp = property.FindPropertyRelative("list_Target");

        var rect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        var labelStyle = new GUIStyle()
        {
            fontStyle = FontStyle.Bold,
        };
        labelStyle.normal.textColor = Color.yellow;
        EditorGUI.LabelField(rect, m_LabelContent, labelStyle);

        rect.y += EditorGUIUtility.singleLineHeight;
        EditorGUI.PropertyField(rect, listProp, m_ListContent);
    }
}
