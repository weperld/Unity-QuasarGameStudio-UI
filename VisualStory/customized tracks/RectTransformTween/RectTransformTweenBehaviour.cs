using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class RectTransformTweenBehaviour : PlayableBehaviour
{
    public TimelineParam.RectTransformTween setter = new TimelineParam.RectTransformTween();

    private Vector2 startPos = Vector2.zero;
    private Vector2 endPos = Vector2.zero;

    private bool isFirstFrame = true;

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        var trackBinding = playerData as RectTransform;
        if (trackBinding == null) return;

        if (!isFirstFrame) return;
        isFirstFrame = false;

        startPos = trackBinding.anchoredPosition;
        endPos = new Vector2(ScreenSetupData.instance.basisScreenWidth * 0.5f * setter.endValue.x,
            ScreenSetupData.instance.basisScreenHeight * 0.5f * setter.endValue.y);
    }

    public bool GetEvaluatedTweenPosition(float normalized, out Vector2 result)
    {
        if (isFirstFrame) { result = Vector2.zero; return false; }

        var curveX = setter._Curve_X;
        var curveY = setter._Curve_Y;

        var eval_X = curveX.Evaluate(normalized);
        var eval_Y = curveY.Evaluate(normalized);

        var directionVector = endPos - startPos;
        result.x = startPos.x + directionVector.x * eval_X;
        result.y = startPos.y + directionVector.y * eval_Y;

        return true;
    }
}
