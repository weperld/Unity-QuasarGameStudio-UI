using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Debug = COA_DEBUG.Debug;

public class UI_CharacterLevelup_StatusInfo : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI tmpu_StatName;
    [SerializeField] private TextMeshProUGUI tmpu_CurrentValue;
    [SerializeField] private GameObject go_IncreaseStat;
    [SerializeField] private TextMeshProUGUI tmpu_NextValue;
    [SerializeField] private TextMeshProUGUI tmpu_IncreaseValue;
    [SerializeField] private Color color_Basic = new Color(1f, 1f, 1f, 1f);
    [SerializeField] private Color color_Increase = new Color(1f, 1f, 1f, 1f);

    private Data.Enum.Stat stat;
    public void SetStatName(Data.Enum.Stat stat)
    {
        this.stat = stat;

        User.onChangeLanguage -= OnChangeLanguage;
        User.onChangeLanguage += OnChangeLanguage;
        OnChangeLanguage(User._Language);
    }
    private void OnDisable()
    {
        User.onChangeLanguage -= OnChangeLanguage;
    }

    public void SetCurValue(int val)
    {
        if (tmpu_CurrentValue != null) tmpu_CurrentValue.text = val.ToString("#,0");
    }

    public void SetActiveIncreaseStat(bool active)
    {
        go_IncreaseStat?.SetActive(active);
    }

    public void SetIncreaseValue(int curVal, int nextVal)
    {
        if (tmpu_NextValue != null)
        {
            tmpu_NextValue.text = nextVal.ToString("#,0");
            tmpu_NextValue.color = curVal == nextVal ? color_Basic : color_Increase;
        }
        if (tmpu_IncreaseValue != null)
        {
            var interval = nextVal - curVal;
            tmpu_IncreaseValue.gameObject.SetActive(interval > 0);
            tmpu_IncreaseValue.text = "+" + interval.ToString();
        }
    }

    private void OnChangeLanguage(Data.Enum.Language lang)
    {
        if (tmpu_StatName == null) return;
        tmpu_StatName.text = Localizer.GetLocalizedStringName(Localizer.SheetType.NONE, stat.ToString(), lang);
    }
}