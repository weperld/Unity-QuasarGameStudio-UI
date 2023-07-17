using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[TrackColor(1f, 0.2290724f, 0.2122642f)]
[TrackClipType(typeof(VStorySetDialogueBranchDataClip))]
[TrackBindingType(typeof(UIBase_VisualStory))]
public class VStorySetDialogueBranchDataTrack : TrackAsset
{
    protected override void OnCreateClip(TimelineClip clip)
    {
        clip.duration = 1d;
    }
    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        return ScriptPlayable<VStorySetDialogueBranchDataMixerBehaviour>.Create (graph, inputCount);
    }
}
