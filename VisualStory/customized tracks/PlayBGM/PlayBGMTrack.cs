using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[TrackColor(1f, 0.9332834f, 0.4669811f)]
[TrackClipType(typeof(PlayBGMClip))]
public class PlayBGMTrack : TrackAsset
{
    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        return ScriptPlayable<PlayBGMMixerBehaviour>.Create (graph, inputCount);
    }
}
