using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class SetAsLastSiblingBehaviour : PlayableBehaviour
{
    public Transform target;

    private bool processing = false;
    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        //processing = false;
    }
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        if (processing || target == null) return;
        processing = true;

        target.SetAsLastSibling();
    }
}
