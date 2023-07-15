using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using Debug = COA_DEBUG.Debug;

public static class GachaHelper
{
    private static int gachaRandomSeed = int.MinValue;
    public static int _GachaRandomSeed
    {
        get
        {
            try
            {
                gachaRandomSeed = checked(gachaRandomSeed + 1);
            }
            catch
            {
                gachaRandomSeed = int.MinValue;
            }
            return gachaRandomSeed;
        }
    }

    public static class SlotSize
    {
        public static class Shop
        {
            public static class On
            {
                public const float WIDTH = 635f;
                public const float HEIGHT = 255f;

                public static Vector2 _ToVector2 => new Vector2(WIDTH, HEIGHT);
            }
            public static class Off
            {
                public const float WIDTH = 600f;
                public const float HEIGHT = 180f;

                public static Vector2 _ToVector2 => new Vector2(WIDTH, HEIGHT);
            }
        }
        public static class Detail
        {
            public static class On
            {
                public const float WIDTH = 460f;
                public const float HEIGHT = 205f;

                public static Vector2 _ToVector2 => new Vector2(WIDTH, HEIGHT);
            }
            public static class Off
            {
                public const float WIDTH = 425f;
                public const float HEIGHT = 130f;

                public static Vector2 _ToVector2 => new Vector2(WIDTH, HEIGHT);
            }
        }
    }

    public class SlotScrollData
    {
        public GachaCategoryInfo _CategoryInfo { get; private set; } = null;
        private bool selection;
        public Action<bool> onUpdateSelection;
        public Action<SlotScrollData> onSelection;

        public bool _Selection
        {
            get => selection;
            set
            {
                selection = value;
                onUpdateSelection?.Invoke(value);
            }
        }

        public SlotScrollData(GachaCategoryInfo info)
        {
            _CategoryInfo = info;
            _Selection = false;
        }

        public void Select()
        {
            onSelection?.Invoke(this);
        }
    }


    public static object[] GetGachaNameArguments(gachaTable data, float originFontSize)
    {
        if (data == null)
            return null;
        object[] args = new object[2];

        var gachaRscData = data._Gacha_Resource_Data;
        args[0] = gachaRscData != null ? gachaRscData._Name_Color : "";
        args[1] = "110%";

        return args;
    }
    public static object[] GetGachaDescriptionArguments(gachaTable data, float originFontSize)
    {
        if (data == null)
            return null;
        object[] args = new object[10];

        var gachaRscData = data._Gacha_Resource_Data;
        args[0] = gachaRscData != null ? gachaRscData._Name_Color : "";
        args[1] = "110%";

        characterTable charData;
        args[2] = (charData = data._Character_Show_Data.Length > 0 ? data._Character_Show_Data[0] : null) != null ? Localizer.GetLocalizedStringName(Localizer.SheetType.NONE, charData._Enum_Id) : "";
        args[3] = (charData = data._Character_Show_Data.Length > 1 ? data._Character_Show_Data[1] : null) != null ? Localizer.GetLocalizedStringName(Localizer.SheetType.NONE, charData._Enum_Id) : "";
        args[4] = (charData = data._Character_Show_Data.Length > 2 ? data._Character_Show_Data[2] : null) != null ? Localizer.GetLocalizedStringName(Localizer.SheetType.NONE, charData._Enum_Id) : "";

        gacha_classTable gachaClassData = data._Gacha_Class_Data;
        args[5] = $"{RoundAtSpecifiedDecimalPlace(3, gachaClassData._Prob[3] * 100f):N2}";
        args[6] = $"{RoundAtSpecifiedDecimalPlace(3, gachaClassData._Prob[2] * 100f):N2}";
        args[7] = $"{RoundAtSpecifiedDecimalPlace(3, gachaClassData._Prob[1] * 100f):N2}";
        args[8] = $"{RoundAtSpecifiedDecimalPlace(3, gachaClassData._Prob[0] * 100f):N2}";

        args[9] = string.Format(Localizer.GetLocalizedStringName(Localizer.SheetType.GACHA, data._Enum_Id), args[0], args[1]);

        return args;
    }

    public static float RoundAtSpecifiedDecimalPlace(int decimalPlaceNumberToRound, float number)
    {
        int placeNum = Mathf.Max(1, decimalPlaceNumberToRound);
        float mult = Mathf.Pow(10f, placeNum - 1);
        return Mathf.Round(number * mult) / mult;
    }
    public static double RoundAtSpecifiedDecimalPlace(int decimalPlaceNumberToRound, double number)
    {
        int placeNum = Mathf.Max(1, decimalPlaceNumberToRound);
        double mult = Mathf.Pow(10f, placeNum - 1);
        return Math.Round(number * mult) / mult;
    }


