using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using DG.Tweening;
using Debug = COA_DEBUG.Debug;
using static TweenHelper;

[Serializable]
public class TMPTweenerBehaviour : PlayableBehaviour
{
    public TimelineParam.TMPTween setter;

    private bool processing = false;
    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        //processing = false;
    }
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        if (processing) return;
        processing = true;

        var tweener = playerData as DOTweenAnimation;
        if (tweener == null) return;

        tweener.animationType = DOTweenAnimation.AnimationType.Text;
        tweener.duration = setter.textPrintSpeed;
        tweener.isSpeedBased = !setter.useSpeedAsDuration;
        tweener.easeType = setter.tweenEase;
        if (tweener.easeType == Ease.INTERNAL_Custom) tweener.easeCurve = setter.tweenCurve;
        tweener.endValueString = setter.targetString;
        tweener.optionalScrambleMode = setter.tweenScrambleMode;
        if (tweener.optionalScrambleMode == ScrambleMode.Custom) tweener.optionalString = setter.customScramble;

        if (setter.playOnSet) tweener.ControlTweener(ControlState.REWIND_AND_PLAY);
    }
}
