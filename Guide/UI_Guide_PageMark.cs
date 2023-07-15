using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Debug = COA_DEBUG.Debug;

public class UI_Guide_PageMark : MonoBehaviour
{
    [SerializeField] private GameObject go_OffMark;
    [SerializeField] private GameObject go_OnMark;

    public void SetOnOff(bool on)
    {
        go_OffMark?.SetActive(!on);
        go_OnMark?.SetActive(on);
    }

    public void SetActive(bool active)
    {
        gameObject.SetActive(active);
    }
}