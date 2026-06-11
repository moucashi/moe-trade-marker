using System.Reflection;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.Helpers;
using SPTarkov.Server.Core.Models.Utils;

namespace MoeTradeMarker.Server.Services;

[Injectable(InjectionType = InjectionType.Singleton)]
public class TradeMarkerConfigService(ISptLogger<TradeMarkerConfigService> logger, ModHelper modHelper)
{
    public MoeTradeMarkerConfig Config { get; private set; } = new();

    public void Load()
    {
        try
        {
            var modPath = modHelper.GetAbsolutePathToModFolder(Assembly.GetExecutingAssembly());
            Config = modHelper.GetJsonDataFromFile<MoeTradeMarkerConfig>(modPath, "config.json") ?? new MoeTradeMarkerConfig();
            logger.Success("Moe-TradeMarker 配置已加载。");
        }
        catch (Exception exception)
        {
            Config = new MoeTradeMarkerConfig();
            logger.Warning($"Moe-TradeMarker 配置读取失败，已使用默认配置：{exception.Message}");
        }
    }

    public bool ShouldMarkPurchase(string traderId)
    {
        return Config.Marking.IsEnabledForTrader(traderId);
    }

    public bool ShouldBlockRagfairListing(string traderId)
    {
        return Config.RagfairRestriction.IsEnabledForTrader(traderId);
    }
}
