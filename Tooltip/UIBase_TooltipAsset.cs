using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;
using TMPro;
using Debug = COA_DEBUG.Debug;
using static UIParam.Tooltip;

public class UIBase_TooltipAsset : UIBase_TooltipBase<Asset>
{
    //[SerializeField] private UI_Thumbnail_Integrate thumbnail;
    [SerializeField] private Image img_Asset;
    [SerializeField] private TextMeshProUGUI text_Name;
    [SerializeField] private TextMeshProUGUI text_Content;

    protected override void OnInit(Asset param)
    {

    }

    protected override void SetInfoToUI(Asset param)
    {
        img_Asset.sprite = param._asset_Table._Image_Reference_Data.GetSpriteFromSMTable(this);
        //thumbnail.SetData(param._ToolTipReward, false);

        text_Name.text = Localizer.GetLocalizedStringName(Localizer.SheetType.NONE, param._asset_Table._Enum_Id);

        text_Content.text = Localizer.GetLocalizedStringDesc(Localizer.SheetType.NONE, param._asset_Table._Enum_Id);
    }
}
