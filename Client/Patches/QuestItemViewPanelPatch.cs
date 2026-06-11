#if SPT_CLIENT
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace MoeTradeMarker.Client.Patches;

[HarmonyPatch]
internal static class QuestItemViewPanelPatch
{
    private const string OverlayName = "MoeTradeMarkerIcon";

    private static IEnumerable<MethodBase> TargetMethods()
    {
        var panelType = AccessTools.TypeByName("QuestItemViewPanel");
        if (panelType is null)
        {
            yield break;
        }

        foreach (var method in panelType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                     .Where(method => method.Name == "Show" && method.GetParameters().Any(IsItemParameter)))
        {
            yield return method;
        }
    }

    private static void Postfix(object __instance, object[] __args)
    {
        if (!TradeMarkerClientConfig.ShowTraderMarker)
        {
            TradeMarkerOverlay.Hide(__instance, OverlayName);
            return;
        }

        var itemId = FindItemId(__args);
        if (!TradeMarkerDataLoader.TryGetTraderNameForItem(itemId, out var traderName))
        {
            TradeMarkerOverlay.Hide(__instance, OverlayName);
            return;
        }

        var iconImage = TradeMarkerOverlay.FindIconImage(__instance);
        if (iconImage is null)
        {
            return;
        }

        var tooltip = TradeMarkerOverlay.FindTooltip(__instance, __args);
        TradeMarkerOverlay.Show(
            iconImage,
            OverlayName,
            traderName,
            tooltip,
            TradeMarkerClientConfig.MarkerPosition,
            TradeMarkerClientConfig.MarkerColor);
    }

    private static bool IsItemParameter(ParameterInfo parameter)
    {
        return parameter.ParameterType.FullName?.Contains("InventoryLogic.Item", StringComparison.Ordinal) == true;
    }

    private static string FindItemId(IEnumerable<object> args)
    {
        foreach (var arg in args)
        {
            if (arg is null || arg.GetType().FullName?.Contains("InventoryLogic.Item", StringComparison.Ordinal) != true)
            {
                continue;
            }

            var value = AccessTools.Property(arg.GetType(), "Id")?.GetValue(arg)
                ?? AccessTools.Field(arg.GetType(), "Id")?.GetValue(arg)
                ?? AccessTools.Field(arg.GetType(), "_id")?.GetValue(arg);

            if (value is not null)
            {
                return value.ToString();
            }
        }

        return string.Empty;
    }
}
#endif
