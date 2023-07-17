using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using DG.Tweening;
using Debug = COA_DEBUG.Debug;


[TrackColor(0.3020135f, 1f, 0.1208054f)]
[TrackClipType(typeof(TMPTweenerClip))]
[TrackBindingType(typeof(DOTweenAnimation))]
public class TMPTweenerTrack : TrackAsset
{
    protected override void OnCreateClip(TimelineClip clip)
    {
        clip.duration = TimelineParam.DEFAULT_CLIP_CREATE_DURATION;
    }

    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        return ScriptPlayable<TMPTweenerMixerBehaviour>.Create (graph, inputCount);
    }
}
