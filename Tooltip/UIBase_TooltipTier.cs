using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using Debug = COA_DEBUG.Debug;

public class UIBase_TooltipTier : UIBase
{
    public Button btnClose;
    public UI_TooltipTierItem[] items;

    private void Start()
    {
        btnClose.onClick.AddListener(OnClickHide);
    }

    public override void Show(object param)
    {
        base.Show(param);
        var sorted = Data._tierTable.Values.OrderBy(x => x._Rank_Up_Necessary).ToArray();
        for (int i = 0; i < sorted.Length; i++)
        {
            var current = sorted[i];
            var next = i < (sorted.Length - 1) ? sorted[i + 1] : sorted[i];
            items[i].Set(current, next);
        }
    }

    public override void Hide()
    {
        base.Hide();
    }

    private void OnClickHide()
    {
        Hide();
    }
}
