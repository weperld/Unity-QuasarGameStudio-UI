using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Debug = COA_DEBUG.Debug;
using static VisualStoryHelper;
using AudioSystem;

public class UI_VisualStory_MenuButtons : MonoBehaviour
{
    [SerializeField] private Button btn_Speed;
    [SerializeField] private TextMeshProUGUI tmpu_Speed;
    [SerializeField] private GameObject go_AutoOff;
    [SerializeField] private GameObject go_AutoOn;
    [SerializeField] private Button btn_ResetScreenView;
    [SerializeField] private GameObject go_ResetViewBlocker;
    [SerializeField] private Button btn_OtherMenu;
    [SerializeField] private GameObject go_OtherMenuRoot;
    [SerializeField] private Button btn_Log;
    [SerializeField] private Button btn_HideUI;
    [SerializeField] private Button btn_Skip;

    public void Set(UnityAction speedClickAction,
        UnityAction resetViewClickAction,
        UnityAction logClickAction,
        UnityAction hideClickAction,
        UnityAction skipClickAction)
    {
        UIUtil.ResetAndAddListener(btn_Speed, speedClickAction);
        UIUtil.ResetAndAddListener(btn_ResetScreenView, resetViewClickAction);
        UIUtil.ResetAndAddListener(btn_OtherMenu, OnClickOtherMenu);
        UIUtil.ResetAndAddListener(btn_Log, logClickAction);
        UIUtil.ResetAndAddListener(btn_HideUI, hideClickAction);
        UIUtil.ResetAndAddListener(btn_Skip, skipClickAction);
    }

    public void OnChangeStoryPlayMode(StoryPlayMode mode)
    {
        if (go_AutoOff != null) go_AutoOff.SetActive(mode != StoryPlayMode.MANUAL);
    }

    public void OnChangeSpeed(int speedValue)
    {
        if (tmpu_Speed == null) return;
        tmpu_Speed.text = speedValue.ToString();
    }

    public void OnChangeAutoPlayState(bool value)
    {
        if (go_AutoOn == null) return;
        go_AutoOn.SetActive(value);
    }

    private void OnClickOtherMenu()
    {
        AudioManager._instance.PlaySound(DefineName.Audio_OutGame.Sound.BUTTON);
        if (go_OtherMenuRoot == null) return;
        SetActiveOtherMenuRoot(!go_OtherMenuRoot.activeSelf);
    }

    public void SetActiveOtherMenuRoot(bool active)
    {
        go_OtherMenuRoot?.SetActive(active);
    }

    public void SetActiveResetViewBlocker(bool active)
    {
        go_ResetViewBlocker?.SetActive(active);
    }
}