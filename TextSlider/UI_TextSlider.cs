using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Debug = COA_DEBUG.Debug;

[DisallowMultipleComponent]
public class UI_TextSlider : MonoBehaviour
{
    public enum SlideDirection
    {
        Left_to_Right,
        Right_to_Left,
        Bottom_to_Top,
        Top_to_Bottom,
    }

    #region Inspector
    [SerializeField] private SlideDirection currentDirection = SlideDirection.Left_to_Right;

    [SerializeField] private TextMeshProUGUI tmpu00;
    [SerializeField] private TextMeshProUGUI tmpu01;

    public bool resumeOnEnable = true;
    [Range(0f, 9999f)] public float space;
    [Range(0f, 9999f)] public float speed;
    [SerializeField] private float offset_H = 0f;
    [SerializeField] private float offset_V = 0f;

    public bool useNotSlidableOptionValues = false;
    [SerializeField] private Vector2 notSlidableAnchor_Min = Vector2.one * 0.5f;
    [SerializeField] private Vector2 notSlidableAnchor_Max = Vector2.one * 0.5f;
    [SerializeField] private Vector2 notSlidablePivot = Vector2.one * 0.5f;
    #endregion

    public RectTransform rectTransform => GetComponent<RectTransform>();
    private SlideDirection prevDirection = SlideDirection.Left_to_Right;
    public SlideDirection _SlideDirection
    {
        get
        {
            return currentDirection;
        }
        set
        {
            currentDirection = value;
            if (prevDirection == value) return;

            prevDirection = value;
            if (Application.isPlaying) OnChangeSlideDirection(value);
        }
    }

    public bool _IsHorizontal => _SlideDirection == SlideDirection.Left_to_Right
        || _SlideDirection == SlideDirection.Right_to_Left;
    public bool _NegativeDelta => _SlideDirection == SlideDirection.Right_to_Left
        || _SlideDirection == SlideDirection.Top_to_Bottom;
    public float _ViewSize => _IsHorizontal ? rectTransform.rect.width : rectTransform.rect.height;
    public bool _Slidable
    {
        get
        {
            var textSize = tmpu00.preferredWidth;
            var ret = textSize >= _ViewSize;

            return ret;
        }
    }
    public float _Offset
    {
        get => _IsHorizontal ? offset_H : offset_V;
        set
        {
            if (_IsHorizontal) offset_H = value;
            else offset_V = value;
        }
    }
    public Vector2 _RestartPosition
    {
        get
        {
            var size = _IsHorizontal ? tmpu00.preferredWidth : tmpu00.preferredHeight;
            var pos = (size + space) * (_NegativeDelta ? 1f : -1f);
            var restart = _IsHorizontal ? new Vector2(pos, _Offset) : new Vector2(_Offset, pos);
            return restart;
        }
    }
    public Vector2 _TurningPosition
    {
        get
        {
            var turningPoint = (_IsHorizontal ? _RestartPosition.x : _RestartPosition.y) * -1f;
            var turningPosition = _IsHorizontal ? new Vector2(turningPoint, _Offset) : new Vector2(_Offset, turningPoint);
            return turningPosition;
        }
    }
    public Vector2 _NotSlidableAnchor_Min
    {
        get => notSlidableAnchor_Min;
        set => Set_NotSlidableAnchor_Min(value);
    }
    public Vector2 _NotSlidableAnchor_Max
    {
        get => notSlidableAnchor_Max;
        set => Set_NotSlidableAnchor_Max(value);
    }
    public Vector2 _NotSlidablePivot
    {
        get => notSlidablePivot;
        set => Set_NotSlidablePivot(value);
    }

    private Vector2 prev_NotSlidableAnchor_Min = Vector2.one * 0.5f;
    private Vector2 prev_NotSlidableAnchor_Max = Vector2.one * 0.5f;

    private void OnValidate()
    {
        prevDirection = currentDirection;
        SetContentSizeFitterOptions();

        var pos = tmpu00.rectTransform.anchoredPosition;
        if (_IsHorizontal) pos.y = _Offset;
        else pos.x = _Offset;
        tmpu00.rectTransform.anchoredPosition = pos;

        pos = tmpu01.rectTransform.anchoredPosition;
        if (_IsHorizontal) pos.y = _Offset;
        else pos.x = _Offset;
        tmpu01.rectTransform.anchoredPosition = pos;

        if (Application.isPlaying)
        {
            RestartSlide();
        }
    }

    private void Awake()
    {
        TMPro_EventManager.TEXT_CHANGED_EVENT.Add(OnTextChange);
    }
    private void OnDestroy()
    {
        TMPro_EventManager.TEXT_CHANGED_EVENT.Remove(OnTextChange);
    }

    #region Public
    public void SetText(string str)
    {
        tmpu00.text = str;
    }
    public void Pause()
    {
        if (slideCorout == null) return;
        StopCoroutine(slideCorout);
    }
    public void Resume()
    {
        if (slideCorout == null) return;
        StartCoroutine(slideCorout);
    }
    public void RestartSlide()
    {
        tmpu01.text = tmpu00.text;
        StopSlideCorout();
        SetTextAnchorAndPivot();
        SetTextStartPosition();
        if (_Slidable) StartSlideCorout();
    }

