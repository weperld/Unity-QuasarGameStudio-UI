using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class VStoryShowDialogueClip : PlayableAsset, ITimelineClipAsset
{
    public VStoryShowDialogueBehaviour template = new VStoryShowDialogueBehaviour ();

    public ClipCaps clipCaps
    {
        get { return ClipCaps.None; }
    }

    public override Playable CreatePlayable (PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<VStoryShowDialogueBehaviour>.Create (graph, template);
        VStoryShowDialogueBehaviour clone = playable.GetBehaviour ();
        return playable;
    }

    private void OnValidate()
    {
        if (template.setter.tmpTween.textPrintSpeed < 0f) template.setter.tmpTween.textPrintSpeed = 0f;
    }
}
