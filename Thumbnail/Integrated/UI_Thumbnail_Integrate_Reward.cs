using EnhancedUI.EnhancedScroller;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Debug = COA_DEBUG.Debug;

public class UI_Thumbnail_Integrate_Reward : EnhancedScrollerCellView
{
    [SerializeField] private UI_Thumbnail_Integrate_Name thumbnail;
    [SerializeField] private Animation ani;
    [SerializeField] private CanvasGroup canvasGroup;

    public CanvasGroup _CanvasGroup => canvasGroup;

    public void SetData(UIParam.Common.Reward reward, bool useAmount)
    {
        if(reward == null) { gameObject.SetActive(false); return; }

        if (thumbnail != null) thumbnail.SetData(reward, useAmount);
    }

    public void Appear()
    {
        gameObject.SetActive(true);

        if (ani == null) return;
        ani.Play();
    }
}