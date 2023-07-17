using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class VStorySetDialogueBranchDataClip : PlayableAsset, ITimelineClipAsset
{
    public VStorySetDialogueBranchDataBehaviour template = new VStorySetDialogueBranchDataBehaviour();

    public ClipCaps clipCaps
    {
        get { return ClipCaps.None; }
    }

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<VStorySetDialogueBranchDataBehaviour>.Create(graph, template);
        VStorySetDialogueBranchDataBehaviour clone = playable.GetBehaviour();
        if (template.list_TimelineBranch.Count == 0) template.list_TimelineBranch.Add(new TimelineParam.TimelineBranch());
        return playable;
    }

    private void OnValidate()
    {
        if (template.list_TimelineBranch.Count == 0) template.list_TimelineBranch.Add(new TimelineParam.TimelineBranch());

        foreach (var element in template.list_TimelineBranch)
        {
            if (element.showingIndex < 0) element.showingIndex = 0;
        }
    }
}
