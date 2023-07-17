using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(VStoryScreenFocusAndZoomBehaviour))]
public class VStoryScreenFocusAndZoomDrawer : PropertyDrawer
{
    GUIContent m_Label = new GUIContent("Tweening(both) using linear interpolation(Lerp)");
    GUIStyle m_LabelStyle = new GUIStyle() { fontStyle = FontStyle.Italic };
    Color m_LabelColor = Color.yellow;
    GUIContent m_UsingZoomContent = new GUIContent("Use Zoom?");
    GUIContent m_ZoomValueContent = new GUIContent("Zoom Value", $"Minimum(Default): {VisualStoryHelper.DEFAULT_ZOOM_SCALE}, Maximum: {VisualStoryHelper.MAX_ZOOM_SCALE}");
    GUIContent m_ZoomCurveContent = new GUIContent("Zoom Tween Curve");
    GUIContent m_UsingFocusContent = new GUIContent("Use Focus?");
    GUIContent m_ResetFocusContent = new GUIContent("Reset The Screen Focus");
    GUIContent m_FocusIndexContent = new GUIContent("Focus Target Character Number");
    GUIContent m_FocusCurveContent = new GUIContent("Focus Tween Curve");

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        property = property.FindPropertyRelative("setter");
        var usingZoomProp = property.FindPropertyRelative("usingZoom");
        var usingFocusProp = property.FindPropertyRelative("usingFocus");
        var resetFocusProp = property.FindPropertyRelative("resetFocus");
        var indexArrProp = property.FindPropertyRelative("arr_FocusTargetIndex");
        var size = Mathf.Max(indexArrProp.arraySize, 1);

        int fieldCount = 1; // Label

        fieldCount += 2; // Property Count
        if (usingZoomProp.boolValue) fieldCount += 2;
        if (usingFocusProp.boolValue)
        {
            fieldCount += 2;
            if (!resetFocusProp.boolValue)
            {
                fieldCount += 1;
                if (indexArrProp.isExpanded)
                    fieldCount += size + 2;
            }
        }

        fieldCount++; // Space

        return fieldCount * EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        property = property.FindPropertyRelative("setter");
        var usingZoomProp = property.FindPropertyRelative("usingZoom");
        var zoomProp = property.FindPropertyRelative("zoom");
        var zoomShiftCurveProp = property.FindPropertyRelative("zoomShiftCurve");
        var usingFocusProp = property.FindPropertyRelative("usingFocus");
        var resetFocusProp = property.FindPropertyRelative("resetFocus");
        var focusTargetIndexProp = property.FindPropertyRelative("arr_FocusTargetIndex");
        var indexSize = focusTargetIndexProp.arraySize;
        var focusShiftCurveProp = property.FindPropertyRelative("focusShiftCurve");

        Rect singleFieldRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        m_LabelStyle.normal.textColor = m_LabelColor;
        EditorGUI.LabelField(singleFieldRect, m_Label, m_LabelStyle);

        singleFieldRect.y += EditorGUIUtility.singleLineHeight;
        EditorGUI.PropertyField(singleFieldRect, usingZoomProp, m_UsingZoomContent);

        if (usingZoomProp.boolValue)
        {
            singleFieldRect.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(singleFieldRect, zoomProp, m_ZoomValueContent);

            singleFieldRect.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(singleFieldRect, zoomShiftCurveProp, m_ZoomCurveContent);
        }

        singleFieldRect.y += EditorGUIUtility.singleLineHeight; // Space

        singleFieldRect.y += EditorGUIUtility.singleLineHeight;
        EditorGUI.PropertyField(singleFieldRect, usingFocusProp, m_UsingFocusContent);

        if (usingFocusProp.boolValue)
        {
            singleFieldRect.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(singleFieldRect, resetFocusProp, m_ResetFocusContent);

            if (!resetFocusProp.boolValue && VisualStoryHelper.USING_CHARACTER_MAX_COUNT > 0)
            {
                singleFieldRect.y += EditorGUIUtility.singleLineHeight;
                EditorGUI.PropertyField(singleFieldRect, focusTargetIndexProp, m_FocusIndexContent);

                if (focusTargetIndexProp.isExpanded)
                {
                    indexSize = Mathf.Max(indexSize, 1);
                    singleFieldRect.y += EditorGUIUtility.singleLineHeight * (indexSize + 2);
                }
            }

            singleFieldRect.y += EditorGUIUtility.standardVerticalSpacing + EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(singleFieldRect, focusShiftCurveProp, m_FocusCurveContent);
        }
    }
}
