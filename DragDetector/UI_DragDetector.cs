using System;
using UnityEngine;
using UnityEngine.EventSystems;
using Debug = COA_DEBUG.Debug;

public class UI_DragDetector : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    private Vector2 prevDragPosition;
    private Vector2 nextDragPosition;

    /// <summary>(prev, current)</summary>
    public Action<Vector2, Vector2, PointerEventData> onDragging;
    /// <summary>(prev, current)</summary>
    public Action<Vector2, Vector2, PointerEventData> onBeginDrag;
    /// <summary>(prev, current)</summary>
    public Action<Vector2, Vector2, PointerEventData> onEndDrag;
    /// <summary>(prev, current, current all touch datas)</summary>
    public Action<float, float, Touch[]> onSpread;
    public Action<PointerEventData> onClick;

    private bool isDragging;
    private bool isPinch;

    private void OnDisable()
    {
        isDragging = false;
        isPinch = false;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (isDragging) return;

        isDragging = true;
#if UNITY_ANDROID
        if (Input.touchCount == 0)
        {
            prevDragPosition = eventData.position - eventData.delta;
            nextDragPosition = eventData.position;
        }
        else
        {
            var firstTouch = Input.GetTouch(0);
            if (firstTouch.fingerId != eventData.pointerId) return;

            prevDragPosition = firstTouch.position - firstTouch.deltaPosition;
            nextDragPosition = firstTouch.position;
        }
#elif UNITY_EDITOR
        prevDragPosition = eventData.position - eventData.delta;
        nextDragPosition = eventData.position;
#endif
        onBeginDrag?.Invoke(prevDragPosition, nextDragPosition, eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging) return;

#if UNITY_ANDROID
        if (Input.touchCount == 0)
        {
            prevDragPosition = eventData.position - eventData.delta;
            nextDragPosition = eventData.position;
        }
        else
        {
            var firstTouch = Input.GetTouch(0);
            if (firstTouch.fingerId != eventData.pointerId) return;

            prevDragPosition = firstTouch.position - firstTouch.deltaPosition;
            nextDragPosition = firstTouch.position;
        }
#elif UNITY_EDITOR
        prevDragPosition = eventData.position - eventData.delta;
        nextDragPosition = eventData.position;
#endif
        onDragging?.Invoke(prevDragPosition, nextDragPosition, eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isDragging) return;

        isDragging = false;
#if UNITY_ANDROID
        if (Input.touchCount == 0)
        {
            prevDragPosition = eventData.position - eventData.delta;
            nextDragPosition = eventData.position;
        }
        else
        {
            var firstTouch = Input.GetTouch(0);
            if (firstTouch.fingerId != eventData.pointerId) return;

            prevDragPosition = firstTouch.position - firstTouch.deltaPosition;
            nextDragPosition = firstTouch.position;
        }
#elif UNITY_EDITOR
        prevDragPosition = eventData.position - eventData.delta;
        nextDragPosition = eventData.position;
#endif
        onEndDrag?.Invoke(prevDragPosition, nextDragPosition, eventData);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //#if UNITY_ANDROID
        //        var firstTouch = Input.GetTouch(0);
        //        if (firstTouch.fingerId != eventData.pointerId) return;
        //#endif
        if (isDragging || isPinch)
        {
            return;
        }
        onClick?.Invoke(eventData);
    }

    private void Update()
    {
#if UNITY_ANDROID
        if (Input.touchCount >= 2)
        {
            var first = Input.GetTouch(0);
            var second = Input.GetTouch(1);

            var firstDeltaPos = first.deltaPosition;
            var secondDeltaPos = second.deltaPosition;

            var firstPrevPos = first.position - firstDeltaPos;
            var secondPrevPos = second.position - secondDeltaPos;

            var prevTouchDis = (firstPrevPos - secondPrevPos).magnitude;
            var curTouchDis = (first.position - second.position).magnitude;

            if (!isPinch) isPinch = true;
            onSpread?.Invoke(prevTouchDis, curTouchDis, GetAllTouches());
        }
#elif UNITY_EDITOR
        if (Input.GetKey(KeyCode.KeypadPlus) || Input.GetKey(KeyCode.Equals))
        {
            if (!isPinch) isPinch = true;
            onSpread?.Invoke(0f, Time.deltaTime * 1000f, GetAllTouches());
        }
        else if (Input.GetKey(KeyCode.KeypadMinus) || Input.GetKey(KeyCode.Minus))
        {
            if (!isPinch) isPinch = true;
            onSpread?.Invoke(0f, -Time.deltaTime * 1000f, GetAllTouches());
        }
        else if (isPinch) isPinch = false;
#endif
    }

    private void LateUpdate()
    {
#if UNITY_ANDROID
        if (isPinch && Input.touchCount == 0) isPinch = false;
#endif
    }

    public Touch[] GetAllTouches()
    {
        var cnt = Input.touchCount;
        var touches = new Touch[cnt];
        for (int i = 0; i < cnt; i++)
            touches[i] = Input.GetTouch(i);

        return touches;
    }
}