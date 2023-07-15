using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Debug = COA_DEBUG.Debug;
using DG.Tweening;
using static TweenHelper;

public class UI_VisualStory_Dialogue : UIBaseBelongings
{
    [SerializeField] private GameObject[] go_Bgs;
    [SerializeField] private TextMeshProUGUI tmpu_Speaker;
    [SerializeField] private TextMeshProUGUI tmpu_Crew;
    [SerializeField] private TextMeshProUGUI tmpu_Dialogue;
    [SerializeField] private DOTweenAnimation tweener;
    [SerializeField] private GameObject go_SpeakerThumbnailRoot;
    [SerializeField] private Image img_SpeakerThumbnail;
    [SerializeField] private Button btn_Resume;
    [SerializeField] private Button btn_Skip;

    private Action onResumeClick;
    private Action onDialogueSkipClick;
    private Action onCompletePrintDialogue;

    public void SetResumeButtonListener(Action action)
    {
        onResumeClick = action;
        UIUtil.ResetAndAddListener(btn_Resume, OnClickResumeBtn);
    }
    public void SetSkipButtonListener(Action action)
    {
        onDialogueSkipClick = action;
        UIUtil.ResetAndAddListener(btn_Skip, OnClickDialogueSkipBtn);
    }
    public void SetOnCompletePrintDialogue(Action action)
    {
        onCompletePrintDialogue = action;
    }

    public void SetActiveStoryProcessButtonState(VisualStoryHelper.StoryProcessButtonState state)
    {
        VisualStoryHelper.storyProcessButtonState = state;
        if (btn_Resume != null) btn_Resume.gameObject.SetActive(state == VisualStoryHelper.StoryProcessButtonState.RESUME);
        if (btn_Skip != null) btn_Skip.gameObject.SetActive(state == VisualStoryHelper.StoryProcessButtonState.SKIP);
    }

    public void ShowDialogue(TimelineParam.VStoryDialogue param)
    {
        var dialogueData = param?._DialogueData;
        if (dialogueData == null) return;

        if (go_Bgs != null && go_Bgs.Length > 0)
        {
            var format = (int)dialogueData._CE_Dialogue_Format;
            format = Mathf.Clamp(format, 0, go_Bgs.Length - 1);
            for (int i = 0; i < go_Bgs.Length; i++)
            {
                go_Bgs[i]?.SetActive(i == format);
            }
        }

        if (tmpu_Speaker != null) tmpu_Speaker.text = Localizer.GetLocalizedStringName(Localizer.SheetType.STRING, dialogueData._Name);
        if (tmpu_Crew != null) tmpu_Crew.text = Localizer.GetLocalizedStringName(Localizer.SheetType.STRING, dialogueData._Crew);

        go_SpeakerThumbnailRoot?.SetActive(dialogueData._Thumbnail_Data != null);
        if (img_SpeakerThumbnail != null) img_SpeakerThumbnail.sprite = dialogueData._Thumbnail_Data?.GetSpriteFromSMTable(ownerUIBase);

        var speak = string.Format(Localizer.GetLocalizedStringTalk(Localizer.SheetType.STRING, dialogueData._Dialogue), User.PlayerNickName);
        var tweenOption = param.tmpTween;
        if (tweener != null && tweenOption != null)
        {
            tweener.ControlTweener(
                ControlState.RESET,
                tweener =>
                {
                    tweener.hasOnComplete = true;
                    tweener.onComplete.RemoveAllListeners();
                    tweener.onComplete.AddListener(OnCompletePrintDialogue);

                    tweener.duration = tweenOption.textPrintSpeed;
                    tweener.isSpeedBased = !tweenOption.useSpeedAsDuration;
                    tweener.easeType = tweenOption.tweenEase;
                    if (tweenOption.tweenEase == Ease.INTERNAL_Custom) tweener.easeCurve = tweenOption.tweenCurve;
                    tweener.optionalScrambleMode = tweenOption.tweenScrambleMode;
                    if (tweenOption.tweenScrambleMode == ScrambleMode.Custom) tweener.optionalString = tweenOption.customScramble;
                    tweener.endValueString = speak;

                    var tmpu = tweener.GetTargetGO()?.GetComponent<TextMeshProUGUI>();
                    if (tmpu == null) return;

                    tmpu.text = "";
                },
                tweener =>
                {
                    if (tweenOption.playOnSet) tweener.ControlTweener(ControlState.PLAY);
                    if (VisualStoryHelper.pressedDialogueSkipButton) CompleteDialogueTween();
                });
        }
        else if (tmpu_Dialogue != null) tmpu_Dialogue.text = speak;

        SetActiveStoryProcessButtonState(VisualStoryHelper.StoryProcessButtonState.SKIP);
    }

    public void CompleteDialogueTween()
        => tweener.ControlTweener(ControlState.COMPLETE);
    public void PauseDialogueTween()
        => tweener.ControlTweener(ControlState.PAUSE);
    public void ResumeDialogueTween()
        => tweener.ControlTweener(ControlState.PLAY);

    #region Listener
    private void OnClickResumeBtn()
    {
        if (onResumeClick != null) onResumeClick();
    }
    private void OnClickDialogueSkipBtn()
    {
        if (onDialogueSkipClick != null) onDialogueSkipClick();
        VisualStoryHelper.pressedDialogueSkipButton = true;
        CompleteDialogueTween();
    }

    /// <summary>
    /// Call by Dialogue Tweener
    /// </summary>
    public void OnCompletePrintDialogue()
    {
        VisualStoryHelper.pressedDialogueSkipButton = false;
        SetActiveStoryProcessButtonState(VisualStoryHelper.StoryProcessButtonState.RESUME);
        if (onCompletePrintDialogue != null) onCompletePrintDialogue();
    }
    #endregion
}