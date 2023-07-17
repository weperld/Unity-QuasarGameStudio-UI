using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ShakeTweenBehaviour))]
public class ShakeTweenDrawer : PropertyDrawer
{
    GUIContent m_VpsContent = new GUIContent("Vibe Per Sec");
    GUIContent m_VpwrContent = new GUIContent("Max Power");
    GUIContent m_CurveContent = new GUIContent("Power Curve");
    GUIContent m_XRateContent = new GUIContent("X-Axis Power Rate");
    GUIContent m_YRateContent = new GUIContent("Y-Axix Power Rate");
    GUIContent m_ZRateContent = new GUIContent("Z-Axis Power Rate");

    public override float GetPropertyHeight (SerializedProperty property, GUIContent label)
    {
        int fieldCount = 5;
        return fieldCount * EditorGUIUtility.singleLineHeight;
    }

    public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
    {
        property = property.FindPropertyRelative("setter");
        var vpsProp = property.FindPropertyRelative("vibePerSec");
        var vpwrProp = property.FindPropertyRelative("vibePower");
        var curveProp = property.FindPropertyRelative("powerCurve");
        var pwrRateProp = property.FindPropertyRelative("axisPowerRate");

        var originWidth = position.width;
        var originX = position.x;

        Rect singleFieldRect = new Rect(position.x, position.y, originWidth / 2f, EditorGUIUtility.singleLineHeight);
        EditorGUI.PropertyField(singleFieldRect, vpsProp, m_VpsContent);

        singleFieldRect.x += originWidth / 2f;
        EditorGUI.PropertyField(singleFieldRect, vpwrProp, m_VpwrContent);

        singleFieldRect.x = originX;
        singleFieldRect.width = originWidth;
        singleFieldRect.y += EditorGUIUtility.singleLineHeight;
        EditorGUI.PropertyField(singleFieldRect, curveProp, m_CurveContent);

        singleFieldRect.y += EditorGUIUtility.singleLineHeight;
        float xVal = EditorGUI.Slider(singleFieldRect, m_XRateContent, pwrRateProp.vector3Value.x, 0f, 1f);

        singleFieldRect.y += EditorGUIUtility.singleLineHeight;
        float yVal = EditorGUI.Slider(singleFieldRect, m_YRateContent, pwrRateProp.vector3Value.y, 0f, 1f);

        singleFieldRect.y += EditorGUIUtility.singleLineHeight;
        float zVal = EditorGUI.Slider(singleFieldRect, m_ZRateContent, pwrRateProp.vector3Value.z, 0f, 1f);

        pwrRateProp.vector3Value = new Vector3(xVal, yVal, zVal);
    }
}