    public enum GachaTryResult
    {
        NO_SELECTED_SLOT,
        NO_AVAILABLE_GACHA,
        CHARACTER_COUNT_IS_OVER_GACHA_AVAIABLE_LIMIT,
        LACK_OF_MONEY,
        SUCCESS_COMMON,
        SUCCESS_OVER_CHARACTER_MAX
    }
    public class GachaTryActionData
    {
        private Dictionary<GachaTryResult, Action<GachaResultData>> dict_GachaTryAction;
        public GachaTryActionData()
        {
            dict_GachaTryAction = new Dictionary<GachaTryResult, Action<GachaResultData>>();
            foreach(var enumValue in Enum.GetValues(typeof(GachaTryResult)))
            {
                var value = (GachaTryResult)enumValue;
                dict_GachaTryAction.Add(value, null);
            }
        }

        public GachaTryActionData(Action<GachaResultData> noSelectedSlotAction,
            Action<GachaResultData> noAvailableAction,
            Action<GachaResultData> characterOverGachaLimit,
            Action<GachaResultData> lackOfMoneyAction,
            Action<GachaResultData> successCommonAction,
            Action<GachaResultData> successOverCharacterMaxAction) : this()
        {
            this[GachaTryResult.NO_SELECTED_SLOT] = noSelectedSlotAction;
            this[GachaTryResult.NO_AVAILABLE_GACHA] = noAvailableAction;
            this[GachaTryResult.CHARACTER_COUNT_IS_OVER_GACHA_AVAIABLE_LIMIT] = characterOverGachaLimit;
            this[GachaTryResult.LACK_OF_MONEY] = lackOfMoneyAction;
            this[GachaTryResult.SUCCESS_COMMON] = successCommonAction;
            this[GachaTryResult.SUCCESS_OVER_CHARACTER_MAX] = successOverCharacterMaxAction;
        }

        public Action<GachaResultData> this[GachaTryResult key]
        {
            get
            {
                if (!dict_GachaTryAction.ContainsKey(key)) dict_GachaTryAction.Add(key, null);
                return dict_GachaTryAction[key];
            }
            set
            {
                if (!dict_GachaTryAction.ContainsKey(key)) dict_GachaTryAction.Add(key, null);
                dict_GachaTryAction[key] = value;
            }
        }
    }
    public class GachaResultData
    {
        public bool isTutorial = false;
        public GachaGoodsInfo goodsInfo;
        public List<UIParam.Common.Reward> resultRewardList = new List<UIParam.Common.Reward>();

        public GachaResultData(GachaGoodsInfo goodsInfo)
        {
            this.goodsInfo = goodsInfo;
            isTutorial = false;
            resultRewardList.Clear();
        }
        public GachaResultData(GachaGoodsInfo goodsInfo, List<UIParam.Common.Reward> resultRewardList) : this(goodsInfo)
        {
            resultRewardList.RemoveAll(a => a.type != Data.Enum.Common_Type.CHARACTER);
            isTutorial = false;
            this.resultRewardList = resultRewardList;
        }

        public GachaResultData(GachaGoodsInfo goodsInfo, List<UIParam.Common.Reward> resultRewardList, bool isTutorial) : this(goodsInfo)
        {
            resultRewardList.RemoveAll(a => a.type != Data.Enum.Common_Type.CHARACTER);
            this.isTutorial = isTutorial;
            this.resultRewardList = resultRewardList;
        }

        public List<UIParam.Common.Reward> GetOnlyAcqusitionShowableRewardList()
        {
            var ret = new List<UIParam.Common.Reward>();
            foreach (var v in resultRewardList)
            {
                if (v == null || v.type != Data.Enum.Common_Type.CHARACTER)
                    continue;

                var data = Data._characterTable.GetDataFromTable(v.key);
                if (data == null || (int)data._CE_Character_Grade < (int)Data.Enum.Character_Grade.GRADE_B)
                    continue;

                ret.Add(new UIParam.Common.Reward(v.type, v.key));
            }

            return ret;
        }

        public characterTable[] RewardToCharacterDatas()
        {
            int len = resultRewardList.Count;
            var ret = new characterTable[len];
            for (int i = 0; i < len; i++)
            {
                ret[i] = Data._characterTable.GetDataFromTable(resultRewardList[i].key);
            }
            return ret;
        }
    }

