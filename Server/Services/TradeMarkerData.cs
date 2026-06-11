using System.Text.Json.Serialization;

namespace MoeTradeMarker.Server.Services;

public record TradeMarkerData
{
    [JsonPropertyName("traderId")]
    public required string TraderId { get; init; }
}
