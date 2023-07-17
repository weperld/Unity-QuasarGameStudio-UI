using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(TimelineParam.OracleSponsorBranch))]
public class VStorySetSponsorBranch_ElementDrawer : PropertyDrawer
{
    GUIContent m_Timeline = new GUIContent("Timeline Table EnumID");
    GUIContent m_DonaContent = new GUIContent("Donation Content EnumID");

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        int fieldCount = 3;
        return fieldCount * EditorGUIUtility.singleLineHeight;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var timelineProp = property.FindPropertyRelative("timelineEnumId");
        var contentProp = property.FindPropertyRelative("donationContentEnumId");

        Rect rect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        EditorGUI.PropertyField(rect, timelineProp, m_Timeline);

        rect.y += EditorGUIUtility.singleLineHeight;
        EditorGUI.PropertyField(rect, contentProp, m_DonaContent);

        rect.y += EditorGUIUtility.singleLineHeight;
        EditorGUI.LabelField(rect, "===============================================================================================================");
    }
}