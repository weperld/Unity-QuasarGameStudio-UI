using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Debug = COA_DEBUG.Debug;

public class UI_GachaResult_Intro_Slot : MonoBehaviour
{
    [SerializeField] private GameObject[] go_Eff;

    public void SetEff(Data.Enum.Character_Grade grade)
    {
        foreach (var v in go_Eff)
            v.SetActive(false);

        switch (grade)
        {
            case Data.Enum.Character_Grade.GRADE_C:
                go_Eff[2].SetActive(true);
                break;
            case Data.Enum.Character_Grade.GRADE_B:
                go_Eff[1].SetActive(true);
                break;
            case Data.Enum.Character_Grade.GRADE_A:
                go_Eff[0].SetActive(true);
                break;
            default:
                break;
        }
    }
}