using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class VStoryScreenFocusAndZoomClip : PlayableAsset, ITimelineClipAsset
{
    public VStoryScreenFocusAndZoomBehaviour template = new VStoryScreenFocusAndZoomBehaviour ();

    public ClipCaps clipCaps
    {
        get { return ClipCaps.None; }
    }

    public override Playable CreatePlayable (PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<VStoryScreenFocusAndZoomBehaviour>.Create (graph, template);
        VStoryScreenFocusAndZoomBehaviour clone = playable.GetBehaviour ();
        return playable;
    }

    private void OnValidate()
    {
        var arr = template.setter.arr_FocusTargetIndex;
        if (arr == null || arr.Length == 0) template.setter.arr_FocusTargetIndex = new int[1];

        for(int i =0; i < arr.Length; i++)
        {
            arr[i] = Mathf.Clamp(arr[i], 0, VisualStoryHelper.USING_CHARACTER_MAX_COUNT - 1);
        }
    }
}
