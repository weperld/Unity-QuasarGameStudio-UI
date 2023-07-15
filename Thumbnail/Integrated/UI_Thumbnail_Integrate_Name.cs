using EnhancedUI.EnhancedScroller;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Debug = COA_DEBUG.Debug;

public class UI_Thumbnail_Integrate_Name : EnhancedScrollerCellView
{
    [SerializeField] private UI_Thumbnail_Integrate thumbnail;
    [SerializeField] private TextMeshProUGUI tmpu_Name;

    public void SetData(UIParam.Common.Reward reward, bool useAmount)
    {
        if (thumbnail != null) thumbnail.SetData(reward, useAmount);
        if (tmpu_Name != null) tmpu_Name.text = Localizer.GetLocalizedStringName(Localizer.SheetType.NONE, reward.key);
    }
}