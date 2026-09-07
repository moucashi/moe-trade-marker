#if SPT_CLIENT
using System.Runtime.CompilerServices;
using EFT;
using EFT.InventoryLogic;
using EFT.UI;
using EFT.UI.DragAndDrop;
using EFT.UI.Insurance;
using EFT.UI.Ragfair;
using HarmonyLib;
using MoeTradeMarker.Client.Data;
using MoeTradeMarker.Shared;
using UnityEngine;

namespace MoeTradeMarker.Client.Patches;

[HarmonyPatch(typeof(ItemTooltip), nameof(ItemTooltip.Show), new[]
{
    typeof(string), typeof(float), typeof(Offer), typeof(Item),
    typeof(InventoryController), typeof(ItemUiContext), typeof(InsuranceCompany)
})]
internal static class ItemTooltipTradeMarkerPatch
{
    private static void Prefix(Item __3, out bool __state)
    {
        TradeMarkerTooltipContext.PushItem(__3);
        __state = true;
    }

    private static void Finalizer(bool __state)
    {
        if (__state) TradeMarkerTooltipContext.PopItem();
    }
}

[HarmonyPatch(typeof(GridItemView), nameof(GridItemView.ShowTooltip))]
internal static class GridItemViewTooltipTradeMarkerPatch
{
    private static void Prefix(GridItemView __instance, out bool __state)
    {
        TradeMarkerTooltipContext.PushItem(__instance.Item);
        __state = true;
    }

    private static void Finalizer(bool __state)
    {
        if (__state) TradeMarkerTooltipContext.PopItem();
    }
}

[HarmonyPatch(typeof(SimpleTooltip), nameof(SimpleTooltip.Show), new[] { typeof(string), typeof(Vector2?), typeof(float), typeof(float?) })]
internal static class SimpleTooltipShowTradeMarkerPatch
{
    private static void Prefix(SimpleTooltip __instance) => TradeMarkerTooltipContext.TrackTooltip(__instance);
}

[HarmonyPatch(typeof(SimpleTooltip), nameof(SimpleTooltip.SetText))]
internal static class SimpleTooltipSetTextTradeMarkerPatch
{
    private static void Prefix(SimpleTooltip __instance, ref string __0) =>
        TradeMarkerTooltipContext.SetText(__instance, ref __0);
}

internal static class TradeMarkerTooltipContext
{
    private static readonly Stack<Item?> ActiveItems = new();
    private static readonly ConditionalWeakTable<SimpleTooltip, TooltipText> TooltipTexts = new();
    private static readonly List<WeakReference<SimpleTooltip>> TrackedTooltips = new();

    public static void PushItem(Item? item) => ActiveItems.Push(item);

    public static void PopItem()
    {
        if (ActiveItems.Count > 0) ActiveItems.Pop();
    }

    public static void TrackTooltip(SimpleTooltip tooltip)
    {
        var item = ActiveItems.Count > 0 ? ActiveItems.Peek() : null;
        if (item is null)
        {
            if (TooltipTexts.TryGetValue(tooltip, out var old)) old.Reset(null);
            return;
        }
        if (!TooltipTexts.TryGetValue(tooltip, out var state))
        {
            state = new TooltipText();
            TooltipTexts.Add(tooltip, state);
            TrackedTooltips.Add(new WeakReference<SimpleTooltip>(tooltip));
        }
        // Track the item even before its marker is known, so a later cache fill can add the note.
        state.Reset(item.Id.ToString());
        TradeMarkerDataLoader.RequestRefresh();
    }

    public static void SetText(SimpleTooltip tooltip, ref string text)
    {
        if (!TooltipTexts.TryGetValue(tooltip, out var state) || state.ItemId is null) return;
        var marker = TradeMarkerDataLoader.TryGetTraderNameForItem(state.ItemId, out var trader, requestRefresh: false)
            ? TradeMarkerLocalization.Format(TradeMarkerText.TooltipTraderMarker, trader)
            : string.Empty;
        text = state.SetText(text, marker);
    }

    public static void RefreshVisibleTooltips()
    {
        TrackedTooltips.RemoveAll(reference => !reference.TryGetTarget(out var tooltip) || tooltip == null);
        foreach (var reference in TrackedTooltips)
        {
            if (!reference.TryGetTarget(out var tooltip) || tooltip == null || !tooltip.Displayed ||
                !TooltipTexts.TryGetValue(tooltip, out var state) || state.ItemId is null || tooltip._label == null) continue;
            // Do not overwrite text another mod changed directly without going through SetText.
            if (tooltip._label.text == state.RenderedText) tooltip.SetText(state.OriginalText);
        }
    }

    public static void Clear()
    {
        foreach (var reference in TrackedTooltips)
        {
            if (reference.TryGetTarget(out var tooltip)) TooltipTexts.Remove(tooltip);
        }
        TrackedTooltips.Clear();
        ActiveItems.Clear();
    }
}
#endif
