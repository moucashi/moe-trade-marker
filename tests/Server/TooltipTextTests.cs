using MoeTradeMarker.Client.Data;
using Xunit;

namespace MoeTradeMarker.Server.Tests;

public class TooltipTextTests
{
    [Fact]
    public void CacheArrivalCanAddANoteToAnInitiallyUnmarkedTooltip()
    {
        var text = new TooltipText();
        text.Reset("item");
        Assert.Equal("Item name", text.SetText("Item name", ""));
        Assert.Equal("Item name\n\nTrader: Prapor", text.SetText(text.OriginalText, "Trader: Prapor"));
    }

    [Fact]
    public void LanguageAndTraderNameChangesReplaceTheNoteWithoutAccumulatingText()
    {
        var text = new TooltipText();
        text.Reset("item");
        text.SetText("<b>Item name</b>", "Trader: bbbbbbbbbbbbbbbbbbbbbbbb");
        Assert.Equal("<b>Item name</b>\n\nTrader: Prapor", text.SetText(text.OriginalText, "Trader: Prapor"));
        Assert.Equal("<b>Item name</b>\n\nBought from Prapor", text.SetText(text.OriginalText, "Bought from Prapor"));
        Assert.Equal("<b>Item name</b>", text.SetText(text.OriginalText, ""));
    }

    [Fact]
    public void GameTextUpdatesRemainTheBaseForFutureRefreshes()
    {
        var text = new TooltipText();
        text.Reset("item");
        text.SetText("Name", "Trader: Prapor");
        text.SetText("Name\nExamined", "Trader: Prapor");
        Assert.Equal("Name\nExamined", text.OriginalText);
        Assert.Equal("Name\nExamined\nTrader: Prapor", text.SetText("Name\nExamined\nTrader: Prapor", "Trader: Prapor"));
    }

    [Fact]
    public void ReusingTooltipForOtherContentClearsTheItemAndOldText()
    {
        var text = new TooltipText();
        text.Reset("item");
        text.SetText("Name", "Trader: Prapor");
        text.Reset(null);
        Assert.Null(text.ItemId);
        Assert.Empty(text.OriginalText);
        Assert.Empty(text.RenderedText);
        text.Reset("other item");
        Assert.Equal("Other name", text.SetText("Other name", ""));
    }
}
