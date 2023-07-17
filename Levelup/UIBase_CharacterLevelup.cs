using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;
using TMPro;
using Debug = COA_DEBUG.Debug;
using EnhancedUI.EnhancedScroller;
using UnityEngine.EventSystems;
using Spine;
using AudioSystem;

public class LevelupItemData
{
    public delegate bool DelegateCheckSelectable(LevelupItemData data);

    public ItemInfo _Info { get; private set; }
    public int _Count { get; private set; } = 0;
    public int _Max => Mathf.Min(User.LEVEUP_MAX_SELECTION_PER_ITEM, _Count);
    public int _AddedCount { get; private set; } = 0;
    public bool _IsDragging
    {
        get
        {
            var ret = checkDragging?.Invoke(this);
            return ret != null && ret is true;
        }
    }

    private DelegateCheckSelectable checkIsMaxLv;
    private DelegateCheckSelectable checkGoldIsEnough;
    private DelegateCheckSelectable checkDragging;
    private Action<LevelupItemData, int> onChangeSelectionStateAction;
    public Action<int> onChangeAddedCount;
    public Action playUseEffAction;

    public LevelupItemData(ItemInfo info,
        int count,
        DelegateCheckSelectable checkIsMaxLv,
        DelegateCheckSelectable checkGoldIsEnough,
        DelegateCheckSelectable checkDragging,
        Action<LevelupItemData, int> onChangeSelectionStateAction)
    {
        _Info = info;
        _Count = count;
        this.checkIsMaxLv = checkIsMaxLv;
        this.checkGoldIsEnough = checkGoldIsEnough;
        this.checkDragging = checkDragging;
        this.onChangeSelectionStateAction = onChangeSelectionStateAction;
    }

    public enum AddOrRemoveRequestResult
    {
        NOT_AVAILABLE,
        A_SUCCESS,
        A_FAIL_MAX_LV,
        A_FAIL_GOLD_NOT_ENOUGH,
        R_SUCCESS,
        R_FAIL,
    }
    public AddOrRemoveRequestResult Add()
    {
        if (_Info == null || _Info._Data == null) return AddOrRemoveRequestResult.NOT_AVAILABLE;

        var val = checkIsMaxLv?.Invoke(this);
        if (val is not null && val is false) return AddOrRemoveRequestResult.A_FAIL_MAX_LV;

        val = checkGoldIsEnough?.Invoke(this);
        if (val is not null && val is false) return AddOrRemoveRequestResult.A_FAIL_GOLD_NOT_ENOUGH;

        var prev = _AddedCount;
        _AddedCount++;
        if (_AddedCount > _Max) _AddedCount = _Max;

        onChangeSelectionStateAction?.Invoke(this, _AddedCount - prev);
        onChangeAddedCount?.Invoke(_AddedCount);

        return AddOrRemoveRequestResult.A_SUCCESS;
    }
    public AddOrRemoveRequestResult Remove()
    {
        if (_Info == null || _Info._Data == null) return AddOrRemoveRequestResult.NOT_AVAILABLE;

        var prev = _AddedCount;
        _AddedCount--;
        if (_AddedCount < 0) _AddedCount = 0;

        onChangeSelectionStateAction?.Invoke(this, _AddedCount - prev);
        onChangeAddedCount?.Invoke(_AddedCount);

        return AddOrRemoveRequestResult.R_SUCCESS;
    }

    public void PlayUseEff()
    {
        playUseEffAction?.Invoke();
    }
}

public class UIBase_CharacterLevelup : UIBase, IEnhancedScrollerDelegate
{
    private class DragHandler : MonoBehaviour, IEndDragHandler, IBeginDragHandler
    {
        public Action onBeginDrag;
        public Action onEndDrag;

        public void OnBeginDrag(PointerEventData eventData)
        {
            onBeginDrag?.Invoke();
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            onEndDrag?.Invoke();
        }
    }

