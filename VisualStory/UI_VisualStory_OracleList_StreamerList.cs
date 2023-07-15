using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Debug = COA_DEBUG.Debug;

public class UI_VisualStory_OracleList_StreamerList : MonoBehaviour
{
    [SerializeField] private VisualStoryHelper.OracleCategory category;
    [SerializeField] private Button btn_ShowList;
    [SerializeField] private RectTransform rtf_ListRoot;
    [SerializeField] private RectTransform rtf_ListView;
    [SerializeField] private GameObject go_OffShow;
    [SerializeField] private GameObject go_OnShow;
    [SerializeField] private UI_VisualStory_OracleList_Streamer[] streamers;
    [SerializeField] private RectTransform rtf_StreamersViewContent;
    [SerializeField] private float showAndHideDuration;
    [SerializeField] private AnimationCurve showAndHideCurve;

    public VisualStoryHelper.OracleCategory _Category => category;

    private Action<VisualStoryHelper.OracleCategory> onClickShowListAction;

    private float _ListHeight => rtf_ListView == null ? 0f : rtf_ListView.rect.height;
    private IEnumerator curveListHeightCorout;

    private void OnDisable()
    {
        if (curveListHeightCorout != null)
        {
            StopCoroutine(curveListHeightCorout);
            curveListHeightCorout = null;
        }
    }

    public void SetListButtonListener(Action<VisualStoryHelper.OracleCategory> action)
    {
        onClickShowListAction = action;
        UIUtil.ResetAndAddListener(btn_ShowList, OnClickShowList);
    }

    public void Set(VisualStoryHelper.OracleCategory category, OracleListInfo oracleList)
    {
        if (streamers == null) return;

        for (int i = 0; i < streamers.Length; i++)
        {
            var streamer = streamers[i];
            if (streamer == null) continue;

            streamer.Set(category, oracleList != null ? oracleList[i] : null);
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(rtf_StreamersViewContent);
    }

    public void SetOnClickStreamerListener(Action<VisualStoryInfo> action)
    {
        if (streamers == null) return;
        foreach (var streamer in streamers)
            streamer?.SetOnClickAction(action);
    }

    public void Show(bool immediately)
    {
        go_OffShow?.SetActive(false);
        go_OnShow?.SetActive(true);
        ChangeListViewSize(true, immediately);
    }
    public void Hide(bool immediately)
    {
        go_OffShow?.SetActive(true);
        go_OnShow?.SetActive(false);
        ChangeListViewSize(false, immediately);
    }

    private void OnClickShowList()
    {
        onClickShowListAction?.Invoke(_Category);
    }

    private void ChangeListViewSize(bool show, bool immediately)
    {
        if (rtf_ListRoot == null) return;

        var endHeight = show ? _ListHeight : 0f;
        var startHeight = rtf_ListRoot.sizeDelta.y;

        if (immediately) SetListViewHeight(endHeight);
        else
        {
            if (curveListHeightCorout != null) StopCoroutine(curveListHeightCorout);
            curveListHeightCorout = CurveListHeightCorout(startHeight, endHeight);
            StartCoroutine(curveListHeightCorout);
        }
    }
    private IEnumerator CurveListHeightCorout(float start, float end)
    {
        float curvedTime = 0f;
        while (curvedTime < showAndHideDuration)
        {
            yield return null;
            curvedTime += Time.deltaTime;

            SetListViewHeight(Mathf.Lerp(start, end, showAndHideCurve.Evaluate(curvedTime / showAndHideDuration)));
        }
        SetListViewHeight(end);
        curveListHeightCorout = null;
    }
    private void SetListViewHeight(float value)
    {
        if (rtf_ListRoot == null) return;

        var size = rtf_ListRoot.sizeDelta;
        size.y = value;
        rtf_ListRoot.sizeDelta = size;
    }
}