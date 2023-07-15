using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Debug = COA_DEBUG.Debug;

public class UI_BroadCastCommentItem : UIBaseBelongings
{
    public Image imgCharacter;
    public TextMeshProUGUI tmpuName;
    public TextMeshProUGUI tmpuComment;
    public TextMeshProUGUI tmpuLike;

    public void SetData(UI_BroadCastComment.commentData data)
    {
        if(data.imgRef != null)
        {
            imgCharacter.sprite = data.imgRef.GetSpriteFromSMTable(ownerUIBase);
        }
        tmpuName.text = string.Format(Localizer.GetLocalizedStringName(Localizer.SheetType.LIBRARY, data.writer), User.PlayerNickName);
        tmpuComment.text = Localizer.GetLocalizedStringDesc(Localizer.SheetType.LIBRARY, data.content);
        tmpuLike.text = string.Format("{0:N0}", data.like);
    }

}