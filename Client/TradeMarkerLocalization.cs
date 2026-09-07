#if SPT_CLIENT
using System.IO;
using EFT;
using MoeTradeMarker.Shared;
using Newtonsoft.Json.Linq;

namespace MoeTradeMarker.Client;

internal static class TradeMarkerLocalization
{
    public static TradeMarkerLanguage Language { get; private set; } = TradeMarkerLanguage.English;

    public static void Refresh()
    {
        Language = TradeMarkerClientConfig.LanguageMode switch
        {
            TradeMarkerLanguageMode.Chinese => TradeMarkerLanguage.Chinese,
            TradeMarkerLanguageMode.English => TradeMarkerLanguage.English,
            TradeMarkerLanguageMode.Czech => TradeMarkerLanguage.Czech,
            TradeMarkerLanguageMode.French => TradeMarkerLanguage.French,
            TradeMarkerLanguageMode.German => TradeMarkerLanguage.German,
            TradeMarkerLanguageMode.Hungarian => TradeMarkerLanguage.Hungarian,
            TradeMarkerLanguageMode.Italian => TradeMarkerLanguage.Italian,
            TradeMarkerLanguageMode.Japanese => TradeMarkerLanguage.Japanese,
            TradeMarkerLanguageMode.Korean => TradeMarkerLanguage.Korean,
            TradeMarkerLanguageMode.Polish => TradeMarkerLanguage.Polish,
            TradeMarkerLanguageMode.Portuguese => TradeMarkerLanguage.Portuguese,
            TradeMarkerLanguageMode.Slovak => TradeMarkerLanguage.Slovak,
            TradeMarkerLanguageMode.Spanish => TradeMarkerLanguage.Spanish,
            TradeMarkerLanguageMode.SpanishMexico => TradeMarkerLanguage.SpanishMexico,
            TradeMarkerLanguageMode.Turkish => TradeMarkerLanguage.Turkish,
            TradeMarkerLanguageMode.Russian => TradeMarkerLanguage.Russian,
            TradeMarkerLanguageMode.Romanian => TradeMarkerLanguage.Romanian,
            _ => TradeMarkerLocalizer.DetectLanguage(DetectLanguageCode()),
        };
    }


    public static string Text(TradeMarkerText key) => TradeMarkerLocalizer.Text(key, Language);
    public static string Format(TradeMarkerText key, params object?[] args) => TradeMarkerLocalizer.Format(key, Language, args);
    public static string LanguageCode => TradeMarkerLocalizer.GetLanguageCode(Language);

    private static string DetectLanguageCode()
    {
        // Do not instantiate LocalizationManager during plugin startup (its constructor touches fonts).
        var manager = LocalizationManager._instance;
        if (manager is not null && !string.IsNullOrEmpty(manager._currentApplicationCulture)
            && TradeMarkerLocalizer.IsSupportedLanguageCode(manager.Culture))
            return manager.Culture;

        var root = AppDomain.CurrentDomain.BaseDirectory;
        foreach (var relative in new[] { "SPT_Runtime", "SPT", "" })
        {
            var path = Path.Combine(root, relative, "user", "sptsettings", "Game.ini");
            try
            {
                if (!File.Exists(path)) continue;
                var token = JObject.Parse(File.ReadAllText(path))["Language"];
                var code = token?.Type == JTokenType.String ? (string?)token : null;
                if (code is not null && TradeMarkerLocalizer.IsSupportedLanguageCode(code)) return code;
            }
            catch (Exception exception) when (exception is IOException || exception is UnauthorizedAccessException || exception is Newtonsoft.Json.JsonException)
            {
                // Settings may be absent or temporarily incomplete while the game writes them.
            }
        }
        return "en";
    }
}
#endif
