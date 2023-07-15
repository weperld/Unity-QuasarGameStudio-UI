using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Debug = COA_DEBUG.Debug;
using EnhancedUI.EnhancedScroller;
using UnityEngine.EventSystems;

public class OracleEventBannerPageCellData
{
    public VisualStoryInfo firstInfo;
    public VisualStoryInfo secondInfo;

    public Action<VisualStoryInfo> onClickBannerAction;

    public OracleEventBannerPageCellData(VisualStoryInfo firstInfo, VisualStoryInfo secondInfo, Action<VisualStoryInfo> action)
    {
        this.firstInfo = firstInfo;
        this.secondInfo = secondInfo;
        onClickBannerAction = action;
    }
}

public class UIBase_VisualStory_OracleHome : UIBase
{
    [Serializable]
    private struct EventBannerPageScrollEaseData
    {
        [Header("Auto Slide Setting")]
        public bool useAutoSlide;
        public EnhancedScroller.TweenType autoSlideEaseType;
        [Range(0f, 1f)] public float autoSlideTweenTime;
        [Range(0f, 60f)] public float autoSlideTerm;

        [Header("Manual Slide Setting")]
        public EnhancedScroller.TweenType manualSlideEaseType;
        [Range(0f, 1f)] public float manualSlideTweenTime;
    }

    #region Inspector
    [Header("Common")]
    [SerializeField] private Button btn_Back;

    [Header("User Profile")]
    [SerializeField] private Image img_UserProfile;
    [SerializeField] private TextMeshProUGUI tmpu_UserName;

    [Header("Streamer List")]
    [SerializeField] private UI_VisualStory_OracleList_StreamerList followList;
    [SerializeField] private UI_VisualStory_OracleList_StreamerList suggestionList;
    [SerializeField] private Button btn_SearchAllStreaming;

    [Header("Streaming Banner")]
    [SerializeField] private CanvasGroup eventBannerCanvasGroup;
    [SerializeField] private EnhancedScroller eventBannerScroller;
    [SerializeField] private EventBannerPageScrollEaseData eventBannerPageSlideSetting;
    [SerializeField] private UI_VisualStory_OracleList_EventBannerPage eventBannerPagePrefab;
    [SerializeField] private RectTransform rtf_EventBannerPageView;
    [SerializeField] private Button btn_LeftEventBannerPage;
    [SerializeField] private Button btn_RightEventBannerPage;
    [SerializeField] private Transform tf_EventBannerPageMarkerRoot;
    [SerializeField] private UI_Common_Page_Marker eventBannerPageMarkerPrefab;
    [SerializeField] private UI_VisualStory_OracleList_StreamBanner[] notEventBanners;
    #endregion

    #region Variables
    private bool init = false;

    private List<OracleEventBannerPageCellData> list_EventStreamPage = new List<OracleEventBannerPageCellData>();

    private int _MaxEventBannerPage => list_EventStreamPage.Count - 1;
    private int currentEventBannerPage = 0;
    private List<UI_Common_Page_Marker> list_EventBannerPageMarker = new List<UI_Common_Page_Marker>();
    public int _CurrentEventBannerPage
    {
        get => currentEventBannerPage;
        set
        {
            if (value < 0) value = _MaxEventBannerPage;
            else if (value > _MaxEventBannerPage) value = 0;

            currentEventBannerPage = value;
        }
    }

    private EnhancedScrollerDelegate eventScrollerDelegate;
    private bool eventBannerPageIsTweening = false;
    private bool _EventBannerPageIsTweening
    {
        get => eventBannerPageIsTweening;
        set
        {
            eventBannerPageIsTweening = value;
            if (eventBannerCanvasGroup != null) eventBannerCanvasGroup.interactable = !value;
        }
    }

    private IEnumerator eventBannerAutoSlideCorout;

    private UIBase confirmUIOnNoAvailiableOracle;
    private UIBase errorUIWhenGetFromServer;

    private VisualStoryHelper.OracleCategory currentNotEventCategory;

    private bool inRequestingOracle = false;
    #endregion

    #region Base Methods
    private void OnDisable()
    {
        StopEventBannerAutoSlideCorout();
        if (!GameManager.IsDetroying)
        {
            GameManager._instance.OnTimerEvent -= OnTimer;
        }

        foreach (var list in User._OracleListCategoryInfo)
        {
            list.Value.onSort -= OnSortEventOracleList;
            list.Value.onSort -= OnSortFollowOracleList;
            list.Value.onSort -= OnSortSuggestionOracleList;
        }
    }

