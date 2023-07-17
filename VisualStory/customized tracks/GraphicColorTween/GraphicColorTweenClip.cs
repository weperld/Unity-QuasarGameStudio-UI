using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class GraphicColorTweenClip : PlayableAsset, ITimelineClipAsset
{
    public GraphicColorTweenBehaviour template = new GraphicColorTweenBehaviour ();

    public ClipCaps clipCaps
    {
        get { return ClipCaps.Blending; }
    }

    public override Playable CreatePlayable (PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<GraphicColorTweenBehaviour>.Create (graph, template);
        GraphicColorTweenBehaviour clone = playable.GetBehaviour ();
        return playable;
    }
}
