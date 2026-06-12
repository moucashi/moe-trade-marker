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

    [Theory]
    [InlineData("cz", TradeMarkerLanguage.Czech)]
    [InlineData("fr", TradeMarkerLanguage.French)]
    [InlineData("ge", TradeMarkerLanguage.German)]
    [InlineData("de-DE", TradeMarkerLanguage.German)]
    [InlineData("hu", TradeMarkerLanguage.Hungarian)]
    [InlineData("it", TradeMarkerLanguage.Italian)]
    [InlineData("jp", TradeMarkerLanguage.Japanese)]
    [InlineData("ja-JP", TradeMarkerLanguage.Japanese)]
    [InlineData("kr", TradeMarkerLanguage.Korean)]
    [InlineData("ko-KR", TradeMarkerLanguage.Korean)]
    [InlineData("pl", TradeMarkerLanguage.Polish)]
    [InlineData("po", TradeMarkerLanguage.Portuguese)]
    [InlineData("pt-BR", TradeMarkerLanguage.Portuguese)]
    [InlineData("sk", TradeMarkerLanguage.Slovak)]
    [InlineData("es", TradeMarkerLanguage.Spanish)]
    [InlineData("es-mx", TradeMarkerLanguage.SpanishMexico)]
    [InlineData("tu", TradeMarkerLanguage.Turkish)]
    [InlineData("tr-TR", TradeMarkerLanguage.Turkish)]
    [InlineData("ru", TradeMarkerLanguage.Russian)]
    [InlineData("ro", TradeMarkerLanguage.Romanian)]
    public void DetectLanguage_ReturnsSupportedSptLanguages(string languageCode, TradeMarkerLanguage expected)
    {
        var language = TradeMarkerLocalizer.DetectLanguage(languageCode);

        Assert.Equal(expected, language);
    }

    [Theory]
    [InlineData(TradeMarkerLanguage.English, "en")]
    [InlineData(TradeMarkerLanguage.Chinese, "ch")]
    [InlineData(TradeMarkerLanguage.Japanese, "jp")]
    [InlineData(TradeMarkerLanguage.Korean, "kr")]
    [InlineData(TradeMarkerLanguage.Russian, "ru")]
    [InlineData(TradeMarkerLanguage.Spanish, "es")]
    [InlineData(TradeMarkerLanguage.SpanishMexico, "es-mx")]
    public void GetLanguageCode_ReturnsSptLanguageCode(TradeMarkerLanguage language, string expectedCode)
    {
        var languageCode = TradeMarkerLocalizer.GetLanguageCode(language);

        Assert.Equal(expectedCode, languageCode);
    }

    [Fact]
    public void Format_ReturnsLocalizedRagfairMessage()
    {
        var english = TradeMarkerLocalizer.Format(TradeMarkerText.RagfairMarkedItemBlocked, TradeMarkerLanguage.English, "Prapor");
        var chinese = TradeMarkerLocalizer.Format(TradeMarkerText.RagfairMarkedItemBlocked, TradeMarkerLanguage.Chinese, "Prapor");
        var japanese = TradeMarkerLocalizer.Format(TradeMarkerText.RagfairMarkedItemBlocked, TradeMarkerLanguage.Japanese, "Prapor");
        var korean = TradeMarkerLocalizer.Format(TradeMarkerText.RagfairMarkedItemBlocked, TradeMarkerLanguage.Korean, "Prapor");
        var russian = TradeMarkerLocalizer.Format(TradeMarkerText.RagfairMarkedItemBlocked, TradeMarkerLanguage.Russian, "Prapor");
        var spanish = TradeMarkerLocalizer.Format(TradeMarkerText.RagfairMarkedItemBlocked, TradeMarkerLanguage.Spanish, "Prapor");

        Assert.Contains("flea market", english);
        Assert.Contains("跳蚤市场", chinese);
        Assert.Contains("フリーマーケット", japanese);
        Assert.Contains("플리 마켓", korean);
        Assert.Contains("барахолку", russian);
        Assert.Contains("mercadillo", spanish);
    }
}
