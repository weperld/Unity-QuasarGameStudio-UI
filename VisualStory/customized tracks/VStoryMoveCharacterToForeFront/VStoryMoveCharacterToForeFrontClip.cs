using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class VStoryMoveCharacterToForeFrontClip : PlayableAsset, ITimelineClipAsset
{
    public VStoryMoveCharacterToForeFrontBehaviour template = new VStoryMoveCharacterToForeFrontBehaviour();

    public ClipCaps clipCaps
    {
        get { return ClipCaps.None; }
    }

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<VStoryMoveCharacterToForeFrontBehaviour>.Create(graph, template);
        VStoryMoveCharacterToForeFrontBehaviour clone = playable.GetBehaviour();
        return playable;
    }

    private void OnValidate()
    {
        for (int i = 0; i < template.setter.list_Target.Count; i++)
        {
            var value = template.setter.list_Target[i];
            template.setter.Set(i, Mathf.Max(value, 0));
        }
    }
}
