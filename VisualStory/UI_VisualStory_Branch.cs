using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static TimelineParam;
using Debug = COA_DEBUG.Debug;
using static TweenHelper;
using AudioSystem;

public abstract class UI_VisualStory_Branch : UIBaseBelongings
{
    [SerializeField] private bool useCloseButton;
    [SerializeField] private Button btn_Close;
    [SerializeField] private Button btn_TouchBlocker;
    [SerializeField] private DOTweenAnimation panelShakeTweener;
    [SerializeField] protected Button[] btn_SelectableBranch;

    protected TimelineBranch[] arr_BranchTimelineEnumId;
    protected Action<TimelineBranch> onSelectBranch;

    private void OnValidate()
    {
        if (btn_Close != null) btn_Close.gameObject.SetActive(useCloseButton);
    }
    private void Awake()
    {
        UIUtil.ResetAndAddListener(btn_TouchBlocker, OnClickTouchBlocker);
        if (btn_SelectableBranch != null)
        {
            for (int i = 0; i < btn_SelectableBranch.Length; i++)
            {
                var btn = btn_SelectableBranch[i];
                if (btn == null) continue;

                var index = i;
                UIUtil.ResetAndAddListener(btn, () => { OnClickSelectableButton(index); });
            }
        }
        UIUtil.ResetAndAddListener(btn_Close, OnClickClose);
    }
    private void OnClickSelectableButton(int index)
    {
        AudioManager._instance.PlaySound(DefineName.Audio_OutGame.Sound.BUTTON);
        onSelectBranch?.Invoke(arr_BranchTimelineEnumId[index]);
        Hide();
    }
    private void OnClickTouchBlocker()
    {
        panelShakeTweener.ControlTweener(ControlState.RESET,
            tweener =>
            {
                var target = tweener.GetTargetGO();
                if (target == null) return;
                target.transform.localPosition = Vector3.zero;
            },
            tweener =>
            {
                tweener.ControlTweener(ControlState.PLAY);
            });
    }
    private void OnClickClose()
    {
        Hide();
    }

    public void SetupAndShow(Action<TimelineBranch> onSelectBranch, params TimelineBranch[] timelineBranchParams)
    {
        if (timelineBranchParams == null || timelineBranchParams.Length == 0) return;
        gameObject.SetActive(true);
        this.onSelectBranch = onSelectBranch;

        arr_BranchTimelineEnumId = timelineBranchParams;
        for (int i = 0; i < btn_SelectableBranch.Length; i++)
        {
            var btn = btn_SelectableBranch[i];
            if (btn == null) continue;

            var activate = i < arr_BranchTimelineEnumId.Length;
            btn.gameObject.SetActive(activate);
            if (activate) SetupSelectable(i, arr_BranchTimelineEnumId[i]);
        }
    }

    protected abstract void SetupSelectable(int index, TimelineBranch branchTimelineParam);

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}