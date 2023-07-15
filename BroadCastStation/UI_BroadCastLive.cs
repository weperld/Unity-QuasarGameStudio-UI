using System;
using System.Collections;
using System.Collections.Generic;
using echo17.Signaler.Demos.Demo4;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Spine.Unity;
using EnhancedUI.EnhancedScroller;
using Debug = COA_DEBUG.Debug;

public class UI_BroadCastLive : UIBaseBelongings, IEnhancedScrollerDelegate
{
    private List<string> listData = new List<string>();
    private OracleInfo oracleInfo;

    public RectTransform rtLayout;

    public TextMeshProUGUI tmpuViewerCount;
    public TextMeshProUGUI tmpuCharacterName;

    public UI_TextSlider uiTextSlider_1;
    public UI_TextSlider uiTextSlider_2;

    public Image imgReadyBg;
    public Button btnLink;

    public GameObject goLive;
    public GameObject goReady;

    public UI_VisualStory_OracleList_StreamBanner liveBanner;

    public EnhancedScroller scroller;
    public EnhancedScrollerCellView cellViewPrefab;

    private void Start()
    {
        btnLink.onClick.AddListener(OnClickLink);
    }

    public void Set(string characterEnumId, OracleListInfo completeList)
    {
        oracleInfo = User._OracleListCategoryInfo.GetOnAirOracleInfoByCharacter(characterEnumId);
        if(oracleInfo != null)
        {
            goLive.SetActive(true);
            goReady.SetActive(false);

            VisualStoryHelper.OracleCategory category = VisualStoryHelper.OracleCategory.EVENT;
            switch (oracleInfo._OracleType)
            {
                case Data.Enum.Oracle_Broadcast_Type.EVENT:
                    category = VisualStoryHelper.OracleCategory.EVENT;
                    break;
                case Data.Enum.Oracle_Broadcast_Type.REGULAR:
                case Data.Enum.Oracle_Broadcast_Type.IRREGULAR:
                    category = VisualStoryHelper.OracleCategory.FOLLOW;
                    break;
                case Data.Enum.Oracle_Broadcast_Type.NOGET:
                    category = VisualStoryHelper.OracleCategory.SUGGESTION;
                    break;
            }
            liveBanner.Set(category, oracleInfo);

            int viewCount = (int)(oracleInfo._RootTimeline._Optional_Int * UnityEngine.Random.Range(VisualStoryHelper.VIEWER_COUNT_MIN_RATE_LIMIT, VisualStoryHelper.VIEWER_COUNT_MAX_RATE_LIMIT));
            tmpuViewerCount.text = string.Format("{0:N0}", viewCount);

            string characterName = Localizer.GetLocalizedStringName(Localizer.SheetType.NONE, characterEnumId);
            string title = Localizer.GetLocalizedStringName(Localizer.SheetType.STRING, oracleInfo._RootTimeline._Enum_Id);

            tmpuCharacterName.text = $"[{characterName}]";
            LayoutRebuilder.ForceRebuildLayoutImmediate(rtLayout);

            uiTextSlider_1.SetText(title);
            uiTextSlider_2.SetText(title);

            //var character = Data.GetDataFromTable(Data._characterTable, characterEnumId);
            //if(character != null)
            //{
            //    var profile = character._Profile_Data;
            //    if(profile != null)
            //    {
            //        if(profile._Profile_Resource_Data != null
            //            && profile._Profile_Resource_Data._On_Air_Illust_Reference_Data != null)
            //        {
            //            imgBg.sprite = profile._Profile_Resource_Data._On_Air_Illust_Reference_Data.GetSpriteFromSMTable();
            //        }
            //    }

            //    var characterResourceData = character._Resource_List_Data;
            //    if(characterResourceData != null)
            //    {
            //        try
            //        {
            //            string skin = "day";
            //            SkeletonDataAsset skeletonDataAsset = ResourceManager.LoadAsset<SkeletonDataAsset>(
            //                string.Format(DefineName.SkeletonData.SD_FORMAT, characterResourceData._Spine, "day"));
            //            if (skeletonDataAsset != null)
            //            {
            //                sgCharacter.skeletonDataAsset = skeletonDataAsset;
            //                sgCharacter.Initialize(true);
            //                if (sgCharacter.SkeletonData.FindSkin(skin) == null)
            //                    skin = "default";
            //                sgCharacter.Skeleton.SetSkin(skin);
            //            }
            //            sgCharacter.AnimationState.SetAnimation(0, Ani.simple_idle.ToString(), true);
            //        }
            //        catch
            //        {
            //            Debug.LogWarning("애니메이션이 존재하지 않음.");
            //        }
            //    }
            //}
        }
        else
        {
            goLive.SetActive(false);
            goReady.SetActive(true);

            var character = Data.GetDataFromTable(Data._characterTable, characterEnumId);
            if (character != null)
            {
                var profile = character._Profile_Data;
                if (profile != null)
                {
                    if (profile._Profile_Resource_Data != null
                        && profile._Profile_Resource_Data._On_Air_Illust_Reference_Data != null)
                    {
                        imgReadyBg.sprite = profile._Profile_Resource_Data._Offline_Illust_Reference_Data.GetSpriteFromSMTable(ownerUIBase);
                    }
                }
            }
        }

        listData.Clear();
        foreach(var data in completeList)
        {
            listData.Add(data._RootTimeline._Enum_Id);
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
        return 100f;
    }

    public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
    {
        UI_ReplayBroadCastItem cellView = scroller.GetCellView(cellViewPrefab) as UI_ReplayBroadCastItem;
        if (cellView != null)
        {
            cellView.name = "Cell " + dataIndex.ToString();
            cellView.SetData(listData[dataIndex]);
        }
        return cellView;
    }

    private void OnClickLink()
    {
        if(oracleInfo != null)
        {
            UIManager._instance.ShowUIBase<UIBase>(DefineName.UIVisualStory.ORACLE, new UIParam.VisualStory.Main(oracleInfo));
        }
    }
}