using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class VStoryScreenFocusAndZoomBehaviour : PlayableBehaviour
{
    public TimelineParam.ScreenView setter = new TimelineParam.ScreenView();

    private bool clipBegin;
    private float prevTime;

    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        clipBegin = true;
    }

    public void ShiftValues(float playedTime, float normalizedTime, UI_VisualStory_Screen target)
    {
        if (clipBegin) prevTime = 0f;

        var isStartFrame = clipBegin && Mathf.Approximately(prevTime, 0f);
        target.TweenZoomAndFocus(isStartFrame, normalizedTime, setter);

        clipBegin = false;
        prevTime = playedTime;
    }
}