    public static Action refreshGachaShop;
    public static bool isRequesting = false;
    /// <summary>
    /// "SUCCESS_OVER_CHARACTER_MAX" is executed with "SUCCESS_COMMON" at the same time.
    /// </summary>
    public static void TryToGacha(GachaGoodsInfo goodsInfo, GachaTryActionData tryActionData, Action<string> onBuyError)
    {
        if (isRequesting) return;

        var gachaResult = new GachaResultData(goodsInfo);
        if (goodsInfo == null)
        { tryActionData[GachaTryResult.NO_SELECTED_SLOT]?.Invoke(gachaResult); return; }

        var data = goodsInfo._Data;
        if (data == null)
        { tryActionData[GachaTryResult.NO_AVAILABLE_GACHA]?.Invoke(gachaResult); return; }

        var currentCharacterCount = User._Character.Count;
        if (currentCharacterCount >= User.GACHA_AVAILABLE_CHARACTER_LIMIT)
        {
            tryActionData[GachaTryResult.CHARACTER_COUNT_IS_OVER_GACHA_AVAIABLE_LIMIT]?.Invoke(gachaResult);
            return;
        }

        bool isOnce = goodsInfo._IsOnce;
        var costValue = goodsInfo.cost;
        var costType = goodsInfo.costType;
        if (costType != Data.Enum.Common_Type.ASSET)
        { Debug.Log("코스트가 에셋 타입이 아님"); return; }

        var assetData = Data._assetTable.GetDataFromTable(goodsInfo.costEnumId);
        if (assetData == null)
        { Debug.LogError("에셋 테이블 데이터 혹은 상품 코스트 이넘아이디 에러"); return; }

        User._Asset.TryGetValue(assetData._CE_Asset, out var assetInfo);
        var possession = assetInfo == null ? 0 : assetInfo._Balance;
        if (costValue > possession)
        { tryActionData[GachaTryResult.LACK_OF_MONEY]?.Invoke(gachaResult); return; }

        bool willBeFull = currentCharacterCount < User.CHARACTER_MAX_POSSESSION
            && currentCharacterCount >= (User.CHARACTER_MAX_POSSESSION - (isOnce ? 1 : 11));

        isRequesting = true;
        User.BuyShopGoods(goodsInfo,
            rewardList =>
            {
                gachaResult.resultRewardList.AddRange(rewardList);
                if (willBeFull)
                    tryActionData[GachaTryResult.SUCCESS_OVER_CHARACTER_MAX]?.Invoke(gachaResult);
                User.SetAsset(assetData._CE_Asset, possession - costValue);
                tryActionData[GachaTryResult.SUCCESS_COMMON]?.Invoke(gachaResult);
                refreshGachaShop?.Invoke();
                isRequesting = false;
            },
            error =>
            {
                onBuyError?.Invoke(error);
                isRequesting = false;
            });
    }

    public class PoolData
    {
        public string poolId;
        public List<string> list_AppearableCharacterId = new List<string>();
        public List<characterTable> _List_AppearableCharacterData
        {
            get
            {
                var ret = Data._characterTable.GetDataListFromTable(a => list_AppearableCharacterId.Contains(a._Enum_Id));
                ret.RemoveAll(a => a == null);
                return ret;
            }
        }

        public PoolData(string poolId, Data.Enum.Gacha_Pool_Grade grade)
        {
            this.poolId = poolId;

            list_AppearableCharacterId.Clear();
            switch (grade)
            {
                case Data.Enum.Gacha_Pool_Grade.A:
                    var tmpA = Data._gacha_drop_aTable.GetDataListFromTable(a =>
                    {
                        foreach (var id in a._Pool_Id)
                            if (id == this.poolId)
                                return true;
                        return false;
                    });
                    tmpA.RemoveAll(a => a == null || a._Drop_Character_Data == null);
                    foreach (var v in tmpA)
                        list_AppearableCharacterId.Add(v._Drop_Character_Data._Enum_Id);
                    break;
                case Data.Enum.Gacha_Pool_Grade.B:
                    var tmpB = Data._gacha_drop_bTable.GetDataListFromTable(a =>
                    {
                        foreach (var id in a._Pool_Id)
                            if (id == this.poolId)
                                return true;
                        return false;
                    });
                    tmpB.RemoveAll(a => a == null || a._Drop_Character_Data == null);
                    foreach (var v in tmpB)
                        list_AppearableCharacterId.Add(v._Drop_Character_Data._Enum_Id);
                    break;
                case Data.Enum.Gacha_Pool_Grade.C:
                    var tmpC = Data._gacha_drop_cTable.GetDataListFromTable(a =>
                    {
                        foreach (var id in a._Pool_Id)
                            if (id == this.poolId)
                                return true;
                        return false;
                    });
                    tmpC.RemoveAll(a => a == null || a._Drop_Character_Data == null);
                    foreach (var v in tmpC)
                        list_AppearableCharacterId.Add(v._Drop_Character_Data._Enum_Id);
                    break;
                case Data.Enum.Gacha_Pool_Grade.D:
                    var tmpD = Data._gacha_drop_dTable.GetDataListFromTable(a =>
                    {
                        foreach (var id in a._Pool_Id)
                            if (id == this.poolId)
                                return true;
                        return false;
                    });
                    tmpD.RemoveAll(a => a == null || a._Drop_Character_Data == null);
                    foreach (var v in tmpD)
                        list_AppearableCharacterId.Add(v._Drop_Character_Data._Enum_Id);
                    break;
            }
        }

