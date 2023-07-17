using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[TrackColor(0.18f, 0.6f, 0.7f)]
[TrackClipType(typeof(VStoryShowDialogueClip))]
[TrackBindingType(typeof(UI_VisualStory_Dialogue))]
public class VStoryShowDialogueTrack : TrackAsset
{
    protected override void OnCreateClip(TimelineClip clip)
    {
        clip.duration = TimelineParam.DEFAULT_CLIP_CREATE_DURATION;
    }
    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        return ScriptPlayable<VStoryShowDialogueMixerBehaviour>.Create (graph, inputCount);
    }
}
