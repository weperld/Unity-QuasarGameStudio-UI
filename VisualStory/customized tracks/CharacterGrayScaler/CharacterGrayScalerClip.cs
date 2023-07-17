using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class CharacterGrayScalerClip : PlayableAsset, ITimelineClipAsset
{
    public CharacterGrayScalerBehaviour template = new CharacterGrayScalerBehaviour ();

    public ClipCaps clipCaps
    {
        get { return ClipCaps.Blending; }
    }

    public override Playable CreatePlayable (PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<CharacterGrayScalerBehaviour>.Create (graph, template);
        CharacterGrayScalerBehaviour clone = playable.GetBehaviour ();
        return playable;
    }
}
