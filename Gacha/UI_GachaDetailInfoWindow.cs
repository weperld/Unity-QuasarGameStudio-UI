using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Debug = COA_DEBUG.Debug;
using EnhancedUI.EnhancedScroller;

public class UI_GachaDetailInfoWindow : MonoBehaviour, IEnhancedScrollerDelegate
{
    #region Inspector
    [SerializeField] private Button btn_Close;
    [SerializeField] private EnhancedScroller scroller;
    [SerializeField] private UI_GachaSlot slotPrefab;
    [SerializeField] private TextMeshProUGUI tmpu_Description;
    #endregion

    #region Variables
    private List<GachaHelper.SlotScrollData> slotList = new List<GachaHelper.SlotScrollData>();
    private GachaHelper.SlotScrollData selected;
    #endregion

    public void SetupAndShow(int selectedSlotNumber)
    {
        gameObject.SetActive(true);
        if (scroller != null) scroller.Delegate = this;
        UIUtil.ResetAndAddListener(btn_Close, OnClickClose);

        slotList.Clear();
        selected = null;
        foreach (var v in User._ShopInfos[Data.Enum.Shop_Type.GACHA]._Categorys)
        {
            GachaCategoryInfo tmp = (v as GachaCategoryInfo).Clone();
            slotList.Add(new GachaHelper.SlotScrollData(tmp));
        }
        foreach (var v in slotList) v.onSelection = OnSelection;
        slotList[selectedSlotNumber].Select();

        scroller.ReloadData();
    }

    #region Scroller Delegate
    public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
    {
        var slot = scroller.GetCellView(slotPrefab) as UI_GachaSlot;
        slot.SetScrollData(slotList[dataIndex]);
        return slot;
    }

    public float GetCellViewSize(EnhancedScroller scroller, int dataIndex)
    {
        return slotList[dataIndex]._Selection ? GachaHelper.SlotSize.Detail.On.WIDTH : GachaHelper.SlotSize.Detail.Off.WIDTH;
    }

    public int GetNumberOfCells(EnhancedScroller scroller)
    {
        return slotList == null ? 0 : slotList.Count;
    }
    #endregion

    #region Listener
    private void OnClickClose()
    {
        gameObject.SetActive(false);
    }
    private void OnSelection(GachaHelper.SlotScrollData select)
    {
        var categoryInfo = select?._CategoryInfo;
        if (categoryInfo == null)
        {
            ChangeSelectedSlot(null);
            return;
        }

        var goodsInfo = categoryInfo._GachaGoodsInfo_Once;
        if (goodsInfo == null) UIMessageBox.OnelineMsg_NotAvailableGoods();
        else ChangeSelectedSlot(select);
    }
    #endregion

    private void ChangeSelectedSlot(GachaHelper.SlotScrollData select)
    {
        var categoryInfo = select?._CategoryInfo;
        var goodsInfo = categoryInfo?._GachaGoodsInfo_Once;
        if (select != null && goodsInfo == null) return;

        if (selected != null) selected._Selection = false;

        if (select == null)
        {
            selected = null;
            if (tmpu_Description != null) tmpu_Description.text = "먼저 슬롯을 선택해 주세요.";

            scroller.IgnoreLoopJump(true);
            scroller.ReloadData();
            scroller.IgnoreLoopJump(false);

            return;
        }

        selected = select;
        selected._Selection = true;
        if (tmpu_Description != null)
        {
            string format = Localizer.GetLocalizedStringDesc(Localizer.SheetType.GACHA, goodsInfo._Data._Enum_Id + "_Detail");
            var args = GachaHelper.GetGachaDescriptionArguments(goodsInfo._Data, tmpu_Description.fontSize);
            tmpu_Description.text = string.Format(format, args);
        }

        scroller.IgnoreLoopJump(true);
        scroller.ReloadData();
        scroller.IgnoreLoopJump(false);
    }
}