    #region Inspector
    [Header("Scene Common UI")]
    [SerializeField] private Button btn_Back;
    [SerializeField] private Button btn_Home;
    [SerializeField] private TextMeshProUGUI tmpu_CharacterName;
    [SerializeField] private Character_UI_Illust characterIllust;

    [Header("Scroller UI")]
    [SerializeField] private UI_CharacterLevelup_ExpConsum_Group itemGroupPrefab;
    [SerializeField] private EnhancedScroller scroller;

    [Header("Levelup Cost UI")]
    [SerializeField] private TextMeshProUGUI tmpu_CostValue;
    [SerializeField] private Button btn_Reset;
    [SerializeField] private Button btn_Request;

    [Header("Exp And Status UI")]
    [SerializeField] private TextMeshProUGUI tmpu_OriginLevel;
    [SerializeField] private GameObject go_OfMaxLevel;
    [SerializeField] private GameObject go_OfIncreaseable;
    [SerializeField] private GameObject go_OfIncreaseable_Others;
    [SerializeField] private TextMeshProUGUI tmpu_CopyLevel;
    [SerializeField] private TextMeshProUGUI tmpu_CopyCurExp;
    [SerializeField] private TextMeshProUGUI tmpu_CopyMaxExp;
    [SerializeField] private GameObject go_ExpIncreaseValue;
    [SerializeField] private TextMeshProUGUI tmpu_ExpIncreaseValue;
    [SerializeField] private Image img_CopyExpFill;
    [SerializeField] private GameObject go_AlarmMaxLevel;
    [SerializeField] private UI_CharacterLevelup_StatusInfo statusInfo_ATK;
    [SerializeField] private UI_CharacterLevelup_StatusInfo statusInfo_DEF;
    [SerializeField] private UI_CharacterLevelup_StatusInfo statusInfo_HP;

    [Header("Tooltip")]
    [SerializeField] private UI_Tooltip_RewardBody rewardTooltip;

    [Header("Particle")]
    [SerializeField] private ParticleSystem[] eff_Levelup;

    [Header("Animation")]
    [SerializeField] private GameObject go_TouchBlocker;
    [SerializeField] private Animator animator;


    [Header("Character Change Slide")]
    [SerializeField] private UI_DragDetector changeDragDetector;
    #endregion

    #region Variables
    private const int MAX_ITEM_COUNT = 20;
    private List<LevelupItemData> list_ExpItemData = new List<LevelupItemData>();
    private Dictionary<int, int> dict_SelectionCount = new Dictionary<int, int>();

    private CharacterInfo originInfo;
    private CharacterInfo copyInfo;

    private int costOfSelections = 0;

    private CharacterStat originStat;

    private DragHandler scrollerAdditionalDragHandler;
    private bool isDragging = false;

    private IEnumerator alphaCorout;

    private List<CharacterInfo> list_ViewableCharacter = new List<CharacterInfo>();
    private int currentInfoIndex;

    private UIBase characterLvUpErrorUI;

    private bool lvUpIsRequesting = false;
    #endregion

