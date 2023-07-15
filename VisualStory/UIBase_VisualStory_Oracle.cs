using AudioSystem;
using DG.Tweening;
using EnhancedUI.EnhancedScroller;
using LeTai.Asset.TranslucentImage;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Timeline;
using UnityEngine.UI;
using static CharacterInfoSlide;
using static TimelineParam;
using Debug = COA_DEBUG.Debug;

public class UIBase_VisualStory_Oracle : UIBase_VisualStory, IEnhancedScrollerDelegate
{
    [Serializable]
    private class InteractionButton
    {
        [SerializeField] private Button btn_Interaction;
        [SerializeField] private GameObject go_Blocker;

        public void SetListener(UnityAction action)
        {
            UIUtil.ResetAndAddListener(btn_Interaction, action);
        }
        public void SetBlock(bool block)
        {
            go_Blocker?.SetActive(block);
        }
    }

    #region Inspector
    [Header("Streaming Title")]
    [SerializeField] private TextMeshProUGUI tmpu_Title;
    [SerializeField] private Image img_StreamerThumbnail;
    [SerializeField] private TextMeshProUGUI tmpu_StreamerName;

    [Header("Production")]
    [SerializeField] private UI_VisualStory_PlaceAlarm placeAlarm;
    [SerializeField] private EnhancedScroller scroller_Chat;
    [SerializeField] private UI_VisualStory_Oracle_Chat chatPrefab;
    [SerializeField] private UI_VisualStory_Oracle_Chat forceRebuildTarget;
    [SerializeField] private UI_VisualStory_Oracle_Donation donation;
    [SerializeField] private TextMeshProUGUI tmpu_ViewCount;
    [SerializeField] private AnimationCurve curve_ViewCount = AnimationCurve.Linear(0f, 0f, 1f, 1f);
    [SerializeField] private GameObject go_ChatDonationBtnEff;

    [Header("Story Complete")]
    [SerializeField] private GameObject storyCompleteTweener;

    [Header("Interaction")]
    [SerializeField] private InteractionButton btn_Sponsor;
    [SerializeField] private InteractionButton btn_Branch;
    [SerializeField] private UI_VisualStory_Branch_Sponsor sponsorUI;

    [Header("Translucent")]
    [SerializeField] private TranslucentImage blurry;
    #endregion

    #region Variables
    private List<VisualStoryHelper.OracleChatCellData> list_ChatData = new List<VisualStoryHelper.OracleChatCellData>();
    private string streamerEnumId;

    private IEnumerator addCommonChatEnumerator;
    private IEnumerator addSpecificChatEnumerator;

    private OracleSponsorBranch[] arr_SponsorBranchData;

    private float _ChatWidth => scroller_Chat.GetComponent<RectTransform>().rect.width;

    private int prevViewCount;
    private int nextViewCount;
    private int evaluatedViewCount;
    private int viewerCountOffset = 0;
    private int _FinalViewerCount => evaluatedViewCount + viewerCountOffset;
    private IEnumerator viewCountProductionCorout;
    private IEnumerator viewCountEvaluateCorout;
    private IEnumerator viewCountOffsetResetCorout;

    private VisualStoryHelper.OnCloseData onCloseData = new VisualStoryHelper.OnCloseData();
    private IEnumerator closeEnumerator;
    #endregion

    private void OnDisable()
    {
        if (addCommonChatEnumerator != null) { StopCoroutine(addCommonChatEnumerator); addCommonChatEnumerator = null; }
        if (addSpecificChatEnumerator != null) { StopCoroutine(addSpecificChatEnumerator); addSpecificChatEnumerator = null; }
        if (viewCountProductionCorout != null) { StopCoroutine(viewCountProductionCorout); viewCountProductionCorout = null; }
        if (viewCountEvaluateCorout != null) { StopCoroutine(viewCountEvaluateCorout); viewCountEvaluateCorout = null; }
        if (closeEnumerator != null) { StopCoroutine(closeEnumerator); closeEnumerator = null; }
        if (viewCountOffsetResetCorout != null) { StopCoroutine(viewCountOffsetResetCorout); viewCountOffsetResetCorout = null; }
    }

