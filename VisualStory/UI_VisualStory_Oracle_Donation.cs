using System.Collections;
using System.ComponentModel;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Debug = COA_DEBUG.Debug;

public class UI_VisualStory_Oracle_Donation : UIBaseBelongings
{
    [SerializeField] private GameObject go_Root;
    [SerializeField] private Image img_Donation;
    [SerializeField] private TextMeshProUGUI tmpu_SystemMsg;
    [SerializeField] private TextMeshProUGUI tmpu_UserMsg;

    [Header("Text Colors")]
    [SerializeField] private Color color_Sponser;
    [SerializeField] private Color color_Character;
    [SerializeField] private Color color_Item;

    [Header("후원 아이템 아이콘 확인용 인덱스")]
    [SerializeField] private int index;

    private float speedMult = 1f;
    private IEnumerator deactiveCoroutine;

    private void OnValidate()
    {
        if (tmpu_SystemMsg != null && !Application.isPlaying)
        {
            tmpu_SystemMsg.text = string.Format(
                "<color=#{0}>스폰서 이름 색깔</color>\n" +
                "<color=#{1}>내 캐릭터 이름 색깔</color>\n" +
                "<color=#{2}>후원 아이템 수량 색깔</color>\n" +
                "후원 아이템 아이콘 예시 [<sprite index={3}>]",
                DuelMentColor.ToHtml(color_Sponser),
                DuelMentColor.ToHtml(color_Character),
                DuelMentColor.ToHtml(color_Item),
                index = Mathf.Clamp(index, 0, 63));
        }
    }
    private void OnDisable()
    {
        if (deactiveCoroutine != null) { StopCoroutine(deactiveCoroutine); deactiveCoroutine = null; }
    }

    public void SetActiveMsg(bool active)
    {
        if (go_Root == null) return;
        go_Root.SetActive(active);
    }

    public void ShowDonation(donation_contentsTable data, string targetEnumId)
    {
        SetActiveMsg(data != null);
        if (data == null) return;

        if (img_Donation != null) img_Donation.sprite = data._Image_Reference_Data?.GetSpriteFromSMTable(ownerUIBase);

        var sponserColorHTML = DuelMentColor.ToHtml(color_Sponser);
        var characterColorHTML = DuelMentColor.ToHtml(color_Character);
        var itemColorHTML = DuelMentColor.ToHtml(color_Item);
        if (tmpu_SystemMsg != null) tmpu_SystemMsg.text = string.Format(
            Localizer.GetLocalizedStringTalk(Localizer.SheetType.STRING, data._System_Message),
            $"<color=#{sponserColorHTML}>{Localizer.GetLocalizedStringName(Localizer.SheetType.STRING, data._Sponser_Nickname)}</color>",
            $"<color=#{characterColorHTML}>{Localizer.GetLocalizedStringName(Localizer.SheetType.NONE, targetEnumId)}</color>",
            $"<color=#{itemColorHTML}>{data._Item_Value}</color>",
            data._CE_Common_Type switch
            {
                Data.Enum.Common_Type.ASSET => Data._assetTable.GetDataFromTable(data._Item)?._Pictogram_Reference_Data?._Sprite_Name,
                //Data.Enum.Common_Type.CHARACTER => Data._characterTable.GetDataFromTable(contentData._Item)?._Resource_List_Data?._Thumbnail_SD_Reference_Data?._Sprite_Name,
                //Data.Enum.Common_Type.ITEM => Data._itemTable.GetDataFromTable(contentData._Item)?._Image_Reference_Data?._Sprite_Name,
                _ => "",
            });
        if (tmpu_UserMsg != null) tmpu_UserMsg.text = Localizer.GetLocalizedStringTalk(Localizer.SheetType.STRING, data._Enum_Id);

        StartDeactiveCoroutine();
    }

    public void SetSpeed(float speed)
    {
        speedMult = speed;
    }

    private void StartDeactiveCoroutine()
    {
        if (deactiveCoroutine != null) StopCoroutine(deactiveCoroutine);
        deactiveCoroutine = DeactiveCoroutine();
        StartCoroutine(deactiveCoroutine);
    }
    private IEnumerator DeactiveCoroutine()
    {
        float time = 0f;
        while (time < 5f / speedMult)
        {
            yield return null;
            time += Time.deltaTime;
        }
        SetActiveMsg(false);
        deactiveCoroutine = null;
    }
}