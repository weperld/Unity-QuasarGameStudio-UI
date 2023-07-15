using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;
using TMPro;
using Debug = COA_DEBUG.Debug;

public class UIBase_ChargeMoney_Quick : UIBase
{
    #region Inspector
    [SerializeField] private Button btn_Close;
    [SerializeField] private UI_ChargeMoney_Quick_Goods[] goods;
    #endregion

    #region Variables
    private UIParam.ChargeMoney.QuickParam paramData;
    #endregion

    #region Base Override Method
    private void Awake()
    {
        UIUtil.ResetAndAddListener(btn_Close, OnClickCloseButton);
    }
    public override void Show(object param)
    {
        if (!(param is UIParam.ChargeMoney.QuickParam) || param == null) { if (gameObject.activeInHierarchy) Hide(); return; }

        base.Show(param);
        paramData = param as UIParam.ChargeMoney.QuickParam;
        if (goods != null)
            foreach (var item in goods)
                if (item != null) item.SetData(paramData.targetType);
    }
    #endregion

    #region Listener
    private void OnClickCloseButton()
    {
        Hide();
    }
    #endregion
}
