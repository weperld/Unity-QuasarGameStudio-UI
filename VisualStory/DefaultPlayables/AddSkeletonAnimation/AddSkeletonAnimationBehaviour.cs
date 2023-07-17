using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using Debug = COA_DEBUG.Debug;

[Serializable]
public class AddSkeletonAnimationBehaviour : PlayableBehaviour
{
    public TimelineParam.CharacterSkeletonGraphicAnim setupParam;

    private bool processFlag = false;

    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        //processFlag = false;
    }
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        if (processFlag) return;
        processFlag = true;

        var skeleton = playerData as Character_UI_Illust_SG_ForVisualStory;
        if (skeleton is null) return;

        skeleton.SetBaseTimeScale(setupParam.timeScale);
        skeleton.AddAnimation(setupParam.ani, setupParam.loopCount == -1, 0f);

        if (setupParam.loopCount < 2) return;
        for (int i = 1; i < setupParam.loopCount; i++)
            skeleton.AddAnimation(setupParam.ani, false, 0f);
    }
}
