using System.Collections.Concurrent;
using System.Reflection;
using MoeTradeMarker.Shared;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Services;

namespace MoeTradeMarker.Server.Services;

[Injectable(InjectionType = InjectionType.Singleton)]
public class TradeMarkerLanguageService(LocaleService localeService)
{
    private readonly ConcurrentDictionary<string, TradeMarkerLanguage> sessionLanguages = new(StringComparer.OrdinalIgnoreCase);

    public TradeMarkerLanguage ServerLanguage => DetectLanguage(SafeGetServerLocale());

    public void SetSessionLanguage(MongoId sessionId, string? languageCode)
    {
        sessionLanguages[sessionId.ToString()] = DetectLanguage(languageCode);
    }

    public TradeMarkerLanguage GetSessionLanguage(MongoId sessionId, params object?[] languageSources)
    {
        if (sessionLanguages.TryGetValue(sessionId.ToString(), out var language))
        {
            return language;
        }

        foreach (var source in languageSources)
        {
            var languageCode = TryReadLanguageCode(source);
            if (!string.IsNullOrWhiteSpace(languageCode))
            {
                return DetectLanguage(languageCode);
            }
        }

        return ServerLanguage;
    }

    public string Text(TradeMarkerText key)
    {
        return TradeMarkerLocalizer.Text(key, ServerLanguage);
    }

    public string Format(TradeMarkerText key, TradeMarkerLanguage language, params object?[] args)
    {
        return TradeMarkerLocalizer.Format(key, language, args);
    }

    public string FormatServer(TradeMarkerText key, params object?[] args)
    {
        return TradeMarkerLocalizer.Format(key, ServerLanguage, args);
    }

    private static TradeMarkerLanguage DetectLanguage(string? languageCode)
    {
        return TradeMarkerLocalizer.DetectLanguage(languageCode);
    }

    private string? SafeGetServerLocale()
    {
        try
        {
            return localeService.GetDesiredServerLocale();
        }
        catch
        {
            return null;
        }
    }

    private static string? TryReadLanguageCode(object? source)
    {
        if (source is null)
        {
            return null;
        }

        if (source is string value)
        {
            return value;
        }

        var type = source.GetType();
        foreach (var memberName in new[] { "Language", "Locale", "GameLocale", "SelectedLanguage", "SelectedLocale" })
        {
            var property = type.GetProperty(memberName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (property?.GetIndexParameters().Length == 0)
            {
                try
                {
                    var result = property.GetValue(source)?.ToString();
                    if (!string.IsNullOrWhiteSpace(result))
                    {
                        return result;
                    }
                }
                catch
                {
                    // Profile/request objects may expose computed members that are not always safe to read.
                }
            }

            var field = type.GetField(memberName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (field is null)
            {
                continue;
            }

            try
            {
                var result = field.GetValue(source)?.ToString();
                if (!string.IsNullOrWhiteSpace(result))
                {
                    return result;
                }
            }
            catch
            {
                // Profile/request objects may expose computed members that are not always safe to read.
            }
        }

        return null;
    }
}
