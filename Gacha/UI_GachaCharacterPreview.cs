using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Debug = COA_DEBUG.Debug;
using Spine.Unity;

public class UI_GachaCharacterPreview : MonoBehaviour
{
    [SerializeField] private Character_UI_Illust illust_Whole;
    [SerializeField] private UI_GachaCharacterPreview_Info info;

    public characterTable _Data { get; private set; }

    public void SetData(characterTable data, bool useSkeleton)
    {
        _Data = data;
        gameObject.SetActive(_Data != null);
        info?.SetActive(_Data != null);
        if (_Data == null) return;

        illust_Whole?.Set(useSkeleton, _Data, exce: true);
        info?.Set(_Data);
    }
}