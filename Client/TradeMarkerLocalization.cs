#if SPT_CLIENT
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using BepInEx;
using MoeTradeMarker.Shared;

namespace MoeTradeMarker.Client;

internal static class TradeMarkerLocalization
{
    public static TradeMarkerLanguage Language { get; private set; } = TradeMarkerLanguage.English;

    public static void Initialize()
    {
        Refresh();
    }

    public static void Refresh()
    {
        Language = TradeMarkerClientConfig.LanguageMode switch
        {
            TradeMarkerLanguageMode.Chinese => TradeMarkerLanguage.Chinese,
            TradeMarkerLanguageMode.English => TradeMarkerLanguage.English,
            _ => TradeMarkerLocalizer.DetectLanguage(DetectLanguageCode()),
        };
    }

    public static string Text(TradeMarkerText key)
    {
        return TradeMarkerLocalizer.Text(key, Language);
    }

    public static string Format(TradeMarkerText key, params object?[] args)
    {
        return TradeMarkerLocalizer.Format(key, Language, args);
    }

    public static string LanguageCode => Language == TradeMarkerLanguage.Chinese ? "zh-CN" : "en";

    private static string? DetectLanguageCode()
    {
        return DetectFromSptGameSettings()
            ?? DetectFromLocaleManager()
            ?? DetectFromEftTypes()
            ?? "en";
    }

    private static string? DetectFromSptGameSettings()
    {
        foreach (var path in GetSptGameSettingsPaths())
        {
            var value = ReadLanguageFromSettingsFile(path);
            if (IsSupportedLanguageCode(value))
            {
                return value;
            }
        }

        return null;
    }

    private static IEnumerable<string> GetSptGameSettingsPaths()
    {
        foreach (var root in GetClientRootPaths())
        {
            yield return Path.Combine(root, "SPT", "user", "sptsettings", "Game.ini");
            yield return Path.Combine(root, "user", "sptsettings", "Game.ini");
        }
    }

    private static IEnumerable<string> GetClientRootPaths()
    {
        if (!string.IsNullOrWhiteSpace(Paths.GameRootPath))
        {
            yield return Paths.GameRootPath;
        }

        if (!string.IsNullOrWhiteSpace(Paths.PluginPath))
        {
            var pluginDirectory = new DirectoryInfo(Paths.PluginPath);
            var bepInExDirectory = pluginDirectory.Parent;
            var clientRoot = bepInExDirectory?.Parent;
            if (clientRoot is not null)
            {
                yield return clientRoot.FullName;
            }
        }
    }

    private static string? ReadLanguageFromSettingsFile(string path)
    {
        try
        {
            if (!File.Exists(path))
            {
                return null;
            }

            var content = File.ReadAllText(path);
            var match = Regex.Match(content, "\"Language\"\\s*:\\s*\"(?<language>[^\"\\\\]*(?:\\\\.[^\"\\\\]*)*)\"");
            return match.Success ? match.Groups["language"].Value : null;
        }
        catch
        {
            return null;
        }
    }

    private static string? DetectFromLocaleManager()
    {
        var localeManagerType = AppDomain.CurrentDomain.GetAssemblies()
            .Select(assembly => assembly.GetType("LocaleManagerClass"))
            .FirstOrDefault(type => type is not null);

        if (localeManagerType is null)
        {
            return null;
        }

        var instance = ReadStaticMember(localeManagerType, "LocaleManagerClass")
            ?? ReadStaticMember(localeManagerType, "LocaleManagerClass_1")
            ?? ReadStaticMember(localeManagerType, "GClass2350_0");

        foreach (var memberName in new[] { "String_0", "String_2" })
        {
            var value = ReadInstanceMember(instance, memberName)
                ?? ReadStaticMember(localeManagerType, memberName);

            if (IsSupportedLanguageCode(value))
            {
                return value?.ToString();
            }
        }

        return null;
    }

    private static string? DetectFromEftTypes()
    {
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            foreach (var type in GetTypes(assembly))
            {
                if (type.FullName?.IndexOf("Locale", StringComparison.OrdinalIgnoreCase) < 0
                    && type.FullName?.IndexOf("Language", StringComparison.OrdinalIgnoreCase) < 0)
                {
                    continue;
                }

                var value = ReadStaticLanguageValue(type);
                if (IsSupportedLanguageCode(value))
                {
                    return value;
                }
            }
        }

