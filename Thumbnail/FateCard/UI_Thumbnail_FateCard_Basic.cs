using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Debug = COA_DEBUG.Debug;

public class UI_Thumbnail_FateCard_Basic : UIBaseBelongings
{
    [SerializeField] private Image img_Icon;
    //[SerializeField] private Image img_Icon_Symmetry;
    [SerializeField] private Image img_Grade;

    public fate_cardTable _Data { get; private set; }

    public void SetData(fate_cardTable data)
    {
        _Data = data;
        if (_Data == null)
        {
            SetActive(false);
            return;
        }

        var icon = _Data._Icon_Reference_Data?.GetSpriteFromSMTable(ownerUIBase);

        if (img_Icon != null)
            img_Icon.sprite = icon;
        if (img_Grade != null)
            img_Grade.sprite = _Data._Reward_Grade_Reference_Data.GetSpriteFromSMTable(ownerUIBase);
    }

    public void SetActive(bool active)
    {
        gameObject.SetActive(active);
    }
}