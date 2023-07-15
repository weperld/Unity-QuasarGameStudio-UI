using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Debug = COA_DEBUG.Debug;

public class UI_BroadCastBasic : MonoBehaviour
{
    public TextMeshProUGUI tmpuNickName;
    public TextMeshProUGUI tmpuProperty;
    public TextMeshProUGUI tmpuClass;
    public TextMeshProUGUI tmpuHabit;
    public TextMeshProUGUI tmpuCareer;
    public TextMeshProUGUI tmpuWeak;
    public TextMeshProUGUI tmpuFandom;
    public TextMeshProUGUI tmpuReaction;
    public TextMeshProUGUI tmpuDesc;

    public void Set(string characterEnumId)
    {
        var character = Data.GetDataFromTable(Data._characterTable, characterEnumId);
        if(character != null)
        {
            tmpuNickName.text = Localizer.GetLocalizedStringName(Localizer.SheetType.NONE, characterEnumId);
            tmpuProperty.text = Localizer.GetLocalizedStringName(Localizer.SheetType.NONE, character._CE_Character_Property.ToString());
            tmpuClass.text = Localizer.GetLocalizedStringName(Localizer.SheetType.NONE, character._CE_Character_Class.ToString());

            var profile = character._Profile_Data;
            if(profile != null)
            {
                tmpuHabit.text = Localizer.GetLocalizedStringName(Localizer.SheetType.LIBRARY, profile._Habit);
                tmpuCareer.text = Localizer.GetLocalizedStringName(Localizer.SheetType.LIBRARY, profile._Career);
                tmpuWeak.text = Localizer.GetLocalizedStringName(Localizer.SheetType.LIBRARY, profile._Weakness);
                tmpuFandom.text = Localizer.GetLocalizedStringName(Localizer.SheetType.LIBRARY, profile._Fandom);
                tmpuReaction.text = Localizer.GetLocalizedStringName(Localizer.SheetType.LIBRARY, profile._Donation_Reaction);
                tmpuDesc.text = Localizer.GetLocalizedStringDesc(Localizer.SheetType.LIBRARY, profile._Channel);
            }
        }
    }
}