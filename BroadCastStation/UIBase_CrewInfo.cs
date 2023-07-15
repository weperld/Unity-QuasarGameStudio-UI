using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;
using TMPro;
using EnhancedUI.EnhancedScroller;
using Debug = COA_DEBUG.Debug;

public class UIBase_CrewInfo : UIBase, IEnhancedScrollerDelegate
{
    public class CrewCharacterData
    {
        public string characterEnumId;
    }
    private List<CrewCharacterData> listData = new List<CrewCharacterData>();
    private crewTable crewData;

    public Image imgBanner;
    public Image imgMark;

    public TextMeshProUGUI tmpuCrewName_1;
    public TextMeshProUGUI tmpuCrewName_2;
    public TextMeshProUGUI tmpuDesc;
    public TextMeshProUGUI tmpuCrewCount;

    public EnhancedScroller scroller;
    public EnhancedScrollerCellView cellViewPrefab;

    public Button btnBack;
    public Button btnCrewShot;

    private void Start()
    {
        btnBack.onClick.AddListener(OnClickBack);
        btnCrewShot.onClick.AddListener(OnClickCrewShot);
    }

    public override void Show(object param)
    {
        base.Show(param);

        listData.Clear();
        if (param is UIParam.BroadCast.BroadCastCrewInfoSetupParam p)
        {
            var character = Data.GetDataFromTable(Data._characterTable, p.characterEnumID);
            if (character != null)
            {
                var profile = character._Profile_Data;
                if (profile != null)
                {
                    crewData = profile._Crew_Data;
                    if (crewData != null)
                    {
                        if (crewData._Crew_Reference_Data != null)
                        {
                            imgMark.sprite = crewData._Crew_Reference_Data.GetSpriteFromSMTable(this);
                        }
                        if(crewData._Crew_Group_Photo_Reference_Data != null)
                        {
                            imgBanner.sprite = crewData._Crew_Group_Photo_Reference_Data.GetSpriteFromSMTable(this);
                        }
                        tmpuCrewName_1.text = Localizer.GetLocalizedStringName(Localizer.SheetType.LIBRARY, crewData._Enum_Id);
                        tmpuCrewName_2.text = Localizer.GetLocalizedStringName(Localizer.SheetType.LIBRARY, crewData._Enum_Id);
                        tmpuDesc.text = Localizer.GetLocalizedStringDesc(Localizer.SheetType.LIBRARY, crewData._Crew_Introduction);

                        if (crewData._Crew_Member_Data != null)
                        {
                            int cnt = crewData._Crew_Member_Data.Length;
                            int activeCount = 0;
                            for (int i = 0; i < cnt; i++)
                            {
                                var member = crewData._Crew_Member_Data[i];
                                if (member == null) continue;

                                activeCount++;
                                listData.Add(new CrewCharacterData
                                {
                                    characterEnumId = member._Enum_Id,
                                });
                            }
                            tmpuCrewCount.text = activeCount.ToString();
                        }
                    }
                }
            }
        }

        scroller.Delegate = this;
        scroller.ReloadData();
    }

    public override void Hide()
    {
        base.Hide();
    }

    public int GetNumberOfCells(EnhancedScroller scroller)
    {
        return listData.Count;
    }

    public float GetCellViewSize(EnhancedScroller scroller, int dataIndex)
    {
        return 386;
    }

    public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
    {
        UI_CrewCharacterItem cellView = scroller.GetCellView(cellViewPrefab) as UI_CrewCharacterItem;
        if (cellView != null)
        {
            cellView.name = "Cell " + dataIndex.ToString();
            cellView.SetData(listData[dataIndex]);
        }
        return cellView;
    }

    public void OnClickBack()
    {
        Hide();
    }

    public void OnClickCrewShot()
    {
        //UIManager._instance.ShowUIBase<UIBase_CrewShot>(
        //    DefineName.BroadCast.UIBASE_CREW_SHOT,
        //    new UIParam.BroadCast.BroadCastCrewShotSetupParam
        //    {
        //        crewData = crewData
        //    });
    }
}
