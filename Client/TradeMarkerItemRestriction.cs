#if SPT_CLIENT
using EFT.InventoryLogic;
using HarmonyLib;

namespace MoeTradeMarker.Client;

internal static class TradeMarkerItemRestriction
{
    public static bool ContainsRagfairRestrictedItem(object? value)
    {
        if (value is not Item item) return false;
        if (TradeMarkerDataLoader.IsItemRestrictedFromRagfair(item.Id.ToString())) return true;
        foreach (var child in item.GetAllVisibleItems())
        {
            if (!ReferenceEquals(child, item) && TradeMarkerDataLoader.IsItemRestrictedFromRagfair(child.Id.ToString()))
                return true;
        }
        return false;
    }

    // Optional third-party trade wrappers still require member discovery.
    public static object? GetFieldOrPropertyValue(object instance, string name)
    {
        var type = instance.GetType();
        var field = AccessTools.Field(type, name);
        return field is not null ? field.GetValue(instance) : AccessTools.Property(type, name)?.GetValue(instance);
    }
}
#endif
