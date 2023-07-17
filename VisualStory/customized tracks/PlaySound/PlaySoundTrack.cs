using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[TrackColor(0.759f, 0.728135f, 0.161667f)]
[TrackClipType(typeof(PlaySoundClip))]
public class PlaySoundTrack : TrackAsset
{
    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        return ScriptPlayable<PlaySoundMixerBehaviour>.Create (graph, inputCount);
    }
}
