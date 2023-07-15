using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Debug = COA_DEBUG.Debug;

public class UI_Thumbnail_Asset_Basic_Old : UIBaseBelongings
{
    [SerializeField] private Image img_Icon;
    [SerializeField] private GameObject[] go_Stars;

    public assetTable _Data { get; private set; }

    public void SetData(assetTable data)
    {
        _Data = data;
        if (_Data == null) { SetActive(false); return; }
        SetActive(true);

        if (img_Icon != null) img_Icon.sprite = _Data._Image_Reference_Data?.GetSpriteFromSMTable(ownerUIBase);

        if (go_Stars != null)
        {
            for (int i = 0; i < go_Stars.Length; i++)
            {
                var go = go_Stars[i];
                go?.SetActive(i <= (int)_Data._CE_Asset_Grade);
            }
        }
    }
    public void SetActive(bool active)
    {
        gameObject.SetActive(active);
    }
}