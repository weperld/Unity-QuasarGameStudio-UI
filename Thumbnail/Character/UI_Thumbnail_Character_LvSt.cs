using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Debug = COA_DEBUG.Debug;

public class UI_Thumbnail_Character_LvSt : MonoBehaviour
{
    [SerializeField] private UI_Thumbnail_Character_Lv thumbnail;
    [SerializeField] private TextMeshProUGUI tmpu_St;
    [SerializeField] private GameObject[] go_StBlocks;

    public CharacterInfo _Info => thumbnail?._Info;

    private CharacterActivityInfo activityInfo;
    public void SetData(CharacterInfo info)
    {
        if (activityInfo != null) activityInfo.OnUpdateStamina -= UpdateStamina;
        activityInfo = null;

        if (thumbnail != null) thumbnail.SetData(info);
        if (_Info == null) return;

        User._CharacterActivity.TryGetValue(_Info?._Data?._Enum_Id, out activityInfo);
        if (activityInfo == null) return;

        UpdateStamina(activityInfo.Stamina, CharacterActivityInfo.MAX_STAMINA);
        activityInfo.OnUpdateStamina -= UpdateStamina;
        activityInfo.OnUpdateStamina += UpdateStamina;
    }


    /// <summary>
    /// 상대 지역 수호자 일 경우
    /// </summary>
    /// <param name="info"></param>
    /// <param name="actInfo"></param>
    public void SetData(CharacterInfo info, CharacterActivityInfo actInfo)
    {
        if (activityInfo != null) activityInfo.OnUpdateStamina -= UpdateStamina;
        activityInfo = null;

        if (thumbnail != null) thumbnail.SetData(info);
        if (_Info == null) return;

        activityInfo = actInfo;
        if (activityInfo == null)
            return;

        UpdateStamina(activityInfo.Stamina, CharacterActivityInfo.MAX_STAMINA);
        activityInfo.OnUpdateStamina -= UpdateStamina;
        activityInfo.OnUpdateStamina += UpdateStamina;
    }

    private void UpdateStamina(int st, int max)
    {
        if (tmpu_St != null) tmpu_St.text = st.ToString();
        if (go_StBlocks == null) return;
        for (int i = 0; i < go_StBlocks.Length; i++)
        {
            var tmp = go_StBlocks[i];
            if (tmp == null) continue;

            tmp.SetActive(i < st);
        }
    }
    public void SetActive(bool active)
    {
        gameObject.SetActive(active);
    }
}