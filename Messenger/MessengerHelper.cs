using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Debug = COA_DEBUG.Debug;

public static class MessengerHelper
{
    //public static int logPoolId = 0;

    //public const string CHAT_FORMAT_IMG = "IMG:";
    //public const string CHAT_FORMAT_DIV = "SPC:";

    //public static string ConvertMsgIdxListToString(this List<ChatEventMsg> msgIdxList)
    //{
    //    string ret = "";
    //    foreach (var idxData in msgIdxList)
    //    {
    //        ret += $"{idxData[0]}{idxData[1]}";
    //    }

    //    return ret;
    //}

    //public static int GetNodeIndex(chat_nodeTable parent, chat_nodeTable child)
    //{
    //    if (child == null) return -1;
    //    if (parent == null) return 0;

    //    int ret = 0;
    //    for (int i = 0; i < parent._Next_Node_Data.Length; i++)
    //    {
    //        var v = parent._Next_Node_Data[i];
    //        if (v == null) continue;
    //        if (v == child) { ret = i; break; }
    //    }
    //    return ret;
    //}

    //public static int GetTalkIndex(chat_nodeTable node)
    //{
    //    if (node == null || node._Selectable) return 0;

    //    List<int> idxList = new List<int>();
    //    for (int i = 0; i < node._Talk.Length; i++)
    //    {
    //        if (string.IsNullOrEmpty(node._Talk[i])) continue;
    //        idxList.Add(i);
    //    }

    //    int rand = UnityEngine.Random.Range(0, idxList.Count);
    //    return idxList.Count > 0 ? idxList[rand] : 0;
    //}

    //public static bool CheckChatEventIsEnd(ChatEventData checkTarget)
    //{
    //    if (checkTarget == null) return true;
    //    if (checkTarget._MsgIdxs.Count == 0) return false;

    //    var nextNode = checkTarget._EventData._Root_Node_Data;
    //    if (nextNode == null) return true;

    //    for (int i = 1; i < checkTarget._MsgIdxs.Count; i++)
    //    {
    //        nextNode = nextNode._Next_Node_Data[checkTarget._MsgIdxs[i][0]];
    //        if (nextNode == null) break;
    //    }
    //    if (nextNode != null)
    //    {
    //        bool nextIsExist = false;
    //        foreach (var v in nextNode._Next_Node_Data)
    //            if (v != null) nextIsExist = true;

    //        return !nextIsExist;
    //    }
    //    return true;
    //}

    //public static bool CheckRoomIsSingle(chat_roomTable roomData)
    //{
    //    if (roomData == null) return false;

    //    string charEnum = "";
    //    foreach (var v in roomData._Character_Data)
    //    {
    //        if (v == null) continue;

    //        if (string.IsNullOrEmpty(charEnum)) charEnum = v._Enum_Id;
    //        else
    //        {
    //            charEnum = "";
    //            break;
    //        }
    //    }

    //    return !string.IsNullOrEmpty(charEnum);
    //}
    //public static bool CheckRoomIsSingleAndInPossession(chat_roomTable roomData)
    //{
    //    if (roomData == null) return false;

    //    string charEnum = "";
    //    foreach (var v in roomData._Character_Data)
    //    {
    //        if (v == null) continue;

    //        if (string.IsNullOrEmpty(charEnum)) charEnum = v._Enum_Id;
    //        else
    //        {
    //            charEnum = "";
    //            break;
    //        }
    //    }
    //    //User._CharacterCollection.TryGetValue(charEnum, out var collectionData);
    //    //bool ret = collectionData != null && collectionData.Count > 0;

    //    return true;//ret;
    //}
}