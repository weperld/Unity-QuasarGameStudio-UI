using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;
using TMPro;
using Debug = COA_DEBUG.Debug;
using System.Text.RegularExpressions;
using AudioSystem;

public class UIBase_GachaRecordInfoWindow : UIBase
{
    private struct Category
    {
        public string key;
        public string value;

        public Category(string key, string value)
        {
            this.key = key;
            this.value = value;
        }
    }

    #region Inspector
    [SerializeField] private Button btn_Close;
    [SerializeField] private UI_GachaRecordInfo[] records;
    [SerializeField] private Button btn_Left;
    [SerializeField] private Button btn_Right;
    [SerializeField] private TextMeshProUGUI tmpu_Page;
    [SerializeField] private TMP_Dropdown slotDropdown;
    [SerializeField] private GameObject[] go_OnExist;
    [SerializeField] private GameObject[] go_NotOnExist;
    #endregion

    #region Variables
    private long maxPage = 1;
    private long currentPage = 1;

    private long _MaxPage
    {
        get => maxPage;
        set
        {
            maxPage = value;
        }
    }
    private long _CurrentPage
    {
        get => currentPage;
        set
        {
            currentPage = value;
        }
    }

    private UIBase onErrorUI;

    private List<Category> list_Category = new List<Category>();
    private int currentCategoryIndex;

    private bool init = false;
    #endregion

    #region Base Methods
    public override void Show(object param = null)
    {
        if (!init)
        {
            init = true;
            UIUtil.ResetAndAddListener(btn_Close, OnClickCloseButton);
            UIUtil.ResetAndAddListener(btn_Left, OnClickLeftPageButton);
            UIUtil.ResetAndAddListener(btn_Right, OnClickRightPageButton);
            UIUtil.ResetAndAddListener(slotDropdown, OnValueChangeSlotDropdown);
        }

        User.GetGachaLogCategories(
            categories =>
            {
                list_Category.Clear();
                list_Category.Add(new Category("All", string.Empty));
                foreach (var category in categories)
                    list_Category.Add(new Category(
                        Regex.Replace(
                            Localizer.GetLocalizedStringName(Localizer.SheetType.GACHA, category),
                            @"<(.|\n)*?>",
                            string.Empty),
                        category));

                if (slotDropdown == null) return;

                slotDropdown.ClearOptions();
                var optionList = new List<TMP_Dropdown.OptionData>();
                foreach (var category in list_Category)
                    optionList.Add(new TMP_Dropdown.OptionData(category.key));
                slotDropdown.AddOptions(optionList);

                slotDropdown.SetValueWithoutNotify(0);
                OnValueChangeSlotDropdown(0);

                base.Show(param);
            },
            categoryError =>
            {
                if (gameObject.activeSelf) Hide();

                onErrorUI = UIMessageBox.Confirm_Old(
                    "RECRUIT ERROR",
                    $"ERROR CODE: [{categoryError}]",
                    () =>
                    {
                        onErrorUI?.Hide();
                    });
            });
    }
    #endregion

    #region Listener
    private void OnClickCloseButton()
    {
        Hide();
    }
    private void OnClickLeftPageButton()
    {
        AudioManager._instance.PlaySound(DefineName.Audio_OutGame.Sound.BUTTON);
        RequestGetGachaLogs(Math.Max(_CurrentPage - 1, 1));
    }
    private void OnClickRightPageButton()
    {
        AudioManager._instance.PlaySound(DefineName.Audio_OutGame.Sound.BUTTON);
        RequestGetGachaLogs(Math.Min(_CurrentPage + 1, _MaxPage));
    }

    private void OnValueChangeSlotDropdown(int value)
    {
        currentCategoryIndex = value;
        RequestGetGachaLogs(1);
    }
    #endregion

    private void RequestGetGachaLogs(long page)
    {
        User.GetGachaLogs(new User.GachaLogReq(list_Category[currentCategoryIndex].value, page),
            success =>
            {
                _CurrentPage = page;
                _MaxPage = success.total_Page;

                if (btn_Left != null) btn_Left.gameObject.SetActive(_MaxPage > 1 && _CurrentPage > 1);
                if (btn_Right != null) btn_Right.gameObject.SetActive(_MaxPage > 1 && _CurrentPage < _MaxPage);

                var logIsExist = _MaxPage > 0;
                if (go_OnExist != null) foreach (var go in go_OnExist) go?.SetActive(logIsExist);
                if (go_NotOnExist != null) foreach (var go in go_NotOnExist) go?.SetActive(!logIsExist);

                if (logIsExist)
                {
                    if (tmpu_Page != null) tmpu_Page.text = $"{_CurrentPage}/{_MaxPage}";

                    if (records != null)
                    {
                        var recordCount = success.list_Record.Count;
                        for (int i = 0; i < records.Length; i++)
                        {
                            var record = records[i];
                            if (record == null) continue;

                            record.SetData(i < recordCount ? success.list_Record[i] : null);
                        }
                    }
                }
            },
            error =>
            {
                if (gameObject.activeSelf) Hide();

                onErrorUI = UIMessageBox.Confirm_Old(
                "가챠 기록 요청 에러 발생",
                $"가챠 로그 로드 에러\n" +
                $"ERROR CODE: [{error}]",
                () =>
                {
                    onErrorUI?.Hide();
                });
            });
    }
}
