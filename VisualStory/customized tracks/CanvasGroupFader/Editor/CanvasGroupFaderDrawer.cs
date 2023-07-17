using UnityEditor;
using UnityEngine;
using Debug = COA_DEBUG.Debug;

[CustomPropertyDrawer(typeof(CanvasGroupFaderBehaviour))]
public class CanvasGroupFaderDrawer : PropertyDrawer
{
    TimelineParam.SEC_Setter.FieldGUIContentSetter guiContentSetter
        = new TimelineParam.SEC_Setter.FieldGUIContentSetter(
            "Tweening alpha using linear interpolation(Lerp)",
            "Start Alpha",
            "End Alpha",
            "Alpha Curve");

    public override float GetPropertyHeight (SerializedProperty property, GUIContent label)
    {
        int fieldCount = 4;
        return fieldCount * EditorGUIUtility.singleLineHeight;
    }

    public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
    {
        property = property.FindPropertyRelative("setter");
        var startProp = property.FindPropertyRelative("start");
        var endProp = property.FindPropertyRelative("end");
        var curveProp = property.FindPropertyRelative("curve");

        Rect pos = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        EditorGUI.LabelField(pos, guiContentSetter.gui_Label, guiContentSetter.guiStyle_Label);

        pos.y += EditorGUIUtility.singleLineHeight;
        startProp.floatValue = EditorGUI.Slider(pos, guiContentSetter.gui_Start, startProp.floatValue, 0f, 1f);

        pos.y += EditorGUIUtility.singleLineHeight;
        endProp.floatValue = EditorGUI.Slider(pos, guiContentSetter.gui_End, endProp.floatValue, 0f, 1f);

        pos.y += EditorGUIUtility.singleLineHeight;
        EditorGUI.PropertyField(pos, curveProp, guiContentSetter.gui_Curve);
    }
}
