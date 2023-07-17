using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class VStorySetSponsorBranchDataBehaviour : PlayableBehaviour
{
    public List<TimelineParam.OracleSponsorBranch> list_OracleSponItem = new List<TimelineParam.OracleSponsorBranch>();

    private bool processing;

    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        //processing = false;
    }
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        var binding = playerData as UIBase_VisualStory_Oracle;
        if (binding == null) return;

        if (processing) return;
        processing = true;

        binding.SetSponsorBranchData(list_OracleSponItem.ToArray());
    }
}
