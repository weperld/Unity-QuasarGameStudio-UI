using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using Debug = COA_DEBUG.Debug;

[TrackColor(0.5499099f, 0.5549549f, 0.56f)]
[TrackClipType(typeof(SetSkeletonAnimationClip))]
[TrackBindingType(typeof(Character_UI_Illust_SG_ForVisualStory))]
public class SetSkeletonAnimationTrack : TrackAsset
{
    protected override void OnCreateClip(TimelineClip clip)
    {
        clip.duration = TimelineParam.DEFAULT_CLIP_CREATE_DURATION;
    }

    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        return ScriptPlayable<SetSkeletonAnimationMixerBehaviour>.Create (graph, inputCount);
    }
}
