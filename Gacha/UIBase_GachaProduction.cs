using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;
using TMPro;
using Debug = COA_DEBUG.Debug;

public class UIBase_GachaProduction : UIBase
{
    #region Inspector
    [SerializeField] private Button btn_Skip;
    #endregion

    #region base Override Method
    private void Awake()
    {
        UIUtil.ResetAndAddListener(btn_Skip, OnClickSkipButton);
    }

    public override void Show(object param = null)
    {
        base.Show(param);
    }
    #endregion

    #region Listener
    private void OnClickSkipButton()
    {
        Hide();
    }
    #endregion
}
