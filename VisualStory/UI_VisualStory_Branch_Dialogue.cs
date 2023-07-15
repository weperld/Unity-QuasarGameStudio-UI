using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Debug = COA_DEBUG.Debug;
using static TimelineParam;

public class UI_VisualStory_Branch_Dialogue : UI_VisualStory_Branch
{
    [Header("Inherit")]
    [SerializeField] private TextMeshProUGUI[] tmpu_BranchText;
    [SerializeField] private GameObject[] go_PressImg;

    protected override void SetupSelectable(int index, TimelineBranch branchTimelineParam)
    {
        go_PressImg[index]?.SetActive(false);
        if (branchTimelineParam == null || tmpu_BranchText == null || tmpu_BranchText.Length <= index) return;

        var tmpu = tmpu_BranchText[index];
        if (tmpu == null) return;

        var timelineData = Data._vstory_timelineTable.GetDataFromTable(branchTimelineParam.timelineEnumId);
        if (timelineData == null) return;

        var dialogueArr = timelineData._Dialogue_Data;
        if (dialogueArr == null || dialogueArr.Length == 0) return;

        var showingIndex = Mathf.Clamp(branchTimelineParam.showingIndex, 0, dialogueArr.Length);
        var dialogueData = timelineData._Dialogue_Data[showingIndex];
        if (dialogueData == null) return;

        tmpu.text = Localizer.GetLocalizedStringTalk(Localizer.SheetType.STRING, dialogueData._Dialogue);
    }
}