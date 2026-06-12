#if SPT_CLIENT
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using HarmonyLib;
using MoeTradeMarker.Shared;

namespace MoeTradeMarker.Client.Patches;

[HarmonyPatch]
internal static class ItemTooltipTradeMarkerPatch
{
    private static IEnumerable<MethodBase> TargetMethods()
    {
        var itemTooltipType = AccessTools.TypeByName("EFT.UI.ItemTooltip");
        if (itemTooltipType is null)
        {
            yield break;
        }

        var itemType = AccessTools.TypeByName("EFT.InventoryLogic.Item");
        if (itemType is null)
        {
            yield break;
        }

        foreach (var method in itemTooltipType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
        {
            if (method.Name != "Show")
            {
                continue;
            }

            var parameters = method.GetParameters();
            if (parameters.Length >= 4 && parameters[0].ParameterType == typeof(string) && parameters[3].ParameterType == itemType)
            {
                yield return method;
            }
        }
    }

    private static void Prefix(ref string __0, object __3)
    {
        if (!TradeMarkerTooltipContext.TryCreateMarkerTextForItem(__3, out var markerText))
        {
            TradeMarkerTooltipContext.PushMarkerText(null);
            return;
        }

        TradeMarkerTooltipContext.PushMarkerText(markerText);
        __0 = TradeMarkerTooltipContext.AppendMarkerText(__0, markerText);
    }

    private static void Finalizer()
    {
        TradeMarkerTooltipContext.PopMarkerText();
    }
}

[HarmonyPatch]
internal static class GridItemViewTooltipTradeMarkerPatch
{
    private static IEnumerable<MethodBase> TargetMethods()
    {
        var gridItemViewType = AccessTools.TypeByName("EFT.UI.DragAndDrop.GridItemView");
        var showTooltipMethod = AccessTools.Method(gridItemViewType, "ShowTooltip", Type.EmptyTypes);
        if (showTooltipMethod is not null)
        {
            yield return showTooltipMethod;
        }
    }

    private static void Prefix(object __instance)
    {
        TradeMarkerTooltipContext.PushItemMarkerText(GetItem(__instance));
    }

    private static void Finalizer()
    {
        TradeMarkerTooltipContext.PopMarkerText();
    }

    private static object? GetItem(object itemView)
    {
        var type = itemView.GetType();
        return AccessTools.Property(type, "Item")?.GetValue(itemView)
            ?? AccessTools.Field(type, "item_0")?.GetValue(itemView);
    }
}

[HarmonyPatch]
internal static class SimpleTooltipShowTradeMarkerPatch
{
    private static IEnumerable<MethodBase> TargetMethods()
    {
        var simpleTooltipType = AccessTools.TypeByName("EFT.UI.SimpleTooltip");
        if (simpleTooltipType is null)
        {
            yield break;
        }

        foreach (var method in simpleTooltipType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
        {
            var parameters = method.GetParameters();
            if (method.Name == "Show" && parameters.Length >= 1 && parameters[0].ParameterType == typeof(string))
            {
                yield return method;
            }
        }
    }

    private static void Prefix(object __instance, ref string __0)
    {
        if (TradeMarkerTooltipContext.TryGetActiveMarkerText(out var markerText))
        {
            TradeMarkerTooltipContext.TrackTooltip(__instance, markerText);
            __0 = TradeMarkerTooltipContext.AppendMarkerText(__0, markerText);
            return;
        }

        TradeMarkerTooltipContext.ForgetTooltip(__instance);
    }
}

[HarmonyPatch]
internal static class SimpleTooltipSetTextTradeMarkerPatch
{
    private static IEnumerable<MethodBase> TargetMethods()
    {
        var simpleTooltipType = AccessTools.TypeByName("EFT.UI.SimpleTooltip");
        var setTextMethod = AccessTools.Method(simpleTooltipType, "SetText", [typeof(string)]);
        if (setTextMethod is not null)
        {
            yield return setTextMethod;
        }
    }

    private static void Prefix(object __instance, ref string __0)
    {
        if (TradeMarkerTooltipContext.TryGetTooltipMarkerText(__instance, out var markerText))
        {
            __0 = TradeMarkerTooltipContext.AppendMarkerText(__0, markerText);
        }
    }
}

internal static class TradeMarkerTooltipContext
{
    private static readonly Stack<string?> ActiveMarkerTexts = new();
    private static readonly ConditionalWeakTable<object, MarkerTextHolder> TooltipMarkerTexts = new();

    public static void PushItemMarkerText(object? item)
    {
        PushMarkerText(TryCreateMarkerTextForItem(item, out var markerText) ? markerText : null);
    }

    public static void PushMarkerText(string? markerText)
    {
        ActiveMarkerTexts.Push(markerText);
    }

    public static void PopMarkerText()
    {
        if (ActiveMarkerTexts.Count > 0)
        {
            ActiveMarkerTexts.Pop();
        }
    }

    public static bool TryGetActiveMarkerText(out string markerText)
    {
        markerText = string.Empty;
        if (ActiveMarkerTexts.Count == 0)
        {
            return false;
        }

        markerText = ActiveMarkerTexts.Peek() ?? string.Empty;
        return !string.IsNullOrWhiteSpace(markerText);
    }

    public static void TrackTooltip(object tooltip, string markerText)
    {
        TooltipMarkerTexts.Remove(tooltip);
        TooltipMarkerTexts.Add(tooltip, new MarkerTextHolder(markerText));
    }

    public static void ForgetTooltip(object tooltip)
    {
        TooltipMarkerTexts.Remove(tooltip);
    }

    public static bool TryGetTooltipMarkerText(object tooltip, out string markerText)
    {
        markerText = TooltipMarkerTexts.TryGetValue(tooltip, out var holder)
            ? holder.MarkerText
            : string.Empty;

        return !string.IsNullOrWhiteSpace(markerText);
    }

    public static string AppendMarkerText(string? text, string markerText)
    {
        if (string.IsNullOrWhiteSpace(markerText) || text?.IndexOf(markerText, StringComparison.Ordinal) >= 0)
        {
            return text ?? string.Empty;
        }

        return string.IsNullOrWhiteSpace(text)
            ? markerText
            : $"{text}\n\n{markerText}";
    }

    public static bool TryCreateMarkerTextForItem(object? item, out string markerText)
    {
        markerText = string.Empty;
        var itemId = GetItemId(item);
        if (string.IsNullOrWhiteSpace(itemId) || !TradeMarkerDataLoader.TryGetTraderNameForItem(itemId, out var traderName))
        {
            return false;
        }

        markerText = TradeMarkerLocalization.Format(TradeMarkerText.TooltipTraderMarker, traderName);
        return true;
    }

    private static string GetItemId(object? item)
    {
        if (item is null)
        {
            return string.Empty;
        }

        var type = item.GetType();
        var value = AccessTools.Property(type, "Id")?.GetValue(item)
            ?? AccessTools.Field(type, "Id")?.GetValue(item)
            ?? AccessTools.Field(type, "_id")?.GetValue(item);

        return value?.ToString() ?? string.Empty;
    }

    private sealed class MarkerTextHolder(string markerText)
    {
        public string MarkerText { get; } = markerText;
    }
}
#endif
