using EnhancedUI.EnhancedScroller;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Debug = COA_DEBUG.Debug;

public class UI_VisualStory_Oracle_Chat : UIBaseBelongingEnhancedScrollerCellView
{
    [SerializeField] private RectTransform rtf_ForceRebuildTarget;

    [Header("Chat")]
    [SerializeField] private GameObject go_ChatRoot;
    [SerializeField] private GameObject go_ChatBg;
    [SerializeField] private GameObject go_BadgeRoot;
    [SerializeField] private Image img_Badge;
    [SerializeField] private TextMeshProUGUI tmpu_NameMark;
    [SerializeField] private TextMeshProUGUI tmpu_Name;
    [SerializeField] private TextMeshProUGUI tmpu_Content;
    [SerializeField] private Color color_NameBgForGod;
    [SerializeField] private Color color_NameForGod;
    [SerializeField] private Color color_NameForCharAndUser;
    [SerializeField] private Color color_NameForNormal;
    [SerializeField] private Vector4 v4_NameBgPadding;

    [Header("Donation")]
    [SerializeField] private GameObject go_DonationRoot;
    [SerializeField] private Image img_Donation;
    [SerializeField] private TextMeshProUGUI tmpu_DonationSystemMsg;
    [SerializeField] private TextMeshProUGUI tmpu_DonationMsg;
    [SerializeField, Range(0f, 200f)] private float donaNicknameSize = 120;
    [SerializeField] private Color color_DonationUser;
    [SerializeField] private Color color_DonationCount;
    [SerializeField] private int egIconIndex;

    private void OnValidate()
    {
        var coloredName = string.Format("<color=#{0}>Nic<color=#{1}>kna<color=#{2}>me</color>",
            ColorUtility.ToHtmlStringRGBA(color_NameForGod),
            ColorUtility.ToHtmlStringRGBA(color_NameForCharAndUser),
            ColorUtility.ToHtmlStringRGBA(color_NameForNormal));
        var colorHtml = ColorUtility.ToHtmlStringRGBA(color_NameBgForGod);
        var paddingStr = string.Format("{0},{1},{2},{3}", v4_NameBgPadding.x, v4_NameBgPadding.y, v4_NameBgPadding.z, v4_NameBgPadding.w);
        if (tmpu_NameMark != null) tmpu_NameMark.text = $"<mark=#{colorHtml} padding={paddingStr}>{coloredName}</mark>";
        if (tmpu_Name != null) tmpu_Name.text = coloredName;

        egIconIndex = Mathf.Clamp(egIconIndex, 0, 63);
        if (tmpu_DonationSystemMsg != null)
            tmpu_DonationSystemMsg.text = string.Format("{0}\n<sprite index={3}>{2} 후원!",
                $"<size={donaNicknameSize}%><color=#{ColorUtility.ToHtmlStringRGBA(color_DonationUser)}>닉네임</color></size>",
                "후원 대상",
                $"<color=#{ColorUtility.ToHtmlStringRGBA(color_DonationCount)}>100</color>",
                egIconIndex);
    }

