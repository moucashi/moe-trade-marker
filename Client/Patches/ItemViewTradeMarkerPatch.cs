#if SPT_CLIENT
using System.Runtime.CompilerServices;
using EFT.UI.DragAndDrop;
using HarmonyLib;
using UnityEngine;

namespace MoeTradeMarker.Client.Patches;

[HarmonyPatch(typeof(ItemView), nameof(ItemView.SetQuestItemViewPanel))]
internal static class ItemViewTradeMarkerPatch
{
    private const string OverlayName = "MoeTradeMarkerIcon";
    private static readonly List<WeakReference<ItemView>> ActiveItemViews = new();
    private static readonly ConditionalWeakTable<ItemView, object> TrackedItemViews = new();
    private static readonly object Tracked = new();

    private static void Postfix(ItemView __instance)
    {
        if (__instance == null) return;
        if (!TrackedItemViews.TryGetValue(__instance, out _))
        {
            TrackedItemViews.Add(__instance, Tracked);
            ActiveItemViews.Add(new WeakReference<ItemView>(__instance));
        }
        Apply(__instance, true);
    }

    private static void Apply(ItemView view, bool requestRefresh)
    {
        var item = view.Item;
        if (!TradeMarkerClientConfig.ShowTraderMarker || item is null ||
            !TradeMarkerDataLoader.TryGetTraderNameForItem(item.Id.ToString(), out _, requestRefresh))
        {
            TradeMarkerOverlay.HideFromItemView(view, OverlayName);
            return;
        }
        TradeMarkerOverlay.ShowOnItemView(view, OverlayName,
            TradeMarkerClientConfig.MarkerPosition, TradeMarkerClientConfig.MarkerColor);
    }

    public static void RefreshTrackedItemViews()
    {
        ActiveItemViews.RemoveAll(reference => !reference.TryGetTarget(out var view) || view == null);
        foreach (var reference in ActiveItemViews)
        {
            // Redraw reads the snapshot without scheduling another refresh.
            if (reference.TryGetTarget(out var view) && view != null) Apply(view, false);
        }
    }

    public static void Clear()
    {
        foreach (var reference in ActiveItemViews)
        {
            if (reference.TryGetTarget(out var view) && view != null)
            {
                TradeMarkerOverlay.HideFromItemView(view, OverlayName);
                TrackedItemViews.Remove(view);
            }
        }
        ActiveItemViews.Clear();
    }
}
#endif
