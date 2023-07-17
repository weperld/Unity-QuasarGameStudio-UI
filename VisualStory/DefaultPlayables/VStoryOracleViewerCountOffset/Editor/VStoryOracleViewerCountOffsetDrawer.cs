using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(VStoryOracleViewerCountOffsetBehaviour))]
public class VStoryOracleViewerCountOffsetDrawer : PropertyDrawer
{
    GUIContent gc_Label = new GUIContent($"※※※ Applicable Value Range\n" +
        $"\tMinimum: -1\n" +
        $"\tMaximum: {float.MaxValue}");
    GUIContent gc_CurveValueLabel = new GUIContent();
    GUIContent gc_Curve = new GUIContent("Additional Viewer Offset Ratio");

    GUIStyle gs_Label = new GUIStyle() { fontStyle = FontStyle.Bold };
    GUIStyle gs_CurveValueLabel = new GUIStyle();

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        int fieldCount = 5;
        return fieldCount * EditorGUIUtility.singleLineHeight;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var offsetCurveProp = property.FindPropertyRelative("offsetCurve");
        gs_Label.normal.textColor = Color.yellow;
        gs_CurveValueLabel.normal.textColor = Color.white;

        Rect rect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        EditorGUI.LabelField(rect, gc_Label, gs_Label);
        rect.y += EditorGUIUtility.singleLineHeight * 2f;
        rect.height = EditorGUIUtility.singleLineHeight;

        var curve = offsetCurveProp.animationCurveValue;
        var startValue = curve.length > 0 ? curve[0].value : 0f;
        var endValue = curve.length > 0 ? curve[curve.length - 1].value : 0f;
        rect.y += EditorGUIUtility.singleLineHeight;
        gc_CurveValueLabel.text = string.Format("Start: {0}, End: {1}", startValue, endValue);
        EditorGUI.LabelField(rect, gc_CurveValueLabel, gs_CurveValueLabel);

        rect.y += EditorGUIUtility.singleLineHeight;
        EditorGUI.PropertyField(rect, offsetCurveProp, gc_Curve);
    }
}