    public void Set_NotSlidableAnchor_Min(Vector2 value)
    {
        value.x = Mathf.Clamp(value.x, 0f, 1f);
        value.y = Mathf.Clamp(value.y, 0f, 1f);

        notSlidableAnchor_Min.x = value.x;
        notSlidableAnchor_Min.y = value.y;

        if (notSlidableAnchor_Max.x < notSlidableAnchor_Min.x) notSlidableAnchor_Max.x = value.x;
        if (notSlidableAnchor_Max.y < notSlidableAnchor_Min.y) notSlidableAnchor_Max.y = value.y;
    }
    public void Set_NotSlidableAnchor_Max(Vector2 value)
    {
        value.x = Mathf.Clamp(value.x, 0f, 1f);
        value.y = Mathf.Clamp(value.y, 0f, 1f);

        notSlidableAnchor_Max.x = value.x;
        notSlidableAnchor_Max.y = value.y;

        if (notSlidableAnchor_Min.x > notSlidableAnchor_Max.x) notSlidableAnchor_Min.x = value.x;
        if (notSlidableAnchor_Min.y > notSlidableAnchor_Max.y) notSlidableAnchor_Min.y = value.y;
    }
    public void Set_NotSlidablePivot(Vector2 value)
    {
        value.x = Mathf.Clamp(value.x, 0f, 1f);
        value.y = Mathf.Clamp(value.y, 0f, 1f);

        notSlidablePivot.x = value.x;
        notSlidablePivot.y = value.y;
    }
    #endregion

    #region Private
    private void OnChangeSlideDirection(SlideDirection changed)
    {
        SetContentSizeFitterOptions();
        RestartSlide();
    }
    private void OnTextChange(System.Object obj)
    {
        TextMeshProUGUI tmpu = obj as TextMeshProUGUI;
        if (tmpu00 == tmpu)
        {
            RestartSlide();
        }
    }

    private void SetContentSizeFitterOptions()
    {
        if (!tmpu00.TryGetComponent<ContentSizeFitter>(out var fitter00))
            fitter00 = tmpu00.gameObject.AddComponent<ContentSizeFitter>();
        if (!tmpu01.TryGetComponent<ContentSizeFitter>(out var fitter01))
            fitter01 = tmpu01.gameObject.AddComponent<ContentSizeFitter>();

        fitter00.horizontalFit = fitter01.horizontalFit = _IsHorizontal ? ContentSizeFitter.FitMode.PreferredSize : ContentSizeFitter.FitMode.Unconstrained;
        fitter00.verticalFit = fitter01.verticalFit = _IsHorizontal ? ContentSizeFitter.FitMode.Unconstrained : ContentSizeFitter.FitMode.PreferredSize;
    }
    private void SetTextAnchorAndPivot()
    {
        var v2 = _SlideDirection switch
        {
            SlideDirection.Left_to_Right => new Vector2(1f, 0.5f),
            SlideDirection.Right_to_Left => new Vector2(0f, 0.5f),
            SlideDirection.Bottom_to_Top => new Vector2(0.5f, 1f),
            SlideDirection.Top_to_Bottom => new Vector2(0.5f, 0f),
            _ => new Vector2(0f, 0.5f),
        };

        if (_Slidable || !useNotSlidableOptionValues)
        {
            tmpu00.rectTransform.anchorMax = v2;
            tmpu00.rectTransform.anchorMin = v2;
            tmpu00.rectTransform.pivot = v2;

            tmpu01.rectTransform.anchorMax = v2;
            tmpu01.rectTransform.anchorMin = v2;
            tmpu01.rectTransform.pivot = v2;
        }
        else
        {
            tmpu00.rectTransform.anchorMax = notSlidableAnchor_Min;
            tmpu00.rectTransform.anchorMin = notSlidableAnchor_Max;
            tmpu00.rectTransform.pivot = notSlidablePivot;
        }
    }
    private void SetTextStartPosition()
    {
        if (!tmpu01.TryGetComponent<CanvasGroup>(out var canvasGroup))
            canvasGroup = tmpu01.gameObject.AddComponent<CanvasGroup>();
        canvasGroup.alpha = _Slidable ? 1f : 0f;

        tmpu00.rectTransform.anchoredPosition = (_IsHorizontal ? Vector2.up : Vector2.right) * _Offset;

        if (_Slidable)
        {
            var size = _IsHorizontal ? tmpu00.preferredWidth : tmpu00.preferredHeight;
            var pos = (size + space) * (_NegativeDelta ? 1f : -1f);
            var coord = _IsHorizontal ? new Vector2(pos, _Offset) : new Vector2(_Offset, pos);
            tmpu01.rectTransform.anchoredPosition = coord;
        }
    }

    private IEnumerator slideCorout = null;
    private void StopSlideCorout()
    {
        if (slideCorout == null) return;
        StopCoroutine(slideCorout);
        slideCorout = null;
    }
    private void StartSlideCorout()
    {
        StopSlideCorout();
        slideCorout = SlideCorout();

        if (!gameObject.activeInHierarchy) return;
        StartCoroutine(slideCorout);
    }

    private IEnumerator SlideCorout()
    {
        while (true)
        {
            yield return null;
            RtfMoveToNextPos(tmpu00.rectTransform);
            RtfMoveToNextPos(tmpu01.rectTransform);
        }
    }
    private void RtfMoveToNextPos(RectTransform rtf)
    {
        var pos = rtf.anchoredPosition;
        pos = Vector2.MoveTowards(pos, _TurningPosition, speed * Time.deltaTime);

        var coordinateValue = _IsHorizontal ? pos.x : pos.y;
        var turningPoint = _IsHorizontal ? _TurningPosition.x : _TurningPosition.y;
        var checkReachToTurningPoint = _NegativeDelta ? coordinateValue <= turningPoint : coordinateValue >= turningPoint;

        if (checkReachToTurningPoint) pos = _RestartPosition;
        rtf.anchoredPosition = pos;
    }
    #endregion
}