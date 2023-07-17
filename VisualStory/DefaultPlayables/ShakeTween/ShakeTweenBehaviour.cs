using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using static VisualStoryHelper;

[Serializable]
public class ShakeTweenBehaviour : PlayableBehaviour
{
    public TimelineParam.ShakeTween setter = new TimelineParam.ShakeTween();
}
