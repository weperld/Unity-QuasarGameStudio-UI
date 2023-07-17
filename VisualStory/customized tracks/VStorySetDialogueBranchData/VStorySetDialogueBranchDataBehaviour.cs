using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

[Serializable]
public class VStorySetDialogueBranchDataBehaviour : PlayableBehaviour
{
    public List<TimelineParam.TimelineBranch> list_TimelineBranch = new List<TimelineParam.TimelineBranch>();

    private bool processing = false;
    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        //processing = false;
    }
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        if (processing) return;
        processing = true;

        var ui = playerData as UIBase_VisualStory;
        if (ui == null || list_TimelineBranch == null || list_TimelineBranch.Count == 0) return;

        ui.SetTimelineBranchData(list_TimelineBranch.ToArray());
    }
}