    #region Base Methods
    private void Awake()
    {
        UIUtil.ResetAndAddListener(btn_Back, OnClickBackButton);
        UIUtil.ResetAndAddListener(btn_Home, OnClickHomeButton);
        UIUtil.ResetAndAddListener(btn_Reset, OnClickResetButton);
        UIUtil.ResetAndAddListener(btn_Request, OnClickLevelupRequestButton);

        if (scroller != null)
        {
            scrollerAdditionalDragHandler = scroller.gameObject.AddComponent<DragHandler>();
            scrollerAdditionalDragHandler.onBeginDrag = ScrollerOnBeginDrag;
            scrollerAdditionalDragHandler.onEndDrag = ScrollerOnEndDrag;
        }

        if (changeDragDetector != null)
        {
            changeDragDetector.onBeginDrag = CharacterInfoSlide.OnBeginChangeCharacter;
            changeDragDetector.onEndDrag = CharacterInfoSlide.OnEndChangeCharacter;
            changeDragDetector.onDragging = CharacterInfoSlide.OnChangingCharacter;
        }
    }
    private void OnEnable()
    {
        CharacterInfoSlide.onBeginSlide -= OnBeginSlideChangeCharacter;
        CharacterInfoSlide.onBeginSlide += OnBeginSlideChangeCharacter;
        CharacterInfoSlide.onSliding -= OnSlidingChangeCharacter;
        CharacterInfoSlide.onSliding += OnSlidingChangeCharacter;
        CharacterInfoSlide.onEndSlide -= OnEndSlideChangeCharacter;
        CharacterInfoSlide.onEndSlide += OnEndSlideChangeCharacter;
        lvUpIsRequesting = false;
    }
    private void OnDisable()
    {
        CharacterInfoSlide.onBeginSlide -= OnBeginSlideChangeCharacter;
        CharacterInfoSlide.onSliding -= OnSlidingChangeCharacter;
        CharacterInfoSlide.onEndSlide -= OnEndSlideChangeCharacter;
        if (alphaCorout != null)
        {
            StopCoroutine(alphaCorout);
            alphaCorout = null;
        }
    }

    UIBase confirm;
    public override void Show(object param)
    {
        go_TouchBlocker?.SetActive(false);
        base.Show(param);
        var cast = param as UIParam.Character_Inventory.CharacterInfoWindow;
        if (cast == null || !(User._Character.TryGetValue(cast.selectedId, out originInfo) && originInfo != null))
        {
            confirm = UIMessageBox.Confirm_Old("캐릭터 정보 에러",
                "캐릭터 정보가 올바르지 않습니다.",
                () =>
                {
                    Hide();
                    confirm?.Hide();
                });
            return;
        }

        list_ViewableCharacter.Clear();
        foreach (var v in cast.characterIdArr)
        {
            var info = User._Character.GetDataFromTable(v);
            if (info == null) continue;

            list_ViewableCharacter.Add(info);
        }
        currentInfoIndex = list_ViewableCharacter.IndexOf(originInfo);

        Setup();

        characterIllust?.Set(true, originInfo._Data);
        if (tmpu_CharacterName != null) tmpu_CharacterName.text = Localizer.GetLocalizedStringName(Localizer.SheetType.NONE, originInfo._EnumID);
    }

    #endregion

    #region Scroller Delegate
    public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
    {
        var cellGroup = scroller.GetCellView(itemGroupPrefab) as UI_CharacterLevelup_ExpConsum_Group;
        int cellCountPerRow = itemGroupPrefab != null ? itemGroupPrefab._CellCount : 1;
        cellGroup.SetDatas(list_ExpItemData, dataIndex * cellCountPerRow);
        return cellGroup;
    }

    public float GetCellViewSize(EnhancedScroller scroller, int dataIndex)
    {
        return itemGroupPrefab != null ? itemGroupPrefab.GetComponent<RectTransform>().rect.height : 0f;
    }

    public int GetNumberOfCells(EnhancedScroller scroller)
    {
        int cellCountPerRow = itemGroupPrefab != null ? itemGroupPrefab._CellCount : 1;
        return list_ExpItemData.Count / cellCountPerRow + 1;
    }
    #endregion

    #region Listener
    private void OnClickBackButton()
    {
        go_TouchBlocker?.SetActive(true);
        if (animator != null) animator.SetTrigger("Hide");
        else Hide();
    }
    private void OnClickHomeButton()
    {
        UIManager._instance.HideAll();
    }

