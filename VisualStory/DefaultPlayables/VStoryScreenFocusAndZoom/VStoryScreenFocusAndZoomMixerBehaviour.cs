using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class VStoryScreenFocusAndZoomMixerBehaviour : PlayableBehaviour
{
    private int prevInputIndex = -1;
    private int currentInputIndex = -1;

    private UI_VisualStory_Screen m_TrackBinding;

    public override void OnGraphStart(Playable playable)
    {
        prevInputIndex = -1;
        currentInputIndex = -1;
    }

    // NOTE: This function is called at runtime and edit time.  Keep that in mind when setting the values of properties.
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        m_TrackBinding = playerData as UI_VisualStory_Screen;

        if (!m_TrackBinding)
            return;

        currentInputIndex = -1;
        int inputCount = playable.GetInputCount();
        float totalWeight = 0f;
        for (int i = 0; i < inputCount; i++)
        {
            float inputWeight = playable.GetInputWeight(i);
            ScriptPlayable<VStoryScreenFocusAndZoomBehaviour> inputPlayable = (ScriptPlayable<VStoryScreenFocusAndZoomBehaviour>)playable.GetInput(i);
            VStoryScreenFocusAndZoomBehaviour input = inputPlayable.GetBehaviour();

            // Use the above variables to process each frame of this playable.
            totalWeight += inputWeight;
            if (Mathf.Approximately(inputWeight, 1f) && input != null)
            {
                var playedTime = (float)inputPlayable.GetTime();
                var normalized = (float)(playedTime / inputPlayable.GetDuration());
                input.ShiftValues(playedTime, normalized, m_TrackBinding);

                currentInputIndex = i;
                break;
            }
        }
        m_TrackBinding.ChangeScreenResetable(Mathf.Approximately(totalWeight, 0f));

        if (currentInputIndex == -1 && prevInputIndex >= 0)
        {
            var inputPlayable = (ScriptPlayable<VStoryScreenFocusAndZoomBehaviour>)playable.GetInput(prevInputIndex);
            var input = inputPlayable.GetBehaviour();
            if (input != null) input.ShiftValues((float)inputPlayable.GetDuration(), 1f, m_TrackBinding);
        }

        prevInputIndex = currentInputIndex;
    }
}
