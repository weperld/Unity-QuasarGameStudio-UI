using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[TrackColor(0f, 0.8396716f, 1f)]
[TrackClipType(typeof(SetAsLastSiblingClip))]
public class SetAsLastSiblingTrack : TrackAsset
{
    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        return ScriptPlayable<SetAsLastSiblingMixerBehaviour>.Create (graph, inputCount);
    }
    protected override void OnCreateClip(TimelineClip clip)
    {
        clip.duration = 1f;
    }
}