    private void OnClickLevelupRequestButton()
    {
        AudioManager._instance.PlaySound(DefineName.Audio_OutGame.Sound.BUTTON);
        if (lvUpIsRequesting) return;

        int selectionCount = 0;
        foreach (var cnt in dict_SelectionCount.Values)
            selectionCount += cnt;
        if (selectionCount < 1) return;

        long possession = 0;
        if (User._Asset.TryGetValue(Data.Enum.Asset.GOLD, out var info)) possession = info._Balance;
        if (possession < costOfSelections)
        {
            characterLvUpErrorUI = UIMessageBox.Confirm_Old("알림", "소지한 골드가 부족합니다.",
                () => { characterLvUpErrorUI?.Hide(); });
            return;
        }

        var prevLv = originInfo._Level;
        var list_SelectItemIdx = new List<int>();
        for (int i = 0; i < list_ExpItemData.Count; i++)
        {
            var item = list_ExpItemData[i];
            if (item._AddedCount < 1) continue;

            list_SelectItemIdx.Add(i);
        }

        lvUpIsRequesting = true;
        User.UseConsumableItem(Data.Enum.Common_Type.CHARACTER, originInfo._ID, dict_SelectionCount,
            (result, addedExp) =>
            {
                var nextLv = originInfo._Level;
                if (prevLv < nextLv)
                {
                    if (eff_Levelup != null) foreach (var eff in eff_Levelup) if (eff != null) eff.Play();
                    AudioManager._instance.PlaySound(DefineName.Audio_OutGame.Sound.LEVEL_UP);
                }
                Setup();
                foreach (var idx in list_SelectItemIdx)
                {
                    if (idx >= list_ExpItemData.Count) break;
                    list_ExpItemData[idx].PlayUseEff();
                }

                statusInfo_ATK?.SetActiveIncreaseStat(false);
                statusInfo_DEF?.SetActiveIncreaseStat(false);
                statusInfo_HP?.SetActiveIncreaseStat(false);
                lvUpIsRequesting = false;
            },
            error =>
            {
                lvUpIsRequesting = false;
                Setup();
                characterLvUpErrorUI = UIMessageBox.Confirm_Old("캐릭터 레벨업 ERROR", $"ERROR CODE: [{error}]",
                    () =>
                    {
                        characterLvUpErrorUI?.Hide();
                    });
            });
    }
    private void OnClickResetButton()
    {
        int selection = 0;
        foreach (var v in dict_SelectionCount)
            selection += v.Value;
        if (selection < 1) return;

        Setup();
    }

    private bool CheckIsMaxLv(LevelupItemData data)
    {
        return !copyInfo._IsMaxLevel;
    }
    private bool CheckGoldIsEnough(LevelupItemData data)
    {
        var staticData = data._Info._Data;
        if (staticData == null) return false;

        var cost = staticData._Cost_Value;
        var possession = User.GetAsset(Data.Enum.Asset.GOLD)._Balance;
        return possession >= cost + costOfSelections;
    }
    private bool CheckDragging(LevelupItemData data)
    {
        return isDragging;
    }
    private void OnChangeSelectionState(LevelupItemData data, int selectionDelta)
    {
        var info = data?._Info;
        if (info == null) return;

        var id = info._ID;
        if (!dict_SelectionCount.ContainsKey(id)) dict_SelectionCount.Add(id, 0);
        dict_SelectionCount[id] += selectionDelta;
        if (dict_SelectionCount[id] < 0) dict_SelectionCount[id] = 0;

        SetCopyInfoExp();

        var staticData = info._Data;
        var costDelta = 0;
        if (staticData != null) costDelta = (int)staticData._Cost_Value * selectionDelta;
        costOfSelections += costDelta;

        if (costOfSelections <= 0) RewardTooltipHide();
        else RewardTooltipShow(data);

        UpdateCostUI();
    }

