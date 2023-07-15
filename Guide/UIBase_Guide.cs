using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;
using TMPro;
using Debug = COA_DEBUG.Debug;
using LeTai.Asset.TranslucentImage;
using EnhancedUI.EnhancedScroller;
using UnityEngine.EventSystems;
using Spine;

public class GuidePageScrollData
{
    public int index;
    public guide_pageTable pageData;

    public Action<Vector2, Vector2, PointerEventData> onHorizontalDragStart;
    public Action<Vector2, Vector2, PointerEventData> onHorizontalDragging;
    public Action<Vector2, Vector2, PointerEventData> onHorizontalDragEnd;

    public GuidePageScrollData(int index, guide_pageTable pageData,
        Action<Vector2, Vector2, PointerEventData> onHorizontalDragStart,
        Action<Vector2, Vector2, PointerEventData> onHorizontalDragging,
        Action<Vector2, Vector2, PointerEventData> onHorizontalDragEnd)
    {
        this.index = index;
        this.pageData = pageData;
        this.onHorizontalDragStart = onHorizontalDragStart;
        this.onHorizontalDragging = onHorizontalDragging;
        this.onHorizontalDragEnd = onHorizontalDragEnd;
    }
}

public class UIBase_Guide : UIBase, IEnhancedScrollerDelegate
{
    #region Inspector
    [Header("COMMON")]
    [SerializeField] private TranslucentImage translucentImage;
    [SerializeField] private Button btn_Close;

    [Header("Guide")]
    [SerializeField] private EnhancedScroller scroller;
    [SerializeField] private UI_Guide_Page guidePagePrefab;
    [SerializeField] private UI_DragDetector dragDetector;
    [SerializeField] private Button btn_ToLeftPage;
    [SerializeField] private Button btn_ToRightPage;
    [SerializeField] private EnhancedScroller.TweenType snapTweenType;
    [SerializeField, Range(0f, 2f)] private float snapTweenTime;
    [SerializeField] private float minScrollDeltaForSnapping;

    [Header("Page Mark")]
    [SerializeField] private Transform tf_PageMarkParent;
    [SerializeField] private UI_Guide_PageMark pageMarkPrefab;
    [SerializeField] private Button btn_TutorialSkip;
    #endregion

    #region Variables
    private bool init = false;
    private UIParam.Common.GuideParam guideParam;
    private List<GuidePageScrollData> list_GuidePage = new List<GuidePageScrollData>();
    private GuidePageScrollData currentPage;
    private Vector2 dragStartPos;
    private int _PageCount => list_GuidePage.Count;

    private List<UI_Guide_PageMark> list_PageMarkInst = new List<UI_Guide_PageMark>();

    private bool isSnapping = false;
    #endregion

    #region Base Methods
    private void OnValidate()
    {
        if (minScrollDeltaForSnapping < 0f)
            minScrollDeltaForSnapping *= -1f;
    }

    public override void Show(object param)
    {
        isSnapping = false;
        translucentImage.source = UIManager._instance._TranslucentImageSource;
        if (!init)
        {
            UIUtil.ResetAndAddListener(btn_Close, OnClickClose);
            UIUtil.ResetAndAddListener(btn_ToLeftPage, OnClickToLeftPage);
            UIUtil.ResetAndAddListener(btn_ToRightPage, OnClickToRightPage);

            btn_TutorialSkip.gameObject.SetActive(User.Info.isTutorial);

            if(User.Info.isTutorial)
            {
                btn_TutorialSkip.gameObject.SetActive(true);
                UIUtil.ResetAndAddListener(btn_TutorialSkip, OnClickClose);
            }
            else
                btn_TutorialSkip.gameObject.SetActive(false);

            if (scroller != null)
            {
                scroller.Delegate = this;
            }
            if (dragDetector != null)
            {
                dragDetector.onBeginDrag = OnBeginDragging;
                dragDetector.onDragging = OnDragging;
                dragDetector.onEndDrag = OnEndDragging;
            }

            init = true;
        }

        base.Show(param);
        guideParam = param as UIParam.Common.GuideParam;
        if (guideParam == null || guideParam.guideData == null)
        { Hide(); return; }

        btn_Close.enabled = !guideParam.isTutorial;

        LoadPageDataList();
        scroller.ReloadData();
    }

