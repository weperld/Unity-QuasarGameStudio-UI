using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Debug = COA_DEBUG.Debug;

[Serializable]
public struct DuelMentColor
{
    public Color color_SponserName;
    public Color color_TargetName_Mine;
    public Color color_TargetName_Others;
    public Color color_Item;

    public static string ToHtml(Color color)
    {
        return ColorUtility.ToHtmlStringRGBA(color);
    }
    public string ToHtmlSponserName() => ToHtml(color_SponserName);
    public string ToHtmlTargetName_Mine() => ToHtml(color_TargetName_Mine);
    public string ToHtmlTargetName_Others() => ToHtml(color_TargetName_Others);
    public string ToHtmlItem() => ToHtml(color_Item);
}

public static class DuelTriggerChecker
{
    public class RelayMentionParam
    {
        public string performerNickname = default;
        public string subjectNickname = default;
        public string performerCharEnumId = default;
        public string subjectCharEnumId = default;
    }

    public static Action<uint, List<mention_triggerTable>, RelayMentionParam> onCheckDuelTriggerOfRelayMention;
    public static void CheckDuelTriggerOfRelayMention(RelayMentionParam param, params Data.Enum.Duel_Trigger[] triggerValues)
    {
        var table = Data._mention_triggerTable;
        var triggerList = new List<Data.Enum.Duel_Trigger>(triggerValues);
        var triggerDataList = table.GetDataListFromTable(d => triggerList.Contains(d._CE_Mention_Trigger));
        if (triggerDataList == null || triggerDataList.Count == 0) return;

        triggerDataList.OrderByDescending(o => o._Mention_Tier);
        var highest = triggerDataList[0]._Mention_Tier;
        triggerDataList.RemoveAll(r => r._Mention_Tier < highest);
        triggerDataList.OrderByDescending(o => o._Mention_Majority);

        var rand = UnityEngine.Random.Range(0f, 1f);
        triggerDataList.RemoveAll(r => rand > r._Mention_Rate);
        if (triggerDataList.Count == 0) return;

        if (highest == 1) triggerDataList.RemoveRange(1, triggerDataList.Count - 1);

        onCheckDuelTriggerOfRelayMention?.Invoke(highest, triggerDataList, param);

        //if (AudioSystem.AudioManager.IsDetroying) return;
        //AudioSystem.AudioManager._instance.PlaySound(DefineName.Audio_InGame.DUEL_MENTION);
    }


    public class DonationParam
    {
        public string characterOwnerNickname = default;
        public string characterEnumId = default;
        public string sponserEnumId = default;
        public int[] bonusRateCountArr = new int[10];
    }
    public enum DonationMsgPrintState
    {
        NONE,
        WAITING,
        SHOWING
    }
    public static DonationMsgPrintState _DonaPrintState { get; private set; } = DonationMsgPrintState.NONE;

    public static Action<float, bool, string, donation_contentsTable> onCheckDuelTriggerOfDonation;
    public static void CheckDuelTriggerOfDonation(DonationParam param, Data.Enum.Duel_Trigger trigger)
    {
        if (_DonaPrintState == DonationMsgPrintState.SHOWING) return;

        var donaTriggerData = Data._donation_triggerTable.GetDataFromTable(trigger);
        if (donaTriggerData == null) return;

        var rand = UnityEngine.Random.Range(0f, 1f);
        var rate = donaTriggerData._Rate;
        if (param.bonusRateCountArr != null)
            for (int i = 0; i < donaTriggerData._Bonus_Rate_Plus.Length && i < param.bonusRateCountArr.Length; i++)
                rate += donaTriggerData._Bonus_Rate_Plus[i] * param.bonusRateCountArr[i];
        if (rand > rate) return;

        var donaMsgData = Data._donation_messageTable.GetDataFromTable(d =>
        {
            return d._Sponser_Enum == param.sponserEnumId && d._CE_Donation_Trigger == trigger;
        });
        if (donaMsgData == null) return;

        List<int> contentIndexList = new List<int>();
        for (int i = 0; i < donaMsgData._Content_Data.Length; i++)
        {
            if (donaMsgData._Content_Data[i] == null) continue;
            contentIndexList.Add(i);
        }
        if (contentIndexList.Count == 0) return;

        int randIdx = contentIndexList[UnityEngine.Random.Range(0, contentIndexList.Count)];
        var contentData = donaMsgData._Content_Data[randIdx];

        onCheckDuelTriggerOfDonation?.Invoke(donaTriggerData._Delay, User.PlayerNickName == param.characterOwnerNickname, param.characterEnumId, contentData);
    }

    public static void SetDonationPrintState(DonationMsgPrintState state)
    {
        _DonaPrintState = state;
    }
}