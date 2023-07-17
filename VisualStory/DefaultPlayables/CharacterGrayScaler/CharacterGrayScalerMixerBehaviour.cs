using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class CharacterGrayScalerMixerBehaviour : PlayableBehaviour
{
    private readonly float m_DefGrayScale = 0f;
    private Character_UI_Illust_SG_ForVisualStory sg;

    // NOTE: This function is called at runtime and edit time.  Keep that in mind when setting the values of properties.
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        sg = playerData as Character_UI_Illust_SG_ForVisualStory;
        if (sg == null) return;

        int inputCount = playable.GetInputCount();
        float blendedGrayScale = 0f;
        float totalWeight = 0f;
        for (int i = 0; i < inputCount; i++)
        {
            float inputWeight = playable.GetInputWeight(i);
            ScriptPlayable<CharacterGrayScalerBehaviour> inputPlayable = (ScriptPlayable<CharacterGrayScalerBehaviour>)playable.GetInput(i);
            CharacterGrayScalerBehaviour input = inputPlayable.GetBehaviour();

            // Use the above variables to process each frame of this playable.
            var noramlized = (float)(inputPlayable.GetTime() / inputPlayable.GetDuration());
            var evaluated = input.GetEvaluatedSaturationValue(noramlized);
            blendedGrayScale += evaluated * inputWeight;
            totalWeight += inputWeight;
        }

        sg.SetGrayScale(blendedGrayScale + m_DefGrayScale * (1f - totalWeight));
    }

    public override void OnPlayableDestroy(Playable playable)
    {
        if (sg == null) return;
        sg.ResetGrayScale();
    }
}
