using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Debug = COA_DEBUG.Debug;

public class UI_Thumbnail_Character_GradeFrame : MonoBehaviour
{
    [SerializeField] private GameObject go_A;
    [SerializeField] private GameObject go_B;
    [SerializeField] private GameObject go_C;
    [SerializeField] private GameObject go_D;
    [SerializeField] private UI_Thumbnail_Character_Basic_Old thumbnail;
    [SerializeField] private Button btn_Change;

    public void SetData(characterTable data)
    {
        thumbnail?.SetData(data);
        if (data == null) return;

        var grade = data._CE_Character_Grade;
        go_A?.SetActive(grade == Data.Enum.Character_Grade.GRADE_A);
        go_B?.SetActive(grade == Data.Enum.Character_Grade.GRADE_B);
        go_C?.SetActive(grade == Data.Enum.Character_Grade.GRADE_C);
        go_D?.SetActive(grade == Data.Enum.Character_Grade.GRADE_D);
    }
}