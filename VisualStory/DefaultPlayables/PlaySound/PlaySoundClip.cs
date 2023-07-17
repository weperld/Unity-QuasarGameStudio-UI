using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class PlaySoundClip : PlayableAsset, ITimelineClipAsset
{
    public PlaySoundBehaviour template = new PlaySoundBehaviour ();

    public ClipCaps clipCaps
    {
        get { return ClipCaps.Blending; }
    }

    public override Playable CreatePlayable (PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<PlaySoundBehaviour>.Create (graph, template);
        PlaySoundBehaviour clone = playable.GetBehaviour ();
        return playable;
    }
}
