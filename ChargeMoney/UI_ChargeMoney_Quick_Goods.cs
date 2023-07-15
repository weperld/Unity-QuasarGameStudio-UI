using AudioSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Debug = COA_DEBUG.Debug;

public class UI_ChargeMoney_Quick_Goods : UIBaseBelongings
{
    [SerializeField] private Image img_Icon;
    [SerializeField] private TextMeshProUGUI tmpu_ChargeValue;
    [SerializeField] private TextMeshProUGUI tmpu_CostValue;
    [SerializeField] private Button btn_Charge;

    public int chargeValue;
    public int costValue;
    public string monetaryUnit = "₩";

    private Data.Enum.Asset chargeType = Data.Enum.Asset.CRYSTAL;
    private void Awake()
    {
        UIUtil.ResetAndAddListener(btn_Charge, OnClickChargeButton);
    }

    private void OnValidate()
    {
        if (tmpu_ChargeValue != null) tmpu_ChargeValue.text = $"+{chargeValue:#,0}";
        if (tmpu_CostValue != null) tmpu_CostValue.text = $"{monetaryUnit}{costValue:#,0}";
    }

    public void SetData(Data.Enum.Asset assetType)
    {
        chargeType = assetType;
        var assetData = Data._assetTable.GetDataFromTable(a => a._CE_Asset == assetType);
        if (assetData == null) return;

        if (img_Icon != null)
        {
            img_Icon.sprite = assetData._Pictogram_Reference_Data?.GetSpriteFromSMTable(ownerUIBase);
        }
        if (tmpu_ChargeValue != null) tmpu_ChargeValue.text = $"+{chargeValue:#,0}";
        if (tmpu_CostValue != null) tmpu_CostValue.text = $"{monetaryUnit}{costValue:#,0}";
    }

    UIBase errorUI;
    private void OnClickChargeButton()
    {
        AudioManager._instance.PlaySound(DefineName.Audio_OutGame.Sound.BUTTON);

        var assetData = Data._assetTable.GetDataFromTable(a => a._CE_Asset == chargeType);
        if (assetData == null) return;

        User.CheatCreateAsset(assetData._Enum_Id, chargeValue,
            () =>
            {
                Debug.Log($"<color=#FF0000>{chargeType} 충전 성공");
            },
            error =>
            {
                errorUI = UIMessageBox.Confirm_Old("에셋 충전 치트 에러 발생",
                    $"에러 코드: {error}",
                    () => { errorUI?.Hide(); });
            });
    }
}