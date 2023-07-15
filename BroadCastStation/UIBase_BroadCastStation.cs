using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;
using TMPro;
using Debug = COA_DEBUG.Debug;

public class UIBase_BroadCastStation : UIBase
{
    private string characterEnumId;
    private int currentCategoryIndex;

    public Image imgBanner;
    public Image imgBannerCharacter;
    public TextMeshProUGUI tmpuBannerCharacterName;
    public TextMeshProUGUI tmpuChannelName;
    public UI_GradeStar bannerGradeStar;

    public TextMeshProUGUI tmpuSubscriberCount;

    public UI_BroadCastBasic uiBasicPanel;
    public UI_BroadCastCrew uiCrewPanel;
    public UI_BroadCastLive uiLivePanel;
    public UI_BroadCastComment uiCommentPanel;
    public UI_BroadCastProfile uiProfilePanel;

    public GameObject goSubscrib;
    public GameObject goNoSubscrib;

    public Button btnBack;
    public Button btnHome;

    public UI_OnOff[] uIOnOffs;

    private void Start()
    {
        btnBack.onClick.AddListener(OnClickBack);
        btnHome.onClick.AddListener(OnClickHome);
    }

    public override void Show(object param)
    {
        base.Show(param);

        UIParam.BroadCast.BroadCastStationSetupParam p = param as UIParam.BroadCast.BroadCastStationSetupParam;
        if(p != null)
        {
            characterEnumId = p.characterEnumID;

            var character = Data.GetDataFromTable(Data._characterTable, characterEnumId);
            if(character != null)
            {
                if(character._Resource_List_Data != null
                    && character._Resource_List_Data._Thumbnail_SD_Reference_Data != null)
                {
                    imgBannerCharacter.sprite = character._Resource_List_Data._Thumbnail_SD_Reference_Data.GetSpriteFromSMTable(this);
                }
                tmpuBannerCharacterName.text = Localizer.GetLocalizedStringName(Localizer.SheetType.NONE, characterEnumId, Data.Enum.Language.LANGUAGE_EN);
                bannerGradeStar.SetStars((int)character._CE_Character_Grade);

                var profile = character._Profile_Data;
                if(profile != null)
                {
                    if(profile._Profile_Resource_Data != null
                        && profile._Profile_Resource_Data._Banner_Image_Reference_Data != null)
                    {
                        imgBanner.sprite = profile._Profile_Resource_Data._Banner_Image_Reference_Data.GetSpriteFromSMTable(this);
                    }
                    tmpuChannelName.text = Localizer.GetLocalizedStringName(Localizer.SheetType.LIBRARY, profile._Channel);
                    tmpuSubscriberCount.text = string.Format("{0:N0}", profile._Viewers);
                }
            }

            if (User._CharacterCollection.TryGetValue(characterEnumId, out CharacterCollectionInfo info))
            {
                goSubscrib.SetActive(info.Count > 0);
                goNoSubscrib.SetActive(info.Count <= 0);
            }
            else
            {
                goSubscrib.SetActive(false);
                goNoSubscrib.SetActive(true);
            }

            currentCategoryIndex = 0;
            uiBasicPanel.gameObject.SetActive(false);
            uiCrewPanel.gameObject.SetActive(false);
            uiLivePanel.gameObject.SetActive(false);
            uiCommentPanel.gameObject.SetActive(false);
            uiProfilePanel.gameObject.SetActive(false);
            foreach (var ui in uIOnOffs) ui.Off();

            uIOnOffs[currentCategoryIndex].On();
            SetActivePanel(currentCategoryIndex, true);
        }
    }

    public override void Hide()
    {
        base.Hide();
    }

    private void SetActivePanel(int index, bool isActive)
    {
        switch (index)
        {
            case 0:
                uiBasicPanel.gameObject.SetActive(isActive);
                uiBasicPanel.Set(characterEnumId);
                break;
            case 1:
                uiCrewPanel.gameObject.SetActive(isActive);
                uiCrewPanel.Set(characterEnumId);
                break;
            case 2:
                if (isActive)
                {
                    var waiting = UIMessageBox.Waiting("UI_Oneline_Server_Load_0_DESC");
                    User.GetOnAirOracles(
                        (result) =>
                        {
                            User.GetCompletedOracles(characterEnumId,
                                (result) =>
                                {
                                    waiting.InActive();
                                    uiLivePanel.Set(characterEnumId, result);
                                }, null);
                        }, null);
                }
                uiLivePanel.gameObject.SetActive(isActive);
                break;
            case 3:
                uiCommentPanel.gameObject.SetActive(isActive);
                uiCommentPanel.Set(characterEnumId);
                break;
            case 4:
                if (isActive)
                {
                    var waiting = UIMessageBox.Waiting("UI_Oneline_Server_Load_0_DESC");
                    User.GetCharacterCosutme(
                        characterEnumId,
                        (list) =>
                        {
                            waiting.InActive();
                            uiProfilePanel.Set(characterEnumId, list);
                        }, null);
                }
                uiProfilePanel.gameObject.SetActive(isActive);
                break;
        }
    }

    public void OnClickBack()
    {
        Hide();
    }

    public void OnClickHome()
    {
        UIManager._instance.HideAll();
    }

    public void OnClickCategory(int index)
    {
        uIOnOffs[currentCategoryIndex].Off();
        SetActivePanel(currentCategoryIndex, false);

        currentCategoryIndex = index;

        uIOnOffs[currentCategoryIndex].On();
        SetActivePanel(currentCategoryIndex, true);
    }
}
