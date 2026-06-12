#if SPT_CLIENT
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using MoeTradeMarker.Shared;

namespace MoeTradeMarker.Client;

internal static class TradeMarkerDataLoader
{
    private static readonly TimeSpan RefreshInterval = TimeSpan.FromSeconds(5);
    private static readonly object SyncRoot = new();
    private static DateTime lastRefreshUtc = DateTime.MinValue;
    private static string? lastLoggedLanguageCode;
    private static Dictionary<string, string> traderNames = new(StringComparer.OrdinalIgnoreCase);
    private static Dictionary<string, string> itemMarkers = new(StringComparer.OrdinalIgnoreCase);
    private static HashSet<string> ragfairRestrictedTraderIds = new(StringComparer.OrdinalIgnoreCase);

    public static bool TryGetTraderNameForItem(string itemId, out string traderName)
    {
        traderName = string.Empty;
        if (string.IsNullOrWhiteSpace(itemId))
        {
            return false;
        }

        Refresh(force: false);
        if (TryGetTraderNameFromCache(itemId, out traderName))
        {
            return true;
        }

        Refresh(force: true);
        var found = TryGetTraderNameFromCache(itemId, out traderName);
        if (!found)
        {
            Plugin.Log.LogDebug(TradeMarkerLocalization.Format(TradeMarkerText.ClientItemMarkerMissing, itemId));
        }

        return found;
    }

    public static bool IsItemRestrictedFromRagfair(string itemId)
    {
        if (string.IsNullOrWhiteSpace(itemId))
        {
            return false;
        }

        Refresh(force: false);
        if (IsItemRestrictedFromRagfairCache(itemId))
        {
            return true;
        }

        Refresh(force: true);
        return IsItemRestrictedFromRagfairCache(itemId);
    }

    private static bool TryGetTraderNameFromCache(string itemId, out string traderName)
    {
        traderName = string.Empty;
        lock (SyncRoot)
        {
            if (!itemMarkers.TryGetValue(itemId, out var traderId) || string.IsNullOrWhiteSpace(traderId))
            {
                return false;
            }

            traderName = traderNames.TryGetValue(traderId, out var name) && !string.IsNullOrWhiteSpace(name)
                ? name
                : traderId;
            return true;
        }
    }

    private static bool IsItemRestrictedFromRagfairCache(string itemId)
    {
        lock (SyncRoot)
        {
            return itemMarkers.TryGetValue(itemId, out var traderId)
                && !string.IsNullOrWhiteSpace(traderId)
                && ragfairRestrictedTraderIds.Contains(traderId);
        }
    }

    public static void Refresh(bool force)
    {
        var now = DateTime.UtcNow;
        if (!force && now - lastRefreshUtc < RefreshInterval)
        {
            return;
        }

        lastRefreshUtc = now;
        try
        {
            TradeMarkerLocalization.Refresh();
            PostLanguage();
            var loadedTraderNames = GetDictionary(TradeMarkerConstants.TraderInfoRoute);
            var loadedItemMarkers = GetDictionary(TradeMarkerConstants.ItemMarkerRoute);
            var loadedRestrictedTraderIds = GetStringSet(TradeMarkerConstants.RagfairRestrictedTraderRoute);

            lock (SyncRoot)
            {
                if (loadedTraderNames is not null)
                {
                    traderNames = loadedTraderNames;
                }

                if (loadedItemMarkers is not null)
                {
                    itemMarkers = loadedItemMarkers;
                }

                if (loadedRestrictedTraderIds is not null)
                {
                    ragfairRestrictedTraderIds = loadedRestrictedTraderIds;
                }
            }
        }
        catch (Exception exception)
        {
            Plugin.Log.LogDebug(TradeMarkerLocalization.Format(TradeMarkerText.ClientMarkerRefreshFailed, exception.Message));
        }
    }

    public static void SyncLanguage()
    {
        try
        {
            TradeMarkerLocalization.Refresh();
            PostLanguage();
        }
        catch (Exception exception)
        {
            Plugin.Log.LogDebug(TradeMarkerLocalization.Format(TradeMarkerText.ClientLanguageSyncFailed, exception.Message));
        }
    }

    private static Dictionary<string, string>? GetDictionary(string route)
    {
        var json = GetJson(route);
        if (string.IsNullOrWhiteSpace(json))
        {
            return null;
        }

        return ParseStringDictionary(json!);
    }

    private static HashSet<string>? GetStringSet(string route)
    {
        var json = GetJson(route);
        if (string.IsNullOrWhiteSpace(json))
        {
            return null;
        }

        return ParseStringArray(json!);
    }

