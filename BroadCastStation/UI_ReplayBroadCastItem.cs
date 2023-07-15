using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnhancedUI.EnhancedScroller;
using UnityEngine.UI;
using TMPro;
using Debug = COA_DEBUG.Debug;

public class UI_ReplayBroadCastItem : EnhancedScrollerCellView
{
    public TextMeshProUGUI tmpuTitle;

    public void SetData(string title)
    {
        tmpuTitle.text = Localizer.GetLocalizedStringName(Localizer.SheetType.STRING, title);
    }
}