using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Debug = COA_DEBUG.Debug;

[RequireComponent(typeof(Toggle))]
public class UI_ToggleSlider : MonoBehaviour
{
    private Toggle m_Toggle;
    private Toggle _Toggle
    {
        get
        {
            if (m_Toggle == null) m_Toggle = GetComponent<Toggle>();
            return m_Toggle;
        }
    }

    [SerializeField] private GameObject go_On;
    [SerializeField] private GameObject go_Off;
    [SerializeField] private RectTransform rtf_Slider;
    [SerializeField] private bool reverse;
    [SerializeField, Range(0f, 1f)] private float delay = 0f;

    private IEnumerator slideCo;

    private void OnEnable()
    {
        SetSliderAnchorPos(GetTargetPos(_Toggle.isOn));
        SetOnOffObj(_Toggle.isOn);
    }

    private void OnDisable()
    {
        if (slideCo != null) StopCoroutine(slideCo);
        slideCo = null;
    }

    public void MoveSlider(bool isOn)
    {
        if (!gameObject.activeInHierarchy) return;

        if (slideCo != null) StopCoroutine(slideCo);
        slideCo = MoveSliderCo(isOn);
        StartCoroutine(slideCo);
        SetOnOffObj(isOn);
    }
    private IEnumerator MoveSliderCo(bool isOn)
    {
        if (rtf_Slider == null) yield break;

        float targetPos = GetTargetPos(isOn);
        if (delay <= 0f) SetSliderAnchorPos(targetPos);
        else
            while (!Mathf.Approximately(rtf_Slider.anchorMin.x, targetPos))
            {
                yield return null;
                SetSliderAnchorPos(Mathf.MoveTowards(rtf_Slider.anchorMin.x, targetPos, Time.deltaTime / delay));
            }

        slideCo = null;
    }

    private float GetTargetPos(bool isOn)
    {
        float targetPos = (isOn ? 1f : 0f);
        if (reverse) targetPos = 1f - targetPos;

        return targetPos;
    }
    private void SetSliderAnchorPos(float value)
    {
        if (rtf_Slider == null) return;

        rtf_Slider.anchorMin = new Vector2(value, 0f);
        rtf_Slider.anchorMax = new Vector2(value, 1f);
        var pos = rtf_Slider.anchoredPosition;
        pos.x = 0f;
        rtf_Slider.anchoredPosition = pos;
    }
    private void SetOnOffObj(bool isOn)
    {
        go_On?.SetActive(!reverse == isOn);
        go_Off?.SetActive(reverse == isOn);
    }
}