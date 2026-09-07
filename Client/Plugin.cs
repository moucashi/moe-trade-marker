#if SPT_CLIENT
using BepInEx;
using BepInEx.Logging;
using EFT.UI;
using HarmonyLib;
using MoeTradeMarker.Client.Patches;
using MoeTradeMarker.Shared;

namespace MoeTradeMarker.Client;

[BepInPlugin(TradeMarkerConstants.ClientGuid, TradeMarkerConstants.ModName, "1.2.6")]
[BepInDependency("com.blackhawk.quicksell", BepInDependency.DependencyFlags.SoftDependency)]
[BepInDependency("com.swiftxp.spt.showmethemoney.quicksell", BepInDependency.DependencyFlags.SoftDependency)]
public sealed class Plugin : BaseUnityPlugin
{
    private Harmony? harmony;

    internal static ManualLogSource Log { get; private set; } = null!;

    private void Awake()
    {
        Log = Logger;
        TradeMarkerClientConfig.Bind(Config);
        TradeMarkerDataLoader.Start();
        TradeMarkerDataLoader.QueueLanguage(TradeMarkerLocalization.LanguageCode);

        try
        {
            harmony = new Harmony(TradeMarkerConstants.ClientGuid);
            InstallPatches(harmony);
            TradeMarkerDataLoader.RequestRefresh();
        }
        catch (Exception exception)
        {
            Logger.LogError(TradeMarkerLocalization.Format(TradeMarkerText.ClientInitFailed, exception));
        }

        Logger.LogInfo(TradeMarkerLocalization.Text(TradeMarkerText.ClientLoaded));
    }

    private void InstallPatches(Harmony harmonyInstance)
    {
        var patchTypes = typeof(ItemViewTradeMarkerPatch).Assembly.GetTypes()
            .Where(type => type.GetCustomAttributes(typeof(HarmonyPatch), inherit: false).Length > 0);

        foreach (var patchType in patchTypes)
        {
            try
            {
                harmonyInstance.CreateClassProcessor(patchType).Patch();
            }
            catch (Exception exception)
            {
                Logger.LogError($"Moe-TradeMarker failed to install patch {patchType.FullName}: {exception}");
            }
        }
    }

    private void Update()
    {
        TradeMarkerClientConfig.Tick();
        TradeMarkerDataLoader.Tick();
        if (TradeMarkerDataLoader.ConsumeRefreshCompleted())
        {
            ItemViewTradeMarkerPatch.RefreshTrackedItemViews();
            TradeMarkerTooltipContext.RefreshVisibleTooltips();
            BaseContextInteractions.RequestGlobalRedraw();
        }
    }

    private void OnDestroy()
    {
        TradeMarkerDataLoader.Stop();
        TradeMarkerTooltipContext.Clear();
        ItemViewTradeMarkerPatch.Clear();
        TradeMarkerOverlay.Clear();
        harmony?.UnpatchSelf();
        harmony = null;
    }
}
#endif
