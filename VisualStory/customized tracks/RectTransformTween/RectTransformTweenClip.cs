using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class RectTransformTweenClip : PlayableAsset, ITimelineClipAsset
{
    public RectTransformTweenBehaviour template = new RectTransformTweenBehaviour ();

    public ClipCaps clipCaps
    {
        get { return ClipCaps.Blending; }
    }

    public override Playable CreatePlayable (PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<RectTransformTweenBehaviour>.Create (graph, template);
        RectTransformTweenBehaviour clone = playable.GetBehaviour ();
        return playable;
    }
}
