using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class CharacterGrayScalerBehaviour : PlayableBehaviour
{
    public TimelineParam.SEC_Setter setter = new TimelineParam.SEC_Setter() { start = 1f, end = 1f };

    public float GetEvaluatedSaturationValue(float normalized) => setter.GetEvaluatedValue(normalized);
}
