using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Debug = COA_DEBUG.Debug;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using DG.Tweening;
using static UIParam.VisualStory;
using static TimelineParam;
using static VisualStoryHelper;
using AudioSystem;
using Spine;
using Spine.Unity;

public abstract class UIBase_VisualStory : UIBase
{
    [Serializable]
    protected struct ResetScreenViewData
    {
        public AnimationCurve curve;
        public float duration;
    }
    #region Inspector
    [Header("Top Menu")]
    [SerializeField] protected UI_VisualStory_MenuButtons menuButtonsUI;
    [SerializeField] protected Button btn_ShowUI;
    [SerializeField] protected GameObject[] go_HideTargetObjs;
    [SerializeField] protected ResetScreenViewData screenResetSetting;

    [Header("Scene")]
    [SerializeField] protected PlayableDirector playableDirector;
    [SerializeField] protected UI_VisualStory_Screen screenUI;
    [SerializeField] protected UI_VisualStory_Dialogue dialogueUI;
    [SerializeField] protected UI_VisualStory_Branch branchUI;
    #endregion

    #region Variables
    protected bool _IsTestMode { get; private set; }

    protected VisualStoryHelper.PlayState _PlayState { get; private set; }

    private int selectedSpeedIndex = 0;
    protected int _SelectedSpeedIndex
    {
        get => selectedSpeedIndex;
        set
        {
            selectedSpeedIndex = value;
            OnChangeSpeedOfStory(_CurrentSpeedOfStory);
        }
    }
    protected int _CurrentSpeedOfStory => VisualStoryHelper._StorySpeedArrayData[_SelectedSpeedIndex];

    private bool isOnAutoPlay = false;
    protected bool _IsOnAutoPlay
    {
        get => isOnAutoPlay;
        set
        {
            isOnAutoPlay = value;
            OnChangeAutoPlayState(value);
        }
    }

    private StoryPlayMode storyPlayMode = StoryPlayMode.MANUAL;
    protected StoryPlayMode _StoryPlayMode
    {
        get => storyPlayMode;
        set
        {
            storyPlayMode = value;

            switch (value)
            {
                case StoryPlayMode.MANUAL:
                    ChangeAutoPlayState(false);
                    ChangeSpeedIndex(0);
                    break;
                case StoryPlayMode.AUTO_x1:
                    ChangeAutoPlayState(true);
                    ChangeSpeedIndex(0);
                    break;
                case StoryPlayMode.AUTO_x2:
                    ChangeAutoPlayState(true);
                    ChangeSpeedIndex(1);
                    break;
            }
            menuButtonsUI?.OnChangeStoryPlayMode(value);
        }
    }

    private bool isInited = false;

    protected VisualStoryInfo rootStoryInfo;
    protected vstory_timelineTable timelineData;

    protected List<VisualStoryHelper.MarkerData> list_PauseMark = new List<VisualStoryHelper.MarkerData>();
    protected int pauseMarkerCount;
    protected int nextMarkerIndex;

    protected UIBase changeTimelineError;

    protected TimelineBranch[] arr_BranchData;

    private double durationOffset = 1d / 60d;
    #endregion

    protected virtual void Init()
    {
        if (dialogueUI != null)
        {
            dialogueUI.SetOnCompletePrintDialogue(OnCompletePrintDialogue);
            dialogueUI.SetResumeButtonListener(TimelineResume);
            dialogueUI.SetSkipButtonListener(TimelineSkip);
        }
        if (screenUI != null)
        {
            screenUI.SetOnChangeScreenResetable(OnChangeScreenResetable);
            screenUI.Init(new ScreenInit()
            {
                onClickAction = OnClickScreenUI
            });
        }
        menuButtonsUI?.Set(
            OnClickSpeedButton,
            OnClickResetScreenView,
            OnClickOpenLogButton,
            OnClickHideUIButton,
            OnClickSkipButton);
        UIUtil.ResetAndAddListener(btn_ShowUI, OnClickShowUIButton);
    }

