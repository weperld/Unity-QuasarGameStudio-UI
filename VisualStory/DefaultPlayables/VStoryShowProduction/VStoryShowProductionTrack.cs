using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[TrackColor(0f, 0.7f, 1f)]
[TrackClipType(typeof(VStoryShowProductionClip))]
[TrackBindingType(typeof(UIBase_VisualStory))]
public class VStoryShowProductionTrack : TrackAsset
{
    protected override void OnCreateClip(TimelineClip clip)
    {
        clip.duration = TimelineParam.DEFAULT_CLIP_CREATE_DURATION;
    }

    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        return ScriptPlayable<VStoryShowProductionMixerBehaviour>.Create (graph, inputCount);
    }
}