    private void OnUpdateCopyExp(CharacterInfo info)
    {
        if (info != copyInfo) return;

        bool isSameExp = info._Exp <= originInfo._Exp;
        go_OfIncreaseable_Others?.SetActive(!isSameExp);
        go_ExpIncreaseValue?.SetActive(!isSameExp);

        bool originIsMaxLv = originInfo._IsMaxLevel;
        bool copyIsMaxLv = info._IsMaxLevel;
        if (tmpu_CopyLevel != null) tmpu_CopyLevel.text = info._Level.ToString();
        if (tmpu_CopyCurExp != null)
        {
            string txt = copyIsMaxLv switch
            {
                true => "-",
                _ => info._CurrentExp.ToString(),
            };
            tmpu_CopyCurExp.text = txt;
        }
        if (tmpu_CopyMaxExp != null)
        {
            string txt = copyIsMaxLv switch
            {
                true => "-",
                _ => info._RequireExp.ToString(),
            };
            tmpu_CopyMaxExp.text = txt;
        }
        if (tmpu_ExpIncreaseValue != null) tmpu_ExpIncreaseValue.text = "+" + (info._Exp - originInfo._Exp).ToString();

        go_AlarmMaxLevel?.SetActive(copyIsMaxLv);
        if (img_CopyExpFill != null) img_CopyExpFill.fillAmount = copyIsMaxLv ? 1f : (float)info._CurrentExp / info._RequireExp;

        var stat = new CharacterStat(copyInfo);
        if (statusInfo_ATK != null)
        {
            var type = Data.Enum.Stat.ATK;
            statusInfo_ATK.SetActiveIncreaseStat(true);
            statusInfo_ATK.SetIncreaseValue((int)originStat.GetStat(type), (int)stat.GetStat(type));
        }
        if (statusInfo_DEF != null)
        {
            var type = Data.Enum.Stat.DEF;
            statusInfo_DEF.SetActiveIncreaseStat(true);
            statusInfo_DEF.SetIncreaseValue((int)originStat.GetStat(type), (int)stat.GetStat(type));
        }
        if (statusInfo_HP != null)
        {
            var type = Data.Enum.Stat.HP;
            statusInfo_HP.SetActiveIncreaseStat(true);
            statusInfo_HP.SetIncreaseValue((int)originStat.GetStat(type), (int)stat.GetStat(type));
        }
    }

    private void ScrollerOnBeginDrag()
    {
        isDragging = true;
    }
    private void ScrollerOnEndDrag()
    {
        isDragging = false;
    }


    private void OnBeginSlideChangeCharacter(float delta)
    {

    }
    private void OnSlidingChangeCharacter(float delta)
    {

    }
    private void OnEndSlideChangeCharacter(float delta, bool conditionMet)
    {
        if (!conditionMet) return;

        bool isLeft = delta >= 0f;
        var nextIndex = currentInfoIndex + (isLeft ? -1 : 1);
        nextIndex = Mathf.Clamp(nextIndex, 0, list_ViewableCharacter.Count - 1);
        if (currentInfoIndex == nextIndex) return;

        currentInfoIndex = nextIndex;
        originInfo = list_ViewableCharacter[currentInfoIndex];
        Setup();
        characterIllust?.Set(true, originInfo._Data);
        if (tmpu_CharacterName != null) tmpu_CharacterName.text = Localizer.GetLocalizedStringName(Localizer.SheetType.NONE, originInfo._EnumID);
    }
    #endregion

