using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Debug = COA_DEBUG.Debug;

public class MsgData
{
    //public chat_eventTable eventData;
    //public chat_nodeTable nodeData;
    //public int branchIdx;
    //public int talkIdx;
    //public bool isLast;
    ////public CharacterQuestInfo questInfo;
    //public object[] param;
    //public object[] _LocalizedParams
    //{
    //    get
    //    {
    //        if (param == null || param.Length == 0) return null;

    //        var ret = new object[param.Length];
    //        switch (eventData._CE_Chat_Trigger)
    //        {
    //            case Data.Enum.Chat_Trigger.GIFT:
    //                ret[0] = Localizer.GetLocalizedStringName(Localizer.LocalType.NONE, param[0] as string);
    //                ret[1] = Localizer.GetLocalizedStringName(Localizer.LocalType.NONE, param[1] as string);
    //                ret[2] = param[2];
    //                break;
    //            case Data.Enum.Chat_Trigger.STAMINA_LOW:
    //            case Data.Enum.Chat_Trigger.STAMINA_ZERO:
    //            case Data.Enum.Chat_Trigger.GAME_ACCESS:
    //            case Data.Enum.Chat_Trigger.COMMON_TILE_OCCUPY:
    //            case Data.Enum.Chat_Trigger.COMMON_BIGTILE_OCCUPY:
    //            case Data.Enum.Chat_Trigger.SPECIFIED_TILE_OCCUPY:
    //            case Data.Enum.Chat_Trigger.NIGHT:
    //            case Data.Enum.Chat_Trigger.DAY:
    //            case Data.Enum.Chat_Trigger.CHARACTER_QUEST:
    //            case Data.Enum.Chat_Trigger.CHARACTER_GET:
    //            case Data.Enum.Chat_Trigger.CNT:
    //                for (int i = 0; i < ret.Length; i++)
    //                    ret[i] = param[i];
    //                break;
    //        }
    //        return ret;
    //    }
    //}

    //public MsgData(chat_eventTable eventData, chat_nodeTable nodeData, int branchIdx = 0, int talkIdx = 0, bool isLast = false, /*CharacterQuestInfo questInfo = null, */object[] param = null)
    //{
    //    this.eventData = eventData;
    //    this.nodeData = nodeData;
    //    this.branchIdx = branchIdx;
    //    this.talkIdx = talkIdx;
    //    this.isLast = isLast;
    //    //this.questInfo = questInfo;
    //    this.param = param;
    //}
    //public MsgData(MsgData msgData)
    //{
    //    if (msgData == null) return;
    //    eventData = msgData.eventData;
    //    nodeData = msgData.nodeData;
    //    branchIdx = msgData.branchIdx;
    //    talkIdx = msgData.talkIdx;
    //    isLast = msgData.isLast;
    //    //questInfo = msgData.questInfo;
    //    param = msgData.param;
    //}
    //public MsgData Clone() => new MsgData(this);
}

public class OptionData
{
    public readonly MsgData msgData;
    public Data.Enum.Node_Branch branchType;
    public string branchValue;

    public OptionData(MsgData msgData, Data.Enum.Node_Branch branchType, string branchValue)
    {
        this.msgData = msgData;
        this.branchType = branchType;
        this.branchValue = branchValue;
    }
}

[Serializable]
public struct ChatEventMsg
{
    [SerializeField] public int nextBranchIdx;
    [SerializeField] public int talkIdx;

    public ChatEventMsg(int nextBranchIdx, int talkIdx)
    {
        this.nextBranchIdx = nextBranchIdx;
        this.talkIdx = talkIdx;
    }

    public int this[int key]
    {
        get
        {
            key = Mathf.Clamp(key, 0, 1);
            if (key == 0) return nextBranchIdx;
            else return talkIdx;
        }
    }
}

[Serializable]
public class ChatEventData
{
    //[SerializeField] public string eventId;
    //[SerializeField] public List<ChatEventMsg> msgIdxs;
    //[SerializeField] public object[] param;

    //public string _EventId => eventId;
    //public List<ChatEventMsg> _MsgIdxs => msgIdxs;

    //public chat_eventTable _EventData => Data._chat_eventTable.GetDataFromTable(_EventId);

    //public ChatEventData()
    //{
    //    this.eventId = "";
    //    this.msgIdxs = new List<ChatEventMsg>();
    //    this.param = null;
    //}

    //public ChatEventData(string eventId, string msgIdxValues, object[] param)
    //{
    //    this.eventId = eventId;
    //    this.msgIdxs = new List<ChatEventMsg>();
    //    this.param = param;

    //    if (string.IsNullOrEmpty(msgIdxValues)) return;

    //    for (int i = 0; i < msgIdxValues.Length; i += 2)
    //    {
    //        string tmpStr = $"{msgIdxValues[i]}";
    //        int branch = Convert.ToInt32(tmpStr);
    //        tmpStr = $"{msgIdxValues[i + 1]}";
    //        int local = Convert.ToInt32(tmpStr);
    //        msgIdxs.Add(new ChatEventMsg(branch, local));
    //    }
    //}

