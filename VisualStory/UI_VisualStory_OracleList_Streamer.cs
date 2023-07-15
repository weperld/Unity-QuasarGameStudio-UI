using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Debug = COA_DEBUG.Debug;

public class UI_VisualStory_OracleList_Streamer : UI_VisualStory_OracleList_StreamingThumbnail
{
    [SerializeField] private Image img_StreamerThumbnail;
    [SerializeField] private GameObject go_OnCompleteWatching;

    protected override void OnChangeTakeRewardsState(bool value)
    {
        go_OnCompleteWatching?.SetActive(value);
        if (img_StreamerThumbnail != null)
        {
            var mat = new Material(img_StreamerThumbnail.material);
            mat.SetFloat("_Saturation", value ? 0f : 1f);
            mat.SetFloat("_Brightness", value ? -0.5f : 0f);
            mat.SetFloat("_Contrast", value ? 0.5f : 1f);
            img_StreamerThumbnail.material = mat;
        }
    }

    protected override void OnSet(VisualStoryHelper.OracleCategory category, VisualStoryInfo info)
    {
        gameObject.SetActive(true);
        if (img_StreamerThumbnail != null) img_StreamerThumbnail.sprite = info._Character?._Resource_List_Data?._Thumbnail_Reference_Data?.GetSpriteFromSMTable(ownerUIBase);
    }

    protected override void OnSetToNullInfo(VisualStoryHelper.OracleCategory category)
    {
        gameObject.SetActive(false);
    }
}