    private static string? GetJson(string route)
    {
        var requestHandler = GetRequestHandlerType();
        var method = requestHandler?
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .FirstOrDefault(info => info.Name == "GetJson" && info.GetParameters().Length == 1);

        return method?.Invoke(null, new object[] { route })?.ToString();
    }

    private static void PostLanguage()
    {
        try
        {
            var requestHandler = GetRequestHandlerType();
            var method = FindPostJsonMethod(requestHandler);

            if (method is null)
            {
                return;
            }

            var languageCode = TradeMarkerLocalization.LanguageCode;
            var payload = $"{{\"language\":\"{languageCode}\"}}";
            var parameters = method.GetParameters();
            var args = new object?[parameters.Length];
            args[0] = TradeMarkerConstants.LanguageRoute;
            args[1] = payload;

            for (var index = 2; index < parameters.Length; index++)
            {
                args[index] = parameters[index].HasDefaultValue ? parameters[index].DefaultValue : null;
            }

            method.Invoke(null, args);
            LogLanguageSync(languageCode);
        }
        catch
        {
            // Language sync is best effort; marker data loading must keep working without it.
        }
    }

    private static void LogLanguageSync(string languageCode)
    {
        if (string.Equals(lastLoggedLanguageCode, languageCode, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        lastLoggedLanguageCode = languageCode;
        Plugin.Log.LogInfo(TradeMarkerLocalization.Format(TradeMarkerText.ClientLanguageSynced, languageCode));
    }

    private static Type? GetRequestHandlerType()
    {
        return Type.GetType("SPT.Common.Http.RequestHandler, spt-common")
            ?? AppDomain.CurrentDomain.GetAssemblies()
                .Select(assembly => assembly.GetType("SPT.Common.Http.RequestHandler"))
                .FirstOrDefault(type => type is not null);
    }

    private static MethodInfo? FindPostJsonMethod(Type? requestHandler)
    {
        if (requestHandler is null)
        {
            return null;
        }

        var candidates = requestHandler.GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Where(info => info.GetParameters().Length >= 2
                && info.GetParameters()[0].ParameterType == typeof(string)
                && info.GetParameters()[1].ParameterType == typeof(string));

        return candidates.FirstOrDefault(info => info.Name == "PostJson")
            ?? candidates.FirstOrDefault(info => info.Name == "Post")
            ?? candidates.FirstOrDefault(info => info.Name == "PostJsonAsync");
    }

    private static Dictionary<string, string> ParseStringDictionary(string json)
    {
        var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        foreach (Match match in Regex.Matches(json, "\"(?<key>(?:\\\\.|[^\"])*)\"\\s*:\\s*\"(?<value>(?:\\\\.|[^\"])*)\""))
        {
            result[DecodeJsonString(match.Groups["key"].Value)] = DecodeJsonString(match.Groups["value"].Value);
        }

        return result;
    }

    private static HashSet<string> ParseStringArray(string json)
    {
        var result = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (Match match in Regex.Matches(json, "\"(?<value>(?:\\\\.|[^\"])*)\""))
        {
            var value = DecodeJsonString(match.Groups["value"].Value);
            if (!string.IsNullOrWhiteSpace(value))
            {
                result.Add(value);
            }
        }

        return result;
    }

    private static string DecodeJsonString(string value)
    {
        var builder = new StringBuilder(value.Length);
        for (var index = 0; index < value.Length; index++)
        {
            var current = value[index];
            if (current != '\\' || index + 1 >= value.Length)
            {
                builder.Append(current);
                continue;
            }

            var escaped = value[++index];
            switch (escaped)
            {
                case '"':
                case '\\':
                case '/':
                    builder.Append(escaped);
                    break;
                case 'b':
                    builder.Append('\b');
                    break;
                case 'f':
                    builder.Append('\f');
                    break;
                case 'n':
                    builder.Append('\n');
                    break;
                case 'r':
                    builder.Append('\r');
                    break;
                case 't':
                    builder.Append('\t');
                    break;
                case 'u' when index + 4 < value.Length && ushort.TryParse(
                    value.Substring(index + 1, 4),
                    System.Globalization.NumberStyles.HexNumber,
                    System.Globalization.CultureInfo.InvariantCulture,
                    out var code):
                    builder.Append((char)code);
                    index += 4;
                    break;
                default:
                    builder.Append(escaped);
                    break;
            }
        }

        return builder.ToString();
    }
}
#endif
