using MoeTradeMarker.Server.Services;
using Xunit;

namespace MoeTradeMarker.Server.Tests;

public class FeatureSwitchConfigTests
{
    [Fact]
    public void IsEnabledForTrader_ReturnsTrueByDefault()
    {
        var config = new FeatureSwitchConfig();

        Assert.True(config.IsEnabledForTrader("trader-a"));
    }

    [Fact]
    public void IsEnabledForTrader_ReturnsFalseWhenGloballyDisabled()
    {
        var config = new FeatureSwitchConfig { Enabled = false };

        Assert.False(config.IsEnabledForTrader("trader-a"));
    }

    [Fact]
    public void IsEnabledForTrader_ReturnsFalseForDisabledTraderOnly()
    {
        var config = new FeatureSwitchConfig
        {
            DisabledTraderIds = ["trader-a"],
        };

        Assert.False(config.IsEnabledForTrader("TRADER-A"));
        Assert.True(config.IsEnabledForTrader("trader-b"));
    }
}
