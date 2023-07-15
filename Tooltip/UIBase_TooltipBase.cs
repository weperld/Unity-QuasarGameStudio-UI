using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;
using TMPro;
using Debug = COA_DEBUG.Debug;
using static UIParam.Tooltip;
using UnityEngine.EventSystems;

public abstract class UIBase_TooltipBase<TTooltipParam> : UIBase, IDeselectHandler
    where TTooltipParam : UIParam.Tooltip.Base
{
    #region Inspector
    [Header("Base")]
    [SerializeField] protected RectTransform rtf_Body;
    [SerializeField] private Button btn_Close;
    #endregion

    #region Variables
    private bool init = false;
    private HorizontalCreateDirection hcd;
    private VerticalCreateDirection vcd;
    private TTooltipParam tooltipParam;

    public HorizontalCreateDirection _HCD => hcd;
    public VerticalCreateDirection _VCD => vcd;
    public TTooltipParam _TooltipParam => tooltipParam;
    #endregion

    #region Base Methods
    public override void Show(object param)
    {
        base.Show(param);
        tooltipParam = param as TTooltipParam;
        if (tooltipParam == null) { Hide(); return; }

        if (!init)
        {
            UIUtil.ResetAndAddListener(btn_Close, Hide);
            OnInit(tooltipParam);
            init = true;
        }

        Setup();
    }
    #endregion

    /// <summary>
    /// Do not need "null check"
    /// </summary>
    /// <param name="param"></param>
    protected abstract void OnInit(TTooltipParam param);

    private void Setup()
    {
        if (btn_Close != null) btn_Close.gameObject.SetActive(tooltipParam.useCloseBackButton);
        else tooltipParam.useCloseBackButton = false;
        if (!tooltipParam.useCloseBackButton) EventSystem.current.SetSelectedGameObject(gameObject);

        SetInfoToUI(tooltipParam);
        SetTooltipPosition();
    }

    /// <summary>
    /// Do not need "null check"
    /// </summary>
    /// <param name="param"></param>
    protected abstract void SetInfoToUI(TTooltipParam param);

    private void SetTooltipPosition()
    {
        if (rtf_Body == null) return;

        rtf_Body.anchorMax = Vector2.zero;
        rtf_Body.anchorMin = Vector2.zero;

        var layout = rtf_Body.GetComponentsInChildren<LayoutGroup>();
        foreach (var v in layout) LayoutRebuilder.ForceRebuildLayoutImmediate(v.GetComponent<RectTransform>());

        Vector2 pos = tooltipParam.showPosition;
        var correctionalPos = tooltipParam.correctionalPosTfs;

        SetCreateDirectionAndPos(pos);
        ApplyTooltipCorrectionalPos(correctionalPos);
        AdjustPosition();
    }
    private void SetCreateDirectionAndPos(Vector2 pos)
    {
        hcd =
            pos.x > ScreenSetupData.instance.basisScreenWidth / 2f
            ? HorizontalCreateDirection.LEFT
            : HorizontalCreateDirection.RIGHT;
        vcd =
            pos.y >= ScreenSetupData.instance.basisScreenHeight / 2f
            ? VerticalCreateDirection.DOWN
            : VerticalCreateDirection.UP;

        rtf_Body.pivot = new Vector2((float)hcd, (float)vcd);
        rtf_Body.anchoredPosition = pos;
    }
    private void ApplyTooltipCorrectionalPos(UIParam.TooltipCorrectionalPosTFs correctionalPos)
    {
        if (correctionalPos == null) return;

        var cp = vcd switch
        {
            VerticalCreateDirection.UP => correctionalPos.tf_Up,
            VerticalCreateDirection.DOWN => correctionalPos.tf_Down,
            _ => null,
        };
        if (cp == null) return;

        var canvasCam = cp.GetComponentInParent<Canvas>().rootCanvas.worldCamera;
        var screenPos = canvasCam != null
            ? canvasCam.WorldToScreenPoint(cp.transform.position)
            : Camera.main.WorldToScreenPoint(cp.transform.position);
        var showingPos = rtf_Body.anchoredPosition;
        showingPos.y = screenPos.y;
        rtf_Body.anchoredPosition = showingPos;
    }
    public void AdjustPosition()
    {
        var height = rtf_Body.rect.height;
        var width = rtf_Body.rect.width;
        Vector2 ancPos = rtf_Body.anchoredPosition;
        float v_Offset = 0f;
        float h_Offset = 0f;

        switch (vcd)
        {
            case VerticalCreateDirection.UP:
                if (height + ancPos.y > Screen.height)
                {
                    v_Offset = Screen.height - (height + ancPos.y);
                }
                break;
            case VerticalCreateDirection.DOWN:
                if (height > ancPos.y)
                {
                    v_Offset = height - ancPos.y;
                }
                break;
        }
        ancPos.y += v_Offset;

        switch (hcd)
        {
            case HorizontalCreateDirection.RIGHT:
                if (width + ancPos.x > Screen.width)
                {
                    h_Offset = Screen.width - (width + ancPos.x);
                }
                break;
            case HorizontalCreateDirection.LEFT:
                if (width > ancPos.x)
                {
                    h_Offset = width - ancPos.x;
                }
                break;
        }
        ancPos.x += h_Offset;

        rtf_Body.anchoredPosition = ancPos;
    }
    public void ChangeHorizontalPivotPos(HorizontalCreateDirection value)
    {
        hcd = value;
        var pos = rtf_Body.anchoredPosition;
        rtf_Body.pivot = new Vector2((float)hcd, (float)vcd);
        rtf_Body.anchoredPosition = pos;

        AdjustPosition();
    }
    public void ChangeVerticalPivotPos(VerticalCreateDirection value)
    {
        vcd = value;
        var pos = rtf_Body.anchoredPosition;
        rtf_Body.pivot = new Vector2((float)hcd, (float)vcd);
        rtf_Body.anchoredPosition = pos;

        AdjustPosition();
    }

    public void OnDeselect(BaseEventData eventData)
    {
        if (tooltipParam.useCloseBackButton) return;
        Hide();
    }
}
