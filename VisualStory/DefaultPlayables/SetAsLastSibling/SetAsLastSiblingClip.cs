using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class SetAsLastSiblingClip : PlayableAsset, ITimelineClipAsset
{
    public SetAsLastSiblingBehaviour template = new SetAsLastSiblingBehaviour();
    public ExposedReference<Transform> target;

    public ClipCaps clipCaps
    {
        get { return ClipCaps.None; }
    }

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<SetAsLastSiblingBehaviour>.Create(graph, template);
        SetAsLastSiblingBehaviour clone = playable.GetBehaviour();
        clone.target = target.Resolve(graph.GetResolver());
        return playable;
    }
}
