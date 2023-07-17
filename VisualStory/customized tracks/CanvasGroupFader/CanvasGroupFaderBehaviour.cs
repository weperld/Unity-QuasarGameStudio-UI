using System;
using UnityEngine;
using UnityEngine.Playables;
using Debug = COA_DEBUG.Debug;

[Serializable]
public class CanvasGroupFaderBehaviour : PlayableBehaviour
{
    public TimelineParam.SEC_Setter setter = new TimelineParam.SEC_Setter() { start = 1f, end = 1f };

    public float GetEvaluatedAlphaValue(float normalized) => setter.GetEvaluatedValue(normalized);
}