    public void Set(VisualStoryHelper.OracleChatCellData data)
    {
        var active = data != null;
        gameObject.SetActive(active);
        if (!active) return;

        go_ChatRoot?.SetActive(data.cellType == VisualStoryHelper.OracleChatCellData.Type.NORMAL);
        go_DonationRoot?.SetActive(data.cellType == VisualStoryHelper.OracleChatCellData.Type.DONATION);

        switch (data.cellType)
        {
            case VisualStoryHelper.OracleChatCellData.Type.NORMAL: {
                    if (go_ChatBg != null) go_ChatBg.SetActive(data.format == Data.Enum.Oracle_Chat_Format.USER);
                    var badge = data.format != Data.Enum.Oracle_Chat_Format.COMMON
                    && data.format != Data.Enum.Oracle_Chat_Format.USER
                    && data.format != Data.Enum.Oracle_Chat_Format.CNT;
                    go_BadgeRoot?.SetActive(badge);
                    if (badge && img_Badge != null)
                    {
                        img_Badge.sprite = data.format switch
                        {
                            Data.Enum.Oracle_Chat_Format.SPECIAL => Data._characterTable.GetDataFromTable(data.optional_Str)?._Resource_List_Data?._Thumbnail_Reference_Data?.GetSpriteFromSMTable(ownerUIBase),
                            Data.Enum.Oracle_Chat_Format.GOD => Data._sprite_managementTable.GetDataFromTable(data.optional_Str)?.GetSpriteFromSMTable(ownerUIBase),
                            _ => null,
                        };
                        img_Badge.GetComponent<RectTransform>().sizeDelta = data.format switch
                        {
                            Data.Enum.Oracle_Chat_Format.SPECIAL => Vector2.one * 60f,
                            Data.Enum.Oracle_Chat_Format.GOD => Vector2.one * 50f,
                            _ => Vector2.zero
                        };
                    }

                    var nameColor = data.format switch
                    {
                        Data.Enum.Oracle_Chat_Format.SPECIAL => color_NameForCharAndUser,
                        Data.Enum.Oracle_Chat_Format.GOD => color_NameForGod,
                        Data.Enum.Oracle_Chat_Format.USER => color_NameForCharAndUser,
                        _ => color_NameForNormal,
                    };
                    var colorHtml = ColorUtility.ToHtmlStringRGBA(nameColor);
                    var coloredName = $"<color=#{colorHtml}>{data._UserNickName}</color>";
                    if (tmpu_Name != null) tmpu_Name.text = coloredName;
                    if (tmpu_Content != null) tmpu_Content.text = data.content;

                    colorHtml = data.format == Data.Enum.Oracle_Chat_Format.GOD
                        ? ColorUtility.ToHtmlStringRGBA(color_NameBgForGod)
                        : ColorUtility.ToHtmlStringRGBA(Color.clear);
                    var paddingStr = string.Format("{0},{1},{2},{3}", v4_NameBgPadding.x, v4_NameBgPadding.y, v4_NameBgPadding.z, v4_NameBgPadding.w);
                    if (tmpu_NameMark != null) tmpu_NameMark.text = $"<mark=#{colorHtml} padding={paddingStr}>{coloredName}</mark>";
                } break;
            case VisualStoryHelper.OracleChatCellData.Type.DONATION: {
                    if (data.donationData == null) break;

                    if (img_Donation != null) img_Donation.sprite = data.donationData._Image_Reference_Data?.GetSpriteFromSMTable(ownerUIBase);

                    var systemLocal = Localizer.GetLocalizedStringTalk(Localizer.SheetType.STRING, data.donationData._System_Message + "_SIMPLE");
                    if (tmpu_DonationSystemMsg != null) tmpu_DonationSystemMsg.text = string.Format(systemLocal, GetDonationSystemMsgArgs(data));
                    if (tmpu_DonationMsg != null)
                        tmpu_DonationMsg.text = Localizer.GetLocalizedStringTalk(Localizer.SheetType.STRING, data.donationData._Enum_Id);
                } break;
        }
    }

    public float ForceRebuildAndGetHeight(VisualStoryHelper.OracleChatCellData data, float width)
    {
        if (rtf_ForceRebuildTarget == null) return 0f;

        var size = new Vector2(rtf_ForceRebuildTarget.rect.width, rtf_ForceRebuildTarget.rect.height);
        size.x = width;
        rtf_ForceRebuildTarget.sizeDelta = size;

        Set(data);
        LayoutRebuilder.ForceRebuildLayoutImmediate(rtf_ForceRebuildTarget);

        return rtf_ForceRebuildTarget.rect.height;
    }

    private object[] GetDonationSystemMsgArgs(VisualStoryHelper.OracleChatCellData data)
    {
        var itemType = data.donationData._CE_Common_Type;
        var itemEnum = data.donationData._Item;
        var icon = itemType switch
        {
            Data.Enum.Common_Type.ASSET => Data._assetTable.GetDataFromTable(itemEnum)?._Pictogram_Reference_Data?._Sprite_Name,
            Data.Enum.Common_Type.ITEM => Data._itemTable.GetDataFromTable(itemEnum)?._Image_Reference_Data?._Sprite_Name,
            _ => string.Empty,
        };
        var count = (int)data.donationData._Item_Value;

        var ret = new object[]
        {
            $"<size={donaNicknameSize}%><color=#{ColorUtility.ToHtmlStringRGBA(color_DonationUser)}>{data._UserNickName}</color></size>",
            Localizer.GetLocalizedStringName(Localizer.SheetType.NONE, data.targetEnumId),
            $"<color=#{ColorUtility.ToHtmlStringRGBA(color_DonationCount)}>{count}</color>",
            icon
        };

        return ret;
    }
}