using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Spine.Unity;
using Debug = COA_DEBUG.Debug;
using static VisualStoryHelper;
using UnityEngine.EventSystems;
using LeTai.Asset.TranslucentImage;

public class UI_VisualStory_Screen : MonoBehaviour
{
    [Serializable]
    private class ScreenManualResetData
    {
        [HideInInspector] public float scale;
        public RectTransform rtf_Reset;
    }

    #region Inspector
    [SerializeField] private UI_DragDetector dragDetector;
    [SerializeField] private RectTransform rtf_characterViewScaleTarget;
    [SerializeField] private RectTransform rtf_Reset;
    [SerializeField] private RectTransform rtf_ResetFollower;
    [SerializeField] private Character_UI_Illust_SG_ForVisualStory[] characterSpines;
    [SerializeField] private RectTransform rtf_Bg;
    [SerializeField] private Image img_SceneIllust;
    #endregion

    #region Variables
    private float scale = DEFAULT_ZOOM_SCALE;
    private float startingScale;
    private float _BgScale => 1f + (scale - 1f) * BG_ZOOM_MULT;

    private ScreenManualResetData screenManualResetData = new ScreenManualResetData() { scale = 1f };

    private Vector2 unscaledFocusStartingPos;
    private Vector2 unscaledFocusDirection;

    private IEnumerator resetScreenFocusAndZoomEnumer;

    private Action<bool> onChangeScreenResetable;

    private bool isDraggable = true;

    private Action onClickAction;
    #endregion

    private void OnValidate()
    {
        if (characterSpines != null)
        {
            for (int i = 0; i < USING_CHARACTER_MAX_COUNT; i++)
            {
                var spine = characterSpines[i];
                if (spine == null) continue;

                spine.gameObject.name = $"#{i}.\tCharacter Spine";
            }
        }
    }
    private void OnDisable()
    {
        if (resetScreenFocusAndZoomEnumer != null) { StopCoroutine(resetScreenFocusAndZoomEnumer); resetScreenFocusAndZoomEnumer = null; }
    }
    private void OnEnable()
    {
        isDraggable = true;
    }

    public void Init(UIParam.VisualStory.ScreenInit param)
    {
        if (rtf_Bg != null)
        {
            rtf_Bg.localScale = Vector2.one;
            rtf_Bg.anchoredPosition = Vector2.zero;
        }
        if (rtf_characterViewScaleTarget != null)
        {
            rtf_characterViewScaleTarget.localScale = Vector2.one;
            rtf_characterViewScaleTarget.anchoredPosition = Vector2.zero;
        }

        if (dragDetector != null)
        {
            dragDetector.onDragging = OnDragging;
            dragDetector.onSpread = OnZooming;
            dragDetector.onClick = OnClick;
        }

        if (param != null)
        {
            onClickAction = param.onClickAction;
        }
    }

    private void SetUnscaledFocusDirection(Vector2 end)
    {
        unscaledFocusStartingPos = rtf_characterViewScaleTarget != null
            ? rtf_characterViewScaleTarget.anchoredPosition
            : Vector2.zero;
        unscaledFocusStartingPos /= scale;

        unscaledFocusDirection = end / scale;
    }
    private Vector2 GetUnscaledInterpolatedFocusDirection(float interpolation)
    {
        return new Vector2(Mathf.Lerp(0f, unscaledFocusDirection.x, interpolation),
            Mathf.Lerp(0f, unscaledFocusDirection.y, interpolation));
    }
    private Vector2 GetUnscaledNextPos(float interpolation)
    {
        return unscaledFocusStartingPos - GetUnscaledInterpolatedFocusDirection(interpolation);
    }

    public void TweenZoomAndFocus(bool isStartFrame,
        float normalizedTime,
        TimelineParam.ScreenView setter)
    {
        if (setter == null) return;

        if (isStartFrame)
        {
            if (resetScreenFocusAndZoomEnumer != null)
            {
                StopCoroutine(resetScreenFocusAndZoomEnumer);
                resetScreenFocusAndZoomEnumer = null;
            }

            Vector2 end = Vector2.zero;
            if (setter.usingFocus)
            {
                if (setter.resetFocus && rtf_characterViewScaleTarget != null)
                    end = rtf_characterViewScaleTarget.anchoredPosition;
                else if (!setter.resetFocus
                    && characterSpines != null
                    && characterSpines.Length != 0)
                {
                    int count = 0;
                    foreach (var index in setter.arr_FocusTargetIndex)
                    {
                        var clamped = Mathf.Clamp(index, 0, characterSpines.Length - 1);
                        var targetFocusRect = characterSpines[clamped]?._CamBoneFollower.GetComponent<RectTransform>();
                        if (targetFocusRect == null) continue;
                        count++;
                        end += targetFocusRect.anchoredPosition;
                    }
                    if (count > 0) end /= count;
                }
            }
            SetUnscaledFocusDirection(end);

            startingScale = scale = Mathf.Clamp(scale, MIN_ZOOM_SCALE, MAX_ZOOM_SCALE);
        }

        if (setter.usingZoom)
            scale = Mathf.Lerp(startingScale, setter.zoom, setter.zoomShiftCurve.Evaluate(normalizedTime));

        var interpolation = setter.usingFocus
            ? setter.focusShiftCurve.Evaluate(normalizedTime)
            : 1f;
        TweenBackgroundRtf(interpolation);
        TweenCharacterViewRtf(interpolation);
        screenManualResetData.scale = scale;
        screenManualResetData.rtf_Reset = rtf_ResetFollower;
        rtf_Reset.transform.position = rtf_characterViewScaleTarget.parent.position;
    }
    private void TweenBackgroundRtf(float focusInterpolation)
    {
        if (rtf_Bg == null) return;

        rtf_Bg.localScale = Vector2.one * _BgScale;
        var unscaledNextPos = GetUnscaledNextPos(focusInterpolation);
        rtf_Bg.anchoredPosition = unscaledNextPos * BG_ZOOM_MULT * _BgScale;
    }
    private void TweenCharacterViewRtf(float focusInterpolation)
    {
        if (rtf_characterViewScaleTarget == null) return;

        rtf_characterViewScaleTarget.localScale = Vector2.one * scale;
        var unscaledNextPos = GetUnscaledNextPos(focusInterpolation);
        rtf_characterViewScaleTarget.anchoredPosition = unscaledNextPos * scale;
    }

