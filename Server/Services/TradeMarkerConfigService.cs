using System.Reflection;
using MoeTradeMarker.Shared;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.Helpers;
using SPTarkov.Server.Core.Models.Utils;

namespace MoeTradeMarker.Server.Services;

[Injectable(InjectionType = InjectionType.Singleton)]
public class TradeMarkerConfigService(
    ISptLogger<TradeMarkerConfigService> logger,
    ModHelper modHelper,
    TradeMarkerLanguageService languageService)
{
    public MoeTradeMarkerConfig Config { get; private set; } = new();

    public void Load()
    {
        try
        {
            var modPath = modHelper.GetAbsolutePathToModFolder(Assembly.GetExecutingAssembly());
            Config = modHelper.GetJsonDataFromFile<MoeTradeMarkerConfig>(modPath, "config.json") ?? new MoeTradeMarkerConfig();
            logger.Success(languageService.Text(TradeMarkerText.ServerConfigLoaded));
        }
        catch (Exception exception)
        {
            Config = new MoeTradeMarkerConfig();
            logger.Warning(languageService.FormatServer(TradeMarkerText.ServerConfigLoadFailed, exception.Message));
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
