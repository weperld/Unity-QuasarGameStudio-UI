using UnityEngine;
using UnityEditor;
using Debug = COA_DEBUG.Debug;
using UnityEditor.Rendering;

[CustomPropertyDrawer(typeof(RectTransformTweenBehaviour))]
public class RectTransformTweenDrawer : PropertyDrawer
{
    GUIContent gc_Label = new GUIContent("Transform tween using linear interpolation(Lerp).\n" +
        "End Position is used as a ratio to the screen size.");
    GUIStyle gs_Label = new GUIStyle() { fontStyle = FontStyle.Bold };

    GUIContent gc_EndPos = new GUIContent("End Position");
    GUIContent gc_TweenIndividual = new GUIContent("Individual Tweening?");

    GUIContent gc_TweenType_All = new GUIContent("Tween Type_All");
    GUIContent gc_TweenCurve_All = new GUIContent("Tween Curve_All");

    GUIContent gc_FoldLabel_X = new GUIContent("Horizontal Tween Values");
    GUIContent gc_TweenType_X = new GUIContent("Tween Type_Horizontal");
    GUIContent gc_TweenCurve_X = new GUIContent("Tween Curve_Horizontal");

    GUIContent gc_FoldLabel_Y = new GUIContent("Vertical Tween Values");
    GUIContent gc_TweenType_Y = new GUIContent("Tween Type_Vertical");
    GUIContent gc_TweenCurve_Y = new GUIContent("Tween Curve_Vertical");

    private static bool xFoldOut = false;
    private static bool yFoldOut = false;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        property = property.FindPropertyRelative("setter");
        var boolValue = property.FindPropertyRelative("curve_Individual").boolValue;

        int fieldCount = 2; // Label;
        fieldCount += 2; // Always Showing Property;

        int spaceCount = 0;

        if (!boolValue) // tween All
        {
            fieldCount += 1;

            var typeEnumValue = property.FindPropertyRelative("curveType_All").GetEnumValue<TimelineParam.RectTransformTween.CurveType>();
            if (typeEnumValue == TimelineParam.RectTransformTween.CurveType.Custom) fieldCount++;
        }
        else // tween individual
        {
            spaceCount++; // box
            fieldCount += 2; // fold

            if (xFoldOut)
            {
                fieldCount++;
                var typeEnumValue = property.FindPropertyRelative("curveType_X").GetEnumValue<TimelineParam.RectTransformTween.CurveType>();
                if (typeEnumValue == TimelineParam.RectTransformTween.CurveType.Custom) fieldCount++;
            }

            spaceCount++; // space between fold

            if (yFoldOut)
            {
                fieldCount++;
                var typeEnumValue = property.FindPropertyRelative("curveType_Y").GetEnumValue<TimelineParam.RectTransformTween.CurveType>();
                if (typeEnumValue == TimelineParam.RectTransformTween.CurveType.Custom) fieldCount++;
            }
        }

        return fieldCount * EditorGUIUtility.singleLineHeight + spaceCount * EditorGUIUtility.standardVerticalSpacing;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        property = property.FindPropertyRelative("setter");
        gs_Label.normal.textColor = Color.yellow;

        var endProp = property.FindPropertyRelative("endValue");
        var indiProp = property.FindPropertyRelative("curve_Individual");

        var typeAllProp = property.FindPropertyRelative("curveType_All");
        var curveAllProp = property.FindPropertyRelative("ac_Custom_All");

        var typeXProp = property.FindPropertyRelative("curveType_X");
        var curveXProp = property.FindPropertyRelative("ac_Custom_X");
        var typeYProp = property.FindPropertyRelative("curveType_Y");
        var curveYProp = property.FindPropertyRelative("ac_Custom_Y");

        var rect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight * 2f);
        EditorGUI.LabelField(rect, gc_Label, gs_Label);

        rect.y += EditorGUIUtility.singleLineHeight * 2f; rect.height = EditorGUIUtility.singleLineHeight;
        EditorGUI.PropertyField(rect, endProp, gc_EndPos);

        rect.y += EditorGUIUtility.singleLineHeight;
        EditorGUI.PropertyField(rect, indiProp, gc_TweenIndividual);
        var indiValue = indiProp.boolValue;

        if (!indiValue)
        {
            rect.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(rect, typeAllProp, gc_TweenType_All);
            if (typeAllProp.GetEnumValue<TimelineParam.RectTransformTween.CurveType>()
                == TimelineParam.RectTransformTween.CurveType.Custom)
            {
                rect.y += EditorGUIUtility.singleLineHeight;
                EditorGUI.PropertyField(rect, curveAllProp, gc_TweenCurve_All);
            }
        }
        else
        {
            rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            EditorGUILayout.BeginVertical();
            EditorGUI.indentLevel += 1;
            xFoldOut = EditorGUI.Foldout(rect, xFoldOut, gc_FoldLabel_X);
            if (xFoldOut)
            {
                EditorGUI.indentLevel += 1;

                rect.y += EditorGUIUtility.singleLineHeight;
                EditorGUI.PropertyField(rect, typeXProp, gc_TweenType_X);
                if (typeXProp.GetEnumValue<TimelineParam.RectTransformTween.CurveType>()
                    == TimelineParam.RectTransformTween.CurveType.Custom)
                {
                    rect.y += EditorGUIUtility.singleLineHeight;
                    EditorGUI.PropertyField(rect, curveXProp, gc_TweenCurve_X);
                }

                EditorGUI.indentLevel -= 1;
            }
            EditorGUI.indentLevel -= 1;
            EditorGUILayout.EndVertical();

            rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            EditorGUI.indentLevel += 1;
            yFoldOut = EditorGUI.Foldout(rect, yFoldOut, gc_FoldLabel_Y);
            if (yFoldOut)
            {
                EditorGUI.indentLevel += 1;

                rect.y += EditorGUIUtility.singleLineHeight;
                EditorGUI.PropertyField(rect, typeYProp, gc_TweenType_Y);
                if (typeYProp.GetEnumValue<TimelineParam.RectTransformTween.CurveType>()
                    == TimelineParam.RectTransformTween.CurveType.Custom)
                {
                    rect.y += EditorGUIUtility.singleLineHeight;
                    EditorGUI.PropertyField(rect, curveYProp, gc_TweenCurve_Y);
                }

                EditorGUI.indentLevel -= 1;
            }
            EditorGUI.indentLevel -= 1;
        }
    }
}