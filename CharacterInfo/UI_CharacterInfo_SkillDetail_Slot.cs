using AudioSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Debug = COA_DEBUG.Debug;

public class UI_CharacterInfo_SkillDetail_Slot : UIBaseBelongings
{
    [SerializeField] private UIBase_CharacterInfoWindow.SkillUI baseSkillUI;
    [SerializeField] private UI_TextSlider nameSlider;
    [SerializeField] private TextMeshProUGUI tmpu_CD;
    [SerializeField] private TMProHyperLink tmpu_Desc;
    [SerializeField] private Button btn_Range;

    private skillradiusTable radiusData;

    private void Awake()
    {
        UIUtil.ResetAndAddListener(btn_Range, OnClickRadiusButton);
        if (tmpu_Desc != null) tmpu_Desc.onClickLinkText = OnClickStatusEffectTxt;
    }

    public void SetSkillInfoUI(skillpresetTable spData, Data.Enum.Skill_Type type)
    {
        baseSkillUI?.SetSkillInfoUI(ownerUIBase, spData, type);
        if (spData == null) return;

        var index = spData._Enum_Id.IndexOf("_LV");
        var key = spData._Enum_Id.Substring(0, index);
        if (nameSlider != null) nameSlider.SetText(Localizer.GetLocalizedStringName(Localizer.SheetType.SKILL, key));
        var content = Localizer.GetLocalizedStringDesc(Localizer.SheetType.SKILL, key);
        if (tmpu_Desc != null) tmpu_Desc.text =
                string.Format(content,
                GetSkillDescArguments(spData));

        if (type == Data.Enum.Skill_Type.ULTIMATE && tmpu_CD != null) tmpu_CD.text = spData._CD.ToString("N0");
    }

    private void OnClickRadiusButton()
    {
        AudioManager._instance.PlaySound(DefineName.Audio_OutGame.Sound.BUTTON);
        UIMessageBox.OnelineMsg_NotReady();
    }

    private object[] GetSkillDescArguments(skillpresetTable spData)
    {
        int len = 10;
        object[] args = new object[len];

        args[0] = "0ec2cb";
        args[1] = "ff7f00";

        if (spData == null) return args;

        //int div = 3;

        //args[1] = spData._Range == 99 ? "전장 전체" : spData._Range.ToString();
        //args[2] = (spData._Trigger_Data._Activate_Chance * 100f).ToString("N0") + "%";

        //for (int i = 3; i < len; i += div)
        //{
        //    var spe = spData._Element_Data[i / div - 1];
        //    if (spe == null) continue;

        //}

        return args;
    }

    private void OnClickStatusEffectTxt(TMP_LinkInfo linkInfo)
    {
        var txtInfo = tmpu_Desc._TMPU.textInfo;
        var linkFirstCharInfo = txtInfo.characterInfo[linkInfo.linkTextfirstCharacterIndex];
        var linkLastCharInfo = txtInfo.characterInfo[linkInfo.linkTextfirstCharacterIndex + linkInfo.linkTextLength - 1];

        var linkId = linkInfo.GetLinkID();
        var tooltip = UIManager._instance.ShowUIBase<UIBase_Tooltip_SkillStatusEffect>(
            DefineName.UITooltip.SKILL_STATUS_EFFECT,
            new UIParam.Tooltip.SkillStatusEffect(Data._skilleffectsTable.GetDataFromTable(linkId)));
    }
}