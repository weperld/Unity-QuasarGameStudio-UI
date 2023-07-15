using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;
using TMPro;
using LeTai.Asset.TranslucentImage;
using Debug = COA_DEBUG.Debug;

public class UIBase_Tooltip_Weapon : UIBase
{
    [SerializeField] private Image img_SW;
    [SerializeField] private TextMeshProUGUI text_SW_Title;
    [SerializeField] private TextMeshProUGUI text_SW_Content;
    [SerializeField] private TranslucentImage blurry;
    [SerializeField] private Button btn_Back;

    private signature_weaponTable sw_Table;

    public override void Show(object param)
    {
        base.Show();

        sw_Table = param as signature_weaponTable;
        if (sw_Table == null)
            return;

        gameObject.SetActive(true);

        UIUtil.ResetAndAddListener(btn_Back, OnClickBackBtn);
        blurry.source = UIManager._instance._TranslucentImageSource;


        img_SW.sprite = sw_Table._Weapon_Reference_Data.GetSpriteFromSMTable(this);
        text_SW_Title.text = Localizer.GetLocalizedStringName(Localizer.SheetType.NONE, sw_Table._Enum_Id);
        text_SW_Content.text = Localizer.GetLocalizedStringDesc(Localizer.SheetType.NONE, sw_Table._Enum_Id);
    }

    private void OnClickBackBtn()
    {
        Hide();
    }
}
