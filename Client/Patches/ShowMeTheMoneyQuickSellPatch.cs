#if SPT_CLIENT
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using MoeTradeMarker.Client;

namespace MoeTradeMarker.Client.Patches;

[HarmonyPatch]
internal static class ShowMeTheMoneyQuickSellPatch
{
    private const string BrokerServiceTypeName = "SwiftXP.SPT.ShowMeTheMoney.QuickSell.Client.Services.BrokerService";

    private static IEnumerable<MethodBase> TargetMethods()
    {
        var brokerServiceType = AccessTools.TypeByName(BrokerServiceTypeName);
        if (brokerServiceType is null)
        {
            yield break;
        }

        var sellItemsOnFleaMethod = AccessTools.Method(brokerServiceType, "SellItemsOnFlea");
        if (sellItemsOnFleaMethod is not null)
        {
            yield return sellItemsOnFleaMethod;
        }
    }

    private static bool Prefix(object[] __args)
    {
        if (__args.Length < 2 || !ContainsRestrictedTradeItem(__args[1]))
        {
            return true;
        }

        return false;
    }

    private static bool ContainsRestrictedTradeItem(object? tradeItems)
    {
        if (tradeItems is not IEnumerable items)
        {
            return false;
        }

        foreach (var tradeItem in items)
        {
            var item = GetItemFromTradeItem(tradeItem);
            if (TradeMarkerItemRestriction.ContainsRagfairRestrictedItem(item))
            {
                return true;
            }
        }

        return false;
    }

    private static object? GetItemFromTradeItem(object? tradeItem)
    {
        return tradeItem is null
            ? null
            : TradeMarkerItemRestriction.GetFieldOrPropertyValue(tradeItem, "Item");
    }
}

[HarmonyPatch]
internal static class ShowMeTheMoneyQuickSellFleaTradeBuilderPatch
{
    private const string BrokerServiceTypeName = "SwiftXP.SPT.ShowMeTheMoney.QuickSell.Client.Services.BrokerService";

    private static IEnumerable<MethodBase> TargetMethods()
    {
        var brokerServiceType = AccessTools.TypeByName(BrokerServiceTypeName);
        if (brokerServiceType is null)
        {
            yield break;
        }

        var getBrokerFleaTradesMethod = AccessTools.Method(brokerServiceType, "GetBrokerFleaTrades");
        if (getBrokerFleaTradesMethod is not null)
        {
            yield return getBrokerFleaTradesMethod;
        }
    }

    private static void Postfix(object[] __args, object __result)
    {
        if (__args.Length < 1)
        {
            return;
        }

        RemoveRestrictedFleaTrades(__args[0], __result);
    }

    private static void RemoveRestrictedFleaTrades(object? tradeItems, object? brokerFleaTrades)
    {
        if (tradeItems is not IList remainingTradeItems || brokerFleaTrades is not IList fleaTrades)
        {
            return;
        }

        var emptyFleaTrades = new List<object>();
        foreach (var fleaTrade in CopyItems(fleaTrades))
        {
            if (TradeMarkerItemRestriction.GetFieldOrPropertyValue(fleaTrade, "TradeItems") is not IList groupedTradeItems)
            {
                continue;
            }

            RemoveRestrictedTradeItems(groupedTradeItems, remainingTradeItems);
            if (groupedTradeItems.Count == 0)
            {
                emptyFleaTrades.Add(fleaTrade);
            }
        }

        foreach (var fleaTrade in emptyFleaTrades)
        {
            fleaTrades.Remove(fleaTrade);
        }
    }

    private static void RemoveRestrictedTradeItems(IList groupedTradeItems, IList remainingTradeItems)
    {
        foreach (var tradeItem in CopyItems(groupedTradeItems))
        {
            if (!IsRestrictedTradeItem(tradeItem))
            {
                continue;
            }

            groupedTradeItems.Remove(tradeItem);
            if (!remainingTradeItems.Contains(tradeItem))
            {
                remainingTradeItems.Add(tradeItem);
            }
        }
    }

    private static bool IsRestrictedTradeItem(object? tradeItem)
    {
        var item = tradeItem is null
            ? null
            : TradeMarkerItemRestriction.GetFieldOrPropertyValue(tradeItem, "Item");

        return TradeMarkerItemRestriction.ContainsRagfairRestrictedItem(item);
    }

    private static List<object> CopyItems(IList items)
    {
        var result = new List<object>();
        foreach (var item in items)
        {
            if (item is not null)
            {
                result.Add(item);
            }
        }

        return result;
    }
}
#endif
