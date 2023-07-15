using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;
using TMPro;
using Debug = COA_DEBUG.Debug;
using static UIParam.Tooltip;

public class UIBase_Tooltip_SkillStatusEffect : UIBase_TooltipBase<SkillStatusEffect>
{
    [SerializeField] private Image img_Icon;
    [SerializeField] private TextMeshProUGUI tmpu_Name;
    [SerializeField] private TextMeshProUGUI tmpu_Desc;

    protected override void OnInit(SkillStatusEffect param)
    {
        
    }

    protected override void SetInfoToUI(SkillStatusEffect param)
    {
        var seData = param.seData;
        var category = seData._CE_Effect_Category;
        if (category != Data.Enum.Effect_Category.POS_STATUS_EFFECT
            && category != Data.Enum.Effect_Category.NEG_STATUS_EFFECT)
        {
            Hide();
            return;
        }

        if (img_Icon != null)
        {
            var refData = seData._Icon_Reference_Data;
            bool isIcon = refData != null;
            img_Icon.gameObject.SetActive(isIcon);
            if (isIcon) img_Icon.sprite = refData.GetSpriteFromSMTable(this);
        }
        if (tmpu_Name != null) tmpu_Name.text = Localizer.GetLocalizedStringName(Localizer.SheetType.SKILL, seData._Enum_Id);
        if (tmpu_Desc != null) tmpu_Desc.text = Localizer.GetLocalizedStringDesc(Localizer.SheetType.SKILL, seData._Enum_Id);
    }
}
