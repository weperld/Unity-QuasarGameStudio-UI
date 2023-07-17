using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class VStoryShowProductionBehaviour : PlayableBehaviour
{
    public TimelineParam.VStoryProduction setter = new TimelineParam.VStoryProduction();

    private bool isFirstFrame = true;

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        if (!isFirstFrame) return;
        isFirstFrame = false;

        var trackBinding = playerData as UIBase_VisualStory;
        if (trackBinding == null) return;

        trackBinding.ShowProductionFromTrack(setter);
    }
}