    public override void Show(object param = null)
    {
        if (!init)
        {
            eventScrollerDelegate = new EnhancedScrollerDelegate(GetEventBannerCellView, GetEventBannerSize, GetEventBannerCount);
            if (eventBannerScroller != null)
            {
                eventBannerScroller.Delegate = eventScrollerDelegate;
                eventBannerScroller.scrollerTweeningChanged = EventBannerScrollerOnTweening;
                eventBannerScroller.scrollerScrolled = EventBannerScrollerOnScrolling;

                if (!eventBannerScroller.TryGetComponent<EventTrigger>(out var et))
                    et = eventBannerScroller.gameObject.AddComponent<EventTrigger>();

                var beginDragEntry = new EventTrigger.Entry();
                beginDragEntry.eventID = EventTriggerType.BeginDrag;
                beginDragEntry.callback.AddListener(EventpageOnBeginDrag);
                et.triggers.Add(beginDragEntry);

                var draggingEntry = new EventTrigger.Entry();
                draggingEntry.eventID = EventTriggerType.Drag;
                draggingEntry.callback.AddListener(EventPageOnDragging);
                et.triggers.Add(draggingEntry);

                var endDragEntry = new EventTrigger.Entry();
                endDragEntry.eventID = EventTriggerType.EndDrag;
                endDragEntry.callback.AddListener(EventPageOnEndDrag);
                et.triggers.Add(endDragEntry);
            }

            followList?.SetListButtonListener(OnClickCategory);
            suggestionList?.SetListButtonListener(OnClickCategory);

            followList?.SetOnClickStreamerListener(OnClickBanner);
            suggestionList?.SetOnClickStreamerListener(OnClickBanner);

            if (notEventBanners != null)
                foreach (var banner in notEventBanners)
                {
                    if (banner == null) continue;
                    banner.SetOnClickAction(OnClickBanner);
                }

            UIUtil.ResetAndAddListener(btn_SearchAllStreaming, OnClickSearchAllStreaming);
            UIUtil.ResetAndAddListener(btn_Back, Hide);
            UIUtil.ResetAndAddListener(btn_LeftEventBannerPage, OnClickLeftEventPage);
            UIUtil.ResetAndAddListener(btn_RightEventBannerPage, OnClickRightEventPage);

            init = true;
        }

        base.Show(param);
        if (tmpu_UserName != null) tmpu_UserName.text = User.PlayerNickName;
        Setup();
    }
    #endregion

    #region Enhanced Scroller Delegate
    #region Event Banner Delegate
    private EnhancedScrollerCellView GetEventBannerCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
    {
        var cellView = scroller.GetCellView(eventBannerPagePrefab) as UI_VisualStory_OracleList_EventBannerPage;
        cellView.Set(list_EventStreamPage[dataIndex]);
        return cellView;
    }
    private float GetEventBannerSize(EnhancedScroller scroller, int dataIndex)
    {
        return rtf_EventBannerPageView != null ? rtf_EventBannerPageView.rect.width : 0f;
    }
    private int GetEventBannerCount(EnhancedScroller scroller)
    {
        return list_EventStreamPage.Count;
    }
    private void EventBannerScrollerOnTweening(EnhancedScroller scroller, bool tweening)
    {
        _EventBannerPageIsTweening = tweening;
    }
    private void EventBannerScrollerOnScrolling(EnhancedScroller scroller, Vector2 val, float scrollPosition)
    {
        Debug.Log(scrollPosition);
        var pageAfterSlide = GetCurrentEventBannerPageSection(scroller.ScrollRectSize, scrollPosition);
        OnChangedEventBannerPage(pageAfterSlide);
    }

    private void EventpageOnBeginDrag(BaseEventData eventData)
    {
        if (_MaxEventBannerPage <= 0 || _EventBannerPageIsTweening) return;
        StopEventBannerAutoSlideCorout();
    }
    private void EventPageOnDragging(BaseEventData eventData)
    {
        if (_MaxEventBannerPage <= 0 || _EventBannerPageIsTweening) return;

        var pageAfterSlide = GetCurrentEventBannerPageSection(eventBannerScroller.ScrollRectSize, eventBannerScroller.ScrollPosition);
        if (_CurrentEventBannerPage != pageAfterSlide) _CurrentEventBannerPage = pageAfterSlide;
    }
    private void EventPageOnEndDrag(BaseEventData eventData)
    {
        if (_MaxEventBannerPage <= 0 || _EventBannerPageIsTweening) return;
        MoveTo(_CurrentEventBannerPage, false);
    }
    #endregion
    #endregion

    #region Listener
    private void OnClickCategory(VisualStoryHelper.OracleCategory category)
    {
        SelectNotEventOracleCategory(category, false);
    }

