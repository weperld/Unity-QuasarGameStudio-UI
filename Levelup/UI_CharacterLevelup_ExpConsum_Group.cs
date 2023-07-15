using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Debug = COA_DEBUG.Debug;
using EnhancedUI.EnhancedScroller;

public class UI_CharacterLevelup_ExpConsum_Group : EnhancedScrollerCellView
{
    [SerializeField] private UI_Thumbnail_Consum_Levelup[] thumbnails;

    public int _CellCount => thumbnails != null ? thumbnails.Length : 1;

    public void SetDatas(List<LevelupItemData> dataList, int startIndex)
    {
        if (thumbnails == null) return;

        for (int i = 0; i < thumbnails.Length; i++)
        {
            var thumb = thumbnails[i];
            if (thumb == null) continue;

            var index = i + startIndex;
            if (index < dataList.Count) thumb.SetData(dataList[index]);
            else thumb.SetData(null);
        }
    }
}