    //public ChatEventData(GrpcModel.MessengerData data)
    //{
    //    eventId = data.Title;
    //    msgIdxs = new List<ChatEventMsg>();
    //    param = null;

    //    for (int i = 0; i < data.Body.Length; i += 2)
    //    {
    //        string tmp = $"{data.Body[i]}";
    //        int branch = Convert.ToInt32(tmp);
    //        tmp = $"{data.Body[i + 1]}";
    //        int local = Convert.ToInt32(tmp);
    //        msgIdxs.Add(new ChatEventMsg(branch, local));
    //    }
    //}

    //public void Update(ChatEventData eventData)
    //{
    //    this.eventId = eventData.eventId;
    //    this.msgIdxs = eventData.msgIdxs;
    //    this.param = eventData.param;
    //}
}

public class ToastMsgData
{
    //public Data.Enum.Toast_Message_Type toastType;
    //public chat_nodeTable nodeData;
    //public int talkIdx;
    //public string[] args;

    //public characterTable _SenderData => nodeData?._Sender_Data;
    //public string _Local
    //{
    //    get
    //    {
    //        string ret = "";
    //        if (nodeData != null)
    //        {
    //            var talk = nodeData._Talk[talkIdx];
    //            talk = Localizer.GetLocalizedStringTalk(Localizer.LocalType.STRING, talk);
    //            switch (toastType)
    //            {
    //                case Data.Enum.Toast_Message_Type.MY_TILE_ATTACKED:
    //                    var tileName = Localizer.GetLocalizedStringDesc(Localizer.LocalType.WORLD, args[1]);
    //                    ret = string.Format(talk, args[0], tileName);
    //                    break;
    //                case Data.Enum.Toast_Message_Type.MY_TILE_OCCUPIED:
    //                    tileName = Localizer.GetLocalizedStringDesc(Localizer.LocalType.WORLD, args[1]);
    //                    ret = string.Format(talk, args[0], tileName);
    //                    break;
    //                case Data.Enum.Toast_Message_Type.OTHER_BIGTILE_OCCUPIED:
    //                    tileName = Localizer.GetLocalizedStringDesc(Localizer.LocalType.WORLD, args[1]);
    //                    ret = string.Format(talk, args[0], tileName);
    //                    break;
    //                case Data.Enum.Toast_Message_Type.CNT:
    //                    break;
    //            }
    //        }

    //        return ret;
    //    }
    //}

    //public ToastMsgData(Data.Enum.Toast_Message_Type toastType, chat_nodeTable nodeData, int talkIdx, params string[] args)
    //{
    //    this.toastType = toastType;
    //    this.nodeData = nodeData;
    //    this.talkIdx = talkIdx;
    //    this.args = args;
    //}
}

[Serializable]
public class QuestMessageCheckInfo
{
    //public enum QuestResult
    //{
    //    NOT_YET,
    //    COMPLETED,
    //    REJECTED
    //}

    //[SerializeField] public int id;
    //[SerializeField] public string questEnumId;
    //[SerializeField] public List<ChatEventMsg> msgIdxs;
    //[SerializeField] public bool checkState;
    //public QuestResult questResult = QuestResult.NOT_YET;

    //private CharacterQuestInfo questInfo;
    //public CharacterQuestInfo _QuestInfo
    //{
    //    get
    //    {
    //        if (questInfo == null) questInfo = User._CharacterQuest.Find(f => f.ID == id);
    //        return questInfo;
    //    }
    //}
    //public character_quest_listTable _QuestData => Data._character_quest_listTable.GetDataFromTable(questEnumId);

    //public QuestMessageCheckInfo()
    //{
    //    id = 0;
    //    questEnumId = null;
    //    msgIdxs = new List<ChatEventMsg>();
    //    checkState = false;
    //    //questInfo = null;
    //}
    //public QuestMessageCheckInfo(CharacterQuestInfo questInfo)
    //{
    //    id = questInfo.ID;
    //    questEnumId = questInfo.EnumID;
    //    msgIdxs = new List<ChatEventMsg>();
    //    checkState = false;
    //    this.questInfo = questInfo;
    //}
    //public QuestMessageCheckInfo(/*CharacterQuestInfo questInfo, */string msgIdxValues)// : this(questInfo)
    //{
    //    for (int i = 0; i < msgIdxValues.Length; i += 2)
    //    {
    //        string tmpStr = $"{msgIdxValues[i]}";
    //        int branch = Convert.ToInt32(tmpStr);
    //        tmpStr = $"{msgIdxValues[i + 1]}";
    //        int local = Convert.ToInt32(tmpStr);
    //        msgIdxs.Add(new ChatEventMsg(branch, local));
    //    }
    //}
}

