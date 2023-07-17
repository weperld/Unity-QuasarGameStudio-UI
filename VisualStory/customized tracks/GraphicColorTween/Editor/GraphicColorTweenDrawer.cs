using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(GraphicColorTweenBehaviour))]
public class GraphicColorTweenDrawer : PropertyDrawer
{
    GUIContent m_TweenStartColor = new GUIContent("Tween Start Color");
    GUIContent m_TweenEndColor = new GUIContent("Tween End Color");
    GUIContent m_IndividualTween = new GUIContent("Tween Individual?", "Variables that determine whether individual adjustments are made");
    GUIContent m_CurveAllColor = new GUIContent("Curve All", "RGBA");
    GUIContent m_CurveRed = new GUIContent("Curve Red");
    GUIContent m_CurveBlue = new GUIContent("Curve Blue");
    GUIContent m_CurveGreen = new GUIContent("Curve Green");
    GUIContent m_CurveAlpha = new GUIContent("Curve Alpha");

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        int fieldCount = property.FindPropertyRelative("individualControl").boolValue ? 9 : 5;
        return fieldCount * EditorGUIUtility.singleLineHeight;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var startColorProp = property.FindPropertyRelative("startColor");
        var endColorProp = property.FindPropertyRelative("endColor");
        var indiProp = property.FindPropertyRelative("individualControl");

        var fieldRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        var style = new GUIStyle();
        style.fontStyle = FontStyle.Bold;
        style.normal.textColor = Color.yellow;
        EditorGUI.LabelField(fieldRect, "Tweening colors using linear interpolation(Lerp)", style);

        fieldRect.y += EditorGUIUtility.singleLineHeight;
        EditorGUI.PropertyField(fieldRect, startColorProp, m_TweenStartColor);

        fieldRect.y += EditorGUIUtility.singleLineHeight;
        EditorGUI.PropertyField(fieldRect, endColorProp, m_TweenEndColor);

        fieldRect.y += EditorGUIUtility.singleLineHeight;
        EditorGUI.PropertyField(fieldRect, indiProp, m_IndividualTween);

        if (property.FindPropertyRelative("individualControl").boolValue)
        {
            var redCurveProp = property.FindPropertyRelative("colorCurve00");
            var blueCurveProp = property.FindPropertyRelative("colorCurve01");
            var greenCurveProp = property.FindPropertyRelative("colorCurve02");
            var alphaCurveProp = property.FindPropertyRelative("colorCurve03");

            fieldRect.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(fieldRect, redCurveProp, m_CurveRed);

            fieldRect.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(fieldRect, blueCurveProp, m_CurveBlue);

            fieldRect.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(fieldRect, greenCurveProp, m_CurveGreen);

            fieldRect.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(fieldRect, alphaCurveProp, m_CurveAlpha);
        }
        else
        {
            var allCurveProp = property.FindPropertyRelative("colorCurve00");
            fieldRect.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(fieldRect, allCurveProp, m_CurveAllColor);
        }
    }
}
