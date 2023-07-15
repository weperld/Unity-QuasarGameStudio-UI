using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Debug = COA_DEBUG.Debug;
using EnhancedUI.EnhancedScroller;

public class UI_Thumbnail_Integrate : EnhancedScrollerCellView
{
    [SerializeField] private UI_Thumbnail_Consum_Basic thumbnail_Consum;
    [SerializeField] private UI_Thumbnail_Asset_Basic thumbnail_Asset;
    [SerializeField] private UI_Thumbnail_Character_Basic thumbnail_Character;
    [SerializeField] private UI_Thumbnail_FateCard_Basic thumbnail_FateCard;
    [SerializeField] private TextMeshProUGUI test_Amount;

    private UIParam.Common.Reward _reward;

    public void SetData(UIParam.Common.Reward reward, bool useAmount)
    {
        if (reward == null)
            return;

        gameObject.SetActive(true);

        _reward = reward;

        if (_reward.type == Data.Enum.Common_Type.ASSET && Data._assetTable.TryGetValue(_reward.key, out var asset_Data))
        {
            thumbnail_Asset?.SetData(asset_Data);
            thumbnail_Character?.SetData(null);
            thumbnail_Character?.SetActive(false);
            thumbnail_Consum?.SetData(null);
            thumbnail_FateCard?.SetData(null);

            if (useAmount && test_Amount != null)
                test_Amount.text = _reward.value.ToString();
        }

        else if (_reward.type == Data.Enum.Common_Type.CHARACTER && Data._characterTable.TryGetValue(_reward.key, out var ch_Data))
        {
            thumbnail_Asset?.SetData(null);
            thumbnail_Character?.SetData(ch_Data);
            thumbnail_Character?.SetActive(true);
            thumbnail_Consum?.SetData(null);
            thumbnail_FateCard?.SetData(null);

            if (useAmount && test_Amount != null)
                test_Amount.text = _reward.value.ToString();
        }

        else if (_reward.type == Data.Enum.Common_Type.ITEM && Data._itemTable.TryGetValue(_reward.key, out var item_Data))
        {
            thumbnail_Asset?.SetData(null);
            thumbnail_Character?.SetData(null);
            thumbnail_Character?.SetActive(false);
            thumbnail_Consum?.SetData(item_Data);
            thumbnail_FateCard?.SetData(null);

            if (useAmount && test_Amount != null)
                test_Amount.text = _reward.value.ToString();
        }

        else if (_reward.type == Data.Enum.Common_Type.FATE_CARD && Data._fate_cardTable.TryGetValue(_reward.key, out var fateCard_Data))
        {
            thumbnail_Asset?.SetData(null);
            thumbnail_Character?.SetData(null);
            thumbnail_Character?.SetActive(false);
            thumbnail_Consum?.SetData(null);
            thumbnail_FateCard?.SetData(fateCard_Data);

            if (useAmount && test_Amount != null)
                test_Amount.text = _reward.value.ToString();
        }

        test_Amount.gameObject.SetActive(useAmount);
    }
}