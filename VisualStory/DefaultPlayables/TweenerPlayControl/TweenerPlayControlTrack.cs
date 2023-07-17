using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using Debug = COA_DEBUG.Debug;
using DG.Tweening;

[TrackColor(0.6064269f, 1f, 0.504717f)]
[TrackClipType(typeof(TweenerPlayControlClip))]
[TrackBindingType(typeof(DOTweenAnimation))]
public class TweenerPlayControlTrack : TrackAsset
{
    protected override void OnCreateClip(TimelineClip clip)
    {
        clip.duration = TimelineParam.DEFAULT_CLIP_CREATE_DURATION;
    }
    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        return ScriptPlayable<TweenerPlayControlMixerBehaviour>.Create (graph, inputCount);
    }
}
