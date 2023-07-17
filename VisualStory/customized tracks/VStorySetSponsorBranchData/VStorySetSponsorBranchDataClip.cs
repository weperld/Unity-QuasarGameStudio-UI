using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class VStorySetSponsorBranchDataClip : PlayableAsset, ITimelineClipAsset
{
    public VStorySetSponsorBranchDataBehaviour template = new VStorySetSponsorBranchDataBehaviour ();

    public ClipCaps clipCaps
    {
        get { return ClipCaps.None; }
    }

    public override Playable CreatePlayable (PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<VStorySetSponsorBranchDataBehaviour>.Create (graph, template);
        VStorySetSponsorBranchDataBehaviour clone = playable.GetBehaviour ();
        if (template.list_OracleSponItem.Count == 0) template.list_OracleSponItem.Add(new TimelineParam.OracleSponsorBranch());
        return playable;
    }

    private void OnValidate()
    {
        if (template.list_OracleSponItem.Count == 0) template.list_OracleSponItem.Add(new TimelineParam.OracleSponsorBranch());
    }
}
