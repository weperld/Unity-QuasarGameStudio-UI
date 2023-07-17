using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using Debug = COA_DEBUG.Debug;

[Serializable]
public class CanvasGroupFaderClip : PlayableAsset, ITimelineClipAsset
{
    public CanvasGroupFaderBehaviour template = new CanvasGroupFaderBehaviour();

    public ClipCaps clipCaps
    {
        get { return ClipCaps.Blending; }
    }

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<CanvasGroupFaderBehaviour>.Create(graph, template);
        CanvasGroupFaderBehaviour clone = playable.GetBehaviour();
        return playable;
    }
}