    #region Base Methods
    public override void Show(object param)
    {
        branchUI?.Hide();
        VisualStoryHelper.pressedDialogueSkipButton = false;

        if (!isInited)
        {
            isInited = true;
            Init();
        }

        base.Show(param);

        ResetUIHideState();
        menuButtonsUI?.SetActiveResetViewBlocker(false);

        var cast = param as Main;
        if (cast == null)
        {
            Debug.LogBold($"<color=red>{nameof(UIBase_VisualStory)}.Show:: Parameter Error");
            Hide();
            return;
        }
        _IsTestMode = cast.isTestMode;

        rootStoryInfo = cast.storyInfo;
        timelineData = rootStoryInfo._RootTimeline;
        if (timelineData == null)
        {
            Debug.LogBold($"<color=red>{nameof(UIBase_VisualStory)}.Show:: {nameof(vstory_timelineTable)} Data Error");
            base.Hide();
            return;
        }

        if (playableDirector != null)
        {
            var timelineAsset = ResourceManager.LoadAsset<TimelineAsset>(
                string.Format(DefineName.UIVisualStory.TIMELINEASSET_PATH_FORMAT, timelineData._Timeline_Track), this);
            SetTimelineAssetToPlayableDirector(timelineAsset);
            SetupPauseMarkerInfo((TimelineAsset)timelineAsset,
                playableDirector.playableGraph.GetRootPlayable(0));
            ResetStoryPlayMode();
        }
    }

    public override void Hide()
    {
        OpenStorySkipUI();
    }
    #endregion

    #region Timeline Signal
    internal void TimelinePlay()
    {
        if (playableDirector != null)
        {
            _PlayState = VisualStoryHelper.PlayState.PLAYING;
            playableDirector.Evaluate();
            playableDirector.Play();
            dialogueUI?.SetActiveStoryProcessButtonState(VisualStoryHelper.StoryProcessButtonState.RESUME);
        }
        AfterCallTimelinePlay();
    }

    /// <summary>
    /// Call by the signal receiver.
    /// </summary>
    public void TimelineCheckpoint()
    {
        _PlayState = VisualStoryHelper.PlayState.CHECKPOINT;

        var rootPlayable = playableDirector.playableGraph.GetRootPlayable(0);
        rootPlayable.SetSpeed(_CurrentSpeedOfStory);
        rootPlayable.SetTime(rootPlayable.GetDuration());

        var currentMarker = list_PauseMark[nextMarkerIndex];
        nextMarkerIndex = Math.Clamp(nextMarkerIndex + 1, 0, pauseMarkerCount - 1);
        var nextMarker = list_PauseMark[nextMarkerIndex];
        var duration = list_PauseMark[nextMarkerIndex].time + durationOffset;
        rootPlayable.SetDuration(duration);

        PauseBase();

        AfterCallTimelineCheckpoint(currentMarker, nextMarker);
    }

    internal void TimelineSkip()
    {
        if (playableDirector != null && playableDirector.playableGraph.IsValid())
        {
            var rootPlayable = playableDirector.playableGraph.GetRootPlayable(0);
            rootPlayable.SetSpeed(1000d);
        }
        AfterCallTimelineSkip();
    }

    internal void TimelineResume()
    {
        if (playableDirector != null)
        {
            _PlayState = VisualStoryHelper.PlayState.PLAYING;
            playableDirector.Evaluate();
            playableDirector.Resume();
        }
        dialogueUI?.SetActiveStoryProcessButtonState(VisualStoryHelper.StoryProcessButtonState.NONE);

        AfterCallTimelineResume();
    }

    /// <summary>
    /// Call by the signal receiver.
    /// </summary>
    public void TimelineBranch()
    {
        _PlayState = VisualStoryHelper.PlayState.BRANCH;

        var rootPlayable = playableDirector.playableGraph.GetRootPlayable(0);
        rootPlayable.SetSpeed(_CurrentSpeedOfStory);
        rootPlayable.SetTime(rootPlayable.GetDuration());

        var currentMarker = list_PauseMark[nextMarkerIndex];
        nextMarkerIndex = Math.Clamp(nextMarkerIndex + 1, 0, pauseMarkerCount - 1);
        var nextMarker = list_PauseMark[nextMarkerIndex];
        var duration = list_PauseMark[nextMarkerIndex].time + durationOffset;
        rootPlayable.SetDuration(duration);

        PauseBase();
        dialogueUI?.SetActiveStoryProcessButtonState(VisualStoryHelper.StoryProcessButtonState.NONE);

        ShowTimelineBranch();
        AfterCallTimelineBranch(currentMarker, nextMarker);
    }

