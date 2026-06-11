using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using MoeTradeMarker.Server.Services;
using SPTarkov.Server.Core.DI;
using SPTarkov.Reflection.Patching;
using SPTarkov.Server.Core.Controllers;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common;
using SPTarkov.Server.Core.Models.Eft.ItemEvent;
using SPTarkov.Server.Core.Models.Eft.Ragfair;
using SPTarkov.Server.Core.Servers;

namespace MoeTradeMarker.Server.Patches;

public class RagfairAddPlayerOfferPatch : AbstractPatch
{
    protected override MethodBase GetTargetMethod()
    {
        return typeof(RagfairController).GetMethod(nameof(RagfairController.AddPlayerOffer))!;
    }

    [PatchPrefix]
    public static bool Prefix(PmcData pmcData, AddOfferRequestData offerRequest, MongoId sessionID, ref ItemEventRouterResponse __result)
    {
#pragma warning disable CS0618
        var tradeMarkerService = ServiceLocator.ServiceProvider.GetRequiredService<TradeMarkerService>();
#pragma warning restore CS0618
        if (!tradeMarkerService.TryBlockRagfairOffer(pmcData, offerRequest, sessionID, out var response))
        {
            return true;
        }

        __result = response;
        return false;
    }
}
