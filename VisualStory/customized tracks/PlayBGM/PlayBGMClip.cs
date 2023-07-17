using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class PlayBGMClip : PlayableAsset, ITimelineClipAsset
{
    public PlayBGMBehaviour template = new PlayBGMBehaviour ();

    public ClipCaps clipCaps
    {
        get { return ClipCaps.None; }
    }

    public override Playable CreatePlayable (PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<PlayBGMBehaviour>.Create (graph, template);
        PlayBGMBehaviour clone = playable.GetBehaviour ();
        return playable;
    }
}
