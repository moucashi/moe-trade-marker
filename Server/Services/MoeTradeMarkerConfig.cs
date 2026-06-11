using System.Text.Json.Serialization;

namespace MoeTradeMarker.Server.Services;

public record MoeTradeMarkerConfig
{
    [JsonPropertyName("marking")]
    public FeatureSwitchConfig Marking { get; init; } = new();

    [JsonPropertyName("ragfairRestriction")]
    public FeatureSwitchConfig RagfairRestriction { get; init; } = new();
}

public record FeatureSwitchConfig
{
    [JsonPropertyName("enabled")]
    public bool Enabled { get; init; } = true;

    [JsonPropertyName("disabledTraderIds")]
    public List<string> DisabledTraderIds { get; init; } = [];

    public bool IsEnabledForTrader(string? traderId)
    {
        if (!Enabled)
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(traderId))
        {
            return true;
        }

        return !DisabledTraderIds.Contains(traderId, StringComparer.OrdinalIgnoreCase);
    }
}
