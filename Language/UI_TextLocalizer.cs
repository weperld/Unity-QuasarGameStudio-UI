using TMPro;
using UnityEngine;

public class UI_TextLocalizer : MonoBehaviour
{
    public enum Function
    {
        LOCALIZER,
        HIDE,
        BOTH
    }
    public enum StringCase
    {
        ORIGIN,
        UPPER,
        LOWER,
        JADEN,
    }

    public Function functionType = Function.LOCALIZER;

    public string key;
    public Localizer.LocalTypeString type = Localizer.LocalTypeString._NAME;
    public Localizer.StringCase stringCase = Localizer.StringCase.ORIGIN;
    public bool useUserLanguage = true;
    public Data.Enum.Language customLanguage = Data.Enum.Language.LANGUAGE_KR;

    public Data.Enum.Language hideSpecificLanguage = Data.Enum.Language.LANGUAGE_EN;
    public GameObject go_HideTarget;

    protected Data.Enum.Language prevSelectLanguage = Data.Enum.Language.CNT;
    protected virtual void OnValidate()
    {
        if (customLanguage == Data.Enum.Language.CNT)
        {
            if (prevSelectLanguage == Data.Enum.Language.CNT) prevSelectLanguage = User._Language;
            customLanguage = prevSelectLanguage;
        }
    }

    protected virtual void OnEnable()
    {
        User.onChangeLanguage -= SetText;
        User.onChangeLanguage += SetText;
        SetText(User._Language);
    }
    protected virtual void OnDisable()
    {
        User.onChangeLanguage -= SetText;
    }

    protected virtual void SetText(Data.Enum.Language language)
    {
        Refresh();
    }

    public virtual void Refresh()
    {
        switch (functionType)
        {
            case Function.LOCALIZER:
                SetLocalizedText();
                break;
            case Function.HIDE:
                SetHideState();
                break;
            case Function.BOTH:
                SetHideState();
                SetLocalizedText();
                break;
        }
    }

    private void SetLocalizedText()
    {
        if (!TryGetComponent<TextMeshProUGUI>(out var tmp)) return;

        var lang = useUserLanguage ? User._Language : customLanguage;
        var local = Localizer.GetLocal(Localizer.SheetType.NONE, key, type, lang, stringCase);
        tmp.text = local;
    }
    private void SetHideState()
    {
        if (go_HideTarget == null) return;
        go_HideTarget.SetActive(User._Language != hideSpecificLanguage);
    }
}