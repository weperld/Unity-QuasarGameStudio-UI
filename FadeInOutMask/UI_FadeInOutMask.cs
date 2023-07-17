using System;
using System.Collections;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Debug = COA_DEBUG.Debug;

[RequireComponent(typeof(CanvasGroup))]
public class UI_FadeInOutMask : MonoBehaviour
{
    private const float FADE_TRANSITION_MIN = 0f;
    private const float FADE_TRANSITION_MAX = 10f;
    private const float MASK_ALPHA_MIN = 0f;
    private const float MASK_ALPHA_MAX = 1f;

    public enum FadeType
    {
        DEFAULT_OPTION,
        ONLY_IN_USING
    }
    public enum FadeInOut
    {
        IN,
        OUT
    }

    [Serializable]
    public class AlphaMinMax
    {
        [SerializeField, Range(MASK_ALPHA_MIN, MASK_ALPHA_MAX)] private float min;
        [SerializeField, Range(MASK_ALPHA_MIN, MASK_ALPHA_MAX)] private float max;

        public float MIN => min;
        public float MAX => max;

        public void SetMin(float value)
        {
            min = Mathf.Clamp(value, MASK_ALPHA_MIN, max);
        }
        public void SetMax(float value)
        {
            max = Mathf.Clamp(value, min, MASK_ALPHA_MAX);
        }
    }
    [Serializable]
    public struct FadeTriggerData
    {
        public EventTriggerType eventTriggerType;
        public FadeInOut fadeInOut;
    }


    #region Inspector
    public RectTransform rtf_FadeDetectRect;
    public FadeType fadeType;
    [Range(FADE_TRANSITION_MIN, FADE_TRANSITION_MAX)]
    public float fadeTransition;
    public AlphaMinMax alphaMinMax;

    public FadeTriggerData[] fadeTriggerData_Array;
    #endregion

    #region Variables
    private EventTrigger eventTrigger;
    private CanvasGroup mask;
    private CanvasGroup _Mask
    {
        get
        {
            if (mask == null) mask = GetComponent<CanvasGroup>();
            return mask;
        }
    }
    #endregion

    #region Base Method
    private void Awake()
    {
        AddFadeComponent();
    }

    private void OnDisable()
    {
        if (fadeInOutEnumerator != null)
        {
            StopCoroutine(fadeInOutEnumerator);
            fadeInOutEnumerator = null;
        }
        ResetMaskVal();
    }

    private float minBeforeChange;
    private float maxBeforeChange;
    private void OnValidate()
    {
        if (minBeforeChange != alphaMinMax.MIN)
        {
            minBeforeChange = alphaMinMax.MIN;
            if (alphaMinMax.MIN > alphaMinMax.MAX)
                alphaMinMax.SetMax(alphaMinMax.MIN);
        }
        if (maxBeforeChange != alphaMinMax.MAX)
        {
            maxBeforeChange = alphaMinMax.MAX;
            if (alphaMinMax.MIN > alphaMinMax.MAX)
                alphaMinMax.SetMin(alphaMinMax.MAX);
        }
    }
    #endregion

    #region Fade Component Setup Method
    private void AddFadeComponent()
    {
        if (fadeTriggerData_Array == null || fadeTriggerData_Array.Length == 0) return;

        if (rtf_FadeDetectRect == null) rtf_FadeDetectRect = this.GetComponent<RectTransform>();

        if (!rtf_FadeDetectRect.TryGetComponent(out eventTrigger))
            eventTrigger = rtf_FadeDetectRect.gameObject.AddComponent<EventTrigger>();

        if (fadeTriggerData_Array != null)
            foreach (var eventData in fadeTriggerData_Array)
            {
                var tmpTrigger = eventTrigger.triggers.Find(t => t.eventID == eventData.eventTriggerType);
                UnityAction<BaseEventData> action = eventData.fadeInOut switch
                {
                    FadeInOut.IN => FadeIn,
                    FadeInOut.OUT => FadeOut,
                    _ => FadeIn,
                };

                if (tmpTrigger != null) tmpTrigger.callback.AddListener(action);
                else
                {
                    tmpTrigger = new EventTrigger.Entry();
                    tmpTrigger.eventID = eventData.eventTriggerType;
                    tmpTrigger.callback.AddListener(action);
                    eventTrigger.triggers.Add(tmpTrigger);
                }
            }

        ResetMaskVal();
    }

    private void ResetMaskVal()
    {
        switch (fadeType)
        {
            case FadeType.DEFAULT_OPTION:
                _Mask.alpha = 1f;
                break;
            case FadeType.ONLY_IN_USING:
                _Mask.alpha = alphaMinMax.MIN;
                break;
        }
    }
    #endregion

    #region Fade Method
    private void FadeIn(BaseEventData data) => ExecuteFadeInOut(true);
    private void FadeOut(BaseEventData data) => ExecuteFadeInOut(false);

    private IEnumerator fadeInOutEnumerator;
    private void ExecuteFadeInOut(bool fadeIn)
    {
        if (fadeInOutEnumerator != null)
        {
            StopCoroutine(fadeInOutEnumerator);
            fadeInOutEnumerator = null;
        }

        if (fadeType == FadeType.DEFAULT_OPTION) { ResetMaskVal(); return; }

        fadeInOutEnumerator = FadeInOutCorout(fadeIn);
        StartCoroutine(fadeInOutEnumerator);
    }
    private IEnumerator FadeInOutCorout(bool fadeIn)
    {
        fadeTransition = Mathf.Clamp(fadeTransition, FADE_TRANSITION_MIN, FADE_TRANSITION_MAX);
        float targetMaskVal = fadeIn ? alphaMinMax.MAX : alphaMinMax.MIN;
        if (fadeTransition <= 0f)
        {
            _Mask.alpha = targetMaskVal;
            fadeInOutEnumerator = null;
            yield break;
        }

        var fadeTime = 0f;
        while (fadeTime < fadeTransition)
        {
            yield return null;

            fadeTime += Time.deltaTime;
            _Mask.alpha = Mathf.MoveTowards(_Mask.alpha, targetMaskVal, Time.deltaTime / fadeTransition);
            if (Mathf.Approximately(_Mask.alpha, targetMaskVal)) break;
        }
        _Mask.alpha = targetMaskVal;
        fadeInOutEnumerator = null;
    }
    #endregion
}