    private void PauseBase()
    {
        if (playableDirector == null || !playableDirector.playableGraph.IsValid())
            return;

        playableDirector.Pause();
        playableDirector.Evaluate();
        var rootPlayable = playableDirector.playableGraph.GetRootPlayable(0);
        rootPlayable.SetSpeed(_CurrentSpeedOfStory);
    }
    protected void TimelinePause()
    {
        if (playableDirector == null)
            return;
        PauseBase();
        AfterCallTimelinePause();
    }

    /// <summary>
    /// Call by the signal receiver.
    /// </summary>
    public void TimelineRequestReward()
    {
        var rootPlayable = playableDirector.playableGraph.GetRootPlayable(0);
        nextMarkerIndex = Math.Clamp(nextMarkerIndex + 1, 0, pauseMarkerCount - 1);
        rootPlayable.SetDuration(list_PauseMark[nextMarkerIndex].time + 1 / 60f);

        AfterCallTimelineRequestReward();
    }

    /// <summary>
    /// Call by the signal receiver.
    /// </summary>
    public void TimelineEndpoint()
    {
        dialogueUI?.SetActiveStoryProcessButtonState(VisualStoryHelper.StoryProcessButtonState.NONE);
        if (_IsTestMode) TestClose();
        else Close();
        AfterCallTimelineEndpoint();
    }

    protected abstract void AfterCallTimelinePlay();
    protected abstract void AfterCallTimelineCheckpoint(VisualStoryHelper.MarkerData currentMarker, VisualStoryHelper.MarkerData nextMarker);
    protected abstract void AfterCallTimelineSkip();
    protected abstract void AfterCallTimelineResume();
    protected abstract void AfterCallTimelineBranch(VisualStoryHelper.MarkerData currentMarker, VisualStoryHelper.MarkerData nextMarker);
    protected abstract void AfterCallTimelinePause();
    protected abstract void AfterCallTimelineRequestReward();
    protected abstract void AfterCallTimelineEndpoint();
    #endregion

    #region Listener
    protected virtual void OnClickSpeedButton()
    {
        AudioManager._instance.PlaySound(DefineName.Audio_OutGame.Sound.BUTTON);
        if (_StoryPlayMode == StoryPlayMode.AUTO_x2)
            _StoryPlayMode = StoryPlayMode.MANUAL;
        else
            _StoryPlayMode++;
    }

    protected virtual void OnSelectBranch(TimelineBranch branchData)
    {
        var timelineEnumId = branchData.timelineEnumId;

        if (string.IsNullOrEmpty(timelineEnumId))
        {
            Debug.Log($"{(nameof(timelineEnumId)).ToUpper()} is Empty Or Null.");
            changeTimelineError = UIMessageBox.Confirm_Old(
                "타임라인 교체 에러",
                "입력한 타임 라인 데이터 이넘 아이디가 비어있음",
                () => { changeTimelineError.Hide(); });
            return;
        }

        var branchTimelineData = Data._vstory_timelineTable.GetDataFromTable(timelineEnumId);
        if (branchTimelineData == null)
        {
            Debug.Log($"해당 이넘 아이디({timelineEnumId})에 일치하는 타임라인 데이터가 존재하지 않음");
            changeTimelineError = UIMessageBox.Confirm_Old(
                "타임라인 교체 에러",
                $"입력한 이넘 아이디({timelineEnumId})에 해당하는 타임 라인 데이터가 존재하지 않음",
                () => { changeTimelineError.Hide(); });
            return;
        }

        if (timelineData == branchTimelineData)
        {
            TimelineResume();
            Debug.Log("현재 타임라인 그대로 진행");
            return;
        }

        var asset = ResourceManager.LoadAsset<TimelineAsset>(string.Format(DefineName.UIVisualStory.TIMELINEASSET_PATH_FORMAT, branchTimelineData._Timeline_Track), this);
        if (asset == null)
        {
            Debug.Log($"해당 타임라인 데이터({timelineEnumId})에 입력된 타임라인 에셋이 존재하지 않거나, 어드레서블이 활성화 되어 있지 않음");
            changeTimelineError = UIMessageBox.Confirm_Old(
                "타임라인 교체 에러",
                $"타임라인 테이블 데이터({timelineEnumId})에 입력된 트랙 값과 같은 이름의 타임라인 에셋이 존재하지 않음",
                () => { changeTimelineError.Hide(); });
            return;
        }

        timelineData = branchTimelineData;
        SetTimelineAssetToPlayableDirector(asset);
        SetupPauseMarkerInfo(asset, playableDirector.playableGraph.GetRootPlayable(0));
        ChangeSpeedIndex(_SelectedSpeedIndex);
    }

