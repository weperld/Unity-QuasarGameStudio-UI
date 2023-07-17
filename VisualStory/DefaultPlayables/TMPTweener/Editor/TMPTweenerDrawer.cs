using UnityEditor;
using UnityEngine;
using Debug = COA_DEBUG.Debug;

[CustomPropertyDrawer(typeof(TMPTweenerBehaviour))]
public class TMPTweenerDrawer : PropertyDrawer
{
    GUIContent m_PrintSpeedCont = new GUIContent("Text Tweening Speed");

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        int fieldCount = 6;
        property = property.FindPropertyRelative("setter");

        var easeProp = property.FindPropertyRelative("tweenEase");
        if (easeProp.enumValueIndex == (int)DG.Tweening.Ease.INTERNAL_Custom)
            fieldCount++;

        var scrambleProp = property.FindPropertyRelative("tweenScrambleMode");
        if (scrambleProp.enumValueIndex == (int)DG.Tweening.ScrambleMode.Custom)
            fieldCount++;

        return fieldCount * EditorGUIUtility.singleLineHeight;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        property = property.FindPropertyRelative("setter");
        var targetStringProp = property.FindPropertyRelative("targetString");
        var printSpeedProp = property.FindPropertyRelative("textPrintSpeed");
        var speedAsDurProp = property.FindPropertyRelative("useSpeedAsDuration");
        var easeProp = property.FindPropertyRelative("tweenEase");
        var curveProp = property.FindPropertyRelative("tweenCurve");
        var scrambleModeProp = property.FindPropertyRelative("tweenScrambleMode");
        var customScrambleProp = property.FindPropertyRelative("customScramble");
        var playOnSet = property.FindPropertyRelative("playOnSet");

        var singleFieldRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        EditorGUI.PropertyField(singleFieldRect, playOnSet);

        singleFieldRect.y += EditorGUIUtility.singleLineHeight;
        EditorGUI.PropertyField(singleFieldRect, targetStringProp);

        singleFieldRect.y += EditorGUIUtility.singleLineHeight;
        EditorGUI.PropertyField(singleFieldRect, printSpeedProp, m_PrintSpeedCont);

        singleFieldRect.y += EditorGUIUtility.singleLineHeight;
        EditorGUI.PropertyField(singleFieldRect, speedAsDurProp);

        singleFieldRect.y += EditorGUIUtility.singleLineHeight;
        EditorGUI.PropertyField(singleFieldRect, easeProp);

        if (easeProp.enumValueIndex == (int)DG.Tweening.Ease.INTERNAL_Custom)
        {
            singleFieldRect.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(singleFieldRect, curveProp);
        }

        singleFieldRect.y += EditorGUIUtility.singleLineHeight;
        EditorGUI.PropertyField(singleFieldRect, scrambleModeProp);

        if (scrambleModeProp.enumValueIndex == (int)DG.Tweening.ScrambleMode.Custom)
        {
            singleFieldRect.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(singleFieldRect, customScrambleProp);
        }
    }
}