using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class GraphicColorTweenBehaviour : PlayableBehaviour
{
    public Color startColor = Color.white;
    public Color endColor = Color.white;
    public bool individualControl = false;
    public AnimationCurve colorCurve00 = AnimationCurve.Linear(0f, 0f, 1f, 1f);
    public AnimationCurve colorCurve01 = AnimationCurve.Linear(0f, 0f, 1f, 1f);
    public AnimationCurve colorCurve02 = AnimationCurve.Linear(0f, 0f, 1f, 1f);
    public AnimationCurve colorCurve03 = AnimationCurve.Linear(0f, 0f, 1f, 1f);

    public Color EvaluatedColorValue(float normalizedTime)
    {
        Color ret = Color.white;
        if (individualControl)
        {
            ret.r = Mathf.Lerp(startColor.r, endColor.r, colorCurve00.Evaluate(normalizedTime));
            ret.g = Mathf.Lerp(startColor.g, endColor.g, colorCurve01.Evaluate(normalizedTime));
            ret.b = Mathf.Lerp(startColor.b, endColor.b, colorCurve02.Evaluate(normalizedTime));
            ret.a = Mathf.Lerp(startColor.a, endColor.a, colorCurve03.Evaluate(normalizedTime));
        }
        else
        {
            ret.r = Mathf.Lerp(startColor.r, endColor.r, colorCurve00.Evaluate(normalizedTime));
            ret.g = Mathf.Lerp(startColor.g, endColor.g, colorCurve00.Evaluate(normalizedTime));
            ret.b = Mathf.Lerp(startColor.b, endColor.b, colorCurve00.Evaluate(normalizedTime));
            ret.a = Mathf.Lerp(startColor.a, endColor.a, colorCurve00.Evaluate(normalizedTime));
        }

        return ret;
    }
}