    protected virtual void OnClickResetScreenView()
    {
        AudioManager._instance.PlaySound(DefineName.Audio_OutGame.Sound.BUTTON);
        if (screenUI == null)
            return;
        var duration = Mathf.Max(screenResetSetting.duration, 0f);
        screenUI.InstantResetFocusAndZoom(screenResetSetting.curve, duration);
    }

    protected virtual void OnClickOpenLogButton()
    {
        AudioManager._instance.PlaySound(DefineName.Audio_OutGame.Sound.BUTTON);
        menuButtonsUI?.SetActiveOtherMenuRoot(false);
        UIMessageBox.OnelineMsg_NotReady();
    }
    protected virtual void OnClickHideUIButton()
    {
        AudioManager._instance.PlaySound(DefineName.Audio_OutGame.Sound.BUTTON);
        if (go_HideTargetObjs != null)
        {
            foreach (var obj in go_HideTargetObjs)
                obj?.SetActive(false);
        }
        btn_ShowUI?.gameObject.SetActive(true);
        menuButtonsUI?.SetActiveOtherMenuRoot(false);
    }
    protected virtual void OnClickShowUIButton()
    {
        ResetUIHideState();
    }
    protected virtual void OnClickSkipButton()
    {
        AudioManager._instance.PlaySound(DefineName.Audio_OutGame.Sound.BUTTON);
        OpenStorySkipUI();
        menuButtonsUI?.SetActiveOtherMenuRoot(false);
    }

    protected virtual void OnChangeScreenResetable(bool resetable)
    {
        menuButtonsUI?.SetActiveResetViewBlocker(!resetable);
    }

    protected virtual void OnCompletePrintDialogue()
    {
        if (_PlayState == VisualStoryHelper.PlayState.PLAYING)
        {
            TimelineCheckpoint();
        }

        if (_IsOnAutoPlay) TimelineResume();
    }

    private void OnClickScreenUI()
    {
        switch (VisualStoryHelper.storyProcessButtonState)
        {
            case StoryProcessButtonState.NONE: return;
            case StoryProcessButtonState.RESUME: TimelineResume(); return;
            case StoryProcessButtonState.SKIP:
                TimelineSkip();
                VisualStoryHelper.pressedDialogueSkipButton = true;
                dialogueUI?.CompleteDialogueTween();
                return;
        }
    }
    #endregion

    #region Other
    public void SetTimelineBranchData(params TimelineBranch[] timelineBranchParams)
    {
        arr_BranchData = timelineBranchParams;
    }
    public void ShowTimelineBranch()
    {
        if (branchUI == null)
            return;

        branchUI.SetupAndShow(OnSelectBranch, arr_BranchData);
        ResetStoryPlayMode();
    }
    public void HideTimelineBranch()
    {
        if (branchUI == null)
            return;
        branchUI.Hide();
    }

