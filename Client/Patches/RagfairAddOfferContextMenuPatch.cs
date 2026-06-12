#if SPT_CLIENT
using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using MoeTradeMarker.Client;

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
        return TradeMarkerItemRestriction.GetFieldOrPropertyValue(contextInteractions, "Item_0")
            ?? TradeMarkerItemRestriction.GetFieldOrPropertyValue(contextInteractions, "item_0")
            ?? TradeMarkerItemRestriction.GetFieldOrPropertyValue(contextInteractions, "Item")
            ?? GetItemFromNestedItemContext(contextInteractions);
    }

    private static object? GetItemFromNestedItemContext(object contextInteractions)
    {
        var itemContext = TradeMarkerItemRestriction.GetFieldOrPropertyValue(contextInteractions, "ItemContextAbstractClass")
            ?? TradeMarkerItemRestriction.GetFieldOrPropertyValue(contextInteractions, "ItemContextAbstractClass_0")
            ?? TradeMarkerItemRestriction.GetFieldOrPropertyValue(contextInteractions, "ItemContextAbstractClass_1");

        return itemContext is null
            ? null
            : TradeMarkerItemRestriction.GetFieldOrPropertyValue(itemContext, "Item");
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
        return TradeMarkerItemRestriction.ContainsRagfairRestrictedItem(item);
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
