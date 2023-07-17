using System;
using UnityEditor;
using UnityEngine;
using Debug = COA_DEBUG.Debug;

[CustomPropertyDrawer(typeof(CharacterGrayScalerBehaviour))]
public class CharacterGrayScalerDrawer : PropertyDrawer
{
    TimelineParam.SEC_Setter.FieldGUIContentSetter guiContentSetter
        = new TimelineParam.SEC_Setter.FieldGUIContentSetter(
            "Tweening saturation using linear interpolation(Lerp)",
            "Start Saturation",
            "End Saturation",
            "Saturation Curve");

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        int fieldCount = 4;
        return fieldCount * EditorGUIUtility.singleLineHeight;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        property = property.FindPropertyRelative("setter");
        var startProp = property.FindPropertyRelative("start");
        var endProp = property.FindPropertyRelative("end");
        var curveProp = property.FindPropertyRelative("curve");

        var rect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        EditorGUI.LabelField(rect, guiContentSetter.gui_Label, guiContentSetter.guiStyle_Label);

        rect.y += EditorGUIUtility.singleLineHeight;
        startProp.floatValue = EditorGUI.Slider(rect, guiContentSetter.gui_Start, startProp.floatValue, 0f, 1f);

        rect.y += EditorGUIUtility.singleLineHeight;
        endProp.floatValue = EditorGUI.Slider(rect, guiContentSetter.gui_End, endProp.floatValue, 0f, 1f);

        rect.y += EditorGUIUtility.singleLineHeight;
        EditorGUI.PropertyField(rect, curveProp, guiContentSetter.gui_Curve);
    }
}