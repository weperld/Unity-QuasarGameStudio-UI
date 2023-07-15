using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;
using TMPro;
using Debug = COA_DEBUG.Debug;
using EnhancedUI.EnhancedScroller;
using UnityEngine.Events;
using static GachaHelper;
using AudioSystem;

public class UIBase_GachaShop : UIBase, IEnhancedScrollerDelegate
{
    [Serializable]
    private struct PickupStackInfo
    {
        [SerializeField] private GameObject go_InfoRoot;
        [SerializeField] private TextMeshProUGUI tmpu_Stack;
        [SerializeField] private TextMeshProUGUI tmpu_MaxStack;

        public void SetActive(bool active)
        {
            if (go_InfoRoot == null)
                return;
            go_InfoRoot.SetActive(active);
        }
        public void SetCurrentStackText(string str)
        {
            if (tmpu_Stack == null)
                return;
            tmpu_Stack.text = str;
        }
        public void SetMaxStackText(string str)
        {
            if (tmpu_MaxStack == null)
                return;
            tmpu_MaxStack.text = str;
        }
    }
    [Serializable]
    private struct RemainTimeInfo
    {
        public enum Mode
        {
            NONE,
            REGULAR,
            PICKUP
        }

        [SerializeField] private GameObject go_None;
        [SerializeField] private GameObject go_Regular;
        [SerializeField] private GameObject go_Pickup;
        [SerializeField] private TextMeshProUGUI tmpu_Day;
        [SerializeField] private TextMeshProUGUI tmpu_Hour;
        [SerializeField] private TextMeshProUGUI tmpu_Minute;
        [SerializeField] private TextMeshProUGUI tmpu_Second;
        private Mode mode;

        public void SetMode(Mode mode)
        {
            this.mode = mode;
            go_None?.SetActive(mode == Mode.NONE);
            go_Regular?.SetActive(mode == Mode.REGULAR);
            go_Pickup?.SetActive(mode == Mode.PICKUP);
        }

        public void SetRemainTimeText(TimeSpan remainTime)
        {
            if (mode != Mode.PICKUP)
                return;

            int d = (int)Math.Floor(remainTime.TotalDays);
            int h = remainTime.Hours;
            int m = remainTime.Minutes;
            int s = remainTime.Seconds;

            if (tmpu_Day != null)
                tmpu_Day.text = d.ToString();
            if (tmpu_Hour != null)
                tmpu_Hour.text = h.ToString("D2");
            if (tmpu_Minute != null)
                tmpu_Minute.text = m.ToString("D2");
            if (tmpu_Second != null)
                tmpu_Second.text = s.ToString("D2");
        }
    }
    [Serializable]
    private struct GachaButtonInfo
    {
        [SerializeField] private Button btn_Gacha;
        [SerializeField] private Image img_Icon;
        [SerializeField] private TextMeshProUGUI tmpu_Cost;

        public void SetActive(bool active)
        {
            if (btn_Gacha == null)
                return;
            btn_Gacha.gameObject.SetActive(active);
        }

        public void SetListener(UnityAction listener)
        {
            if (User.Info.isTutorial)
                return;
            UIUtil.ResetAndAddListener(btn_Gacha, listener);
        }

        public void SetButtonData(uint cost, Sprite sprite)
        {
            if (tmpu_Cost != null)
                tmpu_Cost.text = cost.ToString("#,0");
            if (img_Icon != null)
                img_Icon.sprite = sprite;
        }
    }

    #region Inspector
    [Header("Common UI")]
    [SerializeField] private Button btn_Back;
    [SerializeField] private Button btn_Home;
    [SerializeField] private Button btn_Detail;

