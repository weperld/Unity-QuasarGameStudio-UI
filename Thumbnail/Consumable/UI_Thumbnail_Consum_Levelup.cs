using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Debug = COA_DEBUG.Debug;

public class UI_Thumbnail_Consum_Levelup : MonoBehaviour
{
    private class EventHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public Action onDown;
        public Action onUp;

        public void OnPointerDown(PointerEventData eventData)
        {
            onDown?.Invoke();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            onUp?.Invoke();
        }
    }

    [SerializeField] private UI_Thumbnail_Consum_Possession thumbnail;
    [SerializeField] private Button btn_Add;
    [SerializeField] private Button btn_Minus;
    [SerializeField] private TextMeshProUGUI tmpu_AddedCount;
    [SerializeField] private GameObject go_AdditionalExpMark;
    [SerializeField] private GameObject go_AddedCountInfo;
    [SerializeField] private GameObject go_SelectMark;
    [SerializeField] private GameObject[] go_SetActiveTargets;
    [SerializeField] private ParticleSystem[] eff_UseItems;
    [SerializeField] private Animator animator;

    public LevelupItemData _LvItemData { get; private set; }

    private float touchedTime;
    private IEnumerator handlerCorout;
    private IEnumerator touchTimeCalculatorCorout;

    private readonly int CHANGE_PER_SEC_MIN = 2;
    private readonly int CHANGE_PER_SEC_MAX = 30;
    private float changePerSec = 0f;

    private readonly float PRESSING_DETECT_DELAY = 0.5f;
    private readonly float CHANGE_PER_SEC_INCREASE_DELAY = 2f;

    private readonly float MINUS_TERM = 0.1f;


    private void Awake()
    {
        animator?.SetTrigger("Appear");

        if (btn_Add != null)
        {
            if (!btn_Add.TryGetComponent<EventHandler>(out var addHandler)) addHandler = btn_Add.gameObject.AddComponent<EventHandler>();
            addHandler.onDown = OnAddPointerDown;
            addHandler.onUp = OnAddPointerUp;
        }
        if (btn_Minus != null)
        {
            if (!btn_Minus.TryGetComponent<EventHandler>(out var minusHandler)) minusHandler = btn_Minus.gameObject.AddComponent<EventHandler>();
            minusHandler.onDown = OnMinusPointerDown;
            minusHandler.onUp = OnMinusPointerUp;
        }
    }

    private void OnDisable()
    {
        StopCoroutines();
    }

    public void SetData(LevelupItemData data)
    {
        if (_LvItemData != null)
        {
            _LvItemData.onChangeAddedCount -= OnChangeAddedCount;
            _LvItemData.playUseEffAction -= PlayUseEff;
        }

        _LvItemData = data;
        SetActive(_LvItemData != null);
        if (_LvItemData == null) return;

        _LvItemData.playUseEffAction -= PlayUseEff;
        _LvItemData.playUseEffAction += PlayUseEff;
        _LvItemData.onChangeAddedCount -= OnChangeAddedCount;
        _LvItemData.onChangeAddedCount += OnChangeAddedCount;
        OnChangeAddedCount(_LvItemData._AddedCount);

        var staticData = _LvItemData._Info?._Data;
        go_AdditionalExpMark?.SetActive(staticData != null && (int)staticData._CE_Item_Grade >= (int)Data.Enum.Item_Grade.LEGEND);

        thumbnail?.SetData(staticData, _LvItemData._Count);
    }

    public void SetActive(bool active)
    {
        if (go_SetActiveTargets == null) return;
        foreach (var v in go_SetActiveTargets)
            if (v != null) v.SetActive(active);
    }

    private void OnChangeAddedCount(int count)
    {
        go_AddedCountInfo?.SetActive(count > 0);
        go_SelectMark?.SetActive(count > 0);
        if (tmpu_AddedCount != null) tmpu_AddedCount.text = count.ToString();
    }

    private void OnAddPointerDown()
    {
        OnPointerDown(true);
    }
    private void OnAddPointerUp()
    {
        OnPointerUp(true);
    }
    private void OnMinusPointerDown()
    {
        OnPointerDown(false);
    }
    private void OnMinusPointerUp()
    {
        OnPointerUp(false);
    }

    private void OnPointerDown(bool add)
    {
        StopCoroutines();

        TouchTimeCalculator(add);
        handlerCorout = OnPointerDownCorout(add);
        StartCoroutine(handlerCorout);
    }
    private IEnumerator OnPointerDownCorout(bool add)
    {
        changePerSec = CHANGE_PER_SEC_MIN;

        yield return new WaitForSeconds(PRESSING_DETECT_DELAY);
        while (true)
        {
            var result = AddOrRemove(add);
            switch (result)
            {
                case LevelupItemData.AddOrRemoveRequestResult.NOT_AVAILABLE:
                    yield break;
                case LevelupItemData.AddOrRemoveRequestResult.A_SUCCESS:
                    animator?.SetTrigger("Add");
                    yield return new WaitForSeconds(1f / changePerSec);
                    changePerSec = (CHANGE_PER_SEC_MAX - CHANGE_PER_SEC_MIN) * GetTouchTimeRate() + CHANGE_PER_SEC_MIN;
                    if (changePerSec > CHANGE_PER_SEC_MAX) changePerSec = CHANGE_PER_SEC_MAX;
                    break;
                case LevelupItemData.AddOrRemoveRequestResult.A_FAIL_MAX_LV:
                    yield break;
                case LevelupItemData.AddOrRemoveRequestResult.A_FAIL_GOLD_NOT_ENOUGH:
                    UIMessageBox.OnelineMsg_NotEnoughGold(); yield break;
                case LevelupItemData.AddOrRemoveRequestResult.R_SUCCESS:
                    yield return new WaitForSeconds(MINUS_TERM); break;
                case LevelupItemData.AddOrRemoveRequestResult.R_FAIL:
                    yield break;
            }
        }
    }
    private void TouchTimeCalculator(bool add)
    {
        touchedTime = 0f;

        if (!add) return;
        touchTimeCalculatorCorout = TouchTimeCalculatorCorout();
        StartCoroutine(touchTimeCalculatorCorout);
    }
    private IEnumerator TouchTimeCalculatorCorout()
    {
        while (true)
        {
            yield return null;
            touchedTime += Time.deltaTime;
        }
    }

    private void OnPointerUp(bool add)
    {
        StopCoroutines();
        var check = _LvItemData._IsDragging;
        if (check) return;

        if (touchedTime < PRESSING_DETECT_DELAY)
        {
            var result = AddOrRemove(add);
            switch (result)
            {
                case LevelupItemData.AddOrRemoveRequestResult.NOT_AVAILABLE:
                case LevelupItemData.AddOrRemoveRequestResult.A_FAIL_MAX_LV:
                case LevelupItemData.AddOrRemoveRequestResult.R_FAIL:
                case LevelupItemData.AddOrRemoveRequestResult.R_SUCCESS:
                    break;
                case LevelupItemData.AddOrRemoveRequestResult.A_SUCCESS:
                    animator?.SetTrigger("Add"); break;
                case LevelupItemData.AddOrRemoveRequestResult.A_FAIL_GOLD_NOT_ENOUGH:
                    UIMessageBox.OnelineMsg_NotEnoughGold(); break;
            }
        }
    }

    private void StopCoroutines()
    {
        if (touchTimeCalculatorCorout != null) StopCoroutine(touchTimeCalculatorCorout);
        touchTimeCalculatorCorout = null;
        if (handlerCorout != null) StopCoroutine(handlerCorout);
        handlerCorout = null;
    }

    private LevelupItemData.AddOrRemoveRequestResult AddOrRemove(bool add)
    {
        if (_LvItemData == null) return LevelupItemData.AddOrRemoveRequestResult.NOT_AVAILABLE;

        if (add) return _LvItemData.Add();
        else return _LvItemData.Remove();
    }

    private float GetTouchTimeRate()
    {
        var min = PRESSING_DETECT_DELAY;
        var max = min + CHANGE_PER_SEC_INCREASE_DELAY;
        float ret = Mathf.Clamp(touchedTime, min, max) - min;

        return ret / CHANGE_PER_SEC_INCREASE_DELAY;
    }

    private void PlayUseEff()
    {
        if (eff_UseItems == null) return;
        foreach (var eff in eff_UseItems)
            if (eff != null) eff.Play();
    }
}