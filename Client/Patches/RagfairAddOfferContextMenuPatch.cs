#if SPT_CLIENT
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;

namespace MoeTradeMarker.Client.Patches;

[HarmonyPatch]
internal static class RagfairAddOfferInteractionAvailabilityPatch
{
    private const int AddOfferButtonValue = 50;
    private static readonly object? RestrictedResult = CreateFailedResult();

    private static IEnumerable<MethodBase> TargetMethods()
    {
        var contextInteractionsType = AccessTools.TypeByName("ContextInteractionsAbstractClass");
        if (contextInteractionsType is null)
        {
            yield break;
        }

        foreach (var method in contextInteractionsType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
        {
            if (method.Name != "IsInteractive")
            {
                continue;
            }

            var parameters = method.GetParameters();
            if (parameters.Length == 1 && parameters[0].ParameterType.Name == "EItemInfoButton")
            {
                yield return method;
            }
        }
    }

    private static bool Prefix(object __instance, object __0, ref object __result)
    {
        if (!IsAddOfferInteraction(__0)
            || !IsContextRestrictedFromRagfair(__instance)
            || RestrictedResult is null)
        {
            return true;
        }

        __result = RestrictedResult;
        return false;
    }

    private static bool IsContextRestrictedFromRagfair(object contextInteractions)
    {
        var item = GetItemFromContextInteractions(contextInteractions);
        return item is not null && ContainsRagfairRestrictedItem(item);
    }

    private static object? GetItemFromContextInteractions(object contextInteractions)
    {
        return GetFieldOrPropertyValue(contextInteractions, "Item_0")
            ?? GetFieldOrPropertyValue(contextInteractions, "item_0")
            ?? GetFieldOrPropertyValue(contextInteractions, "Item")
            ?? GetItemFromNestedItemContext(contextInteractions);
    }

    private static object? GetItemFromNestedItemContext(object contextInteractions)
    {
        var itemContext = GetFieldOrPropertyValue(contextInteractions, "ItemContextAbstractClass")
            ?? GetFieldOrPropertyValue(contextInteractions, "ItemContextAbstractClass_0")
            ?? GetFieldOrPropertyValue(contextInteractions, "ItemContextAbstractClass_1");

        return itemContext is null
            ? null
            : GetFieldOrPropertyValue(itemContext, "Item");
    }

    private static object? GetFieldOrPropertyValue(object instance, string name)
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

    private static bool IsAddOfferInteraction(object? interaction)
    {
        if (interaction is null)
        {
            return false;
        }

        return Convert.ToInt32(interaction) == AddOfferButtonValue;
    }

    private static bool ContainsRagfairRestrictedItem(object item)
    {
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

    private static IEnumerable<object> EnumerateItemAndChildren(object item)
    {
        yield return item;

        foreach (var child in GetVisibleChildren(item))
        {
            yield return child;
        }
    }

    private static IEnumerable<object> GetVisibleChildren(object item)
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
        var type = item.GetType();
        var value = AccessTools.Property(type, "Id")?.GetValue(item)
            ?? AccessTools.Field(type, "Id")?.GetValue(item)
            ?? AccessTools.Field(type, "_id")?.GetValue(item);

        return value?.ToString() ?? string.Empty;
    }

    private static object? CreateFailedResult()
    {
        try
        {
            var failedResultType = Type.GetType("Comfort.Common.FailedResult, Comfort")
                ?? AppDomain.CurrentDomain.GetAssemblies()
                    .Select(assembly => assembly.GetType("Comfort.Common.FailedResult"))
                    .FirstOrDefault(type => type is not null);

            return failedResultType is null
                ? null
                : Activator.CreateInstance(failedResultType, string.Empty, 0);
        }
        catch (Exception exception)
        {
            Plugin.Log.LogDebug($"Moe-TradeMarker could not create a ragfair restriction interaction result: {exception.Message}");
            return null;
        }
    }
}
#endif
