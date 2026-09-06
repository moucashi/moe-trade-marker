using MoeTradeMarker.Server.Patches;
using MoeTradeMarker.Server.Services;
using MoeTradeMarker.Shared;
using SPTarkov.Common.Models.Logging;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;

namespace MoeTradeMarker.Server;

[Injectable(InjectionType = InjectionType.Singleton, TypePriority = OnLoadOrder.Preload + 1)]
public class MoeTradeMarkerServer(
    ISptLogger<MoeTradeMarkerServer> logger,
    TradeMarkerConfigService configService,
    TradeMarkerStaticRouter staticRouter,
    TradeMarkerLanguageService languageService,
    TradeMarkerService tradeMarkerService) : IOnLoad
{
    public Task OnLoadAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        configService.Load();
        staticRouter.Enable();
        new TradeHelperBuyItemPatch(tradeMarkerService).Enable();
        new RagfairAddPlayerOfferPatch(tradeMarkerService).Enable();
        logger.Success(languageService.Text(TradeMarkerText.ServerLoaded));

        return Task.CompletedTask;
    }
}
