using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using Debug = COA_DEBUG.Debug;
using DG.Tweening;

[Serializable]
public class TweenerPlayControlClip : PlayableAsset, ITimelineClipAsset
{
    public TweenerPlayControlBehaviour template = new TweenerPlayControlBehaviour ();

    public ClipCaps clipCaps
    {
        get { return ClipCaps.None; }
    }

    public override Playable CreatePlayable (PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<TweenerPlayControlBehaviour>.Create (graph, template);
        TweenerPlayControlBehaviour clone = playable.GetBehaviour ();
        return playable;
    }
}