    [Space(20f)]
    [Header("Main UI")]
    [SerializeField] private EnhancedScroller scroller;
    [SerializeField] private UI_GachaSlot slotPrefab;
    [SerializeField] private UI_GachaCharacterPreview_WithBg[] characterPreviews;
    [SerializeField] private Image[] img_Colorables;
    [SerializeField] private GameObject go_EmotionRoot;
    [SerializeField] private Image[] img_Emoticons;
    [SerializeField] private TextMeshProUGUI tmpu_PickupCharacterTitle;
    [SerializeField] private TextMeshProUGUI tmpu_GachaName;
    [SerializeField] private TextMeshProUGUI tmpu_GachaSimpleDesc;
    [SerializeField] private TextMeshProUGUI tmpu_Career;
    [SerializeField] private TextMeshProUGUI tmpu_Habit;
    [SerializeField] private Animation anim_Main;

    [Space(20f)]
    [Header("Bottom UI")]
    [SerializeField] private RemainTimeInfo remainTimeInfo;
    [SerializeField] private Button btn_GachaRecord;
    [SerializeField] private GachaButtonInfo btn_Gacha_Once;
    [SerializeField] private GachaButtonInfo btn_Gacha_ElevenTimes;
    [SerializeField] private PickupStackInfo stackInfo_Percentage;
    [SerializeField] private PickupStackInfo stackInfo_Pickup;

    [Space(20f)]
    [Header("Gacha Detail UI")]
    [SerializeField] private UI_GachaDetailInfoWindow gachaDetailInfo;


    #endregion

    #region Variables
    private GachaHelper.SlotScrollData selectedSlot = null;
    private List<GachaHelper.SlotScrollData> list_SlotData = new List<GachaHelper.SlotScrollData>();

    private int gacha_Result_Index = -1;
    private GachaResultData gachaResultData;

    private GachaHelper.GachaTryActionData gachaTryActionData;

    private UIBase confirmChoiceUI;
    private UIBase gachaShopErrorUI;
    #endregion

    #region Base Override Methods
    private void Awake()
    {
        if (scroller != null)
            scroller.Delegate = this;


        UIUtil.ResetAndAddListener(btn_Back, OnClickBackButton);
        UIUtil.ResetAndAddListener(btn_Home, OnClickHomeButton);
        UIUtil.ResetAndAddListener(btn_Detail, OnClickDetailButton);
        UIUtil.ResetAndAddListener(btn_GachaRecord, OnClickRecordButton);
        btn_Gacha_Once.SetListener(OnClickGachaOnceButton);
        btn_Gacha_ElevenTimes.SetListener(OnClickGacha11TimesButton);

        gachaTryActionData = new GachaHelper.GachaTryActionData();
        gachaTryActionData[GachaTryResult.NO_SELECTED_SLOT] = GachaTryAction_OnNoSelectedSlot;
        gachaTryActionData[GachaTryResult.NO_AVAILABLE_GACHA] = GachaTryAction_OnNoAvailableGacha;
        gachaTryActionData[GachaTryResult.CHARACTER_COUNT_IS_OVER_GACHA_AVAIABLE_LIMIT] = GachaTryAction_OnGachaAvaliableLimit;
        gachaTryActionData[GachaTryResult.LACK_OF_MONEY] = GachaTryAction_OnLackOfMoney;
        gachaTryActionData[GachaTryResult.SUCCESS_COMMON] = GachaTryAction_OnSuccessCommon;
        gachaTryActionData[GachaTryResult.SUCCESS_OVER_CHARACTER_MAX] = GachaTyrAction_OnSuccessOverCharacterMax;
    }
    private void OnEnable()
    {
        GachaHelper.refreshGachaShop -= RefreshShop;
        GachaHelper.refreshGachaShop += RefreshShop;

        GameManager._instance.OnTimerEvent -= OnTimer;
        GameManager._instance.OnTimerEvent += OnTimer;
    }
    private void OnDisable()
    {
        list_SlotData.Clear();
        OnSelectSlot(null);

        if (User._ShopInfos.TryGetValue(Data.Enum.Shop_Type.GACHA, out var shopInfo))
        {
            foreach (var category in shopInfo._Categorys)
            {
                if (category == null)
                    continue;
                var tmp = (category as GachaCategoryInfo)._GachaGoodsInfo_Once;
                if (tmp == null)
                    continue;

                tmp.onExhaustRemainTime -= OnExhaustGachaRemainTime;
            }
        }

        GachaHelper.refreshGachaShop -= RefreshShop;

        if (!GameManager.IsDetroying)
            GameManager._instance.OnTimerEvent -= OnTimer;
    }