    #region Base Methods
    protected override void Init()
    {
        base.Init();
        if (scroller_Chat != null) scroller_Chat.Delegate = this;
        btn_Sponsor.SetListener(OnClickShowSponsor);
        btn_Branch.SetListener(OnClickShowBranch);
    }
    public override void Show(object param)
    {
        onCloseData = new VisualStoryHelper.OnCloseData();

        if (blurry != null)
        {
            blurry.gameObject.SetActive(false);
            blurry.source = UIManager._instance._TranslucentImageSource;
        }
        sponsorUI?.Hide();
        btn_Sponsor?.SetBlock(true);
        btn_Branch?.SetBlock(true);
        donation?.SetActiveMsg(false);
        viewerCountOffset = 0;

        base.Show(param);

        var cast = param as UIParam.VisualStory.Main;
        if (cast == null)
        {
            Debug.LogError($"Show Parameter Type is not {nameof(UIParam.VisualStory.Main)}");
            Hide();
            return;
        }

        streamerEnumId = cast.storyInfo._Character._Enum_Id;

        list_ChatData.Clear();
        list_ChatData.Add(new VisualStoryHelper.OracleChatCellData());
        if (scroller_Chat != null)
        {
            scroller_Chat.ReloadData();
        }

        storyCompleteTweener?.gameObject.SetActive(false);

        if (tmpu_Title != null) tmpu_Title.text = Localizer.GetLocalizedStringName(Localizer.SheetType.STRING, rootStoryInfo._RootTimeline._Enum_Id);
        if (img_StreamerThumbnail != null)
            img_StreamerThumbnail.sprite = Data._characterTable.GetDataFromTable(streamerEnumId)?._Resource_List_Data?._Thumbnail_Reference_Data?.GetSpriteFromSMTable(this);
        if (tmpu_StreamerName != null) tmpu_StreamerName.text = Localizer.GetLocalizedStringName(Localizer.SheetType.NONE, streamerEnumId);

        StartViewCountProductionCorout(rootStoryInfo._RootTimeline, true);
    }

    protected override void AfterCallTimelineBranch(VisualStoryHelper.MarkerData currentMarker, VisualStoryHelper.MarkerData nextMarker)
    {

    }

    protected override void AfterCallTimelineCheckpoint(VisualStoryHelper.MarkerData currentMarker, VisualStoryHelper.MarkerData nextMarker)
    {

    }

    protected override void AfterCallTimelinePause()
    {

    }

    protected override void AfterCallTimelinePlay()
    {

    }

    protected override void AfterCallTimelineResume()
    {

    }

    protected override void AfterCallTimelineSkip()
    {

    }

    protected override void AfterCallTimelineRequestReward()
    {
        RequestRewardSequence();
    }

    protected override void AfterCallTimelineEndpoint()
    {

    }

    protected override void ExecuteProduction(TimelineParam.VStoryProduction param)
    {
        var productionData = Data._vstory_oracle_productionTable.GetDataFromTable(param.productionEnumId);
        if (productionData == null) return;

        if (!string.IsNullOrEmpty(productionData._Place))
        {
            var placeName = Localizer.GetLocalizedStringName(Localizer.SheetType.STRING, productionData._Place);
            placeAlarm?.SetAndPlay(placeName);
        }

        // Create Common Chat
        StartAddCommonChatCoroutine(productionData);

        // Create Specific Chat
        StartAddSpecificChatCoroutine(productionData);

        if (productionData._Donation_Data != null)
        {
            donation?.ShowDonation(productionData._Donation_Data, streamerEnumId);
            AddDonationChat(productionData._Donation_Data);
        }

        btn_Sponsor?.SetBlock(!productionData._Sponsorable);
        btn_Branch?.SetBlock(!productionData._Sponsorable);
        go_ChatDonationBtnEff?.SetActive(productionData._Sponsorable);

        var bgSprite = productionData._Place_Sprite_Data?.GetSpriteFromSMTable(this);
        if (bgSprite != null) screenUI?.SetBackgroundImage(bgSprite);
    }

