#if SPT_CLIENT
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using MoeTradeMarker.Client.Patches;
using MoeTradeMarker.Shared;

namespace MoeTradeMarker.Client;

[BepInPlugin(TradeMarkerConstants.ClientGuid, TradeMarkerConstants.ModName, "0.1.6")]
public sealed class Plugin : BaseUnityPlugin
{
    private Harmony? harmony;

    internal static ManualLogSource Log { get; private set; } = null!;

    private void Awake()
    {
        Log = Logger;
        TradeMarkerClientConfig.Bind(Config);

        try
        {
            harmony = new Harmony(TradeMarkerConstants.ClientGuid);
            harmony.PatchAll(typeof(ItemViewTradeMarkerPatch).Assembly);
            TradeMarkerDataLoader.Refresh(force: true);
        }
        catch (Exception exception)
        {
            Logger.LogError($"Moe-TradeMarker 客户端初始化失败：{exception}");
        }

        Logger.LogInfo("Moe-TradeMarker 客户端已加载。");
    }

    private void OnDestroy()
    {
        harmony?.UnpatchSelf();
        harmony = null;
    }
}
#endif
