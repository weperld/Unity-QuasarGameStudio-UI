using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using Debug = COA_DEBUG.Debug;

[TrackColor(0.38f, 0.38f, 0.38f)]
[TrackClipType(typeof(AddSkeletonAnimationClip))]
[TrackBindingType(typeof(Character_UI_Illust_SG_ForVisualStory))]
public class AddSkeletonAnimationTrack : TrackAsset
{
    protected override void OnCreateClip(TimelineClip clip)
    {
        clip.duration = TimelineParam.DEFAULT_CLIP_CREATE_DURATION;
    }

    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        return ScriptPlayable<AddSkeletonAnimationMixerBehaviour>.Create (graph, inputCount);
    }
}
