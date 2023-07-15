using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Debug = COA_DEBUG.Debug;

public class UI_Thumbnail_Character_Basic : UIBaseBelongings
{
    [SerializeField] private Image img_Icon;
    [SerializeField] private Image img_Icon_Symmetry;
    [SerializeField] private Image img_Grade;
    //[SerializeField] private TextMeshProUGUI text_Content;

    public characterTable _Data
    {
        get; private set;
    }

    public void SetData(characterTable data)
    {
        _Data = data;
        if (_Data == null)
        {
            SetActive(false);
            return;
        }
        SetActive(true);

        var thumbnail = _Data._Resource_List_Data._Thumbnail_Reference_Data.GetSpriteFromSMTable(ownerUIBase);

        if (img_Icon != null)
            img_Icon.sprite = thumbnail;
        if (img_Icon_Symmetry != null)
            img_Icon_Symmetry.sprite = thumbnail;
        if (img_Grade != null)
            img_Grade.sprite = Data.GetCharacterTypeRscData(_Data._CE_Character_Grade)?._Image_Ref_Data[1]?.GetSpriteFromSMTable(ownerUIBase);
    }
    public void SetActive(bool active)
    {
        gameObject.SetActive(active);
    }
}