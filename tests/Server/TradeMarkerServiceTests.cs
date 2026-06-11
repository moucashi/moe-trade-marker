using System.Text.Json;
using MoeTradeMarker.Server.Services;
using MoeTradeMarker.Shared;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using Xunit;

namespace MoeTradeMarker.Server.Tests;

public class TradeMarkerServiceTests
{
    [Fact]
    public void TryGetMarkerTraderId_ReadsTradeMarkerData()
    {
        var item = CreateMarkedItem(new TradeMarkerData { TraderId = "trader-a" });

        var result = TradeMarkerService.TryGetMarkerTraderId(item, out var traderId);

        Assert.True(result);
        Assert.Equal("trader-a", traderId);
    }

    [Fact]
    public void TryGetMarkerTraderId_ReadsJsonMarker()
    {
        using var document = JsonDocument.Parse("""{"traderId":"trader-b"}""");
        var item = CreateMarkedItem(document.RootElement.Clone());

        var result = TradeMarkerService.TryGetMarkerTraderId(item, out var traderId);

        Assert.True(result);
        Assert.Equal("trader-b", traderId);
    }

    [Fact]
    public void TryGetMarkerTraderId_ReturnsFalseWhenMarkerMissing()
    {
        var item = new Item { Id = new MongoId("507f1f77bcf86cd799439011"), Upd = new Upd() };

        var result = TradeMarkerService.TryGetMarkerTraderId(item, out var traderId);

        Assert.False(result);
        Assert.Equal(string.Empty, traderId);
    }

    private static Item CreateMarkedItem(object marker)
    {
        return new Item
        {
            Id = new MongoId("507f1f77bcf86cd799439011"),
            Upd = new Upd
            {
#pragma warning disable CS8714
                ExtensionData = new Dictionary<string?, object?>
                {
                    [TradeMarkerConstants.UpdMarkerProperty] = marker,
                },
#pragma warning restore CS8714
            },
        };
    }
}
