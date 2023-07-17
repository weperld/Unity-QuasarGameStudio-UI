using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using DG.Tweening;
using Debug = COA_DEBUG.Debug;

[Serializable]
public class TMPTweenerClip : PlayableAsset, ITimelineClipAsset
{
    public TMPTweenerBehaviour template = new TMPTweenerBehaviour ();

    public ClipCaps clipCaps
    {
        get { return ClipCaps.None; }
    }

    public override Playable CreatePlayable (PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<TMPTweenerBehaviour>.Create (graph, template);
        TMPTweenerBehaviour clone = playable.GetBehaviour ();
        return playable;
    }

    private void OnValidate()
    {
        if (template.setter.textPrintSpeed < 0f) template.setter.textPrintSpeed = 0f;
    }
}
