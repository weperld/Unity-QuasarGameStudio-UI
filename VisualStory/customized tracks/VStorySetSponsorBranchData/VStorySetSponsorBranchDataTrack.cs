using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[TrackColor(0.773f, 0.4237386f, 0.416647f)]
[TrackClipType(typeof(VStorySetSponsorBranchDataClip))]
[TrackBindingType(typeof(UIBase_VisualStory_Oracle))]
public class VStorySetSponsorBranchDataTrack : TrackAsset
{
    protected override void OnCreateClip(TimelineClip clip)
    {
        clip.duration = 1d;
    }

    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        return ScriptPlayable<VStorySetSponsorBranchDataMixerBehaviour>.Create (graph, inputCount);
    }
}
