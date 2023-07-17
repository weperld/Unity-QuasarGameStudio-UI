using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using Debug = COA_DEBUG.Debug;
using DG.Tweening;
using static TweenHelper;

[Serializable]
public class TweenerPlayControlBehaviour : PlayableBehaviour
{
    public ControlState whatToDo;

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
        tweener.ControlTweener(whatToDo);
    }
}
