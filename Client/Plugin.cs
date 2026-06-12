#if SPT_CLIENT
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using MoeTradeMarker.Client.Patches;
using MoeTradeMarker.Shared;

namespace MoeTradeMarker.Client;

[BepInPlugin(TradeMarkerConstants.ClientGuid, TradeMarkerConstants.ModName, "0.3.0")]
public sealed class Plugin : BaseUnityPlugin
{
    private Harmony? harmony;

    internal static ManualLogSource Log { get; private set; } = null!;

    private void Awake()
    {
        Log = Logger;
        TradeMarkerLocalization.Initialize();
        TradeMarkerClientConfig.Bind(Config);
        TradeMarkerLocalization.Refresh();

        try
        {
            harmony = new Harmony(TradeMarkerConstants.ClientGuid);
            harmony.PatchAll(typeof(ItemViewTradeMarkerPatch).Assembly);
            TradeMarkerDataLoader.Refresh(force: true);
        }
        catch (Exception exception)
        {
            Logger.LogError(TradeMarkerLocalization.Format(TradeMarkerText.ClientInitFailed, exception));
        }

        Logger.LogInfo(TradeMarkerLocalization.Text(TradeMarkerText.ClientLoaded));
    }

    private void OnDestroy()
    {
        harmony?.UnpatchSelf();
        harmony = null;
    }
}
#endif
