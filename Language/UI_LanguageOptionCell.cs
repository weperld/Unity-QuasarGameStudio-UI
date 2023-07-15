using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Debug = COA_DEBUG.Debug;

public class UI_LanguageOptionCell : EnhancedUI.EnhancedScroller.EnhancedScrollerCellView
{
    [SerializeField] private Button btn_Select;
    [SerializeField] private TextMeshProUGUI[] tmpu_Natives;
    [SerializeField] private TextMeshProUGUI[] tmpu_Engs;
    [SerializeField] private GameObject go_On;
    [SerializeField] private GameObject go_Off;
    [SerializeField] private Color color_On;
    [SerializeField] private Color color_Off;

    private bool init = false;

    private LanguageOptionCellData _Data { get; set; }
    public void Set(LanguageOptionCellData data)
    {
        if (!init)
        {
            init = true;
            UIUtil.ResetAndAddListener(btn_Select, OnClickSelect);
        }

        if (_Data != null)
        {
            _Data.onSelect -= OnChangeSelectState;
        }

        _Data = data;
        if (_Data == null)
        {
            gameObject.SetActive(false);
            return;
        }

        gameObject.SetActive(true);
        _Data.onSelect -= OnChangeSelectState;
        _Data.onSelect += OnChangeSelectState;
        OnChangeSelectState(_Data._Select);

        var str = Localizer.GetLocalizedStringName(Localizer.SheetType.NONE, _Data.optionLanguage.ToString(), _Data.optionLanguage);
        if (tmpu_Natives != null) foreach (var tmpu in tmpu_Natives) if (tmpu != null) tmpu.text = str;
        str = Localizer.GetLocalizedStringName(Localizer.SheetType.NONE, _Data.optionLanguage.ToString(), Data.Enum.Language.LANGUAGE_EN);
        if (tmpu_Engs != null) foreach (var tmpu in tmpu_Engs) if (tmpu != null) tmpu.text = str;
    }

    private void OnChangeSelectState(bool value)
    {
        go_On?.SetActive(value);
        go_Off?.SetActive(!value);

        var color = value ? color_On : color_Off;
        if (tmpu_Natives != null) foreach (var tmpu in tmpu_Natives) if (tmpu != null) tmpu.color = color;
        if (tmpu_Engs != null) foreach (var tmpu in tmpu_Engs) if (tmpu != null) tmpu.color = color;
    }

    private void OnClickSelect()
    {
        if (_Data == null) return;
        _Data.Touched();
    }
}