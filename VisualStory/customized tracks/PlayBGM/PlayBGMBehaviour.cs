using AudioSystem;
using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class PlayBGMBehaviour : PlayableBehaviour
{
    public TimelineParam.PlayAudio setter = new TimelineParam.PlayAudio();
    public AnimationCurve volumeCurve = AnimationCurve.Constant(0f, 1f, 1f);

    public void PlayBGM(float normalized)
    {
        if (AudioManager.IsDetroying) return;
        var volume = volumeCurve.Evaluate(normalized);
        AudioManager._instance.PlayBGM(setter.audioName, volume);
    }
}
