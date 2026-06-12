using System.Text.Json;
using MoeTradeMarker.Shared;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.Extensions;
using SPTarkov.Server.Core.Helpers;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Eft.ItemEvent;
using SPTarkov.Server.Core.Models.Eft.Ragfair;
using SPTarkov.Server.Core.Models.Eft.Trade;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Routers;
using SPTarkov.Server.Core.Servers;
using SPTarkov.Server.Core.Services;
using SPTarkov.Server.Core.Utils;

namespace MoeTradeMarker.Server.Services;

[Injectable(InjectionType = InjectionType.Singleton)]
public class TradeMarkerService(
    TradeMarkerConfigService configService,
    DatabaseServer databaseServer,
    EventOutputHolder eventOutputHolder,
    ProfileHelper profileHelper,
    HttpResponseUtil httpResponseUtil,
    TradeMarkerLanguageService languageService)
{
    private static readonly HashSet<string> TraderPurchaseTypes =
    [
        "buy_from_trader",
        "buy_from_ragfair_trader",
    ];

    public void MarkPurchasedItems(ProcessBuyTradeRequestData request, MongoId sessionId, ItemEventRouterResponse output, int firstNewItemIndex)
    {
        if (!IsTraderPurchase(request) || output.Warnings?.Count > 0)
        {
            return;
        }

        var traderId = request.TransactionId.ToString();
        if (!configService.ShouldMarkPurchase(traderId))
        {
            return;
        }

        if (!output.ProfileChanges.TryGetValue(sessionId, out var profileChange))
        {
            return;
        }

        var newItems = profileChange.Items?.NewItems;
        if (newItems is null || firstNewItemIndex >= newItems.Count)
        {
            return;
        }

        foreach (var item in newItems.Skip(Math.Max(firstNewItemIndex, 0)))
        {
            SetMarker(item, traderId);
        }
    }

    public bool TryBlockRagfairOffer(PmcData pmcData, AddOfferRequestData offerRequest, MongoId sessionId, out ItemEventRouterResponse response)
    {
        response = eventOutputHolder.GetOutput(sessionId);
        if (offerRequest.Items is null || offerRequest.Items.Count == 0)
        {
            return false;
        }

        foreach (var itemId in offerRequest.Items)
        {
            var itemAndChildren = pmcData.Inventory?.Items?.GetItemWithChildren(itemId) ?? [];
            foreach (var item in itemAndChildren)
            {
                if (!TryGetMarkerTraderId(item, out var traderId) || !configService.ShouldBlockRagfairListing(traderId))
                {
                    continue;
                }

                var traderName = GetTraderDisplayName(traderId);
                var language = languageService.GetSessionLanguage(sessionId, offerRequest, pmcData);
                var message = languageService.Format(TradeMarkerText.RagfairMarkedItemBlocked, language, traderName);
                response = httpResponseUtil.AppendErrorToOutput(response, message);
                return true;
            }
        }

        return false;
    }

    public Dictionary<string, string> GetTraderNames()
    {
        var result = new Dictionary<string, string>();
        var traders = databaseServer.GetTables().Traders;
        if (traders is null)
        {
            return result;
        }

        foreach (var (traderId, trader) in traders)
        {
            result[traderId.ToString()] = trader.Base.Nickname ?? trader.Base.Name ?? traderId.ToString();
        }

        return result;
    }

    public List<string> GetRagfairRestrictedTraderIds()
    {
        if (!configService.IsRagfairRestrictionEnabled())
        {
            return [];
        }

        var traders = databaseServer.GetTables().Traders;
        if (traders is null)
        {
            return [];
        }

        return traders.Keys
            .Select(traderId => traderId.ToString())
            .Where(configService.ShouldBlockRagfairListing)
            .ToList();
    }

    public Dictionary<string, string> GetMarkedItems(MongoId sessionId)
    {
        var result = new Dictionary<string, string>();
        var pmcData = profileHelper.GetPmcProfile(sessionId);
        if (pmcData is null)
        {
            return result;
        }

        var items = pmcData.Inventory?.Items;
        if (items is null)
        {
            return result;
        }

        foreach (var item in items)
        {
            if (TryGetMarkerTraderId(item, out var traderId))
            {
                result[item.Id.ToString()] = traderId;
            }
        }

        return result;
    }

    public string GetTraderDisplayName(string traderId)
    {
        try
        {
            var mongoId = new MongoId(traderId);
            var traders = databaseServer.GetTables().Traders;
            if (traders is not null && traders.TryGetValue(mongoId, out var trader))
            {
                return trader.Base.Nickname ?? trader.Base.Name ?? traderId;
            }
        }
        catch
        {
            // Invalid modded ids should still produce a readable error.
        }

        return traderId;
    }

    public static bool TryGetMarkerTraderId(Item item, out string traderId)
    {
        traderId = string.Empty;
        var extensionData = item.Upd?.ExtensionData;
        if (extensionData is null)
        {
            return false;
        }

        if (!extensionData.TryGetValue(TradeMarkerConstants.UpdMarkerProperty, out var markerValue) || markerValue is null)
        {
            return false;
        }

        traderId = ExtractTraderId(markerValue);
        return !string.IsNullOrWhiteSpace(traderId);
    }

    private static bool IsTraderPurchase(ProcessBuyTradeRequestData request)
    {
        return request.Type is not null && TraderPurchaseTypes.Contains(request.Type);
    }

    private void SetMarker(Item item, string traderId)
    {
        item.Upd ??= new Upd();
#pragma warning disable CS8619
        item.Upd.ExtensionData ??= new Dictionary<string, object?>();
#pragma warning restore CS8619

        item.Upd.ExtensionData[TradeMarkerConstants.UpdMarkerProperty] = new TradeMarkerData { TraderId = traderId };
    }

    private static string ExtractTraderId(object markerValue)
    {
        return markerValue switch
        {
            TradeMarkerData data => data.TraderId,
            JsonElement element => ExtractTraderId(element),
            Dictionary<string, object> dictionary when dictionary.TryGetValue(TradeMarkerConstants.TraderIdProperty, out var value) => value?.ToString() ?? string.Empty,
            IDictionary<string, object> dictionary when dictionary.TryGetValue(TradeMarkerConstants.TraderIdProperty, out var value) => value?.ToString() ?? string.Empty,
            _ => string.Empty,
        };
    }

    private static string ExtractTraderId(JsonElement element)
    {
        if (element.ValueKind == JsonValueKind.Object
            && element.TryGetProperty(TradeMarkerConstants.TraderIdProperty, out var traderIdElement))
        {
            return traderIdElement.GetString() ?? string.Empty;
        }

        return string.Empty;
    }
}
