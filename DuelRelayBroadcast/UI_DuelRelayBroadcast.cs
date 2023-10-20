using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Debug = COA_DEBUG.Debug;

public class UI_DuelRelayBroadcast : UIBaseBelongings
{
    private const float RELAY_MSG_DEFAULT_SHOWING_TIME = 5f;
    private const float RELAY_MSG_SHOWING_CHANGE_TIME = 3f;

    [SerializeField] private GameObject go_MsgRoot;
    [SerializeField] private TextMeshProUGUI tmpu_Msg;
    [SerializeField] private Image img_SpeakerThumb;
    [SerializeField] private DuelMentColor duelMentColor;

    private class RelayMsgData
    {
        public int tier;
        public string msg;
        public Sprite spr_SpeakerThumb;
        public DuelTriggerChecker.RelayMentionParam param;

        public RelayMsgData(IAssetOwner assetOwner, mention_tier_2Table t2MentionData, DuelTriggerChecker.RelayMentionParam param, DuelMentColor duelMentColor)
        {
            tier = 2;
            spr_SpeakerThumb = t2MentionData._Character_Appear_Data?.GetSpriteFromSMTable(assetOwner);
            this.param = param;
            msg = Localizer.GetLocalizedStringTalk(Localizer.SheetType.STRING, t2MentionData._Enum_Id);
            msg = string.Format(msg, GetMsgArguments(duelMentColor));
        }
        public RelayMsgData(IAssetOwner assetOwner,
            string mentKey,
            mention_tier_1Table t1MentionData,
            DuelTriggerChecker.RelayMentionParam param,
            DuelMentColor duelMentColor)
        {
            tier = 1;
            spr_SpeakerThumb = t1MentionData._Character_Appear_Data?.GetSpriteFromSMTable(assetOwner);
            this.param = param;
            msg = Localizer.GetLocalizedStringTalk(Localizer.SheetType.STRING, mentKey);
            msg = string.Format(msg, GetMsgArguments(duelMentColor));
        }

        private object[] GetMsgArguments(DuelMentColor duelMentColor)
        {
            var ret = new object[4];
            if (param == null) return ret;

            var isMine = param.performerNickname == User.PlayerNickName;
            var perfColor = isMine ? duelMentColor.ToHtmlTargetName_Mine() : duelMentColor.ToHtmlTargetName_Others();
            var subjColor = !isMine ? duelMentColor.ToHtmlTargetName_Mine() : duelMentColor.ToHtmlTargetName_Others();

            ret[0] = $"<color=#{perfColor}>{param.performerNickname}</color>";
            ret[1] = $"<color=#{subjColor}>{param.subjectNickname}</color>";
            ret[2] = $"<color=#{perfColor}>{Localizer.GetLocalizedStringName(Localizer.SheetType.NONE, param.performerCharEnumId)}</color>";
            ret[3] = $"<color=#{subjColor}>{Localizer.GetLocalizedStringName(Localizer.SheetType.NONE, param.subjectCharEnumId)}</color>";

            return ret;
        }
    }
    private RelayMsgData curRelayMsg;
    private Queue<RelayMsgData> qu_RelayMsg = new Queue<RelayMsgData>();

    private IEnumerator showMsgCorout;

    private void OnEnable()
    {
        DuelTriggerChecker.onCheckDuelTriggerOfRelayMention -= ReceiveRelayMention;
        DuelTriggerChecker.onCheckDuelTriggerOfRelayMention += ReceiveRelayMention;

        if (go_MsgRoot != null) go_MsgRoot.SetActive(false);
        curRelayMsg = null;
        qu_RelayMsg.Clear();
    }
    private void OnDisable()
    {
        DuelTriggerChecker.onCheckDuelTriggerOfRelayMention -= ReceiveRelayMention;

        if (showMsgCorout != null) { StopCoroutine(showMsgCorout); showMsgCorout = null; }
    }

    private void ReceiveRelayMention(uint tier, List<mention_triggerTable> triggerDataList, DuelTriggerChecker.RelayMentionParam param)
    {
        if (triggerDataList == null || triggerDataList.Count == 0) return;

        var curQueueMsgTier = -1;
        if (curRelayMsg != null) curQueueMsgTier = curRelayMsg.tier;

        if (curQueueMsgTier > tier) return;
        else
        {
            foreach (var trigger in triggerDataList)
            {
                var converted = ConvertFromMentionTrigger(trigger, param);
                if (converted != null) qu_RelayMsg.Enqueue(converted);
            }
            if (curQueueMsgTier < tier) ShowMsg();
        }
    }

