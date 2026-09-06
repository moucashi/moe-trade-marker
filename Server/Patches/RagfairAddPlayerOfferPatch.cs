using System.Reflection;
using MoeTradeMarker.Server.Services;
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
    private static TradeMarkerService tradeMarkerService = null!;

    public RagfairAddPlayerOfferPatch(TradeMarkerService service)
    {
        tradeMarkerService = service;
    }

    protected override MethodBase GetTargetMethod()
    {
        return typeof(RagfairController).GetMethod(nameof(RagfairController.AddPlayerOffer))!;
    }

    [PatchPrefix]
    public static bool Prefix(PmcData __0, AddOfferRequestData __1, MongoId __2, ref ItemEventRouterResponse __result)
    {
        if (!tradeMarkerService.TryBlockRagfairOffer(__0, __1, __2, out var response))
        {
            return true;
        }

        __result = response;
        return false;
    }
}
