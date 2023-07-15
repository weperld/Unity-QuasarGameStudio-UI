using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Debug = COA_DEBUG.Debug;

public class UI_BroadCastComment : MonoBehaviour
{
    public class commentData
    {
        public string writer;
        public string content;
        public sprite_managementTable imgRef;
        public uint like;
    }

    public UI_BroadCastCommentItem[] items;

    public void Set(string characterEnumId)
    {
        var character = Data.GetDataFromTable(Data._characterTable, characterEnumId);
        if (character != null)
        {
            var profile = character._Profile_Data;
            if(profile != null)
            {
                var comment = profile._Comment_Data;
                if(comment != null)
                {
                    foreach(var item in items)
                    {
                        item.gameObject.SetActive(false);
                    }
                    int cnt = comment._Comment_Contents.Length - 1;
                    for(int i = cnt; i >= 0; i--)
                    {
                        if (string.IsNullOrEmpty(comment._Comment_Contents[i])) continue;
                        items[i].SetData(new commentData
                        {
                            content = comment._Comment_Contents[i],
                            writer = comment._Comment_Writer[i],
                            imgRef = comment._Comment_Reference_Data[i],
                            like = comment._Comment_Like[i]
                        });
                        items[i].gameObject.SetActive(true);
                    }
                }
            }
        }
    }
}