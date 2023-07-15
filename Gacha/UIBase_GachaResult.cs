using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;
using TMPro;
using Debug = COA_DEBUG.Debug;
using static GachaHelper;
using AudioSystem;

public class UIBase_GachaResult : UIBase
{
    [SerializeField] private Button btn_Check;
    [SerializeField] private Button btn_Again;
    [SerializeField] private GameObject go_Filter;
    [SerializeField] private UI_Card_GachaResult_Eff[] card_GachaResult_Eff;
    [SerializeField] private UI_Card_GachaResult[] card_GachaResult;

    private GachaHelper.GachaTryActionData gachaTryActionData;

    private UIBase ui_OnLackOfMoney;
    private GachaHelper.GachaResultData gachaResultData;
    private int gacha_Result_Index = -1;

    private UIBase confirmUI;
    private UIBase gachaResultErrorUI;

    private void Awake()
    {
        gachaTryActionData = new GachaHelper.GachaTryActionData();
        gachaTryActionData[GachaTryResult.NO_AVAILABLE_GACHA] = GachaTryAction_OnNoAvailableGacha;
        gachaTryActionData[GachaTryResult.CHARACTER_COUNT_IS_OVER_GACHA_AVAIABLE_LIMIT] = GachaTryAction_OnGachaAvaliableLimit;
        gachaTryActionData[GachaTryResult.LACK_OF_MONEY] = GachaTryAction_OnLackOfMoney;
        gachaTryActionData[GachaTryResult.SUCCESS_COMMON] = GachaTryAction_OnSuccessCommon;
        gachaTryActionData[GachaTryResult.SUCCESS_OVER_CHARACTER_MAX] = GachaTyrAction_OnSuccessOverCharacterMax;
    }

    public override void Show(object param)
    {
        if (param == null)
            return;

        base.Show();
        gachaResultData = param as GachaHelper.GachaResultData;
        UIUtil.ResetAndAddListener(btn_Check, OnClickCheckBtn);

        if (!User.Info.isTutorial)
        {
            go_Filter.SetActive(false);
            UIUtil.ResetAndAddListener(btn_Again, OnClickAgainBtn);
        }
        else
            go_Filter.SetActive(true);

        for (int i = 0; i < gachaResultData.RewardToCharacterDatas().Length; i++)
        {
            card_GachaResult[i].SetData(gachaResultData.RewardToCharacterDatas()[i]);
            card_GachaResult_Eff[i].SetEff(gachaResultData.RewardToCharacterDatas()[i]._CE_Character_Grade);
        }

    }

    public override void Hide()
    {
        base.Hide();
    }

    private void OnClickCheckBtn()
    {
        AudioManager._instance.PlayBGM(DefineName.Audio_OutGame.BGM.LOBBY_BGM);
        if (User.Info.isTutorial)
            TutorialCtrl.Tutorial_Ctrl_Group();
        Hide();
    }

    private void OnClickAgainBtn()
    {
        AudioManager._instance.StopBGM();
        AudioManager._instance.PlayBGM(DefineName.Audio_OutGame.BGM.RECRUIT_BGM);

        GachaHelper.TryToGacha(gachaResultData.goodsInfo, gachaTryActionData, OnGachaBuyError);
        Hide();
    }

    #region Gacha Try Actions
    private void GachaTryAction_OnNoAvailableGacha(GachaResultData gachaResult)
    {
        UIMessageBox.OnelineMsg_NotAvailableGoods();
    }

    private void GachaTryAction_OnGachaAvaliableLimit(GachaResultData gachaResult)
    {
        //confirmUI = UIMessageBox.Confirm_Choice(
        //    "UI_Character_Over_200_NAME", null,
        //    "UI_Character_Over_200_DESC", null,
        //    () =>
        //    {
        //        confirmUI.Hide();
        //        UIManager._instance.ShowUIBase<UIBase>(
        //            DefineName.UICharacterInventory.INVENTORY,
        //            new InventoryType()
        //            {
        //                type = UIParam.Character_Inventory.UICharacterInventoryTypeParam.CharacterInventoryType.DISASSEMBLE,
        //            });
        //    },
        //    () =>
        //    {
        //        confirmUI.Hide();
        //    });
        UIMessageBox.OnelineMessage(Localizer.GetLocalizedStringDesc(Localizer.SheetType.NONE, "UI_Character_Over_200_Temp"), 1.5f);
    }