    protected override void OnChangeSpeedOfStory(int speed)
    {
        base.OnChangeSpeedOfStory(speed);
        donation?.SetSpeed(speed);
    }

    protected override void OnSelectBranch(TimelineBranch branchData)
    {
        base.OnSelectBranch(branchData);
        if (timelineData != null) StartViewCountProductionCorout(timelineData, false);
        StartViewCountOffsetResetCorout();
        try
        {
            var dialogue = timelineData._Dialogue_Data[branchData.showingIndex];
            var chatParam = new VisualStoryHelper.OracleChatCellData(
                    true,
                    User.PlayerNickName,
                    Localizer.GetLocalizedStringTalk(Localizer.SheetType.STRING, dialogue._Dialogue),
                    Data.Enum.Oracle_Chat_Format.USER);
            var size = forceRebuildTarget.ForceRebuildAndGetHeight(chatParam, _ChatWidth);
            chatParam.size = size;

            AddNewChat(chatParam);
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
        }
    }

    protected override void OnStorySkipAction()
    {
        base.OnStorySkipAction();
        RequestRewardSequence();
    }

    public override void Close()
    {
        if (closeEnumerator != null) StopCoroutine(closeEnumerator);
        closeEnumerator = CloseEnumerator();
        StartCoroutine(closeEnumerator);
    }
    private IEnumerator CloseEnumerator()
    {
        while (onCloseData.state == VisualStoryHelper.OnCloseData.State.WAITING)
        {
            yield return null;
        }
        switch (onCloseData.state)
        {
            case VisualStoryHelper.OnCloseData.State.REWARD:
                // TODO: 보상 ui 띄우는 코드 추가 필요
                break;
            case VisualStoryHelper.OnCloseData.State.ERROR:
                onCloseData.onErrorUI = UIMessageBox.Confirm_Old("스토리 시청 완료 에러 발생",
                    $"ERROR CODE: [{onCloseData.errorMsg}]",
                    () => { if (onCloseData.onErrorUI != null) onCloseData.onErrorUI.Hide(); });
                break;
        }

        closeEnumerator = null;
        base.Close();
    }
    #endregion

    #region Add Chat
    private void StartAddCommonChatCoroutine(vstory_oracle_productionTable productionData)
    {
        if (addCommonChatEnumerator != null) StopCoroutine(addCommonChatEnumerator);

        var ccpIdList = new List<string>();
        var ccpList = productionData._Oracle_Common_Chat_Pool_Data.ToList();
        ccpList.RemoveAll(r => r == null);
        foreach (var v in ccpList)
            ccpIdList.AddRange(v._Chat);
        ccpIdList.RemoveAll(id => string.IsNullOrEmpty(id));

        var npIdList = new List<string>();
        var npList = productionData._Oracle_Nickname_Pool_Data.ToList();
        npList.RemoveAll(r => r == null);
        foreach (var v in npList)
            npIdList.AddRange(v._Nickname);
        npIdList.RemoveAll(id => string.IsNullOrEmpty(id));

        if (ccpIdList.Count == 0 || npIdList.Count == 0) { addCommonChatEnumerator = null; return; }

        addCommonChatEnumerator = AddCommonChatCoroutine(
            productionData._Chat_Cycle_Min,
            productionData._Chat_Cycle_Max,
            ccpIdList,
            npIdList);
        StartCoroutine(addCommonChatEnumerator);
    }
    private IEnumerator AddCommonChatCoroutine(float cycle_Min, float cycle_Max, List<string> chatIdList, List<string> nickIdList)
    {
        var unscaledChatTerm = UnityEngine.Random.Range(cycle_Min, cycle_Max);
        var cycle = 0f;
        while (true)
        {
            if (cycle >= unscaledChatTerm / _CurrentSpeedOfStory)
            {
                var nickId = nickIdList[UnityEngine.Random.Range(0, nickIdList.Count)];
                var contentId = chatIdList[UnityEngine.Random.Range(0, chatIdList.Count)];
                var newChatData = new VisualStoryHelper.OracleChatCellData(
                    false,
                    Localizer.GetLocalizedStringName(Localizer.SheetType.STRING, nickId),
                    Localizer.GetLocalizedStringTalk(Localizer.SheetType.STRING, contentId),
                    Data.Enum.Oracle_Chat_Format.COMMON);
                var size = forceRebuildTarget.ForceRebuildAndGetHeight(newChatData, _ChatWidth);
                newChatData.size = size;

                AddNewChat(newChatData);

                unscaledChatTerm = UnityEngine.Random.Range(cycle_Min, cycle_Max);
                cycle = 0f;
            }
            yield return null;
            cycle += Time.deltaTime;
        }
    }

