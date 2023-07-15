using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Debug = COA_DEBUG.Debug;

public abstract class UI_VisualStory_OracleList_StreamingThumbnail : UIBaseBelongings
{
    [SerializeField] protected Button btn_Streaming;
    [SerializeField] protected RectTransform rtf_LayoutRebuildTarget;
    [SerializeField] protected UI_TextSlider titleSlider;

    private Action<VisualStoryInfo> onClickStreamingAction;

    protected VisualStoryInfo _Info { get; private set; }
    protected VisualStoryHelper.OracleCategory _Category { get; private set; }

    private void OnDisable()
    {
    }

    public void Set(VisualStoryHelper.OracleCategory category, VisualStoryInfo info)
    {
        _Category = category;

        if (_Info != null)
        {
            _Info.onChangeTakeRewardsState -= OnChangeTakeRewardsState;
        }

        _Info = info;
        if (_Info == null) { OnSetToNullInfo(category); return; }

        OnSet(category, _Info);

        if (rtf_LayoutRebuildTarget != null) LayoutRebuilder.ForceRebuildLayoutImmediate(rtf_LayoutRebuildTarget);
        if (titleSlider != null) titleSlider.SetText(Localizer.GetLocalizedStringName(Localizer.SheetType.STRING, _Info._RootTimeline._Enum_Id));

        _Info.onChangeTakeRewardsState -= OnChangeTakeRewardsState;
        _Info.onChangeTakeRewardsState += OnChangeTakeRewardsState;
        OnChangeTakeRewardsState(_Info._TookRewards);
    }

    protected abstract void OnChangeTakeRewardsState(bool value);
    protected abstract void OnSetToNullInfo(VisualStoryHelper.OracleCategory category);
    protected abstract void OnSet(VisualStoryHelper.OracleCategory category, VisualStoryInfo info);

    public void SetOnClickAction(Action<VisualStoryInfo> action)
    {
        onClickStreamingAction = action;
        UIUtil.ResetAndAddListener(btn_Streaming, () => onClickStreamingAction?.Invoke(_Info));
    }
}