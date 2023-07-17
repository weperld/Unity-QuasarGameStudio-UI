using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class UI_TextLocalizer : MonoBehaviour
{
    enum Table
    {
        WORLD,
        TABLE,
    }
    enum Type
    {
        NAME,
        DESC,
        TALK,
    }

    [SerializeField] Table _Table = Table.TABLE;
    [SerializeField] Type _Type = Type.NAME;
    [SerializeField] string _Key = "";

    private TextMeshProUGUI tmpu;

    private void Awake()
    {
        tmpu = GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        SetText();
    }

    public void Set(string key)
    {
        _Key = key;
        SetText();
    }

    private void SetText()
    {
        if (tmpu == null) return;

        if (string.IsNullOrEmpty(_Key))
        {
            if (_Table == Table.TABLE)
            {
                switch (_Type)
                {
                    case Type.NAME:
                        tmpu.text = Localizer.GetLocalizedStringName(Localizer.SheetType.STRING, _Key);
                        break;
                    case Type.DESC:
                        tmpu.text = Localizer.GetLocalizedStringDesc(Localizer.SheetType.STRING, _Key);
                        break;
                    case Type.TALK:
                        tmpu.text = Localizer.GetLocalizedStringTalk(Localizer.SheetType.STRING, _Key);
                        break;
                }
            }
            else if (_Table == Table.WORLD)
            {
                switch (_Type)
                {
                    case Type.NAME:
                        tmpu.text = Localizer.GetLocalizedStringName(Localizer.SheetType.WORLD, _Key);
                        break;
                    case Type.DESC:
                        tmpu.text = Localizer.GetLocalizedStringDesc(Localizer.SheetType.WORLD, _Key);
                        break;
                    case Type.TALK:
                        tmpu.text = Localizer.GetLocalizedStringTalk(Localizer.SheetType.WORLD, _Key);
                        break;
                }
            }
        }
    }
}
