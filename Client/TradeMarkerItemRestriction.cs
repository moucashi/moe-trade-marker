#if SPT_CLIENT
using System;
using System.Collections;
using HarmonyLib;

namespace MoeTradeMarker.Client;

internal static class TradeMarkerItemRestriction
{
    public static bool ContainsRagfairRestrictedItem(object? item)
    {
        if (item is null)
        {
            return false;
        }

        foreach (var current in EnumerateItemAndChildren(item))
        {
            var itemId = GetItemId(current);
            if (TradeMarkerDataLoader.IsItemRestrictedFromRagfair(itemId))
            {
                return true;
            }
        }

        return false;
    }

    public static object? GetFieldOrPropertyValue(object instance, string name)
    {
        for (var type = instance.GetType(); type is not null; type = type.BaseType)
        {
            var field = AccessTools.Field(type, name);
            if (field is not null)
            {
                return field.GetValue(instance);
            }

            var property = AccessTools.Property(type, name);
            if (property is not null)
            {
                return property.GetValue(instance);
            }
        }

        return null;
    }

    private static IEnumerable EnumerateItemAndChildren(object item)
    {
        yield return item;

        foreach (var child in GetVisibleChildren(item))
        {
            yield return child;
        }
    }

    private static IEnumerable GetVisibleChildren(object item)
    {
        var method = AccessTools.Method(item.GetType(), "GetAllVisibleItems", Type.EmptyTypes);
        if (method?.Invoke(item, []) is not IEnumerable children)
        {
            yield break;
        }

        foreach (var child in children)
        {
            if (child is not null && !ReferenceEquals(child, item))
            {
                yield return child;
            }
        }
    }

    private static string GetItemId(object item)
    {
        var value = GetFieldOrPropertyValue(item, "Id")
            ?? GetFieldOrPropertyValue(item, "_id");

        return value?.ToString() ?? string.Empty;
    }
}
#endif
