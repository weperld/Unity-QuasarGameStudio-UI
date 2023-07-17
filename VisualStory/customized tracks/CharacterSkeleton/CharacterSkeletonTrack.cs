using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using Debug = COA_DEBUG.Debug;

[TrackColor(0.855f, 0.8623f, 0.87f)]
[TrackClipType(typeof(CharacterSkeletonClip))]
[TrackBindingType(typeof(Character_UI_Illust_SG_ForVisualStory))]
public class CharacterSkeletonTrack : TrackAsset
{
    protected override void OnCreateClip(TimelineClip clip)
    {
        clip.duration = TimelineParam.DEFAULT_CLIP_CREATE_DURATION;
    }

    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        return ScriptPlayable<CharacterSkeletonMixerBehaviour>.Create (graph, inputCount);
    }
}
