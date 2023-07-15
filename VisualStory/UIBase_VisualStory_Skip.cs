using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;
using TMPro;
using Debug = COA_DEBUG.Debug;
using AudioSystem;

public class UIBase_VisualStory_Skip : UIBase
{
    #region Inspector
    [SerializeField] private Character_UI_Portrait_UpperBody illust;
    [SerializeField] private TextMeshProUGUI tmpu_Title;
    [SerializeField] private TextMeshProUGUI tmpu_Description;
    [SerializeField] private Button btn_Cancel;
    [SerializeField] private Button btn_Skip;
    #endregion

    #region Variables
    private bool init = false;
    private UIParam.VisualStory.Skip param;
    #endregion

    #region Base Methods
    public override void Show(object param = null)
    {
        if (!init)
        {
            UIUtil.ResetAndAddListener(btn_Cancel, OnClickCancelButton);
            UIUtil.ResetAndAddListener(btn_Skip, OnClickSkipButton);
            init = true;
        }

        base.Show(param);
        this.param = param as UIParam.VisualStory.Skip;
        if (this.param == null) { Hide(); return; }

        if (illust != null) illust.Set(this.param.storyInfo._Character, this.param.storyInfo.costumeIndex, false);
        if (tmpu_Title != null) tmpu_Title.text = Localizer.GetLocalizedStringName(Localizer.SheetType.STRING, this.param.storyInfo._RootTimeline._Enum_Id);
        if (tmpu_Description != null) tmpu_Description.text = Localizer.GetLocalizedStringDesc(Localizer.SheetType.STRING, this.param.storyInfo._RootTimeline._Enum_Id);
    }
    #endregion

    #region Listener
    private void OnClickCancelButton()
    {
        Hide();
        param?.onCancel?.Invoke();
    }
    private void OnClickSkipButton()
    {
        Hide();
        AudioManager._instance.PlaySound(DefineName.Audio_OutGame.Sound.BUTTON);
        param?.onSkip?.Invoke();
    }
    #endregion
}
