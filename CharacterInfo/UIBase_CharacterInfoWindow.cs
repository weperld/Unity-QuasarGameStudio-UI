using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Debug = COA_DEBUG.Debug;
using UnityEngine.EventSystems;
using AudioSystem;

public static class CharacterInfoSlide
{
    public static Action<float> onBeginSlide;
    public static Action<float> onSliding;
    public static Action<float, bool> onEndSlide;

    public static float _MinimumDeltaForChange => Screen.width / 8f;
    public static float _TotalSlideDelta { get; private set; }

    public static void OnBeginChangeCharacter(Vector2 p, Vector2 n, PointerEventData e)
    {
        _TotalSlideDelta = 0f;
        onBeginSlide?.Invoke(_TotalSlideDelta);
    }
    public static void OnChangingCharacter(Vector2 p, Vector2 n, PointerEventData e)
    {
        _TotalSlideDelta += (n - p).x;
        onSliding?.Invoke(_TotalSlideDelta);
    }
    public static void OnEndChangeCharacter( Vector2 p, Vector2 n, PointerEventData e)
    {
        _TotalSlideDelta += (n - p).x;
        onEndSlide?.Invoke(_TotalSlideDelta, Mathf.Abs(_TotalSlideDelta) >= _MinimumDeltaForChange);
    }



    [Serializable, Tooltip("\t포지션 이동은 참조 오브젝트만 적용, 알파값은 참조한 캔버스 그룹에 적용")]
    public class ProductionData
    {
        [SerializeField] private RectTransform rtf_Root;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private AnimationCurve curve_Alpha;
        [SerializeField] private AnimationCurve curve_PositionValue_X;
        [SerializeField] private float positionValue_X_Mult;
    }
}

public class UIBase_CharacterInfoWindow : UIBase
{
    [Serializable]
    public class SkillUI
    {
        [SerializeField] private GameObject go_Root;
        [SerializeField] protected Image img_Icon;
        [SerializeField] protected TextMeshProUGUI tmpu_Type;

        public void SetActive(bool active)
        {
            if (go_Root == null)
                return;
            go_Root.SetActive(active);
        }
        public virtual void SetSkillInfoUI(IAssetOwner assetOwner, skillpresetTable spData, Data.Enum.Skill_Type type)
        {
            if (tmpu_Type != null)
            {
                tmpu_Type.text = Localizer.GetLocalizedStringName(Localizer.SheetType.SKILL, type.ToString());
            }

            bool active = spData != null;
            SetActive(active);
            if (!active)
                return;

            if (img_Icon != null && spData != null)
            {
                img_Icon.sprite = spData._Icon_Reference_Data?.GetSpriteFromSMTable(assetOwner);
            }
        }
    }

    #region Inspector
    [Header("Scene Button")]
    [SerializeField] private Button btn_Back;
    [SerializeField] private Button btn_Home;
    [SerializeField] private Button btn_SceneDesc;
    [SerializeField] private Button btn_FocusIllust;
    [SerializeField] private Button btn_Signature_Weapon;
    [SerializeField] private Button btn_FateCard;

    [Header("Background")]
    [SerializeField] private GameObject go_NightBg;
    [SerializeField] private GameObject go_DayBg;
    [SerializeField] private GameObject go_SceneGradationImg_Night;
    [SerializeField] private GameObject go_SceneGradationImg_Day;

    [Header("Character Illust")]
    [SerializeField] private GameObject go_IllustRoot;
    [SerializeField] private GameObject[] go_OffsWhenFocusIllust;
    [SerializeField] private Character_UI_Illust_withBg character_Illust_WithBg;
    [SerializeField] private UI_DragDetector dragArea_Illust;
    [SerializeField] private GameObject go_RootOnFocusIllust;

    [Header("Characteristic Info")]
    [SerializeField] private UI_CharacterInfo_NotSkillInfo notSkillInfo;
    [SerializeField] private SkillUI skillUI_ULT;
    [SerializeField] private SkillUI skillUI_ACT;
    [SerializeField] private SkillUI skillUI_BSC;
    [SerializeField] private Button btn_OpenSkillDesc;
    [SerializeField] private Image img_SkillButton_Off;
    [SerializeField] private Image img_SkillButton_On;
    [SerializeField] private Image img_SkillDecoBar;
    [SerializeField] private GameObject go_SkillDescOpenState;
    [SerializeField] private UI_CharacterInfo_SkillDescriptionPanel ui_SkillDesc;
    [SerializeField] private Image img_AffectionGauge;
    [SerializeField] private TextMeshProUGUI tmpu_Affection;
    [SerializeField] private Toggle tgl_ChangeCostume;
    [SerializeField] private Button btm_NoneCostume;
    [SerializeField] private TextMeshProUGUI tmpu_CrewName;
    [SerializeField] private TextMeshProUGUI tmpu_CharacterDesc;

