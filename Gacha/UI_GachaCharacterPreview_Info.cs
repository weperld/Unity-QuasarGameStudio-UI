using GrpcModel;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Debug = COA_DEBUG.Debug;

public class UI_GachaCharacterPreview_Info : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI tmpu_Grade;
    [SerializeField] private TextMeshProUGUI tmpu_Name;
    [SerializeField] private TextMeshProUGUI tmpu_SecondName;
    [SerializeField] private Button btn_Info;

    private characterTable data;

    private void Awake()
    {
        UIUtil.ResetAndAddListener(btn_Info, OnClickInfoButton);
    }

    private void OnClickInfoButton()
    {
        UIManager._instance.ShowUIBase<UIBase>(
            DefineName.CharacterCollection.UIBASE_BATTLE_INFO,
            new UIParam.CharacterCollection.BattleInfoSetupParam
            {
                characterEnumID = data._Enum_Id
            });
    }

    public void Set(characterTable data)
    {
        this.data = data;
        if (data == null) return;

        if (tmpu_Grade != null) tmpu_Grade.text = ((int)data._CE_Character_Grade + 1).ToString();
        if (tmpu_Name != null) tmpu_Name.text = Localizer.GetLocalizedStringName(Localizer.SheetType.NONE, data._Enum_Id);
        if (tmpu_SecondName != null) tmpu_SecondName.text = Localizer.GetLocalizedStringName(Localizer.SheetType.NONE, data._Second_Name);
    }

    public void SetActive(bool active)
    {
        gameObject.SetActive(active);
    }
}