    private void OnClickSearchAllStreaming()
    {

    }

    private void OnClickBanner(VisualStoryInfo selected)
    {
        var cast = selected as OracleInfo;
        if (cast == null || cast._RootTimeline == null || cast._TookRewards) return;

        UIManager._instance.ShowUIBase<UIBase>(DefineName.UIVisualStory.ORACLE, new UIParam.VisualStory.Main(cast));
    }

    private void OnChangedEventBannerPage(int changedPage)
    {
        for (int i = 0; i < list_EventBannerPageMarker.Count; i++)
            list_EventBannerPageMarker[i]?.SetOnOff(i == changedPage);
    }

    private void OnClickLeftEventPage()
    {
        if (_MaxEventBannerPage <= 0 || _EventBannerPageIsTweening) return;

        StopEventBannerAutoSlideCorout();
        _CurrentEventBannerPage -= 1;
        MoveTo(_CurrentEventBannerPage, false);
    }
    private void OnClickRightEventPage()
    {
        if (_MaxEventBannerPage <= 0 || _EventBannerPageIsTweening) return;

        StopEventBannerAutoSlideCorout();
        _CurrentEventBannerPage += 1;
        MoveTo(_CurrentEventBannerPage, false);
    }

    private void OnSortEventOracleList()
    {
        SetupEventOraclePage();
    }
    private void OnSortFollowOracleList()
    {
        var category = VisualStoryHelper.OracleCategory.FOLLOW;
        var oracleList = User._OracleListCategoryInfo[category];
        if (followList != null) followList.Set(category, oracleList);

        if (currentNotEventCategory == category) ChangeShowingStreamingListTo(category);
    }
    private void OnSortSuggestionOracleList()
    {
        var category = VisualStoryHelper.OracleCategory.SUGGESTION;
        var oracleList = User._OracleListCategoryInfo[category];
        if (suggestionList != null) suggestionList.Set(category, oracleList);

        if (currentNotEventCategory == category) ChangeShowingStreamingListTo(category);
    }

    private void OnTimer()
    {
        if (User._OracleListCategoryInfo._RenewTime > GrpcManager.ServerDateTime || inRequestingOracle) return;

        Setup();
    }
    #endregion

    private void Setup()
    {
        GameManager._instance.OnTimerEvent -= OnTimer;

        _EventBannerPageIsTweening = false;

        inRequestingOracle = true;

        User.GetOnAirOracles(
            success =>
            {
                inRequestingOracle = false;

                if (!success)
                {
                    confirmUIOnNoAvailiableOracle = UIMessageBox.Confirm(
                        string.Empty,
                        "UI_Oneline_Oracle_DESC",
                        () =>
                        {
                            confirmUIOnNoAvailiableOracle.Hide();
                            Hide();
                        });

                    return;
                }

                GameManager._instance.OnTimerEvent -= OnTimer;
                GameManager._instance.OnTimerEvent += OnTimer;

                // 이벤트 배너 페이지 세팅
                SetupEventOraclePage();

                var followOracleList = User._OracleListCategoryInfo[VisualStoryHelper.OracleCategory.FOLLOW];
                followOracleList.onSort -= OnSortFollowOracleList;
                followOracleList.onSort += OnSortFollowOracleList;
                followList.Set(VisualStoryHelper.OracleCategory.FOLLOW, followOracleList);

                var suggesgionOracleList = User._OracleListCategoryInfo[VisualStoryHelper.OracleCategory.SUGGESTION];
                suggesgionOracleList.onSort -= OnSortSuggestionOracleList;
                suggesgionOracleList.onSort += OnSortSuggestionOracleList;
                suggestionList.Set(VisualStoryHelper.OracleCategory.SUGGESTION, suggesgionOracleList);

                SelectNotEventOracleCategory(VisualStoryHelper.OracleCategory.FOLLOW, true);
            },
            error =>
            {
                errorUIWhenGetFromServer = UIMessageBox.Confirm_Old(
                    "Oracle 목록 요청 에러 발생",
                    $"ERROR CODE: [{error}]\n" +
                    "오라클 홈페이지를 닫습니다.",
                    () => { Hide(); if (errorUIWhenGetFromServer != null) errorUIWhenGetFromServer.Hide(); });
            });
    }

