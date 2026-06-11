using MoeTradeMarker.Server.Patches;
using MoeTradeMarker.Server.Services;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Utils;

namespace MoeTradeMarker.Server;

[Injectable(InjectionType = InjectionType.Singleton, TypePriority = OnLoadOrder.PreSptModLoader + 1)]
public class MoeTradeMarkerServer(
    ISptLogger<MoeTradeMarkerServer> logger,
    TradeMarkerConfigService configService,
    TradeMarkerStaticRouter staticRouter) : IOnLoad
{
    public Task OnLoad()
    {
        configService.Load();
        staticRouter.Enable();
        new TradeHelperBuyItemPatch().Enable();
        new RagfairAddPlayerOfferPatch().Enable();
        logger.Success("MoeTradeMarker 服务端已加载。");

        return Task.CompletedTask;
    }
}
