using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Debug = COA_DEBUG.Debug;

public class UI_Thumbnail_Integrate_Old : MonoBehaviour
{
    [SerializeField] private UI_Thumbnail_Asset_Basic_Old assetThumb;
    [SerializeField] private UI_Thumbnail_Character_Basic_Old charThumb;
    [SerializeField] private UI_Thumbnail_Consum_Basic consumThumb;
    [SerializeField] private GameObject go_Check;
    [SerializeField] private TextMeshProUGUI text_RewardCount;

    public void SetData(MailInfo.MailInfoReward data, int status)
    {
        if (data == null)
        {
            gameObject.SetActive(false);
            return;
        }
        gameObject.SetActive(true);

        go_Check?.SetActive(status == 2);

        if (data.type == Data.Enum.Common_Type.ASSET && Data._assetTable.TryGetValue(data.value, out var v))
        {
            assetThumb?.SetData(v);
            charThumb?.SetData(null); charThumb?.SetActive(false);
            consumThumb?.SetData(null);
        }

        else if (data.type == Data.Enum.Common_Type.CHARACTER && Data._characterTable.TryGetValue(data.value, out var a))
        {
            assetThumb?.SetData(null);
            charThumb?.SetData(a); charThumb?.SetActive(true);
            consumThumb?.SetData(null);
        }

        else if (data.type == Data.Enum.Common_Type.ITEM && Data._itemTable.TryGetValue(data.value, out var r))
        {
            assetThumb?.SetData(null);
            charThumb?.SetData(null); charThumb?.SetActive(false);
            consumThumb?.SetData(r);
        }

        if (text_RewardCount != null)
        {
            if (data.count > 1)
                text_RewardCount.text = data.count.ToString();
            else
                text_RewardCount.text = "";
        }
    }

    public void SetData(UIParam.Common.Reward data)
    {
        if (data == null)
        {
            gameObject.SetActive(false);
            return;
        }
        gameObject.SetActive(true);

        if (data.type == Data.Enum.Common_Type.ASSET && Data._assetTable.TryGetValue(data.key, out var v))
        {
            assetThumb?.SetData(v);
            charThumb?.SetData(null);
            charThumb?.SetActive(false);
            consumThumb?.SetData(null);
        }

        else if (data.type == Data.Enum.Common_Type.CHARACTER && Data._characterTable.TryGetValue(data.key, out var a))
        {
            assetThumb?.SetData(null);
            charThumb?.SetData(a);
            charThumb?.SetActive(true);
            consumThumb?.SetData(null);
        }

        else if (data.type == Data.Enum.Common_Type.ITEM && Data._itemTable.TryGetValue(data.key, out var r))
        {
            assetThumb?.SetData(null);
            charThumb?.SetData(null);
            charThumb?.SetActive(false);
            consumThumb?.SetData(r);
        }

        if (text_RewardCount != null)
        {
            if (data.value > 1)
                text_RewardCount.text = data.value.ToString();
            else
                text_RewardCount.text = "";
        }
    }
}