using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Debug = COA_DEBUG.Debug;

public class UI_GachaCharacterPreview_WithBg : UIBaseBelongings
{
    [SerializeField] private UI_GachaCharacterPreview _gachaCharacterPreview;
    [SerializeField] private Image img_Ch_Back;

    public void Set(characterTable data, bool useSkeleton)
    {
        if (data == null)
        {
            gameObject.SetActive(false);
            return;
        }
        gameObject.SetActive(true);
        if (img_Ch_Back != null) img_Ch_Back.sprite = data?._Resource_List_Data?._Background_Reference_Data?.GetSpriteFromSMTable(ownerUIBase);
        _gachaCharacterPreview?.SetData(data, useSkeleton);
    }
}