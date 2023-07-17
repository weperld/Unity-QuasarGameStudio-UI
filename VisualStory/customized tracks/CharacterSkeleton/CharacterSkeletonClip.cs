using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using Debug = COA_DEBUG.Debug;

[Serializable]
public class CharacterSkeletonClip : PlayableAsset, ITimelineClipAsset
{
    public CharacterSkeletonBehaviour template = new CharacterSkeletonBehaviour ();

    public ClipCaps clipCaps
    {
        get { return ClipCaps.None; }
    }

    public override Playable CreatePlayable (PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<CharacterSkeletonBehaviour>.Create (graph, template);
        CharacterSkeletonBehaviour clone = playable.GetBehaviour ();

        return playable;
    }

    private void OnValidate()
    {
        if (template.setupParam.loopCount == 0) template.setupParam.loopCount = 1;
        else if (template.setupParam.loopCount < -1) template.setupParam.loopCount = -1;
    }
}