    public override void Hide()
    {
        base.Hide();
    }
    #endregion

    #region Scroller Delegate
    public int GetNumberOfCells(EnhancedScroller scroller)
    {
        return list_GuidePage.Count;
    }

    public float GetCellViewSize(EnhancedScroller scroller, int dataIndex)
    {
        return scroller.ScrollRectSize - scroller.spacing;
    }

    public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
    {
        var cell = scroller.GetCellView(guidePagePrefab) as UI_Guide_Page;
        cell.Set(list_GuidePage[dataIndex]);
        return cell;
    }
    #endregion

    #region Listener
    private void OnClickClose()
    {
        if (guideParam.isTutorial)
            guideParam.tutorial.Invoke(guideParam.step_Index);
        Hide();
    }
    private void OnClickToLeftPage()
    {
        if (currentPage == null)
            PageSnap(0);
        PageSnap(currentPage.index - 1);
    }
    private void OnClickToRightPage()
    {
        if (currentPage == null)
            PageSnap(0);
        PageSnap(currentPage.index + 1);
    }

    private void OnBeginDragging(Vector2 prev, Vector2 next, PointerEventData eventData)
    {
        if (isSnapping)
            return;
        dragStartPos = next;
    }
    private void OnDragging(Vector2 prev, Vector2 next, PointerEventData eventData)
    {
        if (scroller == null || isSnapping)
            return;

        var delta = next - prev;
        scroller.ScrollPosition -= delta.x;
    }
    private void OnEndDragging(Vector2 prev, Vector2 next, PointerEventData eventData)
    {
        if (currentPage == null || isSnapping)
            return;

        var finalDelta = next - dragStartPos;
        if (finalDelta.x >= minScrollDeltaForSnapping)
        {
            PageSnap(currentPage.index - 1);
        }
        else if (finalDelta.x <= -minScrollDeltaForSnapping)
        {
            PageSnap(currentPage.index + 1);
        }
        else
        {
            PageSnap(currentPage.index);
        }
    }
    #endregion

    private void LoadPageDataList()
    {
        list_GuidePage.Clear();
        foreach (var v in guideParam.guideData._Page_Data)
        {
            if (v == null)
                continue;
            list_GuidePage.Add(new GuidePageScrollData(
                list_GuidePage.Count,
                v,
                OnBeginDragging,
                OnDragging,
                OnEndDragging));
        }

        currentPage = _PageCount > 0 ? list_GuidePage[0] : null;

        if (tf_PageMarkParent == null && pageMarkPrefab == null)
            return;

        while (list_PageMarkInst.Count < _PageCount)
        {
            var inst = Instantiate(pageMarkPrefab, tf_PageMarkParent);
            list_PageMarkInst.Add(inst);
        }

        var len = list_PageMarkInst.Count;
        if (len == 0)
            return;

        list_PageMarkInst[0].SetOnOff(true);
        list_PageMarkInst[0].SetActive(true);
        for (int i = 1; i < len; i++)
        {
            list_PageMarkInst[i].SetOnOff(false);
            list_PageMarkInst[i].SetActive(i < _PageCount);
        }

        btn_ToLeftPage?.gameObject.SetActive(false);
        btn_ToRightPage?.gameObject.SetActive(_PageCount > 1);
    }

    private void PageSnap(int pageNumber)
    {
        if (scroller == null || isSnapping)
            return;
        isSnapping = true;
        pageNumber = Mathf.Clamp(pageNumber, 0, _PageCount - 1);
        currentPage = list_GuidePage[pageNumber];


        if (guideParam.isTutorial)
            btn_Close.enabled = pageNumber == (list_GuidePage.Count - 1);


        btn_ToLeftPage?.gameObject.SetActive(pageNumber > 0);
        btn_ToRightPage?.gameObject.SetActive(pageNumber < _PageCount - 1);
        for (int i = 0; i < _PageCount; i++)
            list_PageMarkInst[i].SetOnOff(i == pageNumber);

        scroller.JumpToDataIndex(pageNumber, tweenType: snapTweenType, tweenTime: snapTweenTime,
            jumpComplete: () =>
            {
                isSnapping = false;
            });
    }
}
