using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using In_Game = DefineName.Audio_InGame.Sound;
using Out_Game = DefineName.Audio_OutGame.Sound;
using Visual_Story = DefineName.Audio_Story.Sound;

[CustomPropertyDrawer(typeof(PlaySoundBehaviour))]
public class PlaySoundDrawer : PropertyDrawer
{
    private string[] arr_MainCategory = new string[] { nameof(In_Game), nameof(Out_Game), nameof(Visual_Story) };
    private string[] arr_Sound;

    GUIContent gc_Label = new GUIContent("사운드 추가 필요 시 개발자에게 요청하여,\n" +
        "DefineName_Audio에 카테고리에 맞춰 파일명 추가 필요");

    private readonly string categoryLabel = "Sound Audio Category";
    private readonly string audioLabel = "Sound Audio Selection";

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        int fieldCount = 5;
        return fieldCount * EditorGUIUtility.singleLineHeight;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var volumeProp = property.FindPropertyRelative("volume");
        property = property.FindPropertyRelative("setter");
        var categoryNameProp = property.FindPropertyRelative("categoryName");
        var prevCategoryNameProp = property.FindPropertyRelative("prevCategoryName");
        var audioNameProp = property.FindPropertyRelative("audioName");

        var categoryIndex = 0;
        var audioIndex = 0;
        foreach (var category in arr_MainCategory)
        {
            var l_CategoryIndex = arr_MainCategory.ToList().IndexOf(category);
            var audios = GetFields(category switch
            {
                nameof(In_Game) => typeof(In_Game),
                nameof(Out_Game) => typeof(Out_Game),
                nameof(Visual_Story) => typeof(Visual_Story),
                _ => typeof(In_Game)
            }).ToList();

            var find = audios.FindIndex(f => f == audioNameProp.stringValue);
            if (find >= 0)
            {
                categoryIndex = l_CategoryIndex;
                audioIndex = find;
                break;
            }
        }

        Rect rect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        categoryIndex = EditorGUI.Popup(rect, categoryLabel, categoryIndex, arr_MainCategory);

        categoryNameProp.stringValue = arr_MainCategory[categoryIndex];
        if (!categoryNameProp.stringValue.Equals(prevCategoryNameProp.stringValue))
        {
            audioIndex = 0;
            audioNameProp.stringValue = string.Empty;
            prevCategoryNameProp.stringValue = categoryNameProp.stringValue;
        }

        Type type = typeof(In_Game);
        if (categoryIndex == 1) type = typeof(Out_Game);
        else if (categoryIndex == 2) type = typeof(Visual_Story);
        arr_Sound = GetFields(type);

        if (arr_Sound.Length > 0)
        {
            EditorGUI.indentLevel += 1;

            rect.y += EditorGUIUtility.singleLineHeight;
            audioIndex = EditorGUI.Popup(rect, audioLabel, audioIndex, arr_Sound);
            audioNameProp.stringValue = arr_Sound[audioIndex];

            rect.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(rect, volumeProp);

            EditorGUI.indentLevel -= 1;
        }
        else audioNameProp.stringValue = string.Empty;

        rect.y += EditorGUIUtility.singleLineHeight;
        rect.height = EditorGUIUtility.singleLineHeight * 2f;
        EditorGUI.LabelField(rect, gc_Label);
    }
    private string[] GetFields(Type type)
    {
        return type
            .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
            .Where(fi => fi.IsLiteral && !fi.IsInitOnly && fi.FieldType == typeof(string))
            .Select(x => (string)x.GetRawConstantValue()).ToArray();
    }
}