    protected void ChangeSpeedIndex(int select)
    {
        _SelectedSpeedIndex = Mathf.Clamp(select, 0, VisualStoryHelper._StorySpeedArrayData.Length - 1);
    }
    protected void ResetSpeedOfStory() => ChangeSpeedIndex(0);
    protected virtual void OnChangeSpeedOfStory(int speed)
    {
        if (playableDirector != null && playableDirector.playableGraph.IsValid())
        {
            var rootPlayable = playableDirector.playableGraph.GetRootPlayable(0);
            rootPlayable.SetSpeed(speed);
        }
        DOTween.timeScale = speed;

        screenUI?.SetSkeletonTimescale(speed);
        menuButtonsUI?.OnChangeSpeed(speed);
    }

    protected void ChangeAutoPlayState(bool state)
    {
        _IsOnAutoPlay = state;
    }
    protected void ResetAutoPlayState() => ChangeAutoPlayState(false);
    protected virtual void OnChangeAutoPlayState(bool state)
    {
        menuButtonsUI?.OnChangeAutoPlayState(state);

        if (_PlayState != VisualStoryHelper.PlayState.CHECKPOINT)
            return;
        if (state)
            TimelineResume();
    }

    protected void ChangeStoryPlayMode(StoryPlayMode mode)
    {
        _StoryPlayMode = mode;
    }
    protected void ResetStoryPlayMode() => ChangeStoryPlayMode(StoryPlayMode.MANUAL);

    private void SetupPauseMarkerInfo(TimelineAsset timeline, Playable rootPlayable)
    {
        list_PauseMark.Clear();
        nextMarkerIndex = 0;
        var markers = timeline.GetRootTrack(0).GetMarkers();
        foreach (var marker in markers)
            list_PauseMark.Add(new VisualStoryHelper.MarkerData() { marker = marker });

        pauseMarkerCount = list_PauseMark.Count;

        if (pauseMarkerCount == 0)
        {
            timeline.durationMode = TimelineAsset.DurationMode.BasedOnClips;
            rootPlayable.SetDuration(timeline.duration);
            durationOffset = 1d / 60d;
        }
        else
        {
            list_PauseMark.Sort((a, b) =>
            {
                return a.time.CompareTo(b.time);
            });
            timeline.durationMode = TimelineAsset.DurationMode.FixedLength;
            var fps = timeline.editorSettings.frameRate;
            durationOffset = 1d / fps;
            rootPlayable.SetDuration(list_PauseMark[0].time + durationOffset);
        }

    }

    private void SetTimelineAssetToPlayableDirector(TimelineAsset asset)
    {
        if (asset == null)
            return;

        this.FixSignalReceiver(asset);

        playableDirector.playableAsset = asset;
        playableDirector.RebuildGraph();
        playableDirector.time = 0d;
        TimelinePlay();
    }

    protected void ResetUIHideState()
    {
        menuButtonsUI?.SetActiveOtherMenuRoot(false);
        if (go_HideTargetObjs != null)
        {
            foreach (var obj in go_HideTargetObjs)
                obj?.SetActive(true);
        }
        btn_ShowUI?.gameObject.SetActive(false);
    }

    protected virtual void OpenStorySkipUI()
    {
        TimelinePause();
        dialogueUI.PauseDialogueTween();

        UIManager._instance.ShowUIBase<UIBase>(
            DefineName.UIVisualStory.SKIP,
            new Skip(
                rootStoryInfo,
                OnStorySkipCancelAction,
                OnStorySkipAction));
    }
    protected virtual void OnStorySkipCancelAction()
    {
        dialogueUI.ResumeDialogueTween();
        if (_PlayState == VisualStoryHelper.PlayState.PLAYING) TimelinePlay();
    }
    protected virtual void OnStorySkipAction()
    {
        Close();
    }
    public void ShowProductionFromTrack(TimelineParam.VStoryProduction param)
    {
        ExecuteProduction(param);
    }
    protected abstract void ExecuteProduction(TimelineParam.VStoryProduction param);

    public void MoveCharacterToForeFrontByIndex(int index)
    {
        if (screenUI == null)
            return;
        screenUI.SetCharacterAsLastSibling(index);
    }

    public virtual void Close()
    {
        ResetSpeedOfStory();
        playableDirector.playableAsset = null;
        base.Hide();
    }

    public virtual void TestClose()
    {
        ResetSpeedOfStory();
        playableDirector.playableAsset = null;
        gameObject.SetActive(false);
    }
    #endregion
}
