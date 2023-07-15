using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Debug = COA_DEBUG.Debug;

public class UI_GachaRecordInfo : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI tmpu_Grade;
    [SerializeField] private TextMeshProUGUI tmpu_Name;
    [SerializeField] private TextMeshProUGUI tmpu_Date;

    public GachaRecordInfo _Info { get; private set; }
    public void SetData(GachaRecordInfo info)
    {
        _Info = info;
        gameObject.SetActive(_Info?._CharData != null);
        if (_Info?._CharData == null) return;

        var charData = _Info._CharData;
        if (tmpu_Grade != null) tmpu_Grade.text = Localizer.GetLocalizedStringName(Localizer.SheetType.NONE, charData._CE_Character_Grade.ToString());
        if (tmpu_Name != null) tmpu_Name.text = Localizer.GetLocalizedStringName(Localizer.SheetType.NONE, charData._Enum_Id);
        if (tmpu_Date != null) tmpu_Date.text = _Info.getTime.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss");
    }
}