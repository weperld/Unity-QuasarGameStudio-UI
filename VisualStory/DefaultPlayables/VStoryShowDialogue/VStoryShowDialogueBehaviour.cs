using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class VStoryShowDialogueBehaviour : PlayableBehaviour
{
    public TimelineParam.VStoryDialogue setter = new TimelineParam.VStoryDialogue();

    private bool isFirstFrame = true;
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        if (!isFirstFrame) return;
        isFirstFrame = false;

        var trackBinding = playerData as UI_VisualStory_Dialogue;
        if (trackBinding == null) return;

        trackBinding.ShowDialogue(setter);
    }
}
