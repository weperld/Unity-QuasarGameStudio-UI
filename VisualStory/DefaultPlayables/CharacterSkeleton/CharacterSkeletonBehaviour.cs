using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using Debug = COA_DEBUG.Debug;

[Serializable]
public class CharacterSkeletonBehaviour : PlayableBehaviour
{
    [Header("If \"Character_EnumId\" is Empty or Not Valueable String,\n" +
        "THIS CLIP WILL NOT WORK")]
    public string character_EnumId;
    public string skin = "day";
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

        var characterData = Data._characterTable.GetDataFromTable(character_EnumId);
        if (characterData is null) return;

        skeleton.Set(characterData, skin);
        skeleton.SetAnimation(setupParam.ani, setupParam.timeScale, setupParam.loopCount == -1);

        if (setupParam.loopCount < 2) return;
        for (int i = 1; i < setupParam.loopCount; i++)
            skeleton.AddAnimation(setupParam.ani, false, 0f);
    }
}
