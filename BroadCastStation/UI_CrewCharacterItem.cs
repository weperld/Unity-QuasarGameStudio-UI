using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using EnhancedUI.EnhancedScroller;
using Debug = COA_DEBUG.Debug;

public class UI_CrewCharacterItem : UIBaseBelongingEnhancedScrollerCellView
{
    private string characterEnumId;

    public Character_UI_Portrait_UpperBody uIPortrait;
    public Image imgThumbnail;
    public TextMeshProUGUI tmpuCharacterName;
    
    public GameObject goSubscrib;
    public GameObject goNoSubscrib;

    public Button btnClick;

    private void Start()
    {
        btnClick.onClick.AddListener(OnClick);
    }

    public void SetData(UIBase_CrewInfo.CrewCharacterData data)
    {
        var character = Data.GetDataFromTable(Data._characterTable, data.characterEnumId);
        if (character != null)
        {
            uIPortrait.Set(character, 0, false);

            characterEnumId = character._Enum_Id;
            if (character._Resource_List_Data != null
                && character._Resource_List_Data._Night_LD_Reference_Data != null)
            {
                imgThumbnail.sprite = character._Resource_List_Data._Thumbnail_Reference_Data.GetSpriteFromSMTable(ownerUIBase);
            }
            tmpuCharacterName.text = Localizer.GetLocalizedStringName(Localizer.SheetType.NONE, data.characterEnumId);

            if(User._CharacterCollection.TryGetValue(data.characterEnumId, out CharacterCollectionInfo info))
            {
                goSubscrib.SetActive(info.Count > 0);
                goNoSubscrib.SetActive(info.Count <= 0);
            }
            else
            {
                goSubscrib.SetActive(false);
                goNoSubscrib.SetActive(true);
            }
        }
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