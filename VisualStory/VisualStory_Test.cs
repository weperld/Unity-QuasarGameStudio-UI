using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Debug = COA_DEBUG.Debug;
using UnityEngine.EventSystems;

namespace VisualStoryTest
{
    public class PointerInOutDetect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public Action onEnter;
        public Action onExit;

        public void OnPointerEnter(PointerEventData eventData)
        {
            onEnter?.Invoke();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            onExit?.Invoke();
        }
    }

    public class VisualStory_Test : MonoBehaviour
    {
        public DataLoader dataLoader;
        public GameObject go_LoadProcess;
        public TextMeshProUGUI tmpu_LoadProcess;

        public UIBase_VisualStory script;
        public UIBase_VisualStory oracle;

        [Header("Visual Story Test Buttons")]
        public Button btn_Restart;
        public Button btn_Reset;
        public GameObject go_ButtonShowDetectArea;
        public RectTransform rtf_ButtonRoot;

        [Header("VisualStory Work Setting UI")]
        public GameObject go_WorkSettingRoot;
        public TMP_Dropdown tmpd_VSType;
        public TMP_InputField tmpif_Character;
        public TMP_InputField tmpif_Timeline;
        public Button btn_Start;

        [Header("Work Start Error Msg")]
        public GameObject go_ErrorUI;
        public TextMeshProUGUI tmpu_Error;
        public Button btn_ErrorClose;

        [Flags]
        private enum WorkStartErrorState
        {
            NONE = 0,
            TYPE_IS_NOT_VALUEABLE = 1,
            CHARACTER_IS_NOT_VALUEABLE = 1 << 1,
            TIMELINE_IS_NOT_VALUEABLE = 1 << 2,
        }
        private WorkStartErrorState workStartErrorState;
        private Data.Enum.Story_Type vsType;
        private string characterEnumId;
        private string timelineEnumId;

        private readonly string playerPrefsKey_VSTYPE = "VSTEST_LastSelectedWorkVisualStoryType";
        private readonly string playerPrefsKey_CharEnum = "VSTEST_LastEnteredCharacterEnumId";
        private readonly string playerPrefsKey_TimelineEnum = "VSTEST_LastEnteredTimelineEnumId";

        private IEnumerator testButtonShowCorout;

        private IEnumerator Start()
        {
            UIUtil.ResetAndAddListener(btn_Start, OnClickWorkStart);
            UIUtil.ResetAndAddListener(btn_ErrorClose, OnClickErrorClose);
            UIUtil.ResetAndAddListener(tmpd_VSType, OnChangeVisualStoryType);
            UIUtil.ResetAndAddListener(btn_Restart, OnClickRestart);
            UIUtil.ResetAndAddListener(btn_Reset, OnClickReset);
            tmpif_Character.onValueChanged.RemoveAllListeners();
            tmpif_Character.onValueChanged.AddListener(OnEnterCharacter);
            tmpif_Timeline.onValueChanged.RemoveAllListeners();
            tmpif_Timeline.onValueChanged.AddListener(OnEnterTimeline);

            if (go_ButtonShowDetectArea != null)
            {
                if (!go_ButtonShowDetectArea.TryGetComponent<PointerInOutDetect>(out var component))
                    component = go_ButtonShowDetectArea.AddComponent<PointerInOutDetect>();

                component.onEnter = OnPointerEnterIntoButtonsShowArea;
                component.onExit = OnPointerExitFromButtonsShowArea;
            }

            go_ErrorUI.SetActive(false);

            dataLoader.LoadTableData();
            go_LoadProcess.SetActive(true);
            while (!dataLoader._Load)
            {
                tmpu_LoadProcess.text = $"{dataLoader._CurCount}/{dataLoader._MaxCount}";
                yield return null;
            }

            List<TMP_Dropdown.OptionData> typeOptionList = new List<TMP_Dropdown.OptionData>();
            for (int i = 0; i < (int)Data.Enum.Story_Type.CNT; i++)
            {
                var option = new TMP_Dropdown.OptionData(((Data.Enum.Story_Type)i).ToString());
                typeOptionList.Add(option);
            }
            tmpd_VSType.ClearOptions();
            tmpd_VSType.AddOptions(typeOptionList);

            tmpd_VSType.value = PlayerPrefs.GetInt(playerPrefsKey_VSTYPE, 0);
            tmpif_Character.text = PlayerPrefs.GetString(playerPrefsKey_CharEnum, string.Empty);
            tmpif_Timeline.text = PlayerPrefs.GetString(playerPrefsKey_TimelineEnum, string.Empty);

            go_LoadProcess.SetActive(false);
        }

        private void OnDisable()
        {
            if (testButtonShowCorout != null) StopCoroutine(testButtonShowCorout); testButtonShowCorout = null;
        }

        private void OnClickWorkStart()
        {
            var infoParam = vsType switch
            {
                Data.Enum.Story_Type.SCRIPT => new VisualStoryInfo(characterEnumId, timelineEnumId),
                Data.Enum.Story_Type.ORACLE => new OracleInfo(characterEnumId, timelineEnumId),
                _ => null
            };

            workStartErrorState = WorkStartErrorState.NONE;
            if (infoParam == null) workStartErrorState |= WorkStartErrorState.TYPE_IS_NOT_VALUEABLE;
            else
            {
                if (infoParam._Character == null) workStartErrorState |= WorkStartErrorState.CHARACTER_IS_NOT_VALUEABLE;
                if (infoParam._RootTimeline == null) workStartErrorState |= WorkStartErrorState.TIMELINE_IS_NOT_VALUEABLE;
            }

            if (workStartErrorState != WorkStartErrorState.NONE)
            {
                OccuredWorkStartError(workStartErrorState);
                return;
            }

            PlayerPrefs.SetInt(playerPrefsKey_VSTYPE, (int)vsType);
            PlayerPrefs.SetString(playerPrefsKey_CharEnum, characterEnumId);
            PlayerPrefs.SetString(playerPrefsKey_TimelineEnum, timelineEnumId);

            go_WorkSettingRoot.SetActive(false);

            var hide = vsType == Data.Enum.Story_Type.SCRIPT ? oracle : script;
            var show = vsType == Data.Enum.Story_Type.SCRIPT ? script : oracle;

            hide.gameObject.SetActive(false);
            show.Show(new UIParam.VisualStory.Main(infoParam) { isTestMode = true });
        }
        private void OnChangeVisualStoryType(int value)
        {
            vsType = (Data.Enum.Story_Type)value;
        }
        private void OnEnterCharacter(string value)
        {
            characterEnumId = value;
        }
        private void OnEnterTimeline(string value)
        {
            timelineEnumId = value;
        }

        private void OccuredWorkStartError(WorkStartErrorState state)
        {
            if (state == WorkStartErrorState.NONE) return;

            go_ErrorUI.SetActive(true);
            var error = "";
            if (state.HasFlag(WorkStartErrorState.TYPE_IS_NOT_VALUEABLE))
                error += $"<color=#F62121>Error on Story Type Value Set.\n" +
                    $"\tSelected Value: [{vsType}]</color>";
            if (state.HasFlag(WorkStartErrorState.CHARACTER_IS_NOT_VALUEABLE))
            {
                if (!string.IsNullOrEmpty(error)) error += "\n";
                error += $"<color=#FC9816>Error on Character Enum Id Set.\n" +
                    $"\tEntered Value: [{characterEnumId}]</color>";
            }
            if (state.HasFlag(WorkStartErrorState.TIMELINE_IS_NOT_VALUEABLE))
            {
                if (!string.IsNullOrEmpty(error)) error += "\n";
                error += $"<color=#FAF25E>Error on Timeline Enum Id Set.\n" +
                    $"\tEntered Value: [{timelineEnumId}]</color>";
            }
            tmpu_Error.text = error;
        }
        private void OnClickErrorClose()
        {
            go_ErrorUI.SetActive(false);
        }

        private void OnClickRestart()
        {
            var infoParam = vsType switch
            {
                Data.Enum.Story_Type.SCRIPT => new VisualStoryInfo(characterEnumId, timelineEnumId),
                Data.Enum.Story_Type.ORACLE => new OracleInfo(characterEnumId, timelineEnumId),
                _ => null
            };
            var show = vsType == Data.Enum.Story_Type.SCRIPT ? script : oracle;
            show.Show(new UIParam.VisualStory.Main(infoParam));
        }
        private void OnClickReset()
        {
            var show = vsType == Data.Enum.Story_Type.SCRIPT ? script : oracle;
            show.TestClose();
            go_WorkSettingRoot.SetActive(true);
        }

        private void OnPointerEnterIntoButtonsShowArea()
        {
            if (testButtonShowCorout != null) StopCoroutine(testButtonShowCorout);
            testButtonShowCorout = ShowOrHideTestButtonCorout(true);
            StartCoroutine(testButtonShowCorout);
        }
        private void OnPointerExitFromButtonsShowArea()
        {
            if (testButtonShowCorout != null) StopCoroutine(testButtonShowCorout);
            testButtonShowCorout = ShowOrHideTestButtonCorout(false);
            StartCoroutine(testButtonShowCorout);
        }
        private IEnumerator ShowOrHideTestButtonCorout(bool show)
        {
            if (rtf_ButtonRoot == null) yield break;

            var endPos = show ? 0f : -200f;
            var ancPos = rtf_ButtonRoot.anchoredPosition;
            var curPos = ancPos.x;

            while (!Mathf.Approximately(curPos, endPos))
            {
                yield return null;
                ancPos.x = curPos = Mathf.MoveTowards(curPos, endPos, Time.deltaTime * 1000f);
                rtf_ButtonRoot.anchoredPosition = ancPos;
            }

            ancPos.x = endPos;
            rtf_ButtonRoot.anchoredPosition = ancPos;
            testButtonShowCorout = null;
        }
    }
}