using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class VStoryOracleViewerCountOffsetBehaviour : PlayableBehaviour
{
    public AnimationCurve offsetCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

    public float EvaluatedValue(float normalized)
    {
        return offsetCurve.Evaluate(normalized);
    }
}
