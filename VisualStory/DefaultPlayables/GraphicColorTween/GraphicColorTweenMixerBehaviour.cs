using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.UI;

public class GraphicColorTweenMixerBehaviour : PlayableBehaviour
{
    private Color DEFAULT_COLOR;
    private Graphic m_GraphicRef;

    private bool init = false;

    // NOTE: This function is called at runtime and edit time.  Keep that in mind when setting the values of properties.
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        m_GraphicRef = playerData as Graphic;
        if (m_GraphicRef == null) return;

        if (!init)
        {
            DEFAULT_COLOR = m_GraphicRef.color;
            init = true;
        }

        int inputCount = playable.GetInputCount();
        Color blenedColor = Color.clear;
        float totalWeight = 0f;

        for (int i = 0; i < inputCount; i++)
        {
            float inputWeight = playable.GetInputWeight(i);
            ScriptPlayable<GraphicColorTweenBehaviour> inputPlayable = (ScriptPlayable<GraphicColorTweenBehaviour>)playable.GetInput(i);
            GraphicColorTweenBehaviour input = inputPlayable.GetBehaviour();

            // Use the above variables to process each frame of this playable.
            float normalizedTime = (float)(inputPlayable.GetTime() / inputPlayable.GetDuration());
            var evalutedColor = input.EvaluatedColorValue(normalizedTime);

            blenedColor += evalutedColor * inputWeight;
            totalWeight += inputWeight;
        }

        m_GraphicRef.color = blenedColor + DEFAULT_COLOR * (1f - totalWeight);
    }

    public override void OnPlayableDestroy(Playable playable)
    {
        if (m_GraphicRef == null) return;
        m_GraphicRef.color = DEFAULT_COLOR;
    }
}
