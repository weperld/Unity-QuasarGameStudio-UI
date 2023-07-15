using EnhancedUI.EnhancedScroller;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UIParam;
using Debug = COA_DEBUG.Debug;

public class UI_Thumbnail_Integrate_Btn : EnhancedScrollerCellView
{
    [SerializeField] private UI_Thumbnail_Integrate thumbnail_Integrate;
    [SerializeField] private TooltipCorrectionalPosTFs tooltipTf;
    [SerializeField] private Button btn_Slot;

    private Common.Reward _reward;

    public void Set_Thumbnail_Btn(Common.Reward reward, bool useAmount)
    {
        if (reward == null)
            return;
        _reward = reward;
        
        gameObject.SetActive(true);
        thumbnail_Integrate?.SetData(_reward, useAmount);
        UIUtil.ResetAndAddListener(btn_Slot, OnClickBtnSlot);
    }


    private void OnClickBtnSlot()
    {
        long amount;
        if (_reward.type == Data.Enum.Common_Type.ASSET)
        {
            Data._assetTable.TryGetValue(_reward.key, out var v);
            amount = User.GetAsset(v._CE_Asset)._Balance;
        }

        else if (_reward.type == Data.Enum.Common_Type.ITEM)
        {
            Data._itemTable.TryGetValue(_reward.key, out var v);
            if (User._Consumables.TryGetValue((int)v._Id, out var v1))
            {
                amount = v1._Count;
            }
            else
                amount = _reward.value;
        }
        else
            amount = 0;

        var rewardParam = new Common.Reward(_reward.type, _reward.key, amount);

        UIManager._instance.ShowUIBase<UIBase_TooltipReward>(
            DefineName.UITooltip.REWARD_TOOLTIP, new Tooltip.Reward(rewardParam, tooltipTf));
    }
}