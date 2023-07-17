using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[TrackColor(0.3499294f, 0.5607843f, 0.5607843f)]
[TrackClipType(typeof(RectTransformTweenClip))]
[TrackBindingType(typeof(RectTransform))]
public class RectTransformTweenTrack : TrackAsset
{
    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        return ScriptPlayable<RectTransformTweenMixerBehaviour>.Create (graph, inputCount);
    }
}
