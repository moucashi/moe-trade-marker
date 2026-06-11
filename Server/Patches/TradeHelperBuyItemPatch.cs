using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using MoeTradeMarker.Server.Services;
using SPTarkov.Server.Core.DI;
using SPTarkov.Reflection.Patching;
using SPTarkov.Server.Core.Helpers;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.ItemEvent;
using SPTarkov.Server.Core.Models.Eft.Trade;
using SPTarkov.Server.Core.Servers;

namespace MoeTradeMarker.Server.Patches;

public class TradeHelperBuyItemPatch : AbstractPatch
{
    protected override MethodBase GetTargetMethod()
    {
        return typeof(TradeHelper).GetMethod(nameof(TradeHelper.BuyItem))!;
    }

    [PatchPrefix]
    public static void Prefix(MongoId sessionID, ItemEventRouterResponse output, out int __state)
    {
        __state = output.ProfileChanges.TryGetValue(sessionID, out var profileChange)
            ? profileChange.Items?.NewItems?.Count ?? 0
            : 0;
    }

    [PatchPostfix]
    public static void Postfix(ProcessBuyTradeRequestData buyRequestData, MongoId sessionID, ItemEventRouterResponse output, int __state)
    {
#pragma warning disable CS0618
        ServiceLocator.ServiceProvider.GetRequiredService<TradeMarkerService>().MarkPurchasedItems(buyRequestData, sessionID, output, __state);
#pragma warning restore CS0618
    }
}
