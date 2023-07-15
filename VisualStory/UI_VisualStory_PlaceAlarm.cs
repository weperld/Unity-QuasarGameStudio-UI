using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Debug = COA_DEBUG.Debug;

public class UI_VisualStory_PlaceAlarm : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI tmpu_Name;
    [SerializeField] private RectTransform rtf_TweenTarget;
    [SerializeField] private DOTweenAnimation moveTweener;
    [SerializeField] private DOTweenAnimation fadeTweener;
    [SerializeField] private float tweenDuration = 4f;
    [SerializeField] private AnimationCurve tweenCurve;

    private void OnValidate()
    {
        if (tweenDuration < 0f) tweenDuration = 0f;
        if (moveTweener != null) { moveTweener.duration = tweenDuration; moveTweener.easeType = Ease.INTERNAL_Custom; moveTweener.easeCurve = tweenCurve; }
        if (fadeTweener != null) { fadeTweener.duration = tweenDuration; fadeTweener.easeType = Ease.INTERNAL_Custom; fadeTweener.easeCurve = tweenCurve; }
    }

    public void SetAndPlay(string name)
    {
        if (rtf_TweenTarget == null) return;

        if (tmpu_Name != null) tmpu_Name.text = name;
        LayoutRebuilder.ForceRebuildLayoutImmediate(rtf_TweenTarget);

        if (moveTweener != null)
        {
            moveTweener.duration = tweenDuration;
            moveTweener.ControlTweener(TweenHelper.ControlState.RESET,
                tweener =>
                {
                    var width = rtf_TweenTarget.rect.width;
                    var pos = rtf_TweenTarget.anchoredPosition;
                    pos.x = -(width + 50f);
                    rtf_TweenTarget.anchoredPosition = pos;
                },
                tweener => tweener.ControlTweener(TweenHelper.ControlState.PLAY));
        }
        if (fadeTweener != null)
        {
            fadeTweener.duration = tweenDuration;
            fadeTweener.ControlTweener(TweenHelper.ControlState.RESET,
                tweener =>
                {
                    if (rtf_TweenTarget.TryGetComponent<CanvasGroup>(out var group))
                    {
                        group.alpha = 0f;
                    }
                },
                tweener => tweener.ControlTweener(TweenHelper.ControlState.PLAY));
        }
    }
}