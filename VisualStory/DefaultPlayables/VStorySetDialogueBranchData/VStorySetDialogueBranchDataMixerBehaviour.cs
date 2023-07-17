using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class VStorySetDialogueBranchDataMixerBehaviour : PlayableBehaviour
{
    private UIBase_VisualStory m_TrackBinding;
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        m_TrackBinding = playerData as UIBase_VisualStory;
    }
    public override void OnPlayableDestroy(Playable playable)
    {
        if (m_TrackBinding == null) return;
        m_TrackBinding.HideTimelineBranch();
    }
}