    private void SelectNotEventOracleCategory(VisualStoryHelper.OracleCategory category, bool immediatelyShow)
    {
        if (category == VisualStoryHelper.OracleCategory.EVENT) return;

        currentNotEventCategory = category;

        var showTarget = category == VisualStoryHelper.OracleCategory.FOLLOW ? followList : suggestionList;
        var hideTarget = category == VisualStoryHelper.OracleCategory.FOLLOW ? suggestionList : followList;

        if (showTarget != null) showTarget.Show(immediatelyShow);
        if (hideTarget != null) hideTarget.Hide(immediatelyShow);

        ChangeShowingStreamingListTo(category);
    }
    private void ChangeShowingStreamingListTo(VisualStoryHelper.OracleCategory category)
    {
        if (notEventBanners == null || category == VisualStoryHelper.OracleCategory.EVENT) return;

        var infoList = User._OracleListCategoryInfo[category];
        for (int i = 0; i < notEventBanners.Length; i++)
        {
            var banner = notEventBanners[i];
            if (banner == null) continue;

            banner.Set(category, infoList[i]);
        }
    }

    private void MoveTo(int page, bool isAutoSlide)
    {
        if (eventBannerScroller == null) return;

        eventBannerScroller.JumpToDataIndex(
            page,
            tweenType: isAutoSlide ? eventBannerPageSlideSetting.autoSlideEaseType : eventBannerPageSlideSetting.manualSlideEaseType,
            tweenTime: isAutoSlide ? eventBannerPageSlideSetting.autoSlideTweenTime : eventBannerPageSlideSetting.manualSlideTweenTime,
            jumpComplete: () =>
            {
                //OnChangedEventBannerPage(_CurrentEventBannerPage);
                StartEventBannerAutoSlideCorout();
            });
    }

    private void StopEventBannerAutoSlideCorout()
    {
        if (eventBannerAutoSlideCorout == null) return;
        StopCoroutine(eventBannerAutoSlideCorout);
        eventBannerAutoSlideCorout = null;
    }
    private void StartEventBannerAutoSlideCorout()
    {
        if (_MaxEventBannerPage <= 0) return;

        StopEventBannerAutoSlideCorout();
        eventBannerAutoSlideCorout = EventBannerAutoSlideCorout();
        StartCoroutine(eventBannerAutoSlideCorout);
    }
    private IEnumerator EventBannerAutoSlideCorout()
    {
        yield return new WaitForSeconds(eventBannerPageSlideSetting.autoSlideTerm);

        _CurrentEventBannerPage += 1;
        MoveTo(_CurrentEventBannerPage, true);

        eventBannerAutoSlideCorout = null;
    }

    private void SetupEventOraclePage()
    {
        StopEventBannerAutoSlideCorout();
        var eventOracleList = User._OracleListCategoryInfo[VisualStoryHelper.OracleCategory.EVENT];
        eventOracleList.onSort -= OnSortEventOracleList;
        eventOracleList.onSort += OnSortEventOracleList;

        list_EventStreamPage.Clear();
        for (int i = 0; i < eventOracleList.Count; i += 2)
        {
            var first = eventOracleList[i];
            var second = eventOracleList[i + 1];

            list_EventStreamPage.Add(new OracleEventBannerPageCellData(first, second, OnClickBanner));
        }
        _CurrentEventBannerPage = 0;

        var countOverOne = list_EventStreamPage.Count > 1;
        if (btn_LeftEventBannerPage != null) btn_LeftEventBannerPage.gameObject.SetActive(countOverOne);
        if (btn_RightEventBannerPage != null) btn_RightEventBannerPage.gameObject.SetActive(countOverOne);
        if (eventBannerScroller != null)
        {
            eventBannerScroller.Loop = countOverOne;
            eventBannerScroller.ScrollRect.movementType = countOverOne ? ScrollRect.MovementType.Elastic : ScrollRect.MovementType.Clamped;
        }

        eventBannerScroller.ReloadData();

        // 이벤트 배너 페이지 마커 세팅
        {
            if (tf_EventBannerPageMarkerRoot != null && eventBannerPageMarkerPrefab != null)
            {
                while (list_EventStreamPage.Count > list_EventBannerPageMarker.Count)
                {
                    var inst = Instantiate(eventBannerPageMarkerPrefab, tf_EventBannerPageMarkerRoot);
                    list_EventBannerPageMarker.Add(inst);
                }
            }
            for (int i = 0; i < list_EventBannerPageMarker.Count; i++)
                list_EventBannerPageMarker[i]?.SetActive(i <= _MaxEventBannerPage);

            OnChangedEventBannerPage(_CurrentEventBannerPage);
        }

        if (list_EventStreamPage.Count > 1) StartEventBannerAutoSlideCorout();
    }

    private int GetCurrentEventBannerPageSection(float pageSize, float currentScrollPosition)
    {
        return (int)((currentScrollPosition + pageSize / 2f) / pageSize) % (_MaxEventBannerPage + 1);
    }
}