using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Debug = COA_DEBUG.Debug;

public class UI_BroadCastProfile : MonoBehaviour
{
    public TextMeshProUGUI tmpuName;
    public TextMeshProUGUI tmpuGender;
    public TextMeshProUGUI tmpuJob;
    public TextMeshProUGUI tmpuTall;
    public TextMeshProUGUI tmpuWeight;
    public TextMeshProUGUI tmpuLike;
    public TextMeshProUGUI tmpuDisLike;
    public TextMeshProUGUI tmpuPersonality;
    public TextMeshProUGUI tmpuDesc;
    public TextMeshProUGUI tmpuLock;

    public GameObject goProfile;
    public GameObject goLock;
    public GameObject goLockTemp;

    public void Set(string characterEnumId, List<string> costumes)
    {
        var character = Data.GetDataFromTable(Data._characterTable, characterEnumId);
        if (character != null)
        {
            var profile = character._Profile_Data;
            if (profile != null)
            {
                tmpuName.text = Localizer.GetLocalizedStringName(Localizer.SheetType.LIBRARY, profile._Real_Name);
                tmpuGender.text = Localizer.GetLocalizedStringName(Localizer.SheetType.LIBRARY, profile._Gender);
                tmpuJob.text = Localizer.GetLocalizedStringName(Localizer.SheetType.LIBRARY, profile._Real_Job);
                tmpuTall.text = Localizer.GetLocalizedStringName(Localizer.SheetType.LIBRARY, profile._Height);
                tmpuWeight.text = Localizer.GetLocalizedStringName(Localizer.SheetType.LIBRARY, profile._Weight);
                tmpuLike.text = Localizer.GetLocalizedStringName(Localizer.SheetType.LIBRARY, profile._Favorite_Thing);
                tmpuDisLike.text = Localizer.GetLocalizedStringName(Localizer.SheetType.LIBRARY, profile._Dislike_Thing);
                tmpuPersonality.text = Localizer.GetLocalizedStringName(Localizer.SheetType.LIBRARY, profile._Personality);
                tmpuDesc.text = Localizer.GetLocalizedStringDesc(Localizer.SheetType.LIBRARY, profile._Daily_Life);

                bool unlock = false;
                if (profile._Daily_Unlock_Condition_Data != null)
                {
                    goLock.SetActive(true);
                    goLockTemp.SetActive(false);

                    string formatStr = Localizer.GetLocalizedStringDesc(Localizer.SheetType.NONE, profile._Daily_Unlock_Condition_Data._CE_Costume_Condition.ToString());
                    string characterName = Localizer.GetLocalizedStringName(Localizer.SheetType.NONE, character._Enum_Id);
                    string formatRes = string.Format(formatStr, characterName, profile._Daily_Unlock_Condition_Data._Condition_Value);
                    tmpuLock.text = formatRes;

                    unlock = costumes.Contains(profile._Daily_Unlock_Condition_Data._Enum_Id);
                }
                else
                {
                    goLock.SetActive(false);
                    goLockTemp.SetActive(true);
                }

                goProfile.SetActive(unlock);
                goLock.SetActive(!unlock);
            }
        }
    }
}