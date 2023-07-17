using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using DG.Tweening;
using static TweenHelper;

public class ShakeTweenMixerBehaviour : PlayableBehaviour
{
    private ControlState currentPlayState = ControlState.STOP;
    private DOTweenAnimation tweener;
    private Vector3 defaultValue;

    private bool isFirst = true;
    public override void OnGraphStart(Playable playable)
    {
        currentPlayState = ControlState.STOP;
    }
    // NOTE: This function is called at runtime and edit time.  Keep that in mind when setting the values of properties.
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        tweener = playerData as DOTweenAnimation;

        if (!tweener)
            return;

        if (isFirst)
        {
            isFirst = false;
            defaultValue = tweener.GetTargetGO().transform.localPosition;
        }

        int inputCount = playable.GetInputCount();
        float totalWeight = 0f;
        float totalWeightedVibePerSec = 0f;
        Vector3 totalWeightedAxisPower = Vector3.zero;

        for (int i = 0; i < inputCount; i++)
        {
            float inputWeight = playable.GetInputWeight(i);
            totalWeight += inputWeight;
            ScriptPlayable<ShakeTweenBehaviour> inputPlayable = (ScriptPlayable<ShakeTweenBehaviour>)playable.GetInput(i);
            ShakeTweenBehaviour input = inputPlayable.GetBehaviour();
            var setter = input.setter;

            var normalized = (float)(inputPlayable.GetTime() / inputPlayable.GetDuration());
            var evaluatedPower = setter.powerCurve.Evaluate(normalized) * setter.vibePower;
            totalWeightedVibePerSec += setter.vibePerSec * inputWeight;
            totalWeightedAxisPower += setter.axisPowerRate * evaluatedPower * inputWeight;
        }

        tweener.endValueV3 = totalWeightedAxisPower;
        tweener.optionalInt0 = (int)totalWeightedVibePerSec;

        if (totalWeight < 0.01f && currentPlayState != ControlState.STOP) tweener.ControlTweener(ControlState.STOP, OnBeforeStop, OnAfterStop);
        if (totalWeight >= 0.01f && currentPlayState != ControlState.PLAY) tweener.ControlTweener(ControlState.RESET, OnBeforeReset, OnAfterReset);
    }

    private void OnStepComplete()
    {
        tweener.ControlTweener(ControlState.RESET, OnBeforeReset, OnAfterReset);
    }
    private void OnBeforeReset(DOTweenAnimation tweener)
    {
        if (tweener == null) return;

        currentPlayState = ControlState.PLAY;
        tweener.ControlTweener(ControlState.STOP);

        var target = tweener.GetTargetGO();
        if (target == null) return;
        target.transform.localPosition = defaultValue;
    }
    private void OnAfterReset(DOTweenAnimation tweener)
    {
        if (tweener == null) return;

        tweener.tween.OnStepComplete(OnStepComplete);
        tweener.ControlTweener(ControlState.PLAY);
    }
    private void OnBeforeStop(DOTweenAnimation tweener)
    {
        currentPlayState = ControlState.STOP;
    }
    private void OnAfterStop(DOTweenAnimation tweener)
    {
        var target = tweener.GetTargetGO();
        if (target == null) return;
        target.transform.localPosition = defaultValue;
    }
}
