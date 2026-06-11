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
    private static Dictionary<string, string> traderNames = new(StringComparer.OrdinalIgnoreCase);
    private static Dictionary<string, string> itemMarkers = new(StringComparer.OrdinalIgnoreCase);

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
            Plugin.Log.LogDebug($"Moe-TradeMarker 未找到物品 {itemId} 的商人标记。");
        }

        return found;
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
            var loadedTraderNames = GetDictionary(TradeMarkerConstants.TraderInfoRoute);
            var loadedItemMarkers = GetDictionary(TradeMarkerConstants.ItemMarkerRoute);

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
            }
        }
        catch (Exception exception)
        {
            Plugin.Log.LogDebug($"Moe-TradeMarker 客户端标记数据刷新失败：{exception.Message}");
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

    private static string? GetJson(string route)
    {
        var requestHandler = Type.GetType("SPT.Common.Http.RequestHandler, spt-common")
            ?? AppDomain.CurrentDomain.GetAssemblies()
                .Select(assembly => assembly.GetType("SPT.Common.Http.RequestHandler"))
                .FirstOrDefault(type => type is not null);

        var method = requestHandler?
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .FirstOrDefault(info => info.Name == "GetJson" && info.GetParameters().Length == 1);

        return method?.Invoke(null, new object[] { route })?.ToString();
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
