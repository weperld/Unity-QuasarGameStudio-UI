using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.UI;

[TrackColor(0.8705882f, 0.6355294f, 0.7453697f)]
[TrackClipType(typeof(GraphicColorTweenClip))]
[TrackBindingType(typeof(Graphic))]
public class GraphicColorTweenTrack : TrackAsset
{
    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        return ScriptPlayable<GraphicColorTweenMixerBehaviour>.Create (graph, inputCount);
    }
}
