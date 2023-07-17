using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class VStoryActiveEmoticonMixerBehaviour : PlayableBehaviour
{
    // NOTE: This function is called at runtime and edit time.  Keep that in mind when setting the values of properties.
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        Character_UI_Illust_SG_ForVisualStory trackBinding = playerData as Character_UI_Illust_SG_ForVisualStory;

        if (!trackBinding)
            return;

        int inputCount = playable.GetInputCount ();

        for (int i = 0; i < inputCount; i++)
        {
            float inputWeight = playable.GetInputWeight(i);
            ScriptPlayable<VStoryActiveEmoticonBehaviour> inputPlayable = (ScriptPlayable<VStoryActiveEmoticonBehaviour>)playable.GetInput(i);
            VStoryActiveEmoticonBehaviour input = inputPlayable.GetBehaviour ();
            
            // Use the above variables to process each frame of this playable.
            
        }
    }
}