    [Header("AddOn")]
    [SerializeField] private Toggle tgl_SpineOnOff;
    [SerializeField] private Button btn_Levelup;
    [SerializeField] private Button[] btn_NotPrepared;

    [Header("Character Change Slide")]
    [SerializeField] private UI_DragDetector changeDragDetector;
    //[SerializeField] private CharacterInfoSlide.ProductionData[] changeProductions;
    #endregion

    #region Variables
    private bool isInit = false;

    private UIParam.Character_Inventory.CharacterInfoWindow showParam;

    private CharacterInfo _Info;
    private characterTable _Table => _Info?._Data;
    private CharacterCollectionInfo _CollectionInfo;
    private List<CharacterInfo> list_ViewableCharacter = new List<CharacterInfo>();
    private List<string> list_ChCostume = new List<string>();
    private int currentInfoIndex;
    private int costume_Index;
    private bool isLock;
    #endregion

    #region Base Methods
    private void OnEnable()
    {
        CharacterInfoSlide.onBeginSlide -= OnBeginSlideChangeCharacter;
        CharacterInfoSlide.onBeginSlide += OnBeginSlideChangeCharacter;
        CharacterInfoSlide.onSliding -= OnSlidingChangeCharacter;
        CharacterInfoSlide.onSliding += OnSlidingChangeCharacter;
        CharacterInfoSlide.onEndSlide -= OnEndSlideChangeCharacter;
        CharacterInfoSlide.onEndSlide += OnEndSlideChangeCharacter;

        User.onChangeLanguage -= OnChangeLanguage;
        User.onChangeLanguage += OnChangeLanguage;
    }
    private void OnDisable()
    {
        CharacterInfoSlide.onBeginSlide -= OnBeginSlideChangeCharacter;
        CharacterInfoSlide.onSliding -= OnSlidingChangeCharacter;
        CharacterInfoSlide.onEndSlide -= OnEndSlideChangeCharacter;
        if (_CollectionInfo != null)
        {
            _CollectionInfo.onUpdateAffection -= OnUpdateAffectionExp;
        }

        User.onChangeLanguage -= OnChangeLanguage;
    }

    UIBase confirm;
    public override void Show(object param)
    {
        if (!isInit)
        {
            ui_SkillDesc?.Setup(() => SetActiveSkillDescOpenState(false));
            UIUtil.ResetAndAddListener(btn_Back, OnClickBackButton);
            UIUtil.ResetAndAddListener(btn_Home, OnClickHomeButton);
            UIUtil.ResetAndAddListener(btn_SceneDesc, OnClickGuideButton);
            UIUtil.ResetAndAddListener(btn_FocusIllust, OnClickFocusIllustButton);
            UIUtil.ResetAndAddListener(btn_OpenSkillDesc, OnClickOpenSkillDescButton);
            UIUtil.ResetAndAddListener(btn_Levelup, OnClickLevelupButton);
            UIUtil.ResetAndAddListener(tgl_SpineOnOff, OnValueChangeSpineOnOffToggle);
            UIUtil.ResetAndAddListener(btn_Signature_Weapon, OnClickSignatureWeaponBtn);
            UIUtil.ResetAndAddListener(btn_FateCard, OnClickFateCard);



            if (btn_NotPrepared != null)
                foreach (var btn in btn_NotPrepared)
                    UIUtil.ResetAndAddListener(btn, OnClickNotPreparedButton);

            if (dragArea_Illust != null)
            {
                dragArea_Illust.onDragging = OnDraggingIllustScroll;
                dragArea_Illust.onClick = _ => { OnClickFocusIllustOffButton(); };
                dragArea_Illust.onSpread = OnPinchToZoomIllust;
            }

            if (changeDragDetector != null)
            {
                changeDragDetector.onBeginDrag = CharacterInfoSlide.OnBeginChangeCharacter;
                changeDragDetector.onEndDrag = CharacterInfoSlide.OnEndChangeCharacter;
                changeDragDetector.onDragging = CharacterInfoSlide.OnChangingCharacter;
            }
            isInit = true;
        }


        showParam = param as UIParam.Character_Inventory.CharacterInfoWindow;
        if (showParam == null || !(User._Character.TryGetValue(showParam.selectedId, out var info) && info != null))
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

        base.Show(param);

        SetCharacterInfo(info);
        list_ViewableCharacter.Clear();
        foreach (var v in showParam.characterIdArr)
        {
            info = User._Character.GetDataFromTable(v);
            if (info == null)
                return;

            list_ViewableCharacter.Add(info);
        }
        currentInfoIndex = list_ViewableCharacter.IndexOf(_Info);
    }
    #endregion

