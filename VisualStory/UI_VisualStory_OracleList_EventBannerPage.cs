using EnhancedUI.EnhancedScroller;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Debug = COA_DEBUG.Debug;

public class UI_VisualStory_OracleList_EventBannerPage : EnhancedScrollerCellView
{
    [SerializeField] private UI_VisualStory_OracleList_StreamBanner firstBanner;
    [SerializeField] private UI_VisualStory_OracleList_StreamBanner secondBanner;

    public void Set(OracleEventBannerPageCellData cellData)
    {
        firstBanner?.Set(VisualStoryHelper.OracleCategory.EVENT, cellData?.firstInfo);
        secondBanner?.Set(VisualStoryHelper.OracleCategory.EVENT, cellData?.secondInfo);

        firstBanner?.SetOnClickAction(cellData?.onClickBannerAction);
        secondBanner?.SetOnClickAction(cellData?.onClickBannerAction);
    }
}