    public override void Show(object param = null)
    {
        User.GetShop(Data.Enum.Shop_Type.GACHA,
            () =>
            {
                list_SlotData.Clear();
                var shopInfo = User._ShopInfos[Data.Enum.Shop_Type.GACHA];
                int[] categoryIds = new int[shopInfo._Categorys.Count];
                for (int i = 0; i < categoryIds.Length; i++)
                    categoryIds[i] = shopInfo._Categorys[i].id;

                User.GetShopGoods(Data.Enum.Shop_Type.GACHA, categoryIds,
                    () =>
                    {
                        foreach (var v in User._ShopInfos[Data.Enum.Shop_Type.GACHA]._Categorys)
                            list_SlotData.Add(new GachaHelper.SlotScrollData(v as GachaCategoryInfo));

                        GachaHelper.SlotScrollData enabledSlot = null;
                        foreach (var v in list_SlotData)
                        {
                            v.onSelection = OnSelectSlot;
                            var tmp = v._CategoryInfo?._GachaGoodsInfo_Once;
                            if (tmp == null)
                                continue;

                            tmp.onExhaustRemainTime -= OnExhaustGachaRemainTime;
                            tmp.onExhaustRemainTime += OnExhaustGachaRemainTime;

                            if (enabledSlot == null && tmp._Data != null)
                                enabledSlot = v;
                        }

                        if (enabledSlot != null)
                        {
                            enabledSlot.Select();
                            scroller.JumpToDataIndex(list_SlotData.FindIndex(a => a == enabledSlot));
                        }
                        else
                            OnSelectSlot(null);

                        scroller.ReloadData();

                        base.Show();

                        if (param != null)
                        {
                            var paramType = param.GetType();
                            if (paramType == typeof(UIParam.Common.GachaShopParam))
                            {
                                var cast = param as UIParam.Common.GachaShopParam;
                                cast.action?.Invoke();
                            }

                            else if (paramType == typeof(UIParam.Gacha.GachaShopFromBanner))
                            {
                                var cast = param as UIParam.Gacha.GachaShopFromBanner;
                                var slot = cast.slotNumber;

                                if (list_SlotData.Count > slot)
                                {
                                    var selected = list_SlotData[slot];
                                    if (selected != null)
                                    {
                                        selected.Select();
                                        scroller.JumpToDataIndex(slot);
                                    }
                                }
                            }
                        }

                        anim_Main.Play();
                    },
                    error =>
                    {
                        gachaShopErrorUI = UIMessageBox.Confirm_Old("가챠 상점 오픈, 상품 정보 로드 요청 에러",
                            $"에러 코드: [{error}]",
                            () =>
                            {
                                gachaShopErrorUI?.Hide();
                                Hide();
                            });
                    });
            },
            error =>
            {
                gachaShopErrorUI = UIMessageBox.Confirm_Old("가챠 상점 정보 로드 요청 에러",
                    $"에러 코드: {error}",
                    () =>
                    {
                        gachaShopErrorUI?.Hide();
                        Hide();
                    });
            });
    }

    public override void Hide()
    {
        base.Hide();
    }

    #endregion

    #region Scroller Delegate
    public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
    {
        var cell = scroller.GetCellView(slotPrefab) as UI_GachaSlot;
        cell.SetScrollData(list_SlotData[dataIndex]);
        return cell;
    }

    public float GetCellViewSize(EnhancedScroller scroller, int dataIndex)
    {
        return slotPrefab == null ? 0f : list_SlotData[dataIndex]._Selection ? GachaHelper.SlotSize.Shop.On.HEIGHT : GachaHelper.SlotSize.Shop.Off.HEIGHT;
    }

