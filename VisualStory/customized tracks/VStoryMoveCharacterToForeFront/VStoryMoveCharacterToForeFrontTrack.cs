using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[TrackColor(0.2971698f, 0.7933915f, 1f)]
[TrackClipType(typeof(VStoryMoveCharacterToForeFrontClip))]
[TrackBindingType(typeof(UIBase_VisualStory))]
public class VStoryMoveCharacterToForeFrontTrack : TrackAsset
{
    protected override void OnCreateClip(TimelineClip clip)
    {
        clip.duration = 1f;
    }
    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        return ScriptPlayable<VStoryMoveCharacterToForeFrontMixerBehaviour>.Create (graph, inputCount);
    }
}