    private void StartAddSpecificChatCoroutine(vstory_oracle_productionTable productionData)
    {
        if (addSpecificChatEnumerator != null) StopCoroutine(addSpecificChatEnumerator);
        if (productionData._Special_Chat_Data == null) { addSpecificChatEnumerator = null; return; }

        addSpecificChatEnumerator = AddSpecificChatCorouine(
            productionData._Chat_Cycle_Min,
            productionData._Chat_Cycle_Max,
            productionData._Special_Chat_Data);
        StartCoroutine(addSpecificChatEnumerator);
    }
    private IEnumerator AddSpecificChatCorouine(float cycle_Min, float cycle_Max, vstory_oracle_special_chatTable spChatPool)
    {
        var tmpList = new List<VisualStoryHelper.OracleChatCellData>();
        for (int i = 0; i < spChatPool._Special_Chat.Length; i++)
        {
            var nick = spChatPool._Nickname[i];
            var content = spChatPool._Special_Chat[i];
            var format = spChatPool._Special_Chat_Format[i];

            if (string.IsNullOrEmpty(nick) || string.IsNullOrEmpty(content) || format == Data.Enum.Oracle_Chat_Format.CNT) continue;

            var data = new VisualStoryHelper.OracleChatCellData(
                false,
                Localizer.GetLocalizedStringName(Localizer.SheetType.STRING, nick),
                Localizer.GetLocalizedStringTalk(Localizer.SheetType.STRING, content),
                format,
                spChatPool._Optional_String[i]);
            var size = forceRebuildTarget.ForceRebuildAndGetHeight(data, _ChatWidth);
            data.size = size;

            tmpList.Add(data);
        }

        if (spChatPool != null)
        {
            var unscaledChatTerm = UnityEngine.Random.Range(cycle_Min, cycle_Max);
            var cycle = 0f;
            int currentIndex = 0;

            while (true)
            {
                if (cycle >= unscaledChatTerm / _CurrentSpeedOfStory)
                {
                    AddNewChat(tmpList[currentIndex]);

                    currentIndex++;
                    if (currentIndex >= tmpList.Count) break;

                    unscaledChatTerm = UnityEngine.Random.Range(cycle_Min, cycle_Max);
                    cycle = 0f;
                }

                yield return null;
                cycle += Time.deltaTime;
            }
        }

        addSpecificChatEnumerator = null;
    }

    private void AddDonationChat(donation_contentsTable data)
    {
        var cellData = new VisualStoryHelper.OracleChatCellData(data, streamerEnumId);
        var size = forceRebuildTarget.ForceRebuildAndGetHeight(cellData, _ChatWidth);
        cellData.size = size;

        AddNewChat(cellData);
    }
    private void AddDonationChat(OracleSponsorBranch branchData) => AddDonationChat(branchData._ContentData);

    private void AddNewChat(VisualStoryHelper.OracleChatCellData newChatData)
    {
        list_ChatData.Add(newChatData);

        if (scroller_Chat == null) return;

        var prevScrollPos = scroller_Chat.ScrollPosition;
        var isBottom = Mathf.Max(scroller_Chat.ScrollSize - prevScrollPos, 0f) <= 0.015f;
        scroller_Chat.ReloadData();
        scroller_Chat.ScrollPosition = isBottom ? scroller_Chat.ScrollSize : prevScrollPos;
    }
    #endregion

