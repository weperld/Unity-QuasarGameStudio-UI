using DG.Tweening;
using LeTai.Asset.TranslucentImage;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using static TutorialCtrl;
using static UIParam.VisualStory;
using Debug = COA_DEBUG.Debug;

public class UIBase_VisualStory_Script : UIBase_VisualStory
{
    #region Inspector
    [Header("Production")]
    [SerializeField] private UI_VisualStory_PlaceAlarm placeAlarm;
    [SerializeField] private UI_VisualStory_Oracle_Donation donationUI;

    [Header("Story Complete")]
    [SerializeField] private GameObject storyCompleteTweener;

    [Header("Translucent")]
    [SerializeField] private TranslucentImage blurry;
    #endregion

    #region Variables
    private bool isTutorial;
    private UIParam.VisualStory.Tutorial vs_Tutorial;

    private VisualStoryHelper.OnCloseData onCloseData = new VisualStoryHelper.OnCloseData();
    private IEnumerator closeEnumerator;
    #endregion

    private void OnDisable()
    {
        if (closeEnumerator != null)
        { StopCoroutine(closeEnumerator); closeEnumerator = null; }
    }

    public override void Show(object param)
    {
        if (blurry != null)
        {
            blurry.gameObject.SetActive(false);
            blurry.source = UIManager._instance._TranslucentImageSource;
        }
        donationUI?.SetActiveMsg(false);

        base.Show(param);
        if (storyCompleteTweener != null)
            storyCompleteTweener.gameObject.SetActive(false);

        if (param is UIParam.VisualStory.Tutorial)
            vs_Tutorial = param as UIParam.VisualStory.Tutorial;
        else
            vs_Tutorial = null;

        isTutorial = param is UIParam.VisualStory.Tutorial;
        if (isTutorial)
            Debug.Log("튜토리얼 스탭" + vs_Tutorial.tutorial_Param.step_Index);
    }

    protected override void AfterCallTimelineBranch(VisualStoryHelper.MarkerData currentMarker, VisualStoryHelper.MarkerData nextMarker)
    {

    }

    protected override void AfterCallTimelineCheckpoint(VisualStoryHelper.MarkerData currentMarker, VisualStoryHelper.MarkerData nextMarker)
    {

    }

    protected override void AfterCallTimelinePause()
    {

    }

    protected override void AfterCallTimelinePlay()
    {
    }

    protected override void AfterCallTimelineResume()
    {

    }

    protected override void AfterCallTimelineSkip()
    {

    }

    protected override void AfterCallTimelineRequestReward()
    {
        if (rootStoryInfo._TookRewards)
        { onCloseData.state = VisualStoryHelper.OnCloseData.State.NONE; return; }

        // 보상 처리 요청
        onCloseData.state = VisualStoryHelper.OnCloseData.State.NONE;
        RequestRewardSequence();
    }

    protected override void AfterCallTimelineEndpoint()
    {
        if (isTutorial)
        {
            if (vs_Tutorial.tutorial_Param.duel_Action != null)
            {
                Debug.Log("다음 액션 시작");
                vs_Tutorial.tutorial_Param.duel_Action.Invoke();
            }
            else
                vs_Tutorial.tutorial_Param.nextTutorial.Invoke(vs_Tutorial.tutorial_Param.step_Index);
        }
    }

    protected override void ExecuteProduction(TimelineParam.VStoryProduction param)
    {
        var productionData = Data._vstory_common_productionTable.GetDataFromTable(param.productionEnumId);
        if (productionData == null)
            return;

        if (!string.IsNullOrEmpty(productionData._Place))
        {
            var placeName = Localizer.GetLocalizedStringName(Localizer.SheetType.STRING, productionData._Place);
            placeAlarm?.SetAndPlay(placeName);
        }

        var bgSprite = productionData._Place_Sprite_Data?.GetSpriteFromSMTable(this);
        if (bgSprite != null)
            screenUI?.SetBackgroundImage(bgSprite);

        if (productionData._Donation_Data != null)
        {
            donationUI?.ShowDonation(productionData._Donation_Data, productionData._Donation_Target);
        }
    }

    protected override void OnChangeSpeedOfStory(int speed)
    {
        base.OnChangeSpeedOfStory(speed);
        donationUI?.SetSpeed(speed);
    }

    protected override void OpenStorySkipUI()
    {
        if (!isTutorial) base.OpenStorySkipUI();
        else
        {
            if (playableDirector.playableGraph.IsValid())
            {
                TimelinePause();
                dialogueUI.PauseDialogueTween();
                UIManager._instance.ShowUIBase<UIBase>(
                    DefineName.UIVisualStory.SKIP,
                    new Skip(
                        rootStoryInfo,
                        () =>
                        {
                            if (_PlayState == VisualStoryHelper.PlayState.PLAYING) TimelinePlay();
                            dialogueUI.ResumeDialogueTween();
                        },
                        () =>
                        {
                            Close();
                            if (vs_Tutorial.tutorial_Param.duel_Action != null)
                            {
                                Debug.Log("다음 액션 시작");
                                vs_Tutorial.tutorial_Param.duel_Action.Invoke();
                            }
                            else
                                vs_Tutorial.tutorial_Param.nextTutorial.Invoke(vs_Tutorial.tutorial_Param.step_Index);
                        }, vs_Tutorial.storyInfo.costumeIndex));
            }
            else
            {
                Close();
            }
            menuButtonsUI?.SetActiveOtherMenuRoot(false);
        }
    }
    protected override void OnStorySkipAction()
    {
        base.OnStorySkipAction();
        RequestRewardSequence();
    }

    public override void Close()
    {
        if (closeEnumerator != null)
            StopCoroutine(closeEnumerator);
        closeEnumerator = CloseEnumerator();
        StartCoroutine(closeEnumerator);
    }
    private IEnumerator CloseEnumerator()
    {
        while (onCloseData.state == VisualStoryHelper.OnCloseData.State.WAITING)
        {
            yield return null;
        }
        switch (onCloseData.state)
        {
            case VisualStoryHelper.OnCloseData.State.REWARD:
                // TODO: 보상 ui 띄우는 코드 추가 필요
                break;
            case VisualStoryHelper.OnCloseData.State.ERROR:
                onCloseData.onErrorUI = UIMessageBox.Confirm_Old("스토리 시청 완료 에러 발생",
                    $"ERROR CODE: [{onCloseData.errorMsg}]",
                    () => { if (onCloseData.onErrorUI != null) onCloseData.onErrorUI.Hide(); });
                break;
        }

        closeEnumerator = null;
        base.Close();
    }

    private void RequestRewardSequence()
    {
        // 예시: 오라클
        //var cast = rootStoryInfo as OracleInfo;
        //if (cast != null && !cast._TookRewards && onCloseData.state == VisualStoryHelper.OnCloseData.State.NONE)
        //{
        //    onCloseData.state = VisualStoryHelper.OnCloseData.State.WAITING;
        //    User.RequestCompleteWatchOracle(cast,
        //        reward =>
        //        {
        //            onCloseData.state = VisualStoryHelper.OnCloseData.State.REWARD;
        //            onCloseData.list_Reward = reward;
        //        },
        //        error =>
        //        {
        //            onCloseData.state = VisualStoryHelper.OnCloseData.State.ERROR;
        //            onCloseData.errorMsg = error;
        //        });
        //}
        //else onCloseData.state = VisualStoryHelper.OnCloseData.State.NONE;
    }
}
