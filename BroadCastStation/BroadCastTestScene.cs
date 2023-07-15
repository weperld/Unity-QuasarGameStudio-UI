using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Debug = COA_DEBUG.Debug;

public class BroadCastTestScene : BaseScene
{
    public UIBase_BroadCastStation uIBroadCast;

    public override void Load(Action onComplete)
    {
        User.GetCharacterCollection(
            () =>
            {
                uIBroadCast.Show(new UIParam.BroadCast.BroadCastStationSetupParam
                {
                    characterEnumID = "Character_Bella"
                });
            },
            (error) =>
            {

            });
        onComplete?.Invoke();
    }
}