    #region Listener
    private void OnClickBackButton()
    {
        Hide();
    }
    private void OnClickHomeButton()
    {
        UIManager._instance.HideAll();
    }
    private void OnClickGuideButton()
    {
        AudioManager._instance.PlaySound(DefineName.Audio_OutGame.Sound.BUTTON);
        UIMessageBox.Guide(Data.Enum.Guide_Type.CHARACTER);
    }
    private void OnClickFocusIllustButton()
    {
        AudioManager._instance.PlaySound(DefineName.Audio_OutGame.Sound.BUTTON);
        if (go_OffsWhenFocusIllust != null)
        {
            foreach (var v in go_OffsWhenFocusIllust)
                v.SetActive(false);
        }
        go_RootOnFocusIllust?.SetActive(true);
    }
    private void OnClickFocusIllustOffButton()
    {
        ResetFocusIllustState();
    }
    private void OnClickOpenSkillDescButton()
    {
        AudioManager._instance.PlaySound(DefineName.Audio_OutGame.Sound.BUTTON);
        SetActiveSkillDescOpenState(true);
        ui_SkillDesc?.Show(_Info?._Data?._Skill_Set_Data);
    }
    private void OnClickLevelupButton()
    {
        AudioManager._instance.PlaySound(DefineName.Audio_OutGame.Sound.BUTTON);
        showParam.selectedId = _Info._ID;
        UIManager._instance.ShowUIBase<UIBase>(DefineName.UILevelup.CHARACTER, showParam);
    }
    private void OnClickNotPreparedButton()
    {
        UIMessageBox.OnelineMsg_NotReady();
    }

    private void OnClickNoneCostumeBtn()
    {
        UIMessageBox.OnelineMessage(Localizer.GetLocalizedStringDesc(Localizer.SheetType.NONE, "UI_Oneline_Costume_Not_Ready"));
    }

    private void OnValueChangeCostumeButton(bool isNight)
    {
        if (isNight)
            costume_Index = 0;
        else
            costume_Index = 1;

        go_NightBg?.SetActive(isNight);
        go_DayBg?.SetActive(!isNight);

        isLock = !list_ChCostume.Contains(_Table._Costume_Bundle_Data._Costume_Data[costume_Index]._Enum_Id);

        var useSG = tgl_SpineOnOff.isOn;
        character_Illust_WithBg?.Set_Illust_withBg(tgl_SpineOnOff.isOn, _Table, costume_Index, isLock);
        if (useSG)
        {
            var sg = character_Illust_WithBg?._Skeleton;
            sg?.SetAnimation(Ani.idle);
        }

        tgl_SpineOnOff.gameObject.SetActive(!isLock);

        go_SceneGradationImg_Night?.SetActive(isNight);
        go_SceneGradationImg_Day?.SetActive(!isNight);
    }

    private void OnValueChangeSpineOnOffToggle(bool isOn)
    {
        character_Illust_WithBg?.SetActiveSkeleton(isOn);
        if (isOn)
        {
            var sg = character_Illust_WithBg?._Skeleton;
            sg?.SetAnimation(Ani.idle);
        }
    }

    private void OnClickSignatureWeaponBtn()
    {
        AudioManager._instance.PlaySound(DefineName.Audio_OutGame.Sound.BUTTON);
        UIManager._instance.ShowUIBase<UIBase_SignatureWeapon>(DefineName.SignatureWeapon.UIBASE_SIGNATUREWEAPON, _Info);
    }

