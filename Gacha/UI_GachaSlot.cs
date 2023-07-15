using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using EnhancedUI.EnhancedScroller;
using Debug = COA_DEBUG.Debug;
using System.Text.RegularExpressions;

public class UI_GachaSlot : UIBaseBelongingEnhancedScrollerCellView
{
    [System.Serializable]
    public struct SlotSize
    {
        [SerializeField] private float width;
        [SerializeField] private float height;

        public float _Width => width;
        public float _Height => height;

        public bool Equals(SlotSize size)
        {
            return _Width == size._Width && _Height == size._Height;
        }

        public Vector2 ToVector2()
        {
            return new Vector2(_Width, _Height);
        }
    }
    public enum ScrollDirection
    {
        VERTICAL,
        HORIZONTAL
    }

    [SerializeField] private Image img_Slot;
    [SerializeField] private Button btn_Slot;
    [SerializeField] private Button btn_Disable;
    [SerializeField] private TextMeshProUGUI tmpu_SlotNumber;
    [SerializeField] private TextMeshProUGUI tmpu_SlotName;
    [SerializeField] private RectTransform rtf_SlotRoot;
    [SerializeField] private GameObject go_Off;
    [SerializeField] private GameObject go_On;
    [SerializeField] private GameObject go_Disable;
    [SerializeField] private Image img_SlotSelectBox;

    [Header("Slot Scroll")]
    [SerializeField] private ScrollDirection scrollDirection;

    public GachaHelper.SlotScrollData _Data { get; private set; }

    public void SetScrollData(GachaHelper.SlotScrollData data)
    {
        if (_Data != null)
        {
            _Data.onUpdateSelection -= SetOn;
        }

        _Data = data;
        UIUtil.ResetAndAddListener(btn_Slot, OnClickSlotButton);
        UIUtil.ResetAndAddListener(btn_Disable, OnClickDisableButton);

        var gachaData = _Data?._CategoryInfo?._GachaGoodsInfo_Once?._Data;
        if (tmpu_SlotName != null)
        {
            Regex rich = new Regex(@"<[^>]*>");
            var name = gachaData != null
                ? Localizer.GetLocalizedStringName(Localizer.SheetType.GACHA, gachaData?._Enum_Id + "_Slot")
                : string.Empty;
            if (rich.IsMatch(name)) name = rich.Replace(name, string.Empty);

            tmpu_SlotName.text = name;
        }
        SetOnDisable(gachaData == null);

        if (_Data == null) return;
        if (tmpu_SlotNumber != null) tmpu_SlotNumber.text = $"{User._ShopInfos[Data.Enum.Shop_Type.GACHA]._Categorys.FindIndex(a => a.id == _Data._CategoryInfo.id) + 1:D2}";

        _Data.onUpdateSelection -= SetOn;
        _Data.onUpdateSelection += SetOn;
        SetOn(_Data._Selection);

        if (img_Slot != null)
        {
            var sprite = gachaData?._Slot_Image_Data?.GetSpriteFromSMTable(ownerUIBase);
            img_Slot.gameObject.SetActive(sprite != null);
            img_Slot.sprite = sprite;
        }

        if (img_SlotSelectBox != null)
        {
            img_SlotSelectBox.sprite = gachaData?._Gacha_Resource_Data?._Box_Reference_Data?.GetSpriteFromSMTable(ownerUIBase);
        }
    }

    private void SetOn(bool isOn)
    {
        go_Off?.SetActive(!isOn);
        go_On?.SetActive(isOn);
        if (rtf_SlotRoot != null)
        {
            rtf_SlotRoot.sizeDelta = scrollDirection switch
            {
                ScrollDirection.VERTICAL => isOn ? GachaHelper.SlotSize.Shop.On._ToVector2 : GachaHelper.SlotSize.Shop.Off._ToVector2,
                ScrollDirection.HORIZONTAL => isOn ? GachaHelper.SlotSize.Detail.On._ToVector2 : GachaHelper.SlotSize.Detail.Off._ToVector2,
                _ => Vector2.zero
            };
        }
    }

    private void OnClickSlotButton()
    {
        if (_Data == null) return;
        _Data.Select();
    }

    private void SetOnDisable(bool set)
    {
        if (go_Disable == null) return;
        go_Disable.SetActive(set);
    }

    private void OnClickDisableButton()
    {
        UIMessageBox.OnelineMsg_NotAvailableGoods();
        if (_Data == null) return;
        _Data.Select();
    }
}