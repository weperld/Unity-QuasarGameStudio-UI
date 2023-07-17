using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using DG.Tweening;

[Serializable]
public class ShakeTweenClip : PlayableAsset, ITimelineClipAsset
{
    public ShakeTweenBehaviour template = new ShakeTweenBehaviour ();

    public ClipCaps clipCaps
    {
        get { return ClipCaps.None; }
    }

    public override Playable CreatePlayable (PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<ShakeTweenBehaviour>.Create (graph, template);
        ShakeTweenBehaviour clone = playable.GetBehaviour ();
        return playable;
    }

    private void OnValidate()
    {
        var setter = template.setter;
        setter.vibePerSec = Mathf.Clamp(setter.vibePerSec, 1, 50);
        setter.vibePower = Mathf.Max(setter.vibePower, 0f);
        if (setter.powerCurve == null) setter.powerCurve = AnimationCurve.Constant(0f, 0f, 1f);
        setter.axisPowerRate = new Vector3(
            Mathf.Clamp(setter.axisPowerRate.x, 0f, 1f),
            Mathf.Clamp(setter.axisPowerRate.y, 0f, 1f),
            Mathf.Clamp(setter.axisPowerRate.z, 0f, 1f));
    }
}
