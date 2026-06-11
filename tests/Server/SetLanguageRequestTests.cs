using System.Text.Json;
using MoeTradeMarker.Server.Services;
using Xunit;

namespace MoeTradeMarker.Server.Tests;

public class SetLanguageRequestTests
{
    [Fact]
    public void GetLanguage_ReadsLowercaseJsonExtensionData()
    {
        var request = JsonSerializer.Deserialize<SetLanguageRequest>("""{"language":"en"}""");

        Assert.Equal("en", request?.GetLanguage());
    }
}
