using EnhancedUI.EnhancedScroller;
using TMPro;
using Debug = COA_DEBUG.Debug;
using static GachaSimulHelper;

public class GachaSimulResultSlot : EnhancedScrollerCellView
{
    public UI_Thumbnail_Character_GradeFrame thumb;
    public TextMeshProUGUI tmpu_Name;
    public TextMeshProUGUI tmpu_AppearCount;
    public TextMeshProUGUI tmpu_AppearPercentage;

    public void SetData(SimulIndividualResultData resultData, uint totalCount)
    {
        thumb.SetData(resultData._CharData);
        tmpu_Name.text = $"({resultData.grade})" + Localizer.GetLocalizedStringName(Localizer.SheetType.NONE, resultData.enumId);
        tmpu_AppearCount.text = $"{resultData.appearCount}회";
        tmpu_AppearPercentage.text = $"{(double)resultData.appearCount / totalCount * 100d:N3}%";
    }
}
