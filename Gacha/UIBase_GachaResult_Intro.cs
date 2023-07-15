using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;
using TMPro;
using Debug = COA_DEBUG.Debug;

public class UIBase_GachaResult_Intro : UIBase
{
    [SerializeField] private UI_GachaResult_Intro_Slot[] uI_GachaResult_Intro_Slot;

    private UIParam.Gacha.GachaIntro intro_Data;
    public override void Show(object param)
    {
        if (param == null)
            return;

        intro_Data = param as UIParam.Gacha.GachaIntro;

        base.Show(param);
        Set_Intro();
    }

    private void Set_Intro()
    {
        AudioSystem.AudioManager._instance.PlaySound(DefineName.Audio_OutGame.Sound.RECRUITANI);
        for (int i = 0; i < uI_GachaResult_Intro_Slot.Length; i++)
        {
            uI_GachaResult_Intro_Slot[i].SetEff(intro_Data.rewardlist.RewardToCharacterDatas()[i]._CE_Character_Grade);
        }
    }
    private void End_Anim()
    {
        Hide();
        intro_Data.action?.Invoke();
    }

    private void End_Effect(int index)
    {
        if (intro_Data.rewardlist.RewardToCharacterDatas()[index]._CE_Character_Grade >= Data.Enum.Character_Grade.GRADE_A)
            AudioSystem.AudioManager._instance.PlaySound(DefineName.Audio_OutGame.Sound.RECRUITEFFECT_1);
        else if (intro_Data.rewardlist.RewardToCharacterDatas()[index]._CE_Character_Grade >= Data.Enum.Character_Grade.GRADE_C)
            AudioSystem.AudioManager._instance.PlaySound(DefineName.Audio_OutGame.Sound.RECRUITEFFECT_2);
    }

    public override void Hide()
    {
        base.Hide();
    }
}