    #region Scroller Delegate
    public int GetNumberOfCells(EnhancedScroller scroller)
    {
        return list_ChatData.Count;
    }

    public float GetCellViewSize(EnhancedScroller scroller, int dataIndex)
    {
        return dataIndex == 0 ? GetSizeFirstSpacerOfChat() : list_ChatData[dataIndex].size;
    }

    public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
    {
        var cell = scroller.GetCellView(chatPrefab) as UI_VisualStory_Oracle_Chat;
        cell.Set(list_ChatData[dataIndex]);
        return cell;
    }
    #endregion

    #region Listener
    #region Interaction Button Listener
    private void OnClickShowSponsor()
    {
        AudioManager._instance.PlaySound(DefineName.Audio_OutGame.Sound.BUTTON);
        ShowSponsorBranch();
    }
    private void OnClickShowBranch()
    {
        AudioManager._instance.PlaySound(DefineName.Audio_OutGame.Sound.BUTTON);
        base.ShowTimelineBranch();
    }
    #endregion

    private void OnClickSponsorBranch(TimelineBranch branchData)
    {
        var cast = branchData as OracleSponsorBranch;
        base.OnSelectBranch(branchData);
        if (timelineData != null) StartViewCountProductionCorout(timelineData, false);
        StartViewCountOffsetResetCorout();
        AddDonationChat(cast);
        donation?.ShowDonation(cast._ContentData, streamerEnumId);
    }
    #endregion

    #region Sponsor Branch
    public void SetSponsorBranchData(params OracleSponsorBranch[] timelineBranchParams)
    {
        arr_SponsorBranchData = timelineBranchParams;
    }
    private void ShowSponsorBranch()
    {
        if (sponsorUI == null) return;
        sponsorUI.SetupAndShow(OnClickSponsorBranch, arr_SponsorBranchData);
        base.ResetStoryPlayMode();
    }
    #endregion

    #region Viewer Count
    private void StartViewCountProductionCorout(vstory_timelineTable staticData, bool start)
    {
        if (viewCountProductionCorout != null) StopCoroutine(viewCountProductionCorout);
        viewCountProductionCorout = ViewCountProdictionCycleCorout(staticData, start);
        StartCoroutine(viewCountProductionCorout);
    }
    private IEnumerator ViewCountProdictionCycleCorout(vstory_timelineTable staticData, bool start)
    {
        if (start)
        {
            evaluatedViewCount = prevViewCount = nextViewCount = staticData.RandomizeViewerCount();
            SetViewCountText();
        }
        else
        {
            prevViewCount = evaluatedViewCount;
        }

        nextViewCount = staticData.GetRandomizeNextViewerCount(prevViewCount);

        var cycle = VisualStoryHelper.VIEWER_COUNT_CHANGE_FIRST_CYCLE;
        var deltaTime = 0f;
        while (deltaTime < cycle / _CurrentSpeedOfStory)
        {
            yield return null;
            deltaTime += Time.deltaTime;
        }
        StartViewCountEvaluateCorout();

        deltaTime = cycle = VisualStoryHelper.VIEWER_COUNT_CHANGE_CYCLE;
        while (true)
        {
            if (deltaTime >= cycle / _CurrentSpeedOfStory)
            {
                deltaTime = 0f;
                nextViewCount = staticData.GetRandomizeNextViewerCount(prevViewCount);
                StartViewCountEvaluateCorout();
            }

            yield return null;
            deltaTime += Time.deltaTime;
        }
    }
    private void StartViewCountEvaluateCorout()
    {
        prevViewCount = evaluatedViewCount;
        if (viewCountEvaluateCorout != null) StopCoroutine(viewCountEvaluateCorout);
        viewCountEvaluateCorout = ViewCountEvaluateCorout();
        StartCoroutine(viewCountEvaluateCorout);
    }
    private IEnumerator ViewCountEvaluateCorout()
    {
        float deltaTime = 0f;
        while (deltaTime < VisualStoryHelper.VIEWER_COUNT_CHANGE_TERM / _CurrentSpeedOfStory)
        {
            yield return null;
            deltaTime += Time.deltaTime;

            var interpolation = curve_ViewCount.Evaluate(deltaTime / (VisualStoryHelper.VIEWER_COUNT_CHANGE_TERM / _CurrentSpeedOfStory));
            evaluatedViewCount = (int)Mathf.Lerp(prevViewCount, nextViewCount, interpolation);
            SetViewCountText();
        }

        prevViewCount = evaluatedViewCount;
        viewCountEvaluateCorout = null;
    }
    private void SetViewCountText()
    {
        if (tmpu_ViewCount == null) return;
        tmpu_ViewCount.text = Mathf.Max(_FinalViewerCount, 0).ToString("#,0");
    }

