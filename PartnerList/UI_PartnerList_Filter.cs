using AudioSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Debug = COA_DEBUG.Debug;

public class UI_PartnerList_Filter : MonoBehaviour
{
    [SerializeField] private Button btn_Filter;
    [SerializeField] private GameObject go_On;
    [SerializeField] private GameObject go_Off;
    [SerializeField] private Data.Enum.Character_Grade[] grades = new Data.Enum.Character_Grade[] { Data.Enum.Character_Grade.GRADE_D };

    private Action<UI_PartnerList_Filter> onClickAction;

    private void Awake()
    {
        UIUtil.ResetAndAddListener(btn_Filter, OnClickFilterButton);
    }

    public void Set(Action<UI_PartnerList_Filter> onClickAction)
    {
        this.onClickAction = onClickAction;
    }

    public void SetOnOff(bool isOn)
    {
        go_On?.SetActive(isOn);
        go_Off?.SetActive(!isOn);
    }

    private void OnClickFilterButton()
    {
        AudioManager._instance.PlaySound(DefineName.Audio_OutGame.Sound.BUTTON);
        onClickAction?.Invoke(this);
    }

    public bool Contains(characterTable data)
    {
        if (data == null) return false;

        var grade = data._CE_Character_Grade;
        foreach (var v in grades)
            if (v == grade) return true;
        return false;
    }
}