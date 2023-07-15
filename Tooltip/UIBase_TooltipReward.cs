using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static UIParam.Tooltip;
using Debug = COA_DEBUG.Debug;


public class UIBase_TooltipReward : UIBase_TooltipBase<Reward>
{
    [SerializeField] private UI_Tooltip_RewardBody rewardContentBody;

    protected override void OnInit(Reward param)
    {
        
    }

    protected override void SetInfoToUI(Reward param)
    {
        if (rewardContentBody != null) rewardContentBody.Set(param?._ToolTipReward);
    }
}