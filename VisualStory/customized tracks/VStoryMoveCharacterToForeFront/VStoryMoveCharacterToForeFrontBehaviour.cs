using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class VStoryMoveCharacterToForeFrontBehaviour : PlayableBehaviour
{
    public TimelineParam.ForeFrontTargetIndex setter = new TimelineParam.ForeFrontTargetIndex();

    private bool processing = false;

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        if (processing) return;
        processing = true;

        var binding = playerData as UIBase_VisualStory;
        if (binding == null) return;

        foreach (var v in setter.list_Target)
            binding.MoveCharacterToForeFrontByIndex(v);
    }
}
