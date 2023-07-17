using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[TrackColor(0f, 0f, 0f)]
[TrackClipType(typeof(CharacterGrayScalerClip))]
[TrackBindingType(typeof(Character_UI_Illust_SG_ForVisualStory))]
public class CharacterGrayScalerTrack : TrackAsset
{
    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        var mixerPlayable = ScriptPlayable<CharacterGrayScalerMixerBehaviour>.Create(graph, inputCount);
        var behaviour = mixerPlayable.GetBehaviour();

        return mixerPlayable;
    }
}
