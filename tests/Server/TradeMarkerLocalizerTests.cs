using MoeTradeMarker.Shared;
using Xunit;

namespace MoeTradeMarker.Server.Tests;

public class TradeMarkerLocalizerTests
{
    [Fact]
    public void DetectLanguage_ReturnsEnglishByDefault()
    {
        var language = TradeMarkerLocalizer.DetectLanguage(null);

        Assert.Equal(TradeMarkerLanguage.English, language);
    }

    [Theory]
    [InlineData("ch")]
    [InlineData("chs")]
    [InlineData("cht")]
    [InlineData("zh-CN")]
    [InlineData("cn")]
    [InlineData("ChineseSimplified")]
    public void DetectLanguage_ReturnsChineseForChineseLanguageCodes(string languageCode)
    {
        var language = TradeMarkerLocalizer.DetectLanguage(languageCode);

        Assert.Equal(TradeMarkerLanguage.Chinese, language);
    }

    [Fact]
    public void Format_ReturnsLocalizedRagfairMessage()
    {
        var english = TradeMarkerLocalizer.Format(TradeMarkerText.RagfairMarkedItemBlocked, TradeMarkerLanguage.English, "Prapor");
        var chinese = TradeMarkerLocalizer.Format(TradeMarkerText.RagfairMarkedItemBlocked, TradeMarkerLanguage.Chinese, "Prapor");

        Assert.Contains("flea market", english);
        Assert.Contains("跳蚤市场", chinese);
    }
}
