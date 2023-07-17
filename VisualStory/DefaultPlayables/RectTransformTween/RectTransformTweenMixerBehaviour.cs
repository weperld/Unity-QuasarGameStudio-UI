using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class RectTransformTweenMixerBehaviour : PlayableBehaviour
{
    // NOTE: This function is called at runtime and edit time.  Keep that in mind when setting the values of properties.
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        var trackBinding = playerData as RectTransform;

        if (!trackBinding)
            return;

        var defaultPosition = trackBinding.anchoredPosition;

        int inputCount = playable.GetInputCount();
        float totalWeight = 0f;
        Vector2 blendedMovePos = Vector2.zero;

        for (int i = 0; i < inputCount; i++)
        {
            float inputWeight = playable.GetInputWeight(i);
            ScriptPlayable<RectTransformTweenBehaviour> inputPlayable = (ScriptPlayable<RectTransformTweenBehaviour>)playable.GetInput(i);
            RectTransformTweenBehaviour input = inputPlayable.GetBehaviour();
            if (input == null) continue;

            // Use the above variables to process each frame of this playable.
            var normalizedTime = (float)(inputPlayable.GetTime() / inputPlayable.GetDuration());
            if (input.GetEvaluatedTweenPosition(normalizedTime, out var result))
            {
                totalWeight += inputWeight;
                blendedMovePos = result * inputWeight;
            }
        }

        var nextPos = Vector2.Lerp(defaultPosition, blendedMovePos, totalWeight);
        trackBinding.anchoredPosition = nextPos;
    }
}
