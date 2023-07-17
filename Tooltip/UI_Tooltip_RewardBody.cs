using TMPro;
using UnityEngine;
using Debug = COA_DEBUG.Debug;

public class UI_Tooltip_RewardBody : MonoBehaviour
{
    [SerializeField] private UI_Thumbnail_Integrate thumbnail;
    [SerializeField] private TextMeshProUGUI tmpu_Name;
    [SerializeField] private TextMeshProUGUI tmpu_Count;
    [SerializeField] private TextMeshProUGUI tmpu_Content;

    public void Set(UIParam.Common.Reward reward)
    {
        if (reward == null) { Debug.LogBold("<color=#ff5555>리워드 데이터 없음"); return; }

        if (thumbnail != null) thumbnail.SetData(reward, false);
        if (tmpu_Name != null) tmpu_Name.text = Localizer.GetLocalizedStringName(Localizer.SheetType.NONE, reward.key);
        if (tmpu_Count != null) tmpu_Count.text = reward.value.ToString();
        if (tmpu_Content != null) tmpu_Content.text = Localizer.GetLocalizedStringDesc(Localizer.SheetType.NONE, reward.key);
    }
}