    private void Setup()
    {
        isDragging = false;
        costOfSelections = 0;
        UpdateCostUI();

        if (copyInfo != null) copyInfo.onUpdateChExp -= OnUpdateCopyExp;
        if (originInfo == null) return;

        SetExpItemDataList();
        if (scroller != null)
        {
            scroller.Delegate = this;
            scroller.ReloadData();
        }

        if (tmpu_OriginLevel != null) tmpu_OriginLevel.text = originInfo._Level.ToString();
        go_OfMaxLevel?.SetActive(originInfo._IsMaxLevel);
        go_OfIncreaseable?.SetActive(!originInfo._IsMaxLevel);
        go_OfIncreaseable_Others?.SetActive(false);
        go_ExpIncreaseValue?.SetActive(false);

        originStat = new CharacterStat(originInfo);
        if (statusInfo_ATK != null) { statusInfo_ATK.SetStatName(Data.Enum.Stat.ATK); statusInfo_ATK.SetCurValue((int)originStat.GetStat(Data.Enum.Stat.ATK)); }
        if (statusInfo_DEF != null) { statusInfo_DEF.SetStatName(Data.Enum.Stat.DEF); statusInfo_DEF.SetCurValue((int)originStat.GetStat(Data.Enum.Stat.DEF)); }
        if (statusInfo_HP != null) { statusInfo_HP.SetStatName(Data.Enum.Stat.HP); statusInfo_HP.SetCurValue((int)originStat.GetStat(Data.Enum.Stat.HP)); }

        copyInfo = originInfo.Clone();
        copyInfo.onUpdateChExp -= OnUpdateCopyExp;
        copyInfo.onUpdateChExp += OnUpdateCopyExp;
        RewardTooltipHide();
        OnUpdateCopyExp(copyInfo);
    }

    private void SetExpItemDataList()
    {
        list_ExpItemData.Clear();
        dict_SelectionCount.Clear();
        foreach (var v in User._Consumables.Values)
        {
            if (v?._Data == null || v._Data._CE_Item_Sub != Data.Enum.Item_Sub.CHARACTER_EXP) continue;

            int bundleMax = (int)v._Data._Bundle_Max;
            for (int i = v._Count; i > 0; i -= bundleMax)
            {
                list_ExpItemData.Add(new LevelupItemData(v,
                    Mathf.Min(i, bundleMax),
                    CheckIsMaxLv,
                    CheckGoldIsEnough,
                    CheckDragging,
                    OnChangeSelectionState));
            }
            if (!dict_SelectionCount.ContainsKey(v._ID)) dict_SelectionCount.Add(v._ID, 0);
        }

        list_ExpItemData.Sort((a, b) =>
        {
            var aData = a._Info?._Data;
            var bData = b._Info?._Data;

            if (aData == null) return 1;
            else if (bData == null) return -1;

            var result = -aData._CE_Item_Grade.CompareTo(bData._CE_Item_Grade);
            if (result == 0) result = aData._Id.CompareTo(bData._Id);
            if (result == 0) result = -a._Count.CompareTo(b._Count);

            return result;
        });

        var count = list_ExpItemData.Count;
        if (count > MAX_ITEM_COUNT) list_ExpItemData.RemoveRange(MAX_ITEM_COUNT, list_ExpItemData.Count - MAX_ITEM_COUNT);
        else if (count < MAX_ITEM_COUNT)
        {
            for (int i = 0; i < MAX_ITEM_COUNT - count; i++)
                list_ExpItemData.Add(new LevelupItemData(
                    null,
                    0,
                    CheckIsMaxLv,
                    CheckGoldIsEnough,
                    CheckDragging,
                    OnChangeSelectionState));
        }
    }

    private void UpdateCostUI()
    {
        if (tmpu_CostValue == null) return;

        tmpu_CostValue.text = costOfSelections.ToString();
    }

    private void SetCopyInfoExp()
    {
        int addedExp = 0;
        foreach (var v in dict_SelectionCount)
        {
            if (!User._Consumables.TryGetValue(v.Key, out var info)) continue;

            var data = info._Data;
            if (data == null) continue;

            int value = data._Value * v.Value;
            addedExp += value;
        }

        copyInfo.SetExp(originInfo._Exp + addedExp);
    }

    private void RewardTooltipShow(LevelupItemData selectData)
    {
        if (rewardTooltip == null || selectData == null || selectData._Info == null) return;
        rewardTooltip.gameObject.SetActive(true);
        rewardTooltip.Set(new UIParam.Common.Reward(Data.Enum.Common_Type.ITEM, selectData._Info._EnumID, selectData._Info._Count));
    }
    private void RewardTooltipHide()
    {
        if (rewardTooltip == null) return;
        rewardTooltip.gameObject.SetActive(false);
    }
}
