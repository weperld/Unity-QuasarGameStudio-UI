using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Debug = COA_DEBUG.Debug;
using static TimelineParam;
using TMPro;

public class UI_VisualStory_Branch_Sponsor : UI_VisualStory_Branch
{
    [Header("Inherit")]
    [SerializeField] private Image[] img_SponsorItemIcons;
    [SerializeField] private TextMeshProUGUI[] tmpu_SponsorItemNames;
    [SerializeField] private TextMeshProUGUI[] tmpu_SponsorItemCounts;
    [SerializeField] private GameObject[] go_TouchBlockers;

    protected override void SetupSelectable(int index, TimelineBranch branchTimelineParam)
    {
        var cast = branchTimelineParam as OracleSponsorBranch;
        if (cast == null || img_SponsorItemIcons == null || img_SponsorItemIcons.Length <= index) return;

        var donationData = cast._ContentData;
        var deactive = false;
        var sprite = null as Sprite;
        switch (donationData._CE_Common_Type)
        {
            case Data.Enum.Common_Type.ASSET:
                {
                    var assetStatic = Data._assetTable.GetDataFromTable(donationData._Item);
                    if (assetStatic == null) goto default;

                    var asset = User.GetAsset(assetStatic._CE_Asset);
                    deactive = /*asset._Balance < cast.sponCount*/false;
                    sprite = assetStatic._Image_Reference_Data?.GetSpriteFromSMTable(ownerUIBase);
                }
                break;
            case Data.Enum.Common_Type.ITEM:
                {
                    var itemStatic = Data._itemTable.GetDataFromTable(donationData._Item);
                    if (itemStatic == null) goto default;

                    var find = User._Consumables.GetDataListFromTable(i => i != null && i._EnumID == donationData._Item);
                    if (find == null || find.Count == 0) goto default;

                    int totalCount = 0;
                    foreach (var info in find)
                        totalCount += info._Count;

                    deactive = /*totalCount < cast.sponCount*/false;
                    sprite = itemStatic._Image_Reference_Data?.GetSpriteFromSMTable(ownerUIBase);
                }
                break;
            default:
                {
                    deactive = false;
                    sprite = null;
                }
                break;
        }

        var name = GetElement(index, tmpu_SponsorItemNames);
        if (name != null) name.text = Localizer.GetLocalizedStringName(Localizer.SheetType.NONE, donationData._Item);

        var count = GetElement(index, tmpu_SponsorItemCounts);
        if (count != null) count.text = donationData._Item_Value.ToString();

        var img = GetElement(index, img_SponsorItemIcons);
        if (img != null)
        {
            img.sprite = sprite;
            if (img.material != null)
            {
                var mat = new Material(img.material);
                mat.SetFloat("_Saturation", deactive ? 0f : 1f);
                img.material = mat;
            }
        }
        var touchBlocker = GetElement(index, go_TouchBlockers);
        if (touchBlocker != null) touchBlocker?.SetActive(deactive);
    }

    private T GetElement<T>(int index, params T[] array) where T : UnityEngine.Object
    {
        if (array == null || array.Length <= index || index < 0) return null;
        return array[index];
    }
}