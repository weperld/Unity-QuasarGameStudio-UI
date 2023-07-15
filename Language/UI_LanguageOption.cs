using AudioSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Debug = COA_DEBUG.Debug;

public class LanguageOptionCellData
{
    private bool select;

    public Data.Enum.Language optionLanguage;
    public bool _Select
    {
        get => select;
        set
        {
            select = value;
            onSelect?.Invoke(value);
        }
    }

    public Action<bool> onSelect;
    public Action<LanguageOptionCellData> onTouchAction;

    public void Touched()
    {
        AudioManager._instance.PlaySound(DefineName.Audio_OutGame.Sound.BUTTON);
        onTouchAction?.Invoke(this);
    }
}

public class UI_LanguageOption : MonoBehaviour
{
    public UI_LanguageOptionCell optionCell_Kr;
    public UI_LanguageOptionCell optionCell_En;

    public Button btn_Cancel;
    public Button btn_Apply;

    public GameObject go_LastCheck;
    public Button btn_LastCheck_Cancel;
    public Button btn_LastCheck_Apply;

    private List<LanguageOptionCellData> list_languageOptionData = new List<LanguageOptionCellData>();

    private Data.Enum.Language selectedLanguage;

    private void Awake()
    {
        UIUtil.ResetAndAddListener(btn_Apply, OnClickApply);
        UIUtil.ResetAndAddListener(btn_Cancel, OnClickCancel);
        UIUtil.ResetAndAddListener(btn_LastCheck_Apply, OnClickLastCheckApply);
        UIUtil.ResetAndAddListener(btn_LastCheck_Cancel, OnClickLastCheckCancel);
    }

    private void OnEnable()
    {
        list_languageOptionData?.Clear();

        for (int i = 0; i < (int)Data.Enum.Language.CNT; i++)
        {
            var lang = (Data.Enum.Language)i;
            list_languageOptionData.Add(
                new LanguageOptionCellData()
                {
                    _Select = User._Language == lang,
                    optionLanguage = lang,
                    onTouchAction = OnTouchLanguageOptionCell
                });
        }
        optionCell_Kr.Set(list_languageOptionData.Find(f => f.optionLanguage == Data.Enum.Language.LANGUAGE_KR));
        optionCell_En.Set(list_languageOptionData.Find(f => f.optionLanguage == Data.Enum.Language.LANGUAGE_EN));

        selectedLanguage = User._Language;
    }

    private void OnTouchLanguageOptionCell(LanguageOptionCellData touched)
    {
        foreach (var option in list_languageOptionData)
            option._Select = touched.optionLanguage == option.optionLanguage;
        selectedLanguage = touched.optionLanguage;
    }

    public void Show()
    {
        go_LastCheck?.SetActive(false);
        gameObject.SetActive(true);
    }

    private void OnClickCancel()
    {
        gameObject.SetActive(false);
    }
    private void OnClickApply()
    {
        AudioManager._instance.PlaySound(DefineName.Audio_OutGame.Sound.BUTTON);
        if (go_LastCheck == null) return;
        go_LastCheck.SetActive(true);
    }

    private void OnClickLastCheckCancel()
    {
        if (go_LastCheck == null) return;
        go_LastCheck.SetActive(false);
    }
    private void OnClickLastCheckApply()
    {
        AudioManager._instance.PlaySound(DefineName.Audio_OutGame.Sound.BUTTON);
        gameObject.SetActive(false);
        User._Language = selectedLanguage;
        User.SaveUserData();
    }
}