        public characterTable GetGachaCharacter(System.Random randomValueClass)
        {
            if (list_AppearableCharacterId.Count == 0)
                return null;

            if (randomValueClass == null)
                randomValueClass = new System.Random(_GachaRandomSeed);
            var randomValue = randomValueClass.Next(list_AppearableCharacterId.Count);
            return Data._characterTable.GetDataFromTable(list_AppearableCharacterId[randomValue]);
        }
    }
    public class PoolGroupRateData
    {
        public int _Length { get; private set; }
        public float[] ratePivots;
        public PoolData[] poolDatas;

        public List<characterTable> _List_AllAppearableCharacter
        {
            get
            {
                if (poolDatas == null || poolDatas.Length == 0)
                    return null;
                var ret = new List<characterTable>();
                foreach (var poolData in poolDatas)
                    ret.AddRange(poolData._List_AppearableCharacterData);
                ret.RemoveAll(a => a == null);
                return ret;
            }
        }
        public bool _IsGettable => _List_AllAppearableCharacter != null && _List_AllAppearableCharacter.Count > 0;

        public PoolGroupRateData(gacha_pool_groupTable pgData, Data.Enum.Gacha_Pool_Grade grade)
        {
            _Length = 0;
            if (pgData == null)
                return;

            for (; _Length < pgData._Pool_Id.Length; _Length++)
                if (string.IsNullOrEmpty(pgData._Pool_Id[_Length]))
                    break;

            if (_Length == 0)
                return;

            ratePivots = new float[_Length];
            poolDatas = new PoolData[_Length];

            ratePivots[0] = pgData._Pool_Id_Rate[0];
            poolDatas[0] = new PoolData(pgData._Pool_Id[0], grade);
            var tmpAllCharacter = new List<string>(poolDatas[0].list_AppearableCharacterId);

            for (int i = 1; i < _Length; i++)
            {
                ratePivots[i] = ratePivots[i - 1] + pgData._Pool_Id_Rate[i];
                poolDatas[i] = new PoolData(pgData._Pool_Id[i], grade);
                poolDatas[i].list_AppearableCharacterId.RemoveAll(a => tmpAllCharacter.Contains(a));
                tmpAllCharacter.AddRange(poolDatas[i].list_AppearableCharacterId);
            }
        }

        public PoolData GetGachaPoolData(System.Random randomValueClass)
        {
            if (!_IsGettable)
                return null;

            if (randomValueClass == null)
                randomValueClass = new System.Random(_GachaRandomSeed);
            var randomValue = randomValueClass.NextDouble() * ratePivots[_Length - 1];

            for (int i = 0; i < _Length; i++)
            {
                if (randomValue > ratePivots[i])
                    continue;
                return poolDatas[i];
            }

            return null;
        }
        public PoolData GetGachaPoolData(System.Random randomValueClass, bool isFullPickupStack)
        {
            if (!_IsGettable)
                return null;
            if (!isFullPickupStack)
                return GetGachaPoolData(randomValueClass);
            else
                return poolDatas[0];
        }

        public characterTable GetGachaCharacter(System.Random randomValueClass) => GetGachaPoolData(randomValueClass)?.GetGachaCharacter(randomValueClass);
        public characterTable GetGachaCharacter(System.Random randomValueClass, bool isFullPickupStack) => GetGachaPoolData(randomValueClass, isFullPickupStack)?.GetGachaCharacter(randomValueClass);

        public bool CheckIsPickupCharacter(string enumId)
        {
            if (_Length == 0)
                return false;

            return poolDatas[0].list_AppearableCharacterId.Contains(enumId);
        }
    }
}