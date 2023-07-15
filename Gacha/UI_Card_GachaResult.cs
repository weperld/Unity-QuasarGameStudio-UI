using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Debug = COA_DEBUG.Debug;

public class UI_Card_GachaResult : UIBaseBelongings
{
    [SerializeField] private UI_GradeStar gradeStar;
    [SerializeField] private Character_UI_Portrait_UpperBody ch_Image;
    [SerializeField] private Image img_Class;
    [SerializeField] private Image img_Property;

    private characterTable _ch_Table;

    public void SetData(characterTable ch_Table)
    {
        if (ch_Table == null)
            return;

        _ch_Table = ch_Table;

        gradeStar.SetStars((int)_ch_Table._CE_Character_Grade);
        gradeStar.SetStars((int)_ch_Table._CE_Character_Grade);

        ch_Image.Set(_ch_Table, 0, false);

        Data._character_type_resourceTable.TryGetValue(_ch_Table._CE_Character_Class.ToString(), out var ch_Class);
        img_Class.sprite = ch_Class._Image_Ref_Data[0].GetSpriteFromSMTable(ownerUIBase);

        Data._character_type_resourceTable.TryGetValue(_ch_Table._CE_Character_Property.ToString(), out var ch_Property);
        img_Property.sprite = ch_Property._Image_Ref_Data[0].GetSpriteFromSMTable(ownerUIBase);
    }
}