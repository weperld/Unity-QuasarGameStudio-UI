using UnityEngine;
using UnityEngine.Playables;
using Debug = COA_DEBUG.Debug;

public class CanvasGroupFaderMixerBehaviour : PlayableBehaviour
{
    private float m_DefAlpha;
    private CanvasGroup m_TrackBinding;
    private bool init = false;

    // NOTE: This function is called at runtime and edit time.  Keep that in mind when setting the values of properties.
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        m_TrackBinding = playerData as CanvasGroup;

        if (!m_TrackBinding)
            return;

        if (!init)
        {
            init = true;
            m_DefAlpha = m_TrackBinding.alpha;
        }

        int inputCount = playable.GetInputCount();

        float blendedAlpha = 0f;
        float totalWeight = 0f;
        for (int i = 0; i < inputCount; i++)
        {
            float inputWeight = playable.GetInputWeight(i);
            ScriptPlayable<CanvasGroupFaderBehaviour> inputPlayable = (ScriptPlayable<CanvasGroupFaderBehaviour>)playable.GetInput(i);
            CanvasGroupFaderBehaviour input = inputPlayable.GetBehaviour();

            // Use the above variables to process each frame of this playable.
            var normalized = (float)(inputPlayable.GetTime() / inputPlayable.GetDuration());
            var evaluatedAlpha = input.GetEvaluatedAlphaValue(normalized);

            blendedAlpha += evaluatedAlpha * inputWeight;
            totalWeight += inputWeight;
        }

        m_TrackBinding.alpha = blendedAlpha + m_DefAlpha * (1f - totalWeight);
    }

    public override void OnPlayableDestroy(Playable playable)
    {
        init = false;
        if (m_TrackBinding == null) return;
        m_TrackBinding.alpha = m_DefAlpha;
    }
}
