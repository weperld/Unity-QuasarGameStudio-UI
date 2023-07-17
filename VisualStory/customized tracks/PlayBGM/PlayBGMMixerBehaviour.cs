using AudioSystem;
using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class PlayBGMMixerBehaviour : PlayableBehaviour
{
    // NOTE: This function is called at runtime and edit time.  Keep that in mind when setting the values of properties.
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        int inputCount = playable.GetInputCount();

        float totalWeight = 0f;
        PlayBGMBehaviour current = null;
        float normalized = 0f;

        for (int i = 0; i < inputCount; i++)
        {
            float inputWeight = playable.GetInputWeight(i);
            ScriptPlayable<PlayBGMBehaviour> inputPlayable = (ScriptPlayable<PlayBGMBehaviour>)playable.GetInput(i);
            PlayBGMBehaviour input = inputPlayable.GetBehaviour();

            // Use the above variables to process each frame of this playable.
            totalWeight += inputWeight;
            if (inputWeight > 0f)
            {
                current = input;
                normalized = (float)(inputPlayable.GetTime() / inputPlayable.GetDuration());
            }
        }

        if (current != null) current.PlayBGM(normalized);
        else if (!AudioManager.IsDetroying) AudioManager._instance.StopBGM();
    }
}
