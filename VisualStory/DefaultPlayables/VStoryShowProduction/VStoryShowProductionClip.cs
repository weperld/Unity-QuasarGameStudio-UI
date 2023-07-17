using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class VStoryShowProductionClip : PlayableAsset, ITimelineClipAsset
{
    public VStoryShowProductionBehaviour template = new VStoryShowProductionBehaviour ();

    public ClipCaps clipCaps
    {
        get { return ClipCaps.None; }
    }

    public override Playable CreatePlayable (PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<VStoryShowProductionBehaviour>.Create (graph, template);
        VStoryShowProductionBehaviour clone = playable.GetBehaviour ();
        return playable;
    }
}
