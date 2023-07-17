using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using Debug = COA_DEBUG.Debug;

[TrackColor(1f, 0.764706f, 0.4117647f)]
[TrackClipType(typeof(CanvasGroupFaderClip))]
[TrackBindingType(typeof(CanvasGroup))]
public class CanvasGroupFaderTrack : TrackAsset
{
    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        return ScriptPlayable<CanvasGroupFaderMixerBehaviour>.Create (graph, inputCount);
    }

    public override void GatherProperties(PlayableDirector director, IPropertyCollector driver)
    {
#if UNITY_EDITOR
        var comp = director.GetGenericBinding(this) as CanvasGroup;
        if (comp == null)
            return;
        var so = new UnityEditor.SerializedObject(comp);
        var iter = so.GetIterator();
        while (iter.NextVisible(true))
        {
            if (iter.hasVisibleChildren)
                continue;
            driver.AddFromName<CanvasGroup>(comp.gameObject, iter.propertyPath);
        }
#endif
        base.GatherProperties(director, driver);
    }
}