    private void GachaTryAction_OnLackOfMoney(GachaResultData gachaResult)
    {
        var goodsInfo = gachaResult.goodsInfo;
        if (goodsInfo.costType != Data.Enum.Common_Type.ASSET)
        {
            gachaResultErrorUI = UIMessageBox.Confirm_Old("충전 불가",
                "코스트의 타입이 에셋이 아님",
                () =>
                {
                    gachaResultErrorUI?.Hide();
                });
            return;
        }

        var costData = Data._assetTable.GetDataFromTable(goodsInfo.costEnumId);
        if (costData == null)
        {
            gachaResultErrorUI = UIMessageBox.Confirm_Old("에셋 테이블 데이터 에러",
                $"{this.name}, GachaTryAction_OnLackOfMoney",
                () =>
                {
                    gachaResultErrorUI?.Hide();
                });
            return;
        }

        var param = new UIParam.ChargeMoney.QuickParam(costData._CE_Asset);
        var costName = Localizer.GetLocalizedStringName(Localizer.SheetType.NONE, goodsInfo.costEnumId);
        ui_OnLackOfMoney = UIMessageBox.Confirm_Choice(
            "UI_Asset_Charge_NAME", new object[] { costName },
            "UI_Asset_Charge_DESC", new object[] { costName },
            () =>
            {
                ui_OnLackOfMoney?.Hide();
                OpenUI(DefineName.ChargeMoney.QUICK, param);
            },
            () => { ui_OnLackOfMoney?.Hide(); });
    }

    private void GachaTryAction_OnSuccessCommon(GachaResultData gachaResult)
    {
        Debug.Log($"가챠 {(gachaResult.goodsInfo._IsOnce ? 1 : 11)}회 뽑기 실행");

        gachaResultData = gachaResult;

        if (gachaResultData.goodsInfo._IsOnce)
        {
            if (gachaResultData.RewardToCharacterDatas()[0]._CE_Character_Grade >= Data.Enum.Character_Grade.GRADE_A)
                GachaDirection.Play(new UIParam.Gacha.GachaDirection
                    (this, gachaResultData.RewardToCharacterDatas()[0]._Enum_Id,
                    (reply) =>
                    {
                        UIManager._instance.ShowUIBase<UIBase_GachaAcquisition>(DefineName.Gacha.ACQUISITION,
                            new UIParam.Gacha.GachaDirection(this, gachaResultData.RewardToCharacterDatas()[0]._Enum_Id, null));
                    }));
            else
                UIManager._instance.ShowUIBase<UIBase_GachaAcquisition>(DefineName.Gacha.ACQUISITION,
                    new UIParam.Gacha.GachaDirection(this, gachaResultData.RewardToCharacterDatas()[0]._Enum_Id, null));
            return;
        }

        gacha_Result_Index = -1;
        UIManager._instance.ShowUIBase<UIBase_GachaResult_Intro>(DefineName.Gacha.INTRO,
            new UIParam.Gacha.GachaIntro(() => ShowGachaResult(), gachaResultData));
    }

    private void GachaTyrAction_OnSuccessOverCharacterMax(GachaResultData gachaResult)
    {
        UIMessageBox.OnelineMsg_OverCharacterAmount();
    }
    #endregion

    private void ShowGachaResult(bool isSkip = false)
    {
        gacha_Result_Index++;
        if (gacha_Result_Index >= gachaResultData.RewardToCharacterDatas().Length)
        {
            UIManager._instance.ShowUIBase<UIBase>(DefineName.Gacha.RESULT, gachaResultData);
            return;
        }


        if (isSkip)
        {
            bool trigger = false;
            for (int i = gacha_Result_Index; i < gachaResultData.RewardToCharacterDatas().Length; i++)
            {
                if (gachaResultData.RewardToCharacterDatas()[i]._CE_Character_Grade >= Data.Enum.Character_Grade.GRADE_A)
                {
                    trigger = true;
                    GachaDirection.Play(new UIParam.Gacha.GachaDirection(this, gachaResultData.RewardToCharacterDatas()[i]._Enum_Id,
                        (reply) =>
                        {
                            UIManager._instance.ShowUIBase<UIBase>(DefineName.Gacha.RESULT, gachaResultData);
                        }, true));
                    break;
                }
            }

            if (!trigger)
                UIManager._instance.ShowUIBase<UIBase>(DefineName.Gacha.RESULT, gachaResultData);
            return;
        }

        if (gachaResultData.RewardToCharacterDatas()[gacha_Result_Index]._CE_Character_Grade >= Data.Enum.Character_Grade.GRADE_A)
            GachaDirection.Play(new UIParam.Gacha.GachaDirection
                (this, gachaResultData.RewardToCharacterDatas()[gacha_Result_Index]._Enum_Id, ShowGachaResult));
        else
            UIManager._instance.ShowUIBase<UIBase_GachaAcquisition>(DefineName.Gacha.ACQUISITION,
                new UIParam.Gacha.GachaDirection(this, gachaResultData.RewardToCharacterDatas()[gacha_Result_Index]._Enum_Id, ShowGachaResult));
    }


    private void OnGachaBuyError(string errMsg)
    {
        gachaResultErrorUI = UIMessageBox.Confirm_Close("가챠 구매 에러 발생",
            $"ERROE CODE: [{errMsg}]",
            () =>
            {
                gachaResultErrorUI?.Hide();
            });
    }


    private void OpenUI(string key, object param)
    {
        UIManager._instance.ShowUIBase<UIBase>(key, param);
    }
}
