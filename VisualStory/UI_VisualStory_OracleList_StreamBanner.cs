using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Debug = COA_DEBUG.Debug;

public class UI_VisualStory_OracleList_StreamBanner : UI_VisualStory_OracleList_StreamingThumbnail
{
    [Serializable]
    private class BannerView
    {
        [SerializeField] private GameObject go_Root;
        [SerializeField] private GameObject go_NoStreaming;
        [SerializeField] private GameObject go_Streaming;
        [SerializeField] private Image img_SDCharacter;
        [SerializeField] private TextMeshProUGUI tmpu_ViewerCount;

        public RectTransform rtf_LayoutRebuildTarget;
        public UI_TextSlider titleSlider;

        public void SetActive(bool active)
        {
            go_Root?.SetActive(active);
        }

        public void Set(IAssetOwner assetOwner, VisualStoryInfo info)
        {
            var staticData = info?._RootTimeline;

            var isValueable = staticData != null;
            go_NoStreaming?.SetActive(!isValueable);
            go_Streaming?.SetActive(isValueable);
            if (!isValueable) return;

            var characterSD = info._Character?._Resource_List_Data?._Night_SD_Reference_Data?.GetSpriteFromSMTable(assetOwner);
            if (img_SDCharacter != null)
            {
                img_SDCharacter.gameObject.SetActive(characterSD != null);
                img_SDCharacter.sprite = characterSD;
            }
            int viewCount = staticData.RandomizeViewerCount();
            if (tmpu_ViewerCount != null) tmpu_ViewerCount.text = viewCount.ToString("#,0");
        }
    }

    [SerializeField] private Image img_Bg;
    [SerializeField] private BannerView view_Event;
    [SerializeField] private BannerView view_NotEvent;

    protected override void OnChangeTakeRewardsState(bool value)
    {

    }

    protected override void OnSet(VisualStoryHelper.OracleCategory category, VisualStoryInfo info)
    {
        gameObject.SetActive(true);
        if (img_Bg != null) img_Bg.sprite = info._RootTimeline?._Banner_Background_Data?.GetSpriteFromSMTable(ownerUIBase);

        view_Event.SetActive(category == VisualStoryHelper.OracleCategory.EVENT);
        view_NotEvent.SetActive(category != VisualStoryHelper.OracleCategory.EVENT);

        var view = category == VisualStoryHelper.OracleCategory.EVENT ? view_Event : view_NotEvent;
        view.Set(ownerUIBase, info);

        rtf_LayoutRebuildTarget = view.rtf_LayoutRebuildTarget;
        titleSlider = view.titleSlider;
    }

    protected override void OnSetToNullInfo(VisualStoryHelper.OracleCategory category)
    {
        if (category != VisualStoryHelper.OracleCategory.EVENT) gameObject.SetActive(false);
        else
        {
            gameObject.SetActive(true);
            view_Event.SetActive(true);
            view_Event.Set(ownerUIBase, null);
            view_NotEvent.SetActive(false);
        }
    }
}