    public int GetNumberOfCells(EnhancedScroller scroller)
    {
        return list_SlotData.Count;
    }
    #endregion

    #region Listener
    private void OnTimer()
    {
        foreach (var v in list_SlotData)
        {
            v?._CategoryInfo?._GachaGoodsInfo_Once?.UpdateRemainTime();
        }
    }

    private void OnClickBackButton()
    {
        GameManager._instance.ChangeScene(DefineName.Scene.LOBBY_SCENE);
        //Hide();
    }
    private void OnClickHomeButton()
    {
        GameManager._instance.ChangeScene(DefineName.Scene.LOBBY_SCENE);
        //UIManager._instance.HideAll();
    }
    private void OnClickDetailButton()
    {
        if (gachaDetailInfo == null || selectedSlot == null || User.Info.isTutorial)
            return;
        gachaDetailInfo.SetupAndShow(list_SlotData.FindIndex(a => a == selectedSlot));
    }

    private void OnClickRecordButton()
    {
        if (User.Info.isTutorial)
            return;
        UIManager._instance.ShowUIBase<UIBase>(DefineName.Gacha.RECORD);
    }
    private void OnClickGachaOnceButton()
    {
        if (User.Info.isTutorial)
            return;
        TryToGacha(true);
    }
    private void OnClickGacha11TimesButton()
    {
        if (User.Info.isTutorial)
            return;
        TryToGacha(false);
    }

    private void OnSelectSlot(GachaHelper.SlotScrollData select)
    {
        if (select == null)
        {
            ChangeSelectedSlot(null);
            return;
        }

        if (select._CategoryInfo._GachaGoodsInfo_Once == null)
            UIMessageBox.OnelineMsg_NotAvailableGoods();
        else
            ChangeSelectedSlot(select);
    }

    private void OnUpdateSlotRemainTime(TimeSpan remainTime)
    {
        remainTimeInfo.SetRemainTimeText(remainTime);

        if (remainTime <= TimeSpan.Zero)
        {
            User.GetShopGoods(Data.Enum.Shop_Type.GACHA, new int[] { selectedSlot._CategoryInfo.id },
                () =>
                {
                    ReloadGachaInfoUI();
                },
                error =>
                {
                    gachaShopErrorUI = UIMessageBox.Confirm_Old("가챠 상품 정보 로드 요청 실패",
                        $"가챠 상품 이용 가능 시간 소진, 정보 재로드, 에러 코드: {error}",
                        () =>
                        {
                            gachaShopErrorUI?.Hide();
                            Hide();
                        });
                });
        }
    }
    private void OnExhaustGachaRemainTime(int categoryId, ShopGoodsInfo.RequestSendingInfo requestSendingInfo)
    {
        if (requestSendingInfo.isSent)
            return;

        requestSendingInfo.isSent = true;
        User.GetShopGoods(Data.Enum.Shop_Type.GACHA, new int[] { categoryId },
            () =>
            {
                requestSendingInfo.isSent = false;
                scroller.ReloadData();

                var info = selectedSlot?._CategoryInfo;
                if (info == null || info.id != categoryId)
                    return;

                ReloadGachaInfoUI();
            },
            error =>
            {
                requestSendingInfo.isSent = false;
                gachaShopErrorUI = UIMessageBox.Confirm_Old($"{categoryId} 남은 시간 고갈, 상품 정보 재요청 에러",
                    $"ERROR CODE: [{error}]",
                    () =>
                    {
                        gachaShopErrorUI?.Hide();
                        Hide();
                    });
            });

    }

