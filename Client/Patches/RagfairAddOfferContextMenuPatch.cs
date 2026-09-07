#if SPT_CLIENT
using Comfort.Common;
using EFT.InventoryLogic;
using EFT.UI;
using HarmonyLib;

namespace MoeTradeMarker.Client.Patches;

[HarmonyPatch(typeof(BaseItemContextInteractions), nameof(BaseItemContextInteractions.IsInteractive))]
internal static class RagfairAddOfferInteractionAvailabilityPatch
{
    private static readonly IResult RestrictedResult = new FailedResult(string.Empty);

    private static bool Prefix(BaseItemContextInteractions __instance, EItemInfoButton __0, ref IResult __result)
    {
        if (__0 != EItemInfoButton.AddOffer || !TradeMarkerItemRestriction.ContainsRagfairRestrictedItem(__instance.Item))
            return true;
        __result = RestrictedResult;
        return false;
    }
}
#endif