    public void ChangeViewerCountOffset(float offsetMult)
    {
        if (timelineData == null) return;

        viewerCountOffset = (int)(timelineData._Optional_Int * offsetMult);
        SetViewCountText();
    }
    private void StartViewCountOffsetResetCorout()
    {
        if (viewCountOffsetResetCorout != null) StopCoroutine(viewCountOffsetResetCorout);
        viewCountOffsetResetCorout = ViewCountOffsetResetCorout();
        StartCoroutine(viewCountOffsetResetCorout);
    }
    private IEnumerator ViewCountOffsetResetCorout()
    {
        int offsetOfPrevTimeline = viewerCountOffset;
        float deltaTime = 0f;
        while (deltaTime < VisualStoryHelper.VIEWER_COUNT_CHANGE_TERM / _CurrentSpeedOfStory)
        {
            yield return null;
            deltaTime += Time.deltaTime;

            viewerCountOffset = (int)Mathf.Lerp(offsetOfPrevTimeline, 0f, deltaTime / (VisualStoryHelper.VIEWER_COUNT_CHANGE_TERM / _CurrentSpeedOfStory));
            if (viewerCountOffset <= 0) break;

            SetViewCountText();
        }
        viewerCountOffset = 0;
        SetViewCountText();
        viewCountOffsetResetCorout = null;
    }
    #endregion

    #region Other
    private void RequestRewardSequence()
    {
        // 이번 빌드 임시처리
        onCloseData.state = VisualStoryHelper.OnCloseData.State.NONE;
        return;

        //var cast = rootStoryInfo as OracleInfo;
        //if (cast != null && !cast._TookRewards)
        //{
        //    if (onCloseData.state == VisualStoryHelper.OnCloseData.State.NONE)
        //    {
        //        onCloseData.state = VisualStoryHelper.OnCloseData.State.WAITING;
        //        User.RequestCompleteWatchOracle(cast,
        //            reward =>
        //            {
        //                onCloseData.state = VisualStoryHelper.OnCloseData.State.REWARD;
        //                onCloseData.list_Reward = reward;
        //            },
        //            error =>
        //            {
        //                onCloseData.state = VisualStoryHelper.OnCloseData.State.ERROR;
        //                onCloseData.errorMsg = error;
        //            });
        //    }
        //}
        //else onCloseData.state = VisualStoryHelper.OnCloseData.State.NONE;
    }

    private float GetSizeFirstSpacerOfChat()
    {
        var scrollerHeight = scroller_Chat.GetComponent<RectTransform>().rect.height;
        var topPadding = scroller_Chat.padding.top;
        var bottomPadding = scroller_Chat.padding.bottom;
        var defaultSize = scrollerHeight - topPadding - bottomPadding;

        var totalSizeExceptSpacer = 0f;
        for (int i = 1; i < list_ChatData.Count; i++)
        {
            totalSizeExceptSpacer += list_ChatData[i].size;
        }
        return Mathf.Max(defaultSize - totalSizeExceptSpacer, 0f);
    }
    #endregion
}