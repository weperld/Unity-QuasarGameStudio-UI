using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class VStoryActiveEmoticonClip : PlayableAsset, ITimelineClipAsset
{
    public VStoryActiveEmoticonBehaviour template = new VStoryActiveEmoticonBehaviour ();

    public ClipCaps clipCaps
    {
        get { return ClipCaps.None; }
    }

    public override Playable CreatePlayable (PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<VStoryActiveEmoticonBehaviour>.Create (graph, template);
        VStoryActiveEmoticonBehaviour clone = playable.GetBehaviour ();
        return playable;
    }
}
