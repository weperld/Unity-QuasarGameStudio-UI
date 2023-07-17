using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class VStoryActiveEmoticonBehaviour : PlayableBehaviour
{
    public VisualStoryHelper.Emoticon emoticon;
    [Range(0f, 1f)] public float volume = 1f;

    private bool processing = false;
    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        //processing = false;
    }
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        if (processing) return;
        processing = true;

        var sg = playerData as Character_UI_Illust_SG_ForVisualStory;
        if (sg is null) return;

        sg.ShowEmoticon(emoticon, volume);
    }

    public override void OnPlayableDestroy(Playable playable)
    {
        processing = false;
    }
}
