using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(VStoryShowProductionBehaviour))]
public class VStoryShowProductionDrawer : PropertyDrawer
{
    GUIContent gc_ProductionEnumId = new GUIContent("Production Enum Id");

    public override float GetPropertyHeight (SerializedProperty property, GUIContent label)
    {
        int fieldCount = 1;
        return fieldCount * EditorGUIUtility.singleLineHeight;
    }

    public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
    {
        property = property.FindPropertyRelative("setter");
        var enumProp = property.FindPropertyRelative("productionEnumId");

        var rect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        EditorGUI.PropertyField(rect, enumProp, gc_ProductionEnumId);
    }
}
