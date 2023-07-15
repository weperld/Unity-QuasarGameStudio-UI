using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Debug = COA_DEBUG.Debug;
using AudioSystem;

public class UI_PartnerList_Card : MonoBehaviour
{
    [SerializeField] private Button btn_Card;
    [SerializeField] private GameObject go_Deregist;
    [SerializeField] private GameObject go_ExistData;
    [SerializeField] private UI_Card_Character card;
    [SerializeField] private GameObject go_PartnerMark;
    [SerializeField] private GameObject go_NotCollectMark;

    private PartnerListScrollData _Data { get; set; }
    private bool isPartner;

    private void Awake()
    {
        UIUtil.AddListener(btn_Card, OnClickCardButton);
    }

    private void OnEnable()
    {
        User.onChangePartnerCharacter -= SetActivePartnerMark;
        User.onChangePartnerCharacter += SetActivePartnerMark;
    }
    private void OnDisable()
    {
        User.onChangePartnerCharacter -= SetActivePartnerMark;
    }

    public void Set(PartnerListScrollData data)
    {
        _Data = data;
        SetActive(_Data != null);
        if (_Data == null || _Data._TableData == null) return;

        var tableData = _Data._TableData;
        var isNoPartner = tableData == User._NoPartnerCharacter;

        go_Deregist?.SetActive(isNoPartner);
        go_ExistData?.SetActive(!isNoPartner);

        if (isNoPartner) return;

        card?.Set(tableData);

        SetActivePartnerMark(User._PartnerCharacter);
    }

    private void SetActive(bool active) => gameObject.SetActive(active);

    private void OnClickCardButton()
    {
        AudioManager._instance.PlaySound(DefineName.Audio_OutGame.Sound.BUTTON);
        if (_Data == null) return;
        _Data.Touch();
    }

    private void SetActivePartnerMark(characterTable partner)
    {
        if (go_PartnerMark == null) return;
        isPartner = partner == _Data?._TableData;
        go_PartnerMark.SetActive(isPartner);
        SetActiveNotCollectMark();
    }
    private void SetActiveNotCollectMark()
    {
        if (go_NotCollectMark == null) return;
        go_NotCollectMark.SetActive(!isPartner && CheckNotCollect());
    }

    private bool CheckNotCollect()
    {
        var notCollect = true;
        foreach (var v in User._Character.Values)
        {
            var tmpData = v?._Data;
            if (tmpData == _Data._TableData)
            {
                notCollect = false;
                break;
            }
        }

        return notCollect;
    }
}