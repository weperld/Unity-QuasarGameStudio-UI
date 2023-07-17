using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[TrackColor(0.7224197f, 0.2869999f, 1f)]
[TrackClipType(typeof(VStoryScreenFocusAndZoomClip))]
[TrackBindingType(typeof(UI_VisualStory_Screen))]
public class VStoryScreenFocusAndZoomTrack : TrackAsset
{
    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        return ScriptPlayable<VStoryScreenFocusAndZoomMixerBehaviour>.Create (graph, inputCount);
    }
}
