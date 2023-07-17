using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class VStoryOracleViewerCountOffsetClip : PlayableAsset, ITimelineClipAsset
{
    public VStoryOracleViewerCountOffsetBehaviour template = new VStoryOracleViewerCountOffsetBehaviour ();

    public ClipCaps clipCaps
    {
        get { return ClipCaps.None; }
    }

    public override Playable CreatePlayable (PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<VStoryOracleViewerCountOffsetBehaviour>.Create (graph, template);
        VStoryOracleViewerCountOffsetBehaviour clone = playable.GetBehaviour ();
        return playable;
    }
}
