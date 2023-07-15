using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;
using TMPro;
using EnhancedUI.EnhancedScroller;
using Debug = COA_DEBUG.Debug;
using System.Linq;
using AudioSystem;

public class PartnerListScrollData
{
    private string enumId = User.NoPartnerCharacterEnumId;
    private bool selection = false;

    public characterTable _TableData => Data._characterTable.GetDataFromTable(enumId);
    public bool _Selection
    {
        get => _Selection;
        set
        {
            selection = value;
            onChangeSelectionState?.Invoke(value);
        }
    }

    private Action<PartnerListScrollData> onTouch;
    private Action<bool> onChangeSelectionState;

    public PartnerListScrollData(characterTable data, Action<PartnerListScrollData> onTouch)
    {
        enumId = data._Enum_Id;
        this.onTouch = onTouch;
    }

    public void Touch()
    {
        AudioManager._instance.PlaySound(DefineName.Audio_OutGame.Sound.BUTTON);
        onTouch?.Invoke(this);
    }

    public void RemoveOnChangeSelectionState(Action<bool> onChangeSelectionState)
    {
        this.onChangeSelectionState -= onChangeSelectionState;
    }
    public void RegistOnChangeSelectionState(Action<bool> onChangeSelectionState)
    {
        RemoveOnChangeSelectionState(onChangeSelectionState);
        this.onChangeSelectionState += onChangeSelectionState;
    }
}

public class UIBase_PartnerList : UIBase, IEnhancedScrollerDelegate
{
    #region Inspector
    [SerializeField] private Button btn_Back;
    [SerializeField] private Button btn_Home;
    [SerializeField] private UI_PartnerList_Filter[] filters;
    [SerializeField] private EnhancedScroller scroller;
    [SerializeField] private UI_PartnerList_CardGroup cellPrefab;
    #endregion

    #region Variables
    private List<PartnerListScrollData> list_Datas = new List<PartnerListScrollData>();
    private PartnerListScrollData selectCharacter;
    private UI_PartnerList_Filter selectFilter;
    private UIBase partnerChangeConfirm;
    #endregion

    #region Base Method
    private void Awake()
    {
        UIUtil.ResetAndAddListener(btn_Back, OnClickBackButton);
        UIUtil.ResetAndAddListener(btn_Home, OnClickHomeButton);

        if (filters == null) return;
        foreach (var v in filters)
        {
            if (v == null) continue;
            v.Set(OnClickFilterButton);
        }
    }

    public override void Show(object param)
    {
        base.Show(param);

        scroller.Delegate = this;
        OnClickFilterButton(filters[0]);
    }
    #endregion

    #region Listener
    private void OnClickBackButton()
    {
        Hide();
    }
    private void OnClickHomeButton()
    {
        UIManager._instance.HideAll();
    }

    private void OnCardTouch(PartnerListScrollData data)
    {
        if (selectCharacter != null) selectCharacter._Selection = false;

        selectCharacter = data;
        if (selectCharacter != null) selectCharacter._Selection = true;

        if (User._PartnerCharacter == selectCharacter._TableData) return;

        var tableData = selectCharacter._TableData;
        var confirmTxt = tableData == User._NoPartnerCharacter
            ? "UI_Remove_Partner_Text_DESC"
            : "UI_Change_Partner_Text_DESC";

        partnerChangeConfirm = UIMessageBox.Confirm_Choice(
            string.Empty, null,
            confirmTxt, null,
            () =>
            {
                User._PartnerCharacter = tableData;
                UIManager._instance.HideAll();
            },
            () =>
            {
                partnerChangeConfirm?.Hide();
            });
    }
    private void OnClickFilterButton(UI_PartnerList_Filter filter)
    {
        selectFilter = filter;
        foreach (var v in filters)
        {
            if (v == null) continue;
            v.SetOnOff(v == selectFilter);
        }

        LoadDataList();
        scroller.ReloadData();
    }
    #endregion

    #region Scroller Delegate
    public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
    {
        var cellGroup = scroller.GetCellView(cellPrefab) as UI_PartnerList_CardGroup;
        cellGroup.Set(list_Datas, dataIndex * (cellPrefab != null ? cellPrefab._CountPerRow : 1));
        return cellGroup;
    }

    public float GetCellViewSize(EnhancedScroller scroller, int dataIndex)
    {
        return cellPrefab != null ? cellPrefab._Height : 0f;
    }

    public int GetNumberOfCells(EnhancedScroller scroller)
    {
        var div = cellPrefab != null ? cellPrefab._CountPerRow : 1;
        var quo = list_Datas.Count / div;
        var rest = list_Datas.Count % div;

        return quo + (rest > 0 ? 1 : 0);
    }
    #endregion

    private void LoadDataList()
    {
        list_Datas.Clear();
        foreach (var v in Data._characterTable)
        {
            var data = v.Value;
            if (data == null || data == User._NoPartnerCharacter || !data._Partner || data == User._PartnerCharacter) continue;
            if (selectFilter != null && !selectFilter.Contains(data)) continue;

            list_Datas.Add(new PartnerListScrollData(data, OnCardTouch));
        }

        list_Datas.Sort((a, b) =>
        {
            var aD = a._TableData;
            var bD = b._TableData;
            var result = -aD._CE_Character_Grade.CompareTo(bD._CE_Character_Grade);
            if (result == 0) result = aD._CE_Character_Class.CompareTo(bD._CE_Character_Class);
            if (result == 0) result = aD._Id.CompareTo(bD._Id);
            return result;
        });
        list_Datas.Insert(0, new PartnerListScrollData(User._NoPartnerCharacter, OnCardTouch));
        if (User._NoPartnerCharacter != User._PartnerCharacter)
            list_Datas.Insert(1, new PartnerListScrollData(User._PartnerCharacter, OnCardTouch));
    }
}
