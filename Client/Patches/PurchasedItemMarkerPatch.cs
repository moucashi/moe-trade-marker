#if SPT_CLIENT
using EFT;
using HarmonyLib;
using JsonType;
using MoeTradeMarker.Client.Data;

namespace MoeTradeMarker.Client.Patches;

[HarmonyPatch(typeof(ProfileUpdatesHandler), nameof(ProfileUpdatesHandler.ManageNewItems))]
internal static class PurchasedItemMarkerPatch
{
    [HarmonyPrefix]
    private static void Prefix(FlatItem[] newItems)
    {
        if (newItems is null || newItems.Length == 0) return;
        var additions = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var item in newItems)
        {
            if (item is not null && MarkerSnapshot.TryReadItemMarker(item._id.ToString(), item.upd?.JToken, out var traderId))
                additions[item._id.ToString()] = traderId;
        }
        TradeMarkerDataLoader.ApplyItemMarkers(additions);
    }
}
#endif
