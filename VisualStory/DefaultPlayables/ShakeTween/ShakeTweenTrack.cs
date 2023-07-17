using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using DG.Tweening;

[TrackColor(0f, 0.5471698f, 0.4160186f)]
[TrackClipType(typeof(ShakeTweenClip))]
[TrackBindingType(typeof(DOTweenAnimation))]
public class ShakeTweenTrack : TrackAsset
{
    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        return ScriptPlayable<ShakeTweenMixerBehaviour>.Create (graph, inputCount);
    }
}
