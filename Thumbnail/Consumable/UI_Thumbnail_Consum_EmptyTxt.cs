using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Debug = COA_DEBUG.Debug;

public class UI_Thumbnail_Consum_EmptyTxt : MonoBehaviour
{
    [SerializeField] private UI_Thumbnail_Consum_Basic baseThumbnail;
    [SerializeField] private GameObject go_Empty;

    public itemTable _Data => baseThumbnail?._Data;

    public void SetData(itemTable data) => baseThumbnail?.SetData(data);
    public void SetActive(bool active) => baseThumbnail?.SetActive(active);
    public void SetActiveEmptyTxt(bool active)
    {
        if (go_Empty == null) return;
        go_Empty.SetActive(active);
    }
}