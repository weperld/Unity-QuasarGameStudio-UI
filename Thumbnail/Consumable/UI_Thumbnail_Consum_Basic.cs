using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Debug = COA_DEBUG.Debug;

public class UI_Thumbnail_Consum_Basic : UIBaseBelongings
{
    [SerializeField] private Image img_Icon;
    [SerializeField] private Image img_Icon_Symmetry;
    [SerializeField] private Image img_Grade;

    public itemTable _Data { get; private set; }

    public void SetData(itemTable data)
    {
        _Data = data;
        if (_Data == null) { SetActive(false); return; }
        SetActive(true);

        var icon = _Data._Image_Reference_Data?.GetSpriteFromSMTable(ownerUIBase);
        if (img_Icon != null) img_Icon.sprite = icon;
        if (img_Icon_Symmetry != null) img_Icon_Symmetry.sprite = icon;
        if (img_Grade != null) img_Grade.sprite = _Data._Grade_Image_Reference_Data?.GetSpriteFromSMTable(ownerUIBase);
    }

    public void SetActive(bool active)
    {
        gameObject.SetActive(active);
    }
}