        return null;
    }

    private static IEnumerable<Type> GetTypes(Assembly assembly)
    {
        try
        {
            return assembly.GetTypes();
        }
        catch (ReflectionTypeLoadException exception)
        {
            return exception.Types.Where(type => type is not null)!;
        }
        catch
        {
            return [];
        }
    }

    private static string? ReadStaticLanguageValue(Type type)
    {
        foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static))
        {
            if (!IsLanguageMember(property.Name) || IsSystemLanguageFallback(property.Name) || property.GetIndexParameters().Length > 0)
            {
                continue;
            }

            try
            {
                var value = property.GetValue(null)?.ToString();
                if (!string.IsNullOrWhiteSpace(value))
                {
                    return value;
                }
            }
            catch
            {
                // Some EFT properties are not safe to evaluate before their services initialize.
            }
        }

        foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static))
        {
            if (!IsLanguageMember(field.Name) || IsSystemLanguageFallback(field.Name))
            {
                continue;
            }

            try
            {
                var value = field.GetValue(null)?.ToString();
                if (!string.IsNullOrWhiteSpace(value))
                {
                    return value;
                }
            }
            catch
            {
                // Some EFT fields are not safe to evaluate before their services initialize.
            }
        }

        return null;
    }

    private static object? ReadStaticMember(Type type, string memberName)
    {
        var property = type.GetProperty(memberName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
        if (property?.GetIndexParameters().Length == 0)
        {
            try
            {
                return property.GetValue(null);
            }
            catch
            {
                // EFT locale state may not be initialized yet.
            }
        }

        var field = type.GetField(memberName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
        if (field is not null)
        {
            try
            {
                return field.GetValue(null);
            }
            catch
            {
                // EFT locale state may not be initialized yet.
            }
        }

        return null;
    }

    private static string? ReadInstanceMember(object? instance, string memberName)
    {
        if (instance is null)
        {
            return null;
        }

        var type = instance.GetType();
        var property = type.GetProperty(memberName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        if (property?.GetIndexParameters().Length == 0)
        {
            try
            {
                return property.GetValue(instance)?.ToString();
            }
            catch
            {
                // EFT locale state may not be initialized yet.
            }
        }

        var field = type.GetField(memberName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        if (field is not null)
        {
            try
            {
                return field.GetValue(instance)?.ToString();
            }
            catch
            {
                // EFT locale state may not be initialized yet.
            }
        }

        return null;
    }

    private static bool IsLanguageMember(string memberName)
    {
        return memberName.IndexOf("language", StringComparison.OrdinalIgnoreCase) >= 0
            || memberName.IndexOf("locale", StringComparison.OrdinalIgnoreCase) >= 0;
    }

    private static bool IsSystemLanguageFallback(string memberName)
    {
        return memberName.IndexOf("default", StringComparison.OrdinalIgnoreCase) >= 0
            || memberName.IndexOf("system", StringComparison.OrdinalIgnoreCase) >= 0
            || memberName.IndexOf("platform", StringComparison.OrdinalIgnoreCase) >= 0;
    }

    private static bool IsSupportedLanguageCode(object? value)
    {
        if (value is null)
        {
            return false;
        }

        var text = value.ToString();
        if (string.IsNullOrWhiteSpace(text))
        {
            return false;
        }

        var normalized = text.Trim().Replace('_', '-').ToLowerInvariant();
        return normalized is "en" or "ch" or "chs" or "cht" or "cn" or "zh" or "zh-cn" or "zh-hans" or "zh-hant" or "chinese" or "chinesesimplified" or "chinesetraditional"
            || normalized.StartsWith("en-", StringComparison.Ordinal)
            || normalized.StartsWith("zh-", StringComparison.Ordinal)
            || normalized.StartsWith("cn-", StringComparison.Ordinal)
            || normalized.StartsWith("ch-", StringComparison.Ordinal)
            || normalized.StartsWith("chinese", StringComparison.Ordinal);
    }
}
#endif
