#if SPT_CLIENT
using System.Reflection;
using System.Runtime.CompilerServices;
using Comfort.Common;
using EFT.InventoryLogic;
using EFT.UI;
using HarmonyLib;

namespace MoeTradeMarker.Client.Patches;

internal static class BlackHawkQuickSellInteractions
{
    // QuickSell's dictionary key identifies the interaction independently of its displayed caption.
    internal const string FleaKey = "QuickSell (Flea)";
    internal static readonly ConditionalWeakTable<DynamicContextInteraction, Item> Items = new();

    internal static MethodBase? FindMethod(string name)
    {
        var type = AccessTools.TypeByName("QuickSell.Patches.ContextMenuPatch");
        return type is null ? null : AccessTools.Method(type, name);
    }
}

[HarmonyPatch(typeof(InteractionButtonsContainer), nameof(InteractionButtonsContainer.CreateDynamicContextButton))]
internal static class BlackHawkQuickSellMenuAvailabilityPatch
{
    private static bool Prefix(InteractionButtonsContainer __instance, DynamicContextInteraction interaction)
    {
        if (!BlackHawkQuickSellInteractions.Items.TryGetValue(interaction, out var item))
        {
            return true;
        }

        // Preserve the game's dynamic-button lifecycle, supplying availability before binding.
        var button = __instance.CreateContextButton(
            interaction.Key, interaction.Key, __instance._buttonTemplate, __instance._buttonsContainer,
            interaction.Icon, interaction.Execute, __instance.CloseSubMenu);
        IResult availability = TradeMarkerItemRestriction.ContainsRagfairRestrictedItem(item)
            ? new FailedResult(string.Empty)
            : SuccessfulResult.New;
        button.SetButtonInteraction(availability);
        __instance.BindButton(button);
        return false;
    }
}

[HarmonyPatch]
internal static class BlackHawkQuickSellContextPatch
{
    private static MethodBase? TargetMethod() =>
        BlackHawkQuickSellInteractions.FindMethod("AddQuickSellEntries");

    private static bool Prepare() => TargetMethod() is not null;

    private static void Postfix(ContextInteractions<EItemInfoButton> __0, Item __1)
    {
        if (__1 is null || !__0._dynamicInteractions.TryGetValue(BlackHawkQuickSellInteractions.FleaKey, out var interaction))
        {
            return;
        }

        BlackHawkQuickSellInteractions.Items.Remove(interaction);
        BlackHawkQuickSellInteractions.Items.Add(interaction, __1);
    }
}

[HarmonyPatch]
internal static class BlackHawkQuickSellExecutionPatch
{
    private static MethodBase? TargetMethod() =>
        BlackHawkQuickSellInteractions.FindMethod("SellToFlea");

    private static bool Prepare() => TargetMethod() is not null;

    private static bool Prefix(Item __0)
    {
        return !TradeMarkerItemRestriction.ContainsRagfairRestrictedItem(__0);
    }
}
#endif
