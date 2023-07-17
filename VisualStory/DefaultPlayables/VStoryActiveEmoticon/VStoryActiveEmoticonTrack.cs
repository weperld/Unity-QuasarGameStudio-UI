using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[TrackColor(1f, 0.9315507f, 0.3160377f)]
[TrackClipType(typeof(VStoryActiveEmoticonClip))]
[TrackBindingType(typeof(Character_UI_Illust_SG_ForVisualStory))]
public class VStoryActiveEmoticonTrack : TrackAsset
{
    protected override void OnCreateClip(TimelineClip clip)
    {
        clip.duration = 1f;
    }

    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        return ScriptPlayable<VStoryActiveEmoticonMixerBehaviour>.Create (graph, inputCount);
    }
}
