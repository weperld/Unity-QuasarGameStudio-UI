using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Debug = COA_DEBUG.Debug;

public class UI_Thumbnail_Consum_Possession : MonoBehaviour
{
    [SerializeField] private UI_Thumbnail_Consum_Basic thumbnail;
    [SerializeField] TMPro.TextMeshProUGUI tmpu_Possession;

    public itemTable _Data => thumbnail?._Data;
    public int _Count { get; private set; }

    public void SetData(itemTable data, int count)
    {
        thumbnail?.SetData(data);
        _Count = count;
        if (tmpu_Possession != null) tmpu_Possession.text = _Count.ToString();
    }
}