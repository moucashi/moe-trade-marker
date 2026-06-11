#if SPT_CLIENT
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using MoeTradeMarker.Shared;
using UnityEngine;
using UnityEngine.UI;

namespace MoeTradeMarker.Client.Patches;

[HarmonyPatch]
internal static class ItemViewTradeMarkerPatch
{
    private const string OverlayName = "MoeTradeMarkerIcon";

    private static IEnumerable<MethodBase> TargetMethods()
    {
        var itemViewType = AccessTools.TypeByName("EFT.UI.DragAndDrop.ItemView");
        if (itemViewType is null)
        {
            Plugin.Log.LogWarning(TradeMarkerLocalization.Text(TradeMarkerText.ClientItemViewTypeMissing));
            yield break;
        }

        var targetMethod = AccessTools.Method(itemViewType, "SetQuestItemViewPanel", Type.EmptyTypes);
        if (targetMethod is null)
        {
            Plugin.Log.LogWarning(TradeMarkerLocalization.Text(TradeMarkerText.ClientSetQuestItemViewPanelMissing));
            yield break;
        }

        Plugin.Log.LogInfo(TradeMarkerLocalization.Text(TradeMarkerText.ClientItemViewPatchInstalled));
        yield return targetMethod;
    }

    private static void Postfix(object __instance)
    {
        if (!TradeMarkerClientConfig.ShowTraderMarker)
        {
            TradeMarkerOverlay.HideFromItemView(__instance, OverlayName);
            return;
        }

        var item = GetItem(__instance);
        if (item is null)
        {
            TradeMarkerOverlay.HideFromItemView(__instance, OverlayName);
            Plugin.Log.LogDebug(TradeMarkerLocalization.Text(TradeMarkerText.ClientItemReadFailed));
            return;
        }

        var itemId = GetItemId(item);
        if (!TradeMarkerDataLoader.TryGetTraderNameForItem(itemId, out var traderName))
        {
            TradeMarkerOverlay.HideFromItemView(__instance, OverlayName);
            return;
        }

        var mainImage = GetMainImage(__instance);
        if (mainImage is null)
        {
            Plugin.Log.LogDebug(TradeMarkerLocalization.Format(TradeMarkerText.ClientMainImageReadFailed, itemId));
            return;
        }

        if (__instance is not Component itemViewComponent)
        {
            Plugin.Log.LogDebug(TradeMarkerLocalization.Format(TradeMarkerText.ClientItemViewComponentReadFailed, itemId));
            return;
        }

        TradeMarkerOverlay.ShowOnItemView(
            itemViewComponent,
            OverlayName,
            traderName,
            FindTooltip(__instance),
            TradeMarkerClientConfig.MarkerPosition,
            TradeMarkerClientConfig.MarkerColor);
    }

    private static object? GetItem(object itemView)
    {
        var type = itemView.GetType();
        return AccessTools.Property(type, "Item")?.GetValue(itemView)
            ?? AccessTools.Field(type, "Item")?.GetValue(itemView)
            ?? AccessTools.Field(type, "item_0")?.GetValue(itemView);
    }

    private static Image? GetMainImage(object itemView)
    {
        var type = itemView.GetType();
        return AccessTools.Field(type, "MainImage")?.GetValue(itemView) as Image
            ?? AccessTools.Property(type, "MainImage")?.GetValue(itemView) as Image;
    }

    private static object? FindTooltip(object itemView)
    {
        var itemUiContext = AccessTools.Field(itemView.GetType(), "ItemUiContext")?.GetValue(itemView)
            ?? AccessTools.Property(itemView.GetType(), "ItemUiContext")?.GetValue(itemView);
        if (itemUiContext is null)
        {
            return null;
        }

        foreach (var field in itemUiContext.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
        {
            if (field.FieldType.FullName?.IndexOf("SimpleTooltip", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return field.GetValue(itemUiContext);
            }
        }

        return null;
    }

    private static string GetItemId(object item)
    {
        var type = item.GetType();
        var value = AccessTools.Property(type, "Id")?.GetValue(item)
            ?? AccessTools.Field(type, "Id")?.GetValue(item)
            ?? AccessTools.Field(type, "_id")?.GetValue(item);

        return value?.ToString() ?? string.Empty;
    }
}
#endif