    public void SetOnChangeScreenResetable(Action<bool> onChangeScreenResetable)
    {
        this.onChangeScreenResetable = onChangeScreenResetable;
    }
    public void ChangeScreenResetable(bool resetable)
    {
        onChangeScreenResetable?.Invoke(resetable);
        isDraggable = resetable;
    }

    public void HardlyResetFocusAndZoom(AnimationCurve curve, float resetDuration = 1f)
    {
        screenManualResetData.scale = 1f;
        screenManualResetData.rtf_Reset = rtf_characterViewScaleTarget;
        InstantResetFocusAndZoom(curve, resetDuration);
    }
    public void InstantResetFocusAndZoom(AnimationCurve curve, float resetDuration = 1f)
    {
        if (!gameObject.activeInHierarchy || rtf_characterViewScaleTarget == null) return;

        if (curve == null) curve = AnimationCurve.Constant(0f, 1f, 1f);

        var target = screenManualResetData.rtf_Reset;
        var endPos = target != null ? target.anchoredPosition : rtf_characterViewScaleTarget.anchoredPosition;
        SetUnscaledFocusDirection(endPos);
        startingScale = Mathf.Clamp(scale, MIN_ZOOM_SCALE, MAX_ZOOM_SCALE);

        if (resetScreenFocusAndZoomEnumer != null) StopCoroutine(resetScreenFocusAndZoomEnumer);
        resetDuration = Mathf.Max(resetDuration, 0f);
        resetScreenFocusAndZoomEnumer = ResetScreenFocusAndZoomCorout(curve, resetDuration);
        StartCoroutine(resetScreenFocusAndZoomEnumer);
    }
    private IEnumerator ResetScreenFocusAndZoomCorout(AnimationCurve curve, float resetDuration = 1f)
    {
        var time = 0f;
        var curveTotalTime = curve[curve.length - 1].time;
        while (time < resetDuration)
        {
            yield return null;
            time += Time.deltaTime;

            var timeRate = resetDuration <= 0f ? 1f : time / resetDuration;
            var interpolation = curve.Evaluate(timeRate * curveTotalTime);
            scale = Mathf.Lerp(startingScale, screenManualResetData.scale, interpolation);
            TweenBackgroundRtf(interpolation);
            TweenCharacterViewRtf(interpolation);
        }

        var lastInterpolation = curve.Evaluate(curveTotalTime);
        scale = Mathf.Lerp(startingScale, screenManualResetData.scale, lastInterpolation);
        TweenBackgroundRtf(lastInterpolation);
        TweenCharacterViewRtf(lastInterpolation);
        resetScreenFocusAndZoomEnumer = null;
    }

    private void OnDragging(Vector2 p, Vector2 n, PointerEventData ed)
    {
        if (!isDraggable) return;

        var d = n - p;
        SetUnscaledFocusDirection(-d);
        TweenBackgroundRtf(1f);
        TweenCharacterViewRtf(1f);
    }
    private void OnZooming(float p, float n, Touch[] touches)
    {
        if (!isDraggable) return;

        //scale = Mathf.Clamp(scale + d * 0.001f, MIN_ZOOM_SCALE, MAX_ZOOM_SCALE);
        //TweenBackgroundRtf(1f);
        //TweenCharacterViewRtf(1f);
    }
    private void OnClick(PointerEventData ed)
    {
        if (onClickAction != null) onClickAction();
    }

    public void SetSkeletonTimescale(float scale)
    {
        if (characterSpines == null) return;

        foreach (var spine in characterSpines)
            if (spine != null) spine.SetStoryTimeScale(scale);
    }

    public void SetBackgroundImage(Sprite sprite)
    {
        if (rtf_Bg == null || !rtf_Bg.TryGetComponent<Image>(out var bg)) return;
        bg.sprite = sprite;
    }

    public void SetCharacterAsLastSibling(int index)
    {
        if (characterSpines == null
            || characterSpines.Length <= index
            || index < 0) return;

        characterSpines[index].rectTransform.SetAsLastSibling();
    }
}