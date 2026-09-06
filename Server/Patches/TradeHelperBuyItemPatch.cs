using System.Reflection;
using MoeTradeMarker.Server.Services;
using SPTarkov.Reflection.Patching;
using SPTarkov.Server.Core.Helpers.Commerce;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.ItemEvent;
using SPTarkov.Server.Core.Models.Eft.Trade;
using SPTarkov.Server.Core.Servers;

namespace MoeTradeMarker.Server.Patches;

public class TradeHelperBuyItemPatch : AbstractPatch
{
    private static TradeMarkerService tradeMarkerService = null!;

    public TradeHelperBuyItemPatch(TradeMarkerService service)
    {
        tradeMarkerService = service;
    }

    protected override MethodBase GetTargetMethod()
    {
        return typeof(TradeHelper).GetMethod(nameof(TradeHelper.BuyItem))!;
    }

    [PatchPrefix]
    public static void Prefix(MongoId __2, ItemEventRouterResponse __4, out int __state)
    {
        __state = __4.ProfileChanges.TryGetValue(__2, out var profileChange)
            ? profileChange.Items?.NewItems?.Count ?? 0
            : 0;
    }

    [PatchPostfix]
    public static void Postfix(ProcessBuyTradeRequestData __1, MongoId __2, ItemEventRouterResponse __4, int __state)
    {
        tradeMarkerService.MarkPurchasedItems(__1, __2, __4, __state);
    }
}
