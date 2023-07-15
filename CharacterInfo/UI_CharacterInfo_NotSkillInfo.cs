using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using EnhancedUI.EnhancedScroller;
using Debug = COA_DEBUG.Debug;
using System.Security.Cryptography;

public class UI_CharacterInfo_NotSkillInfo : UIBaseBelongingEnhancedScrollerCellView
{
    [Serializable]
    private struct CharacterTypeUI
    {
        [SerializeField] private Image img_Icon;
        [SerializeField] private TextMeshProUGUI tmpu_TypeName;

        public void SetTypeUI<TEnum>(IAssetOwner assetOwner, character_type_resourceTable typeRscData, TEnum typeVal) where TEnum : Enum
        {
            if (img_Icon != null && typeRscData != null)
            {
                img_Icon.sprite = typeRscData._Image_Ref_Data[0]?.GetSpriteFromSMTable(assetOwner);
            }

            if (tmpu_TypeName != null)
            {
                tmpu_TypeName.text = Localizer.GetLocalizedStringName(Localizer.SheetType.NONE, typeVal.ToString());
            }
        }
    }
    [Serializable]
    private struct StatusUI
    {
        [SerializeField] private TextMeshProUGUI tmpu_Value;

        public void SetValue(float statValue)
        {
            if (tmpu_Value == null) return;
            tmpu_Value.text = statValue.ToString("N0");
        }
    }

    [SerializeField] private Image img_SDThumbnail;
    [SerializeField] private CharacterTypeUI typeUI_Class;
    [SerializeField] private CharacterTypeUI typeUI_Speices;
    [SerializeField] private CharacterTypeUI typeUI_Property;
    [SerializeField] private TextMeshProUGUI tmpu_CurrentLevel;
    [SerializeField] private TextMeshProUGUI tmpu_MaxLevel;
    [SerializeField] private Image img_ExpGauge;
    [SerializeField, Range(0f, 1f)] private float expFill_Min, expFill_Max;
    [SerializeField] private UI_GradeStar gradeStarGroup;
    [SerializeField] private TextMeshProUGUI tmpu_Name_Kr;
    [SerializeField] private TextMeshProUGUI tmpu_Name_En;
    [SerializeField] private StatusUI statUI_ATK;
    [SerializeField] private StatusUI statUI_DEF;
    [SerializeField] private StatusUI statUI_HP;

    private CharacterInfo _Info;

    private void OnEnable()
    {
        User.onChangeLanguage -= OnChangeLanguage;
        User.onChangeLanguage += OnChangeLanguage;
    }
    private void OnDisable()
    {
        User.onChangeLanguage -= OnChangeLanguage;
        if (_Info != null)
        {
            _Info.onUpdateChExp -= UpdateCharacterInfo;
        }
    }
    public void Set(CharacterInfo info)
    {
        if (_Info != null) _Info.onUpdateChExp -= UpdateCharacterInfo;

        _Info = info;
        var data = _Info?._Data;
        if (data == null) return;

        _Info.onUpdateChExp -= UpdateCharacterInfo;
        _Info.onUpdateChExp += UpdateCharacterInfo;
        UpdateCharacterInfo(info);

        gradeStarGroup?.SetStars((int)data._CE_Character_Grade);

        if (img_SDThumbnail != null) img_SDThumbnail.sprite = data._Resource_List_Data?._Thumbnail_SD_Reference_Data?.GetSpriteFromSMTable(ownerUIBase);

        OnChangeLanguage(User._Language);
    }
    private void UpdateCharacterInfo(CharacterInfo updated)
    {
        if (tmpu_CurrentLevel != null)
        {
            tmpu_CurrentLevel.text = updated._Level.ToString();
        }
        if (tmpu_MaxLevel != null)
        {
            tmpu_MaxLevel.text = Data._character_growthTable.Count.ToString();
        }
        if (img_ExpGauge != null)
        {
            var gaugeRatio = expFill_Max - expFill_Min;
            img_ExpGauge.fillAmount = expFill_Min + gaugeRatio * ((float)updated._CurrentExp / updated._RequireExp);
        }
        var stat = new CharacterStat(updated);
        if (stat != null)
        {
            statUI_ATK.SetValue(stat.GetStat(Data.Enum.Stat.ATK));
            statUI_DEF.SetValue(stat.GetStat(Data.Enum.Stat.DEF));
            statUI_HP.SetValue(stat.GetStat(Data.Enum.Stat.HP));
        }
    }
    private void OnChangeLanguage(Data.Enum.Language lang)
    {
        var data = _Info?._Data;
        if (data == null) return;

        var classType = data._CE_Character_Class;
        var rscData = Data._character_type_resourceTable.GetDataFromTable(classType.ToString());
        typeUI_Class.SetTypeUI(ownerUIBase, rscData, classType);
        var speciesType = data._CE_Character_Species;
        rscData = Data._character_type_resourceTable.GetDataFromTable(speciesType.ToString());
        typeUI_Speices.SetTypeUI(ownerUIBase, rscData, speciesType);
        var propertyType = data._CE_Character_Property;
        rscData = Data._character_type_resourceTable.GetDataFromTable(propertyType.ToString());
        typeUI_Property.SetTypeUI(ownerUIBase, rscData, propertyType);

        if (tmpu_Name_Kr != null) tmpu_Name_Kr.text = Localizer.GetLocalizedStringName(Localizer.SheetType.NONE, data._Enum_Id, lang);

        var isEng = lang == Data.Enum.Language.LANGUAGE_EN;
        if (tmpu_Name_En != null)
        {
            tmpu_Name_En.text = Localizer.GetLocalizedStringName(Localizer.SheetType.NONE, data._Enum_Id, Data.Enum.Language.LANGUAGE_EN, Localizer.StringCase.UPPER);
        }
    }
}