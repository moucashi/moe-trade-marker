#if SPT_CLIENT
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MoeTradeMarker.Shared;
using Newtonsoft.Json;

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
            Plugin.Log.LogDebug($"MoeTradeMarker 客户端标记数据刷新失败：{exception.Message}");
        }
    }

    private static Dictionary<string, string>? GetDictionary(string route)
    {
        var json = GetJson(route);
        if (string.IsNullOrWhiteSpace(json))
        {
            return null;
        }

        return JsonConvert.DeserializeObject<Dictionary<string, string>>(json)
            ?? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
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
}
#endif
