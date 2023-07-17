using UnityEngine;
using UnityEditor;
using Debug = COA_DEBUG.Debug;

[CustomPropertyDrawer(typeof(VStoryActiveEmoticonBehaviour))]
public class VStoryActiveEmoticonDrawer : PropertyDrawer
{
    GUIContent m_Lable = new GUIContent("이모티콘 종류가 추가될 때,\n" +
        "트랙에서 사용 가능하도록 개발자에게 수정 요청이 필요합니다.");
    GUIStyle m_LableStyle = new GUIStyle()
    {
        fontStyle = FontStyle.Italic
    };
    Color lableColor = Color.yellow;

    GUIContent gc_EmoticonType = new GUIContent("Character Emoticon");
    GUIContent gc_SoundVolume = new GUIContent("Effect Sound Volume");

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        int fieldCount = 4;
        return fieldCount * EditorGUIUtility.singleLineHeight;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        m_LableStyle.normal.textColor = lableColor;
        var emoticonProp = property.FindPropertyRelative("emoticon");
        var volumeProp = property.FindPropertyRelative("volume");

        var fieldRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight * 2f);
        EditorGUI.LabelField(fieldRect, m_Lable, m_LableStyle);

        fieldRect.height = EditorGUIUtility.singleLineHeight;
        fieldRect.y += EditorGUIUtility.singleLineHeight * 2f;
        EditorGUI.PropertyField(fieldRect, emoticonProp, gc_EmoticonType);

        fieldRect.y += EditorGUIUtility.singleLineHeight;
        EditorGUI.PropertyField(fieldRect, volumeProp, gc_SoundVolume);
    }
}