using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using Debug = COA_DEBUG.Debug;

[Serializable]
public class SetSkeletonAnimationClip : PlayableAsset, ITimelineClipAsset
{
    public SetSkeletonAnimationBehaviour template = new SetSkeletonAnimationBehaviour ();

    public ClipCaps clipCaps
    {
        get { return ClipCaps.None; }
    }

    public override Playable CreatePlayable (PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<SetSkeletonAnimationBehaviour>.Create (graph, template);
        SetSkeletonAnimationBehaviour clone = playable.GetBehaviour ();
        return playable;
    }

    private void OnValidate()
    {
        if (template.setupParam.loopCount == 0) template.setupParam.loopCount = 1;
        else if (template.setupParam.loopCount < -1) template.setupParam.loopCount = -1;
    }
}
