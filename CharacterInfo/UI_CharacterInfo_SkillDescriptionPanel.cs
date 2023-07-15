using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Debug = COA_DEBUG.Debug;

public class UI_CharacterInfo_SkillDescriptionPanel : MonoBehaviour
{
    #region Inspector
    [SerializeField] private Button btn_Close;
    [SerializeField] private UI_CharacterInfo_SkillDetail_Slot skillUI_ULT;
    [SerializeField] private UI_CharacterInfo_SkillDetail_Slot skillUI_ACT;
    [SerializeField] private UI_CharacterInfo_SkillDetail_Slot skillUI_BSC;
    #endregion

    private Action onClose;


    private void Awake()
    {
        UIUtil.ResetAndAddListener(btn_Close, OnClickCloseButton);
    }
    public void Setup(Action closeAction)
    {
        onClose = closeAction;
    }

    public void Show(characterskillsetTable skillSetData)
    {
        var active = skillSetData != null;
        SetActive(active);
        if (!active) return;

        var offset = "_LV1";
        skillpresetTable[] tmpSkillArr = new skillpresetTable[3] { null, null, null };
        for (int i = 0; i < skillSetData._CE_Skill_Type.Length; i++)
        {
            var type = skillSetData._CE_Skill_Type[i];
            var presetData = Data._skillpresetTable.GetDataFromTable(skillSetData._Skill[i] + offset);
            switch (type)
            {
                case Data.Enum.Skill_Type.ULTIMATE: tmpSkillArr[0] = presetData; break;
                case Data.Enum.Skill_Type.PASSIVE: tmpSkillArr[1] = presetData; break;
                case Data.Enum.Skill_Type.BASIC: tmpSkillArr[2] = presetData; break;
            }
        }
        skillUI_ULT.SetSkillInfoUI(tmpSkillArr[0], Data.Enum.Skill_Type.ULTIMATE);
        skillUI_ACT.SetSkillInfoUI(tmpSkillArr[1], Data.Enum.Skill_Type.PASSIVE);
        skillUI_BSC.SetSkillInfoUI(tmpSkillArr[2], Data.Enum.Skill_Type.BASIC);
    }
    public void SetActive(bool active)
    {
        gameObject.SetActive(active);
        if (!active) onClose?.Invoke();
    }

    private void OnClickCloseButton()
    {
        SetActive(false);
    }
}