    private RelayMsgData ConvertFromMentionTrigger(mention_triggerTable target, DuelTriggerChecker.RelayMentionParam param)
    {
        if (target == null) return null;
        if (target._Mention_Tier <= 1) return GetRelayMsgDataOf1Tier(target._CE_Mention_Trigger, param);
        else return GetRelayMsgDataOf2Tier(target._CE_Mention_Trigger, param);
    }
    private RelayMsgData GetRelayMsgDataOf1Tier(Data.Enum.Duel_Trigger triggerValue, DuelTriggerChecker.RelayMentionParam param)
    {
        var list = Data._mention_tier_1Table.GetDataListFromTable(d =>
        {
            var key = triggerValue switch
            {
                Data.Enum.Duel_Trigger.CHARACTER_DEADLY => d._Character_Deadly,
                Data.Enum.Duel_Trigger.ALLY_KILL => d._Ally_Kill,
                Data.Enum.Duel_Trigger.ENEMY_KILL => d._Enemy_Kill,
                Data.Enum.Duel_Trigger.EXCLUSION => d._Exclusion,
                Data.Enum.Duel_Trigger.FIRSTBLOOD => d._Firstblood,
                Data.Enum.Duel_Trigger.RESOURCE_FULL => d._Resource_Full,
                _ => "",
            };

            return !string.IsNullOrEmpty(key);
        });
        if (list.Count == 0) return null;

        var rand = UnityEngine.Random.Range(0, list.Count);
        var randData = list[rand];

        return new RelayMsgData(
            ownerUIBase,
            triggerValue switch
            {
                Data.Enum.Duel_Trigger.CHARACTER_DEADLY => randData._Character_Deadly,
                Data.Enum.Duel_Trigger.ALLY_KILL => randData._Ally_Kill,
                Data.Enum.Duel_Trigger.ENEMY_KILL => randData._Enemy_Kill,
                Data.Enum.Duel_Trigger.EXCLUSION => randData._Exclusion,
                Data.Enum.Duel_Trigger.FIRSTBLOOD => randData._Firstblood,
                Data.Enum.Duel_Trigger.RESOURCE_FULL => randData._Resource_Full,
                _ => "",
            },
            randData,
            param,
            duelMentColor);
    }
    private RelayMsgData GetRelayMsgDataOf2Tier(Data.Enum.Duel_Trigger triggerValue, DuelTriggerChecker.RelayMentionParam param)
    {
        var mentData = Data._mention_tier_2Table.GetDataFromTable(triggerValue);
        if (mentData == null) return null;

        return new RelayMsgData(ownerUIBase, mentData, param, duelMentColor);
    }

    private void ShowMsg()
    {
        if (showMsgCorout != null) StopCoroutine(showMsgCorout);
        showMsgCorout = ShowMsgCorout();
        StartCoroutine(showMsgCorout);
    }
    private IEnumerator ShowMsgCorout()
    {
        var enqueued = qu_RelayMsg.Count > 0;
        if (enqueued)
        {
            float keepShowingTime = 0f;
            curRelayMsg = qu_RelayMsg.Dequeue();
            SetMsg();
            while (true)
            {
                enqueued = qu_RelayMsg.Count > 0;
                var limitTime = enqueued ? RELAY_MSG_SHOWING_CHANGE_TIME : RELAY_MSG_DEFAULT_SHOWING_TIME;
                if (keepShowingTime >= limitTime)
                {
                    if (!enqueued) break;

                    keepShowingTime = 0f;
                    curRelayMsg = qu_RelayMsg.Dequeue();
                    SetMsg();
                }

                yield return null;
                keepShowingTime += Time.deltaTime;
            }
        }

        curRelayMsg = null;
        go_MsgRoot.SetActive(false);
        showMsgCorout = null;
    }

    private void SetMsg()
    {
        if (curRelayMsg == null || go_MsgRoot == null) return;

        go_MsgRoot.SetActive(true);
        if (tmpu_Msg != null) tmpu_Msg.text = curRelayMsg.msg;
        if (img_SpeakerThumb != null) img_SpeakerThumb.sprite = curRelayMsg.spr_SpeakerThumb;
    }
}