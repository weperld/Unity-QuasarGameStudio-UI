using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;

[CustomPropertyDrawer(typeof(VStoryShowDialogueBehaviour))]
public class VStoryShowDialogueDrawer : PropertyDrawer
{
    GUIContent gc_DialogueEnumId = new GUIContent("Dialogue Enum Id");

    GUIContent gc_TweenOptionLabel = new GUIContent("Tween Options");
    GUIContent gc_TweenSpeed = new GUIContent("Text Print Speed");
    GUIContent gc_UseSpeedAsDuration = new GUIContent("Use Speed As Duration");
    GUIContent gc_TweenEase = new GUIContent("Tween Ease Type");
    GUIContent gc_TweenCurve = new GUIContent("Tween Custom Curve");
    GUIContent gc_ScrambleMode = new GUIContent("Print Scramble Mode");
    GUIContent gc_CustomScramble = new GUIContent("Custom Scramble");

    private static bool tweemOptionFoldOut = false;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        property = property.FindPropertyRelative("setter");
        var tweenOptionProp = property.FindPropertyRelative("tmpTween");
        var easeTypeProp = tweenOptionProp.FindPropertyRelative("tweenEase");
        var scrambleModeProp = tweenOptionProp.FindPropertyRelative("tweenScrambleMode");

        int fieldCount = 2; // always show
        int space = 1;
        if (tweemOptionFoldOut) // in fold out
        {
            fieldCount += 5; // always show
            if (easeTypeProp.GetEnumValue<DG.Tweening.Ease>() == DG.Tweening.Ease.INTERNAL_Custom)
                fieldCount++;
            if (scrambleModeProp.GetEnumValue<DG.Tweening.ScrambleMode>() == DG.Tweening.ScrambleMode.Custom)
                fieldCount++;
        }

        return fieldCount * EditorGUIUtility.singleLineHeight + space * EditorGUIUtility.standardVerticalSpacing;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        property = property.FindPropertyRelative("setter");
        var enumProp = property.FindPropertyRelative("dialogueEnumId");

        property = property.FindPropertyRelative("tmpTween");
        var speedProp = property.FindPropertyRelative("textPrintSpeed");
        var useSpeedProp = property.FindPropertyRelative("useSpeedAsDuration");
        var easeProp = property.FindPropertyRelative("tweenEase");
        var curveProp = property.FindPropertyRelative("tweenCurve");
        var scrambleModeProp = property.FindPropertyRelative("tweenScrambleMode");
        var customScrambleProp = property.FindPropertyRelative("customScramble");
        var playOnSetProp = property.FindPropertyRelative("playOnSet");

        var rect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        EditorGUI.PropertyField(rect, enumProp, gc_DialogueEnumId);

        rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        tweemOptionFoldOut = EditorGUI.Foldout(rect, tweemOptionFoldOut, gc_TweenOptionLabel);

        if (tweemOptionFoldOut)
        {
            EditorGUI.indentLevel += 1;

            rect.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(rect, speedProp, gc_TweenSpeed);

            rect.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(rect, useSpeedProp, gc_UseSpeedAsDuration);

            rect.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(rect, easeProp, gc_TweenEase);

            var customEase = easeProp.GetEnumValue<DG.Tweening.Ease>() == DG.Tweening.Ease.INTERNAL_Custom;
            if (customEase)
            {
                EditorGUI.indentLevel += 1;
                rect.y += EditorGUIUtility.singleLineHeight;
                EditorGUI.PropertyField(rect, curveProp, gc_TweenCurve);
                EditorGUI.indentLevel -= 1;
            }

            rect.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(rect, scrambleModeProp, gc_ScrambleMode);

            var customScramble = scrambleModeProp.GetEnumValue<DG.Tweening.ScrambleMode>() == DG.Tweening.ScrambleMode.Custom;
            if (customScramble)
            {
                EditorGUI.indentLevel += 1;
                rect.y += EditorGUIUtility.singleLineHeight;
                EditorGUI.PropertyField(rect, customScrambleProp, gc_CustomScramble);
                EditorGUI.indentLevel -= 1;
            }

            rect.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(rect, playOnSetProp);

            EditorGUI.indentLevel -= 1;
        }
    }
}
