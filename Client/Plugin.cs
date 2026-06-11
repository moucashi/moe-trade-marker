#if SPT_CLIENT
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using MoeTradeMarker.Client.Patches;
using MoeTradeMarker.Shared;

namespace MoeTradeMarker.Client;

[BepInPlugin(TradeMarkerConstants.ClientGuid, TradeMarkerConstants.ModName, "0.1.0")]
public sealed class Plugin : BaseUnityPlugin
{
    private Harmony? harmony;

    internal static ManualLogSource Log { get; private set; } = null!;

    private void Awake()
    {
        Log = Logger;
        TradeMarkerClientConfig.Bind(Config);
        TradeMarkerDataLoader.Refresh(force: true);

        harmony = new Harmony(TradeMarkerConstants.ClientGuid);
        harmony.PatchAll(typeof(QuestItemViewPanelPatch).Assembly);

        Logger.LogInfo("MoeTradeMarker 客户端已加载。");
    }

    private void OnDestroy()
    {
        harmony?.UnpatchSelf();
        harmony = null;
    }
}
#endif
