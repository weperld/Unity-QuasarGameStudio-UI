using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Debug = COA_DEBUG.Debug;
using UnityEngine.UI;
using TMPro;

public class UI_TooltipTierItem : UIBaseBelongings
{
    public Image imgTier;
    public TextMeshProUGUI tmpuTierName;
    public TextMeshProUGUI tmpuPoint;

    public void Set(tierTable current, tierTable next)
    {
        imgTier.sprite = current._Icon_Reference_Data.GetSpriteFromSMTable(ownerUIBase);
        tmpuTierName.text = Localizer.GetLocalizedStringName(Localizer.SheetType.NONE, current._Enum_Id);
        if (current != null && next != null)
        {
            if(current._Rank_Up_Necessary == next._Rank_Up_Necessary)
            {
                tmpuPoint.text = $"{current._Rank_Up_Necessary}";
            }
            else
            {
                tmpuPoint.text = $"{current._Rank_Up_Necessary} ~ {next._Rank_Up_Necessary - 1}";
            }
        }
    }
}