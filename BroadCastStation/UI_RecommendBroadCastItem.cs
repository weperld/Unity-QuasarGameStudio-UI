using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using EnhancedUI.EnhancedScroller;
using Debug = COA_DEBUG.Debug;

public class UI_RecommendBroadCastItem : UIBaseBelongingEnhancedScrollerCellView
{
    private string characterEnumId;

    public Image imgCharacter;
    public TextMeshProUGUI tmpuName;
    public Button btnClick;

    private void Start()
    {
        btnClick.onClick.AddListener(OnClick);
    }

    public void SetData(UI_BroadCastCrew.RelationshipData data)
    {
        var character = data.characterData;
        if(character != null)
        {
            characterEnumId = character._Enum_Id;
            if (character._Resource_List_Data != null
                && character._Resource_List_Data._Thumbnail_Reference_Data != null)
            {
                imgCharacter.sprite = character._Resource_List_Data._Thumbnail_Reference_Data.GetSpriteFromSMTable(ownerUIBase);
            }
        }
        tmpuName.text = Localizer.GetLocalizedStringName(Localizer.SheetType.LIBRARY, data.nickName);
    }

    public void OnClick()
    {
        UIManager._instance.ShowUIBase<UIBase_BroadCastStation>(
            DefineName.BroadCast.UIBASE_BROADCAST_STATION,
            new UIParam.BroadCast.BroadCastStationSetupParam
            {
                characterEnumID = characterEnumId
            });
    }
}