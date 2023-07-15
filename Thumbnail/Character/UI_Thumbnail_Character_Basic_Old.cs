using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Debug = COA_DEBUG.Debug;

public class UI_Thumbnail_Character_Basic_Old : UIBaseBelongings
{
    [SerializeField] private Image img_Character;
    [SerializeField] private Image img_Property;

    public characterTable _Data { get; private set; }
    public void SetData(characterTable data)
    {
        _Data = data;
        if (_Data == null) return;

        if (img_Character != null) img_Character.sprite = _Data._Resource_List_Data._Thumbnail_Reference_Data.GetSpriteFromSMTable(ownerUIBase);

        if (img_Property != null)
        {
            Data._character_type_resourceTable.TryGetValue(_Data._CE_Character_Property.ToString(), out var v);
            img_Property.sprite = v?._Image_Ref_Data[0]?.GetSpriteFromSMTable(ownerUIBase);
        }
    }
    public void SetActive(bool active)
    {
        gameObject.SetActive(active);
    }
}