using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Debug = COA_DEBUG.Debug;
using LeTai.Asset.TranslucentImage;

public class UIBase_Tooltip_FateCard : UIBase
{
    //[SerializeField] private Image img_FateCard;
    [SerializeField] private UI_FateCard ui_fatecard;
    [SerializeField] private Image img_Back;
    [SerializeField] private TextMeshProUGUI text_FateCard_Title;
    [SerializeField] private TextMeshProUGUI text_Grade;
    [SerializeField] private TextMeshProUGUI text_FateCard_Count;
    [SerializeField] private TextMeshProUGUI text_FateCard_Trigger;
    [SerializeField] private TextMeshProUGUI text_FateCard_Effect;
    [SerializeField] private TextMeshProUGUI text_FateCard_Info;
    [SerializeField] private TextMeshProUGUI text_FateCard_Speech;
    [SerializeField] private TranslucentImage blurry;
    [SerializeField] private Button btn_Back;
    [SerializeField] private Animator anim_FateCard_ToolTip;

    private UIParam.Factory.FateCard_Detail detail_Param;
    private fate_cardTable table;

    public override void Show(object param)
    {
        base.Show();

        detail_Param = param as UIParam.Factory.FateCard_Detail;
        if (detail_Param == null)
            return;
        table = detail_Param.fatecardTable;
        //gameObject.SetActive(true);

        UIUtil.ResetAndAddListener(btn_Back, OnClickBackBtn);
        blurry.source = UIManager._instance._TranslucentImageSource;


        //img_FateCard.sprite = table._Image_Reference_Data.GetSpriteFromSMTable(this);
        ui_fatecard.Set_FateCard(table);
        img_Back.sprite = table._Image_Reference_Data.GetSpriteFromSMTable(this);
        text_FateCard_Title.text = Localizer.GetLocalizedStringName(Localizer.SheetType.NONE, table._Enum_Id);
        User._FateCardCount.TryGetValue(table._Enum_Id, out var fateCardCount);
        text_FateCard_Count.text = fateCardCount.ToString();

        switch (table._CE_Fate_Card_Grade)
        {
            case Data.Enum.Fate_Card_Grade.GRADE_C:
                text_Grade.text = "C";
                break;
            case Data.Enum.Fate_Card_Grade.GRADE_B:
                text_Grade.text = "B";
                break;
            case Data.Enum.Fate_Card_Grade.GRADE_A:
                text_Grade.text = "A";
                break;
            case Data.Enum.Fate_Card_Grade.CNT:
                break;
        }

        if (text_FateCard_Trigger != null)
            text_FateCard_Trigger.text = Localizer.GetLocalizedStringName(Localizer.SheetType.NONE, table._Card_Trigger);

        if (text_FateCard_Speech != null)
            text_FateCard_Speech.text = Localizer.GetLocalizedStringTalk(Localizer.SheetType.NONE, table._Enum_Id);
        ;


        string effect = "";
        string str = "";
        object obj = "";

        object[] args = new object[table._Card_Effect.Length];


        for (int i = 0; i < table._Card_Effect.Length; i++)
        {
            if (table._Card_Effect[i] == Data.Enum.Fate_Card_Effect.NONE)
                break;

            str = Localizer.GetLocalizedStringName(Localizer.SheetType.NONE, table._Card_Effect[i].ToString());
            if (table._Card_Effect[i] == Data.Enum.Fate_Card_Effect.ATTACK)
            {
                obj = table._Card_Effect_Value[i];
                effect += string.Format(str, obj);
            }
            else
            {
                args[0] = table._Card_Effect_Duration[i];
                args[1] = table._Card_Effect_Value[i];
                effect += string.Format(str, args);
            }
            effect += "\n";
        }

        if (text_FateCard_Effect != null)
        {
            text_FateCard_Effect.text = effect;
        }

        if (text_FateCard_Info != null)
        {
            text_FateCard_Info.text = Localizer.GetLocalizedStringDesc(Localizer.SheetType.NONE, table._Enum_Id);
        }
    }

    private void OnClickBackBtn()
    {
        anim_FateCard_ToolTip.SetTrigger("Anim_Trigger");
    }

    private void End_Anim()
    {
        Hide();
        detail_Param?.action.Invoke();
    }
}