    #region Gacha Try Actions
    private void GachaTryAction_OnNoSelectedSlot(GachaResultData gachaResult)
    {
        UIMessageBox.OnelineMessage(Localizer.GetLocal("UI_Oneline_Gacha_Not_Selected_DESC", User._Language), 1f);
    }
    private void GachaTryAction_OnNoAvailableGacha(GachaResultData gachaResult)
    {
        UIMessageBox.OnelineMsg_NotAvailableGoods();
    }
    private void GachaTryAction_OnGachaAvaliableLimit(GachaResultData gachaResult)
    {
        //confirmChoiceUI = UIMessageBox.Confirm_Choice(
        //    "UI_Character_Over_200_NAME", null,
        //    "UI_Character_Over_200_DESC", null,
        //    () =>
        //    {
        //        confirmChoiceUI.Hide();
        //        UIManager._instance.ShowUIBase<UIBase>(
        //            DefineName.UICharacterInventory.INVENTORY,
        //            new InventoryType()
        //            {
        //                type = UIParam.Character_Inventory.UICharacterInventoryTypeParam.CharacterInventoryType.DISASSEMBLE,
        //            });
        //    },
        //    () =>
        //    {
        //        confirmChoiceUI.Hide();
        //    });
        UIMessageBox.OnelineMessage(Localizer.GetLocalizedStringDesc(Localizer.SheetType.NONE, "UI_Character_Over_200_Temp"), 1.5f);
    }
    private void GachaTryAction_OnLackOfMoney(GachaResultData gachaResult)
    {
        var goodsInfo = gachaResult.goodsInfo;

        if (goodsInfo.costType != Data.Enum.Common_Type.ASSET)
        {
            gachaShopErrorUI = UIMessageBox.Confirm_Old("충전 불가",
                "코스트의 타입이 에셋이 아님",
                () => { gachaShopErrorUI?.Hide(); });
            return;
        }

        var costData = Data._assetTable.GetDataFromTable(goodsInfo.costEnumId);
        if (costData == null)
        {
            gachaShopErrorUI =  UIMessageBox.Confirm_Old("에셋 테이블 데이터 에러",
                $"{this.name}, GachaTryAction_OnLackOfMoney",
                () => { gachaShopErrorUI?.Hide(); });
            return;
        }

        var param = new UIParam.ChargeMoney.QuickParam(costData._CE_Asset);
        var costName = Localizer.GetLocalizedStringName(Localizer.SheetType.NONE, goodsInfo.costEnumId);
        confirmChoiceUI = UIMessageBox.Confirm_Choice(
            "UI_Asset_Charge_NAME", new object[] { costName },
            "UI_Asset_Charge_DESC", new object[] { costName },
            () =>
            {
                confirmChoiceUI?.Hide();
                OpenUI(DefineName.ChargeMoney.QUICK, param);
            },
            () => { confirmChoiceUI?.Hide(); });
    }
    private void GachaTyrAction_OnSuccessOverCharacterMax(GachaResultData gachaResult)
    {
        UIMessageBox.OnelineMsg_OverCharacterAmount();
    }
    private void GachaTryAction_OnSuccessCommon(GachaResultData gachaResult)
    {
        //var asyncWork = AsyncWorkManager._instance.CreateAsyncWork();
        Debug.Log($"가챠 {(gachaResult.goodsInfo._IsOnce ? 1 : 11)}회 뽑기 실행");

        gachaResultData = gachaResult;

        if (gachaResultData.goodsInfo._IsOnce)
        {
            if (gachaResultData.RewardToCharacterDatas()[0]._CE_Character_Grade >= Data.Enum.Character_Grade.GRADE_A)
                GachaDirection.Play(new UIParam.Gacha.GachaDirection
                    (this,
                    gachaResultData.RewardToCharacterDatas()[0]._Enum_Id,
                    (reply) =>
                    {
                        UIManager._instance.ShowUIBase<UIBase_GachaAcquisition>(DefineName.Gacha.ACQUISITION,
                            new UIParam.Gacha.GachaDirection(this, gachaResultData.RewardToCharacterDatas()[0]._Enum_Id, null));
                        //(reply) =>
                        //{
                        //    //Hide();
                        //    //UIManager._instance.ShowUIBase<UIBase>(DefineName.Gacha.REWARDRESULT, gachaResultData);
                        //}));
                    }));
            else
                UIManager._instance.ShowUIBase<UIBase_GachaAcquisition>(DefineName.Gacha.ACQUISITION,
                    new UIParam.Gacha.GachaDirection(this, gachaResultData.RewardToCharacterDatas()[0]._Enum_Id, null));
            //(reply) =>
            //        {
            //            //Hide();
            //            //UIManager._instance.ShowUIBase<UIBase>(DefineName.Gacha.REWARDRESULT, gachaResultData);
            //        }));
            return;
        }

        gacha_Result_Index = -1;
        UIManager._instance.ShowUIBase<UIBase_GachaResult_Intro>(DefineName.Gacha.INTRO,
            new UIParam.Gacha.GachaIntro(() => ShowGachaResult(), gachaResultData));

        //ShowGachaResult();
        //for (int i = 0; i < gachaResult.RewardToCharacterDatas().Length; i++)
        //{
        //    if ((int)gachaResult.RewardToCharacterDatas()[i]._CE_Character_Grade == 4)
        //    {
        //        GachaDirection.Play(gachaResult.RewardToCharacterDatas()[i]._Enum_Id);
        //        //break;
        //        //asyncWork.EnqueueAction_GachaProduction(gachaResult.RewardToCharacterDatas()[i], () =>
        //        //{
        //        //    asyncWork.EnqueueAction_ShowRewardCharacter(gachaResult.GetOnlyAcqusitionShowableRewardList());
        //        //    asyncWork.EnqueueAction_ShowGachaResult(gachaResult.goodsInfo, gachaResult.resultRewardList);
        //        //});
        //        //break;
        //    }
        //}

        //asyncWork.EnqueueAction_GachaProduction(gachaResult.RewardToCharacterDatas()[0], direction);

        //asyncWork.EnqueueAction_ShowGachaResult(gachaResult.goodsInfo, gachaResult.resultRewardList);
        //asyncWork.EnqueueAction_ShowRewardCharacter(gachaResult.GetOnlyAcqusitionShowableRewardList());
    }
    #endregion

