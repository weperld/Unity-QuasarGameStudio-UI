using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using EnhancedUI.EnhancedScroller;
using Debug = COA_DEBUG.Debug;

public class UI_BroadCastCrew : UIBaseBelongings, IEnhancedScrollerDelegate
{
    public class RelationshipData
    {
        public string nickName;
        public characterTable characterData;
    }
    private List<RelationshipData> listData = new List<RelationshipData>();
    private string characterEnumId;

    public Image imgUserCrewIcon;
    public Image imgCharacterCrewIcon;

    public TextMeshProUGUI tmpuUserCrewName;
    public TextMeshProUGUI tmpuCharacterCrewName;

    public EnhancedScroller scroller;
    public EnhancedScrollerCellView cellViewPrefab;

    public Button btnCharacterCrewInfo;

    private void Start()
    {
        btnCharacterCrewInfo.onClick.AddListener(OnClickCharacterCrewInfo);
    }

    public void Set(string characterEnumId)
    {
        this.characterEnumId = characterEnumId;

        listData.Clear();
        tmpuUserCrewName.text = $"{User.PlayerNickName} Crew";

        var character = Data.GetDataFromTable(Data._characterTable, characterEnumId);
        if(character != null)
        {
            var profile = character._Profile_Data;
            if(profile != null)
            {
                if(profile._Relationships_Data != null)
                {
                    int cnt = profile._Relationships_Data._Friend_Nickname.Length;
                    for(int i = 0; i < cnt; i++)
                    {
                        var characterData = profile._Relationships_Data._Friend_Data[i];
                        if (characterData == null) continue;

                        listData.Add(new RelationshipData
                        {
                            nickName = profile._Relationships_Data._Friend_Nickname[i],
                            characterData = characterData,
                        });
                    }
                }

                if(profile._Crew_Data != null 
                    && profile._Crew_Data._Crew_Reference_Data != null)
                {
                    imgCharacterCrewIcon.sprite = profile._Crew_Data._Crew_Reference_Data.GetSpriteFromSMTable(ownerUIBase);
                }
                tmpuCharacterCrewName.text = Localizer.GetLocalizedStringName(Localizer.SheetType.LIBRARY, profile._Crew_Data._Enum_Id);
            }
        }
        scroller.Delegate = this;
        scroller.ReloadData();

    }

    public int GetNumberOfCells(EnhancedScroller scroller)
    {
        return listData.Count;
    }

    public float GetCellViewSize(EnhancedScroller scroller, int dataIndex)
    {
        return 220f;
    }

    public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
    {
        UI_RecommendBroadCastItem cellView = scroller.GetCellView(cellViewPrefab) as UI_RecommendBroadCastItem;
        if(cellView != null)
        {
            cellView.name = "Cell " + dataIndex.ToString();
            cellView.SetData(listData[dataIndex]);
        }
        return cellView;
    }

    public void OnClickCharacterCrewInfo()
    {
        UIManager._instance.ShowUIBase<UIBase_CrewInfo>(
            DefineName.BroadCast.UIBASE_CREW_INFO,
            new UIParam.BroadCast.BroadCastCrewInfoSetupParam
            {
                characterEnumID = characterEnumId
            });
    }
}