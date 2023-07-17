using AudioSystem;
using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class PlaySoundBehaviour : PlayableBehaviour
{
    public TimelineParam.PlayAudio setter = new TimelineParam.PlayAudio();
    [Range(0f, 1f)] public float volume;

    private bool first = false;

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        if (first) return;
        first = true;

        if (AudioManager.IsDetroying) return;
        AudioManager._instance.PlaySound(setter.audioName, volume);
    }
}
