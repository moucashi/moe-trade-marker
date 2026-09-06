#if SPT_CLIENT
using System.Collections.Generic;
using System.Reflection;
using Comfort.Common;
using HarmonyLib;

namespace MoeTradeMarker.Client.Patches;

[HarmonyPatch]
internal static class BlackHawkQuickSellMenuAvailabilityPatch
{
    private const string FleaInteractionName = "QuickSell (Flea)";

    private static IEnumerable<MethodBase> TargetMethods()
    {
        var buttonType = AccessTools.TypeByName("EFT.UI.SimpleContextMenuButton");
        if (buttonType is null)
        {
            yield break;
        }

        foreach (var method in buttonType.GetMethods(BindingFlags.Instance | BindingFlags.Public))
        {
            if (method.Name == "Show" && method.GetParameters().FirstOrDefault()?.ParameterType == typeof(string))
            {
                yield return method;
            }
        }
    }

    private static void Postfix(object __instance, object[] __args)
    {
        if (__args.Length == 0
            || !string.Equals(__args[0]?.ToString(), FleaInteractionName, StringComparison.Ordinal)
            || !TradeMarkerItemRestriction.ContainsRagfairRestrictedItem(BlackHawkQuickSellContextPatch.CurrentItem))
        {
            return;
        }

        AccessTools.Method(__instance.GetType(), "SetButtonInteraction")
            ?.Invoke(__instance, [new FailedResult(string.Empty)]);
    }
}

[HarmonyPatch]
internal static class BlackHawkQuickSellContextPatch
{
    public static object? CurrentItem { get; private set; }

    private static IEnumerable<MethodBase> TargetMethods()
    {
        var contextMenuPatchType = AccessTools.TypeByName("QuickSell.Patches.ContextMenuPatch");
        if (contextMenuPatchType is null)
        {
            yield break;
        }

        var addEntriesMethod = AccessTools.Method(contextMenuPatchType, "AddQuickSellEntries");
        if (addEntriesMethod is not null)
        {
            yield return addEntriesMethod;
        }
    }

    private static void Prefix(object __1)
    {
        CurrentItem = __1;
    }
}

[HarmonyPatch]
internal static class BlackHawkQuickSellExecutionPatch
{
    private const string ContextMenuPatchTypeName = "QuickSell.Patches.ContextMenuPatch";

    private static IEnumerable<MethodBase> TargetMethods()
    {
        var contextMenuPatchType = AccessTools.TypeByName(ContextMenuPatchTypeName);
        if (contextMenuPatchType is null)
        {
            yield break;
        }

        var sellToFleaMethod = AccessTools.Method(contextMenuPatchType, "SellToFlea");
        if (sellToFleaMethod is not null)
        {
            yield return sellToFleaMethod;
        }
    }

    private static bool Prefix(object __0)
    {
        return !TradeMarkerItemRestriction.ContainsRagfairRestrictedItem(__0);
    }
}
#endif
