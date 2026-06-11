using System.Text.Json;
using System.Text.Json.Serialization;
using SPTarkov.Server.Core.Models.Utils;

namespace MoeTradeMarker.Server.Services;

public record SetLanguageRequest : IRequestData
{
    public string? Language { get; init; }

    [JsonExtensionData]
    public Dictionary<string, object>? ExtensionData { get; set; }

    public string? GetLanguage()
    {
        if (!string.IsNullOrWhiteSpace(Language))
        {
            return Language;
        }

        if (ExtensionData is null)
        {
            return null;
        }

        foreach (var key in new[] { "language", "Language" })
        {
            if (!ExtensionData.TryGetValue(key, out var value))
            {
                continue;
            }

            return value switch
            {
                JsonElement element when element.ValueKind == JsonValueKind.String => element.GetString(),
                _ => value?.ToString(),
            };
        }

        return null;
    }
}
