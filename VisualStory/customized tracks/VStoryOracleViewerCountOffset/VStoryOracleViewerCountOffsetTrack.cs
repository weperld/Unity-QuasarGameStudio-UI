using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[TrackColor(0.07399998f, 0.4722941f, 0.8705882f)]
[TrackClipType(typeof(VStoryOracleViewerCountOffsetClip))]
[TrackBindingType(typeof(UIBase_VisualStory_Oracle))]
public class VStoryOracleViewerCountOffsetTrack : TrackAsset
{
    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        return ScriptPlayable<VStoryOracleViewerCountOffsetMixerBehaviour>.Create (graph, inputCount);
    }
}
