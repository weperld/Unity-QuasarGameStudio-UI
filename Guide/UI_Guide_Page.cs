using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Debug = COA_DEBUG.Debug;
using UnityEngine.EventSystems;

public class UI_Guide_Page : UIBaseBelongingEnhancedScrollerCellView
{
    [SerializeField] private Image img_Source;
    [SerializeField] private TextMeshProUGUI tmpu_PageTitle;
    [SerializeField] private TextMeshProUGUI tmpu_PageDesc;
    [SerializeField] private UI_DragDetector dragDetector;
    [SerializeField] private ScrollRect descScrollRect;

    private GuidePageScrollData data;
    public void Set(GuidePageScrollData data)
    {
        this.data = data; var pageData = data?.pageData;
        gameObject.SetActive(pageData != null);
        if (dragDetector != null)
        {
            dragDetector.onBeginDrag = null;
            dragDetector.onDragging = null;
            dragDetector.onEndDrag = null;
        }

        if (pageData == null) return;
        OnChangeLanguage(User._Language);
        if (dragDetector != null)
        {
            dragDetector.onBeginDrag = OnBeginDrag;
            dragDetector.onDragging = OnDragging;
            dragDetector.onEndDrag = OnEndDrag;
        }
    }

    private void OnEnable()
    {
        User.onChangeLanguage -= OnChangeLanguage;
        User.onChangeLanguage += OnChangeLanguage;
    }
    private void OnDisable()
    {
        User.onChangeLanguage -= OnChangeLanguage;
    }

    private enum DragDirection
    {
        HORIZONTAL,
        VERTICAL,
    }
    private DragDirection dragDirection;
    private void OnBeginDrag(Vector2 prev, Vector2 next, PointerEventData eventData)
    {
        var delta = next - prev;
        var deltaABS = new Vector2(Mathf.Abs(delta.x), Mathf.Abs(delta.y));
        var angle = Mathf.Atan2(deltaABS.y, deltaABS.x) * Mathf.Rad2Deg;

        dragDirection = angle > 40f ? DragDirection.VERTICAL : DragDirection.HORIZONTAL;
        switch (dragDirection)
        {
            case DragDirection.HORIZONTAL:
                data.onHorizontalDragStart?.Invoke(prev, next, eventData);
                break;
            case DragDirection.VERTICAL:
                descScrollRect?.OnBeginDrag(eventData);
                break;
        }
    }
    private void OnDragging(Vector2 prev, Vector2 next, PointerEventData eventData)
    {
        var delta = next - prev;
        switch (dragDirection)
        {
            case DragDirection.HORIZONTAL:
                data.onHorizontalDragging?.Invoke(prev, next, eventData);
                break;
            case DragDirection.VERTICAL:
                descScrollRect?.OnDrag(eventData);
                break;
        }
    }
    private void OnEndDrag(Vector2 prev, Vector2 next, PointerEventData eventData)
    {
        var delta = next - prev;
        switch (dragDirection)
        {
            case DragDirection.HORIZONTAL:
                data.onHorizontalDragEnd?.Invoke(prev, next, eventData);
                break;
            case DragDirection.VERTICAL:
                descScrollRect?.OnEndDrag(eventData);
                break;
        }
    }

    private void OnChangeLanguage(Data.Enum.Language lang)
    {
        if (data == null || data.pageData == null) return;
        var pageData = data.pageData;

        if (tmpu_PageTitle != null) tmpu_PageTitle.text = Localizer.GetLocalizedStringName(Localizer.SheetType.NONE, pageData._Enum_Id);
        if (tmpu_PageDesc != null) tmpu_PageDesc.text = Localizer.GetLocalizedStringDesc(Localizer.SheetType.NONE, pageData._Enum_Id);
        if (img_Source != null)
        {
            var sprt = (lang switch
            {
                Data.Enum.Language.LANGUAGE_KR => pageData._Guide_Resource_Kr_Data,
                Data.Enum.Language.LANGUAGE_EN => pageData._Guide_Resource_En_Data,
                _ => null,
            })?.GetSpriteFromSMTable(ownerUIBase);
            img_Source.sprite = sprt;
        }
    }
}