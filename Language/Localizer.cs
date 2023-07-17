public static class Localizer
{
    public enum SheetType
    {
        NONE,
        WORLD,
        STRING,
        GACHA,
        SKILL,
        LIBRARY,
    }

    public enum LocalTypeString
    {
        _NAME,
        _TALK,
        _DESC
    }

    public enum StringCase
    {
        ORIGIN,
        UPPER,
        LOWER,
        JADEN,
    }

    public static string GetLocal(SheetType type, string key, Data.Enum.Language language, string def = "", StringCase stringCase = StringCase.ORIGIN)
    {
        var ret = def;
        switch (type)
        {
            case SheetType.NONE:
                if (Data._localizationTable.TryGetValue(key, out var commonLocal) && commonLocal != null)
                {
                    switch (language)
                    {
                        case Data.Enum.Language.LANGUAGE_KR: ret = commonLocal._Kr; break;
                        case Data.Enum.Language.LANGUAGE_EN: ret = commonLocal._En; break;
                    }
                }
                break;
            case SheetType.WORLD:
                if (Data._world_map_localTable.TryGetValue(key, out var worldLocal) && worldLocal != null)
                {
                    switch (language)
                    {
                        case Data.Enum.Language.LANGUAGE_KR: ret = worldLocal._Kr; break;
                        case Data.Enum.Language.LANGUAGE_EN: ret = worldLocal._En; break;
                    }
                }
                break;
            case SheetType.STRING:
                if (Data._string_localTable.TryGetValue(key, out var stringLocal) && stringLocal != null)
                {
                    switch (language)
                    {
                        case Data.Enum.Language.LANGUAGE_KR: ret = stringLocal._Kr; break;
                        case Data.Enum.Language.LANGUAGE_EN: ret = stringLocal._En; break;
                    }
                }
                break;
            case SheetType.GACHA:
                if (Data._gacha_localTable.TryGetValue(key, out var gachaLocal) && gachaLocal != null)
                {
                    switch (language)
                    {
                        case Data.Enum.Language.LANGUAGE_KR: ret = gachaLocal._Kr; break;
                        case Data.Enum.Language.LANGUAGE_EN: ret = gachaLocal._En; break;
                    }
                }
                break;
            case SheetType.SKILL:
                if (Data._skilllocalizationTable.TryGetValue(key, out var skillLocal) && skillLocal != null)
                {
                    switch (language)
                    {
                        case Data.Enum.Language.LANGUAGE_KR:
                            ret = skillLocal._Kr; break;
                        case Data.Enum.Language.LANGUAGE_EN:
                            ret = skillLocal._En; break;
                    }
                }
                break;
            case SheetType.LIBRARY:
                if (Data._library_localTable.TryGetValue(key, out var libLocal) && libLocal != null)
                {
                    switch (language)
                    {
                        case Data.Enum.Language.LANGUAGE_KR:
                            ret = libLocal._Kr; break;
                        case Data.Enum.Language.LANGUAGE_EN:
                            ret = libLocal._En; break;
                    }
                }
                break;
        }

        ret = stringCase switch
        {
            StringCase.UPPER => ret.ToUpper(),
            StringCase.LOWER => ret.ToLower(),
            StringCase.JADEN => ret.ToUpper()[0] + ret.ToLower().Substring(1),
            _ => ret,
        };
        return ret;
    }
    public static string GetLocal(string key, Data.Enum.Language language, string def = "", StringCase stringCase = StringCase.ORIGIN)
        => GetLocal(SheetType.NONE, key, language, def, stringCase);
    public static string GetLocal(SheetType type, string localizationKey, LocalTypeString typeString, Data.Enum.Language language, StringCase stringCase = StringCase.ORIGIN)
    {
        if (localizationKey == null) localizationKey = string.Empty;
        string _default = $"\"{localizationKey}{typeString}\"";
        string key = localizationKey + typeString.ToString();
        return GetLocal(type, key, language, _default, stringCase);
    }
    public static string GetLocal(string localizationKey, LocalTypeString typeString, Data.Enum.Language language, StringCase stringCase = StringCase.ORIGIN)
        => GetLocal(SheetType.NONE, localizationKey, typeString, language, stringCase);

    public static string GetLocalizedStringName(SheetType type, string localizationKey, Data.Enum.Language language, StringCase stringCase = StringCase.ORIGIN)
        => GetLocal(type, localizationKey, LocalTypeString._NAME, language, stringCase);
    public static string GetLocalizedStringTalk(SheetType type, string localizationKey, Data.Enum.Language language, StringCase stringCase = StringCase.ORIGIN)
        => GetLocal(type, localizationKey, LocalTypeString._TALK, language, stringCase);
    public static string GetLocalizedStringDesc(SheetType type, string localizationKey, Data.Enum.Language language, StringCase stringCase = StringCase.ORIGIN)
        => GetLocal(type, localizationKey, LocalTypeString._DESC, language, stringCase);

    public static string GetLocalizedStringName(SheetType type, string localizationKey, StringCase stringCase = StringCase.ORIGIN)
        => GetLocalizedStringName(type, localizationKey, User._Language, stringCase);
    public static string GetLocalizedStringTalk(SheetType type, string localizationKey, StringCase stringCase = StringCase.ORIGIN)
        => GetLocalizedStringTalk(type, localizationKey, User._Language, stringCase);
    public static string GetLocalizedStringDesc(SheetType type, string localizationKey, StringCase stringCase = StringCase.ORIGIN)
        => GetLocalizedStringDesc(type, localizationKey, User._Language, stringCase);
}