[Serializable]
public class QuestMessageCheckInfoList : IEnumerable<QuestMessageCheckInfo>
{
    [SerializeField] public List<QuestMessageCheckInfo> questCheckInfoList;

    private List<QuestMessageCheckInfo> _QuestCheckInfoList
    {
        get
        {
            if (questCheckInfoList == null) questCheckInfoList = new List<QuestMessageCheckInfo>();
            //questCheckInfoList.RemoveAll(q => q._QuestInfo == null || q._QuestData == null);
            return questCheckInfoList;
        }
    }

    public int Count => _QuestCheckInfoList.Count;

    public QuestMessageCheckInfoList()
    {
        questCheckInfoList = new List<QuestMessageCheckInfo>();
    }
    public QuestMessageCheckInfoList(QuestMessageCheckInfoList copy)
    {
        questCheckInfoList = new List<QuestMessageCheckInfo>(copy._QuestCheckInfoList);
    }

    //public QuestMessageCheckInfo this[int questId]
    //{
    //    get
    //    {
    //        var ret = _QuestCheckInfoList.Find(q => q.id == questId);
    //        return ret;
    //    }
    //}
    //public QuestMessageCheckInfo this[string questEnumId]
    //{
    //    get
    //    {
    //        var ret = _QuestCheckInfoList.Find(q => q.questEnumId == questEnumId);
    //        return ret;
    //    }
    //}
    //public QuestMessageCheckInfo this[QuestMessageCheckInfo checkInfo]
    //{
    //    get
    //    {
    //        if (checkInfo == null) return null;
    //        return this[checkInfo.id];
    //    }
    //}
    //public QuestMessageCheckInfo this[CharacterQuestInfo questInfo]
    //{
    //    get
    //    {
    //        if (questInfo == null) return null;
    //        return this[questInfo.ID];
    //    }
    //}

    public void Clear()
    {
        _QuestCheckInfoList.Clear();
    }
    /// <summary>
    /// 존재하지 않는 퀘스트 제거
    /// </summary>
    public void Reset()
    {
        //questCheckInfoList.RemoveAll(q => User._CharacterQuest.Find(f => f.ID == q.id) == null || q._QuestData == null);
    }
    public QuestMessageCheckInfo GetByIndex(int index) => _QuestCheckInfoList[index];
    //public void Remove(int questId)
    //{
    //    if (this[questId] != null) _QuestCheckInfoList.Remove(this[questId]);
    //}
    //public void Remove(string questEnumId)
    //{
    //    if (this[questEnumId] != null) _QuestCheckInfoList.Remove(this[questEnumId]);
    //}
    //public void RemoveAt(int index)
    //{
    //    _QuestCheckInfoList.RemoveAt(index);
    //}
    //public void Add(CharacterQuestInfo questInfo)
    //{
    //    if (questInfo == null) return;
    //    if (this[questInfo.ID] == null) _QuestCheckInfoList.Add(new QuestMessageCheckInfo(questInfo));
    //}
    //public void AddRange(List<CharacterQuestInfo> questInfoList)
    //{
    //    foreach (var v in questInfoList) Add(v);
    //}
    //public void Add(QuestMessageCheckInfo questCheckInfo)
    //{
    //    if (questCheckInfo == null) return;
    //    if (this[questCheckInfo.id] == null) _QuestCheckInfoList.Add(questCheckInfo);
    //}
    //public void AddRange(List<QuestMessageCheckInfo> questCheckInfoList)
    //{
    //    foreach (var v in questCheckInfoList) Add(v);
    //}
    //public void Add(QuestMessageCheckInfoList questCheckInfoList)
    //{
    //    foreach (var v in questCheckInfoList) Add(v);
    //}
    //public QuestMessageCheckInfo Find(Predicate<QuestMessageCheckInfo> predicate) => _QuestCheckInfoList.Find(predicate);
    //public QuestMessageCheckInfoList FindAll(Predicate<QuestMessageCheckInfo> predicate)
    //{
    //    QuestMessageCheckInfoList ret = new QuestMessageCheckInfoList();
    //    foreach (var v in _QuestCheckInfoList)
    //        if (predicate(v)) ret.Add(v);

    //    return ret;
    //}
    public int FindIndex(Predicate<QuestMessageCheckInfo> predicate) => _QuestCheckInfoList.FindIndex(predicate);
    //public bool Contains(CharacterQuestInfo questInfo)
    //{
    //    return Find(a => a._QuestInfo == questInfo) != null;
    //}
    public void Sort(Comparison<QuestMessageCheckInfo> compare)
    {
        questCheckInfoList.Sort(compare);
    }
    public void CopyTo(QuestMessageCheckInfoList target)
    {
        target.questCheckInfoList = new List<QuestMessageCheckInfo>(this._QuestCheckInfoList);
    }

    public IEnumerator<QuestMessageCheckInfo> GetEnumerator()
    {
        return _QuestCheckInfoList.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}