using MoeTradeMarker.Shared;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common;
using SPTarkov.Server.Core.Utils;

namespace MoeTradeMarker.Server.Services;

[Injectable(InjectionType = InjectionType.Singleton)]
public class TradeMarkerStaticRouter : StaticRouter
{
    private static JsonUtil? RouterJsonUtil { get; set; }
    private static TradeMarkerService? RouterTradeMarkerService { get; set; }

    public TradeMarkerStaticRouter(JsonUtil jsonUtil, TradeMarkerService tradeMarkerService)
        : base(jsonUtil, GetRoutes())
    {
        RouterJsonUtil = jsonUtil;
        RouterTradeMarkerService = tradeMarkerService;
    }

    public void Enable()
    {
    }

    private static List<RouteAction> GetRoutes()
    {
        return
        [
            new RouteAction<EmptyRequestData>(
                TradeMarkerConstants.TraderInfoRoute,
                static (url, info, sessionId, output) => HandleTraderInfoRoute(sessionId)
            ),
            new RouteAction<EmptyRequestData>(
                TradeMarkerConstants.ItemMarkerRoute,
                static (url, info, sessionId, output) => HandleItemMarkerRoute(sessionId)
            ),
        ];
    }

    private static ValueTask<string> HandleTraderInfoRoute(MongoId sessionId)
    {
        var traders = RouterTradeMarkerService?.GetTraderNames() ?? [];
        return new ValueTask<string>(RouterJsonUtil?.Serialize(traders) ?? "{}");
    }

    private static ValueTask<string> HandleItemMarkerRoute(MongoId sessionId)
    {
        var markedItems = RouterTradeMarkerService?.GetMarkedItems(sessionId) ?? [];
        return new ValueTask<string>(RouterJsonUtil?.Serialize(markedItems) ?? "{}");
    }
}
