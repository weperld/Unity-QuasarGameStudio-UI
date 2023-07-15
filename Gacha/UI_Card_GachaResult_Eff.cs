using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Debug = COA_DEBUG.Debug;

public class UI_Card_GachaResult_Eff : MonoBehaviour
{
    [SerializeField] private GameObject[] card_GachaResult_Eff;

    public void SetEff(Data.Enum.Character_Grade grade)
    {
        foreach (var v in card_GachaResult_Eff)
            v.SetActive(false);


        if(grade== Data.Enum.Character_Grade.GRADE_A)
            card_GachaResult_Eff[0].SetActive(true);

        //switch (grade)
        //{
        //    case Data.Enum.Character_Grade.GRADE_C:
        //        card_GachaResult_Eff[2].SetActive(true);
        //        break;
        //    case Data.Enum.Character_Grade.GRADE_B:
        //        card_GachaResult_Eff[1].SetActive(true);
        //        break;
        //    case Data.Enum.Character_Grade.GRADE_A:
        //        card_GachaResult_Eff[0].SetActive(true);
        //        break;
        //    default:
        //        break;
        //};
    }
}