    public void Tutorial_Gacha_Result(List<UIParam.Common.Reward> list_Reward)
    {
        var asyncWork = AsyncWorkManager._instance.CreateAsyncWork();
        Debug.Log("튜토리얼 가챠 실행");

        gachaResultData = new GachaResultData(null, list_Reward, true);

        gacha_Result_Index = -1;
        UIManager._instance.ShowUIBase<UIBase_GachaResult_Intro>(DefineName.Gacha.INTRO,
            new UIParam.Gacha.GachaIntro(() => ShowTutorialGacha(), gachaResultData));
        //ShowTutorialGacha();
    }

    private void ShowTutorialGacha(bool isSkip = false)
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
                (this, gachaResultData.RewardToCharacterDatas()[gacha_Result_Index]._Enum_Id, ShowTutorialGacha));
        else
            UIManager._instance.ShowUIBase<UIBase_GachaAcquisition>(DefineName.Gacha.ACQUISITION,
                new UIParam.Gacha.GachaDirection(this, gachaResultData.RewardToCharacterDatas()[gacha_Result_Index]._Enum_Id, ShowTutorialGacha));
    }


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
                            UIManager._instance.ShowUIBase<UIBase_GachaResult>(DefineName.Gacha.RESULT, gachaResultData);
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

    private void OnErrorGachaBuy(string errMsg)
    {
        gachaShopErrorUI = UIMessageBox.Confirm_Close("가챠 구매 에러 발생",
            $"ERROE CODE: [{errMsg}]",
            () =>
            {
                gachaShopErrorUI?.Hide();
            });
    }

    UIBase errorUI;
    private void RefreshShop()
    {
        User.GetShopGoods(Data.Enum.Shop_Type.GACHA, new int[] { selectedSlot._CategoryInfo.id },
            () =>
            {
                scroller.ReloadData();
                ReloadGachaInfoUI();
            },
            error =>
            {
                errorUI = UIMessageBox.Confirm_Close("가챠 상품 정보 재요청 실패",
                    $"ERROR CODE: [{error}]",
                    () =>
                    {
                        Hide();
                        errorUI.Hide();
                    });
            });
    }
    #endregion

    /// <summary>
    /// 파라미터가 null이거나, null이 아니면서 해당 슬롯의 가챠 상품 정보가 존재하면 실행<para/>
    /// 즉, 슬롯 선택 정보가 바뀌는 상황이 맞을 때
    /// </summary>
    /// <param name="slotData"></param>
    private void ChangeSelectedSlot(GachaHelper.SlotScrollData slotData)
    {
        if (slotData != null && slotData._CategoryInfo?._GachaGoodsInfo_Once == null)
            return;

        if (selectedSlot != null)
        {
            selectedSlot._CategoryInfo._GachaGoodsInfo_Once.onUpdateRemainTime -= OnUpdateSlotRemainTime;
            selectedSlot._Selection = false;
        }

        btn_Gacha_Once.SetActive(slotData != null);
        btn_Gacha_ElevenTimes.SetActive(slotData != null);

        selectedSlot = slotData;
        if (selectedSlot != null)
            selectedSlot._Selection = true;

        if (selectedSlot != null
            && selectedSlot._CategoryInfo._GachaGoodsInfo_Once._IsPickupGacha
            && selectedSlot._CategoryInfo._GachaGoodsInfo_Once._RemainTime <= TimeSpan.Zero)
            RefreshShop();
        else
            ReloadGachaInfoUI();
    }
    private void ReloadGachaInfoUI()
    {
        var categoryInfo = selectedSlot?._CategoryInfo;
        var goodsInfo_1 = categoryInfo?._GachaGoodsInfo_Once;
        var goodsInfo_11 = categoryInfo?._GachaGoodsInfo_Eleven;
        var gachaData = goodsInfo_1?._Data;
        if (gachaData == null)
            return;

        bool isInputNullStr = goodsInfo_1 == null || !goodsInfo_1._IsPickupGacha;

        var mainCharacter = gachaData._Character_Show_Data[0];
        if (tmpu_PickupCharacterTitle != null)
            tmpu_PickupCharacterTitle.text = Localizer.GetLocalizedStringName(Localizer.SheetType.NONE, mainCharacter?._Enum_Id, Data.Enum.Language.LANGUAGE_EN).ToUpper();
        if (tmpu_Career != null)
            tmpu_Career.text = Localizer.GetLocalizedStringName(Localizer.SheetType.LIBRARY, mainCharacter?._Profile_Data?._Career);
        if (tmpu_Habit != null)
            tmpu_Habit.text = Localizer.GetLocalizedStringName(Localizer.SheetType.LIBRARY, mainCharacter?._Profile_Data._Habit);

        isInputNullStr = true;
        go_EmotionRoot?.SetActive(!isInputNullStr);
        if (!isInputNullStr && img_Emoticons != null)
        {
            for (int i = 0; i < img_Emoticons.Length; i++)
            {
                var img = img_Emoticons[i];
                if (img == null)
                    continue;

                img.sprite = mainCharacter?._Profile_Data?._Profile_Resource_Data?._Face_Image_Reference_Data[i]?.GetSpriteFromSMTable(this);
            }
        }

        if (selectedSlot == null)
        {
            if (tmpu_GachaName != null)
                tmpu_GachaName.text = "선택된 슬롯 없음";
            if (tmpu_GachaSimpleDesc != null)
                tmpu_GachaSimpleDesc.text = "가챠 슬롯을 먼저 선택해 주세요!";
            if (characterPreviews != null)
                for (int i = 0; i < characterPreviews.Length; i++)
                    characterPreviews[i]?.Set(null, false);

            remainTimeInfo.SetMode(RemainTimeInfo.Mode.NONE);
            stackInfo_Percentage.SetActive(false);
            stackInfo_Pickup.SetActive(false);

            scroller.IgnoreLoopJump(true);
            scroller.ReloadData();
            scroller.IgnoreLoopJump(false);

            return;
        }

        var assetData_1 = Data._assetTable.GetDataFromTable(goodsInfo_1.costEnumId);
        var assetData_11 = Data._assetTable.GetDataFromTable(goodsInfo_11.costEnumId);
        var sprite_1 = assetData_1?._Pictogram_Reference_Data?.GetSpriteFromSMTable(this);
        var sprite_11 = assetData_11?._Pictogram_Reference_Data?.GetSpriteFromSMTable(this);
        btn_Gacha_Once.SetButtonData((uint)goodsInfo_1.cost, sprite_1);
        btn_Gacha_ElevenTimes.SetButtonData((uint)goodsInfo_11.cost, sprite_11);

        if (img_Colorables != null && ColorUtility.TryParseHtmlString("#" + gachaData._Gacha_Resource_Data._Name_Color, out var htmlColor))
            foreach (var img in img_Colorables)
                if (img != null)
                    img.color = htmlColor;

        if (characterPreviews != null)
        {
            var showDatas = gachaData?._Character_Show_Data;
            for (int i = 0; i < characterPreviews.Length; i++)
            {
                var data = showDatas[i];
                var preview = characterPreviews[i];
                preview?.Set(data, false);
            }
        }

        if (tmpu_GachaName != null)
            tmpu_GachaName.text = string.Format(Localizer.GetLocalizedStringName(Localizer.SheetType.GACHA, gachaData._Enum_Id), GachaHelper.GetGachaNameArguments(gachaData, tmpu_GachaName.fontSize));
        if (tmpu_GachaSimpleDesc != null)
            tmpu_GachaSimpleDesc.text = string.Format(Localizer.GetLocalizedStringDesc(Localizer.SheetType.GACHA, gachaData._Enum_Id), GachaHelper.GetGachaDescriptionArguments(gachaData, tmpu_GachaSimpleDesc.fontSize));

        var isPickupGacha = goodsInfo_1 == null ? false : goodsInfo_1._IsPickupGacha;
        remainTimeInfo.SetMode(isPickupGacha ? RemainTimeInfo.Mode.PICKUP : RemainTimeInfo.Mode.REGULAR);
        stackInfo_Percentage.SetActive(true);
        var prob = GachaSimulHelper.GetAdditiveCorrectionProbTotalValue(categoryInfo.gachaTryedCount + 1, goodsInfo_1._StackModel);
        prob += gachaData._Gacha_Class_Data._Prob[(int)Data.Enum.Gacha_Pool_Grade.A];
        stackInfo_Percentage.SetCurrentStackText($"{GachaHelper.RoundAtSpecifiedDecimalPlace(3, prob * 100f):N2}");
        stackInfo_Pickup.SetActive(isPickupGacha);
        if (isPickupGacha)
        {
            remainTimeInfo.SetRemainTimeText(goodsInfo_1._RemainTime);
            stackInfo_Pickup.SetCurrentStackText(categoryInfo.currentPickupStack.ToString());
            stackInfo_Pickup.SetMaxStackText(goodsInfo_1.maxPickupStack.ToString());

            goodsInfo_1.onUpdateRemainTime -= OnUpdateSlotRemainTime;
            goodsInfo_1.onUpdateRemainTime += OnUpdateSlotRemainTime;
        }

        scroller.IgnoreLoopJump(true);
        scroller.ReloadData();
        scroller.IgnoreLoopJump(false);
    }

    private void TryToGacha(bool isOnce)
    {
        AudioManager._instance.PlayBGM(DefineName.Audio_OutGame.BGM.RECRUIT_BGM);
        GachaHelper.TryToGacha(isOnce
            ? selectedSlot?._CategoryInfo?._GachaGoodsInfo_Once
            : selectedSlot?._CategoryInfo?._GachaGoodsInfo_Eleven,
            gachaTryActionData,
            OnErrorGachaBuy);
    }

    private void OpenUI(string key, object param = null)
    {
        UIManager._instance.ShowUIBase<UIBase>(key, param);
    }

    private void End_Anim()
    {
        if (User.Info.isTutorial)
            TutorialCtrl.base_Tutorial.ShowTutorialBtn();
    }
}
