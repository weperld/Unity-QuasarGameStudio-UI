using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Debug = COA_DEBUG.Debug;

public class UI_Thumbnail_Character_Lv : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI tmpu_Level;
    [SerializeField] private UI_Thumbnail_Character_Basic_Old thumbnail;

    public CharacterInfo _Info { get; private set; }
    public characterTable _Data { get; private set; }
    public int _Level { get; private set; }

    private void OnDisable()
    {
        if (_Info != null) { _Info.onUpdateChExp -= OnUpdateExp; _Info = null; }
    }

    //캐릭터
    public void SetData(CharacterInfo info)
    {
        if (_Info != null) { _Info.onUpdateChExp -= OnUpdateExp; _Info = null; }

        _Info = info;
        _Data = null;
        _Level = 1;
        if (_Info == null) return;

        _Info.onUpdateChExp -= OnUpdateExp;
        _Info.onUpdateChExp += OnUpdateExp;

        _Data = _Info._Data;
        _Level = _Info._Level;
        if (thumbnail != null) thumbnail.SetData(_Info._Data);
        if (tmpu_Level != null) tmpu_Level.text = _Info._Level.ToString();
    }

    //몬스터
    public void SetData(characterTable data, int lv)
    {
        if (_Info != null) { _Info.onUpdateChExp -= OnUpdateExp; _Info = null; }

        _Data = data;
        _Level = lv;
        if (_Data == null) return;

        if (thumbnail != null) thumbnail.SetData(_Data);
        if (tmpu_Level != null) tmpu_Level.text = lv.ToString();
    }

    private void OnUpdateExp(CharacterInfo info)
    {
        if (info != _Info) return;
        if (tmpu_Level != null) tmpu_Level.text = _Info._Level.ToString();
    }
}