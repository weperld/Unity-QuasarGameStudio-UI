using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnhancedUI.EnhancedScroller;
using Debug = COA_DEBUG.Debug;
using UnityEngine.UI;

public class UI_PartnerList_CardGroup : EnhancedScrollerCellView
{
    [SerializeField] private UI_PartnerList_Card[] cards;

    public float _Height
    {
        get
        {
            //LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
            return GetComponent<RectTransform>().rect.height;
        }
    }
    public int _CountPerRow => cards != null ? cards.Length : 1;

    public void Set(List<PartnerListScrollData> dataList, int startIndex)
    {
        if (cards == null) return;
        for (int i = 0; i < cards.Length; i++)
        {
            var card = cards[i];
            if (card == null) continue;

            var index = i + startIndex;
            if (index < dataList.Count) card.Set(dataList[i + startIndex]);
            else card.Set(null);
        }
    }
}