    private void OnClickFateCard()
    {
        AudioManager._instance.PlaySound(DefineName.Audio_OutGame.Sound.BUTTON);
        UIManager._instance.ShowUIBase<UIBase_FateCard>(DefineName.UIFateCard.UIBASE_FATECARD, _Info);
    }

    private void OnUpdateAffectionExp(int lv, int curExp, int reqExp, int totalExp)
    {
        if (img_AffectionGauge != null)
        {
            img_AffectionGauge.fillAmount = (float)curExp / reqExp;
        }
        if (tmpu_Affection != null)
        {
            tmpu_Affection.text = lv.ToString();
        }
    }

    private void OnDraggingIllustScroll(Vector2 prev, Vector2 current, PointerEventData eventData)
    {
        if (go_IllustRoot == null)
            return;

        var delta = current - prev;
        var boundary = GetComponent<RectTransform>().rect.size;
        var xB = boundary.x / 2f;
        var yB = boundary.y / 2f;
        var pos = go_IllustRoot.GetComponent<RectTransform>().anchoredPosition;
        pos += delta;
        pos.x = Mathf.Clamp(pos.x, -xB, xB);
        pos.y = Mathf.Clamp(pos.y, -yB, yB);

        go_IllustRoot.GetComponent<RectTransform>().anchoredPosition = pos;
    }
    private void OnPinchToZoomIllust(float prev, float next, Touch[] touches)
    {
        if (go_IllustRoot == null)
            return;

        var delta = next - prev;
        var scal = go_IllustRoot.transform.localScale;
        var curVal = scal.x;
        var mult = curVal + delta * 0.001f;
        mult = Mathf.Clamp(mult, 0.5f, 1.3f);
        scal = Vector3.one * mult;
        go_IllustRoot.transform.localScale = scal;
    }

    private void OnBeginSlideChangeCharacter(float delta)
    {

    }
    private void OnSlidingChangeCharacter(float delta)
    {

    }
    private void OnEndSlideChangeCharacter(float delta, bool conditionMet)
    {
        if (!conditionMet)
            return;

        bool isLeft = delta >= 0f;
        var nextIndex = currentInfoIndex + (isLeft ? -1 : 1);
        nextIndex = Mathf.Clamp(nextIndex, 0, list_ViewableCharacter.Count - 1);
        if (currentInfoIndex == nextIndex)
            return;

        currentInfoIndex = nextIndex;
        SetCharacterInfo(list_ViewableCharacter[currentInfoIndex]);
    }

    private void OnChangeLanguage(Data.Enum.Language lang)
    {
        if (tmpu_CrewName != null)
            tmpu_CrewName.text = Localizer.GetLocalizedStringName(Localizer.SheetType.LIBRARY, _Table._Crew_Data._Enum_Id, lang);
        if (tmpu_CharacterDesc != null)
            tmpu_CharacterDesc.text = Localizer.GetLocalizedStringDesc(Localizer.SheetType.NONE, _Table._Enum_Id, lang);
    }
    #endregion

    private void SetActiveSkillDescOpenState(bool active)
    {
        go_SkillDescOpenState?.SetActive(active);
    }

    private void ResetFocusIllustState()
    {
        if (go_IllustRoot != null)
        {
            go_IllustRoot.transform.position = Vector3.zero;
            go_IllustRoot.transform.localScale = Vector3.one;
        }
        if (go_OffsWhenFocusIllust != null)
        {
            foreach (var v in go_OffsWhenFocusIllust)
                v.SetActive(true);
        }
        go_RootOnFocusIllust?.SetActive(false);
    }

    private List<string> list_Lock = new List<string>
    {
        "Character_Angelina",
        "Character_Seiler",
        "Character_Asnaro",
        "Character_Argo",
        "Character_Lahell"
    };

    private void SetCharacterInfo(CharacterInfo info)
    {
        if (_CollectionInfo != null)
            _CollectionInfo.onUpdateAffection -= OnUpdateAffectionExp;
        ResetFocusIllustState();
        notSkillInfo?.Set(info);

        if (info == null)
            return;

        _Info = info;
        var rsc = _Table?._Resource_List_Data;

        if (list_Lock.Contains(_Info._EnumID))
        {
            tgl_ChangeCostume.gameObject.SetActive(false);
            btm_NoneCostume.gameObject.SetActive(true);
            UIUtil.ResetAndAddListener(btm_NoneCostume, OnClickNoneCostumeBtn);
        }
        else
        {
            tgl_ChangeCostume.gameObject.SetActive(true);
            btm_NoneCostume.gameObject.SetActive(false);
            UIUtil.ResetAndAddListener(tgl_ChangeCostume, OnValueChangeCostumeButton);
        }

        costume_Index = 0;

        character_Illust_WithBg?.gameObject.SetActive(false);

        //if (User._CharacterCollection.TryGetValue(_Table._Enum_Id, out _CollectionInfo) && _CollectionInfo != null)
        //{
        //    _CollectionInfo.onUpdateAffection -= OnUpdateAffectionExp;
        //    _CollectionInfo.onUpdateAffection += OnUpdateAffectionExp;
        //    OnUpdateAffectionExp(_CollectionInfo._AffectionLevel,
        //        _CollectionInfo._AffectionCurrentExp,
        //        _CollectionInfo._AffectionRequireExp,
        //        _CollectionInfo.AffectionExp);
        //}

        var rscData = Data._character_type_resourceTable.GetDataFromTable(_Table._CE_Character_Property.ToString());
        var skillBtnSprite = rscData?._Image_Ref_Data[1]?.GetSpriteFromSMTable(this);
        if (img_SkillButton_Off != null)
            img_SkillButton_Off.sprite = skillBtnSprite;
        if (img_SkillButton_On != null)
            img_SkillButton_On.sprite = skillBtnSprite;
        if (img_SkillDecoBar != null)
            img_SkillDecoBar.sprite = rscData?._Image_Ref_Data[2]?.GetSpriteFromSMTable(this);

        if (tgl_ChangeCostume != null)
            tgl_ChangeCostume.isOn = true;
        if (tgl_SpineOnOff != null)
            tgl_SpineOnOff.isOn = true;

        var useSG = tgl_SpineOnOff != null ? tgl_SpineOnOff.isOn : true;

        list_ChCostume.Clear();

        User._CharacterEnumID.TryGetValue(_Table._Enum_Id, out var enumInfo);

        list_ChCostume.AddRange(enumInfo.Ch_Costume_List);
        isLock = !list_ChCostume.Contains(_Table._Costume_Bundle_Data._Costume_Data[costume_Index]._Enum_Id);
        character_Illust_WithBg?.Set_Illust_withBg(useSG, _Table, costume_Index, isLock);
        character_Illust_WithBg?.gameObject.SetActive(true);

        if (useSG)
        {
            var sg = character_Illust_WithBg?._Skeleton;
            sg?.SetAnimation(Ani.idle);
        }

        tgl_SpineOnOff.gameObject.SetActive(!isLock);


        var skillSetData = _Table._Skill_Set_Data;
        if (skillSetData != null)
        {
            var offset = "_LV1";
            skillpresetTable[] tmpskillarr = new skillpresetTable[3] { null, null, null };
            for (int i = 0; i < skillSetData._CE_Skill_Type.Length; i++)
            {
                switch (skillSetData._CE_Skill_Type[i])
                {
                    case Data.Enum.Skill_Type.ULTIMATE:
                        tmpskillarr[0] = Data._skillpresetTable.GetDataFromTable(skillSetData._Skill[i] + offset);
                        break;
                    case Data.Enum.Skill_Type.PASSIVE:
                        tmpskillarr[1] = Data._skillpresetTable.GetDataFromTable(skillSetData._Skill[i] + offset);
                        break;
                    case Data.Enum.Skill_Type.BASIC:
                        tmpskillarr[2] = Data._skillpresetTable.GetDataFromTable(skillSetData._Skill[i] + offset);
                        break;
                }
            }
            skillUI_ULT.SetSkillInfoUI(this, tmpskillarr[0], Data.Enum.Skill_Type.ULTIMATE);
            skillUI_ACT.SetSkillInfoUI(this, tmpskillarr[1], Data.Enum.Skill_Type.PASSIVE);
            skillUI_BSC.SetSkillInfoUI(this, tmpskillarr[2], Data.Enum.Skill_Type.BASIC);
        }
        ui_SkillDesc?.SetActive(false);

        OnChangeLanguage(User._Language);
    }
}
