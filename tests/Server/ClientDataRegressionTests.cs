using System.ComponentModel;
using MoeTradeMarker.Client.Data;
using MoeTradeMarker.Shared;
using Newtonsoft.Json;
using Xunit;

namespace MoeTradeMarker.Server.Tests;

public class ClientDataRegressionTests
{
    private const string ItemId = "aaaaaaaaaaaaaaaaaaaaaaaa";
    private const string TraderId = "bbbbbbbbbbbbbbbbbbbbbbbb";
    private const string Names = "{\"bbbbbbbbbbbbbbbbbbbbbbbb\":\"商人\\\"名字\"}";
    private const string Markers = "{\"aaaaaaaaaaaaaaaaaaaaaaaa\":\"bbbbbbbbbbbbbbbbbbbbbbbb\"}";
    private const string Restricted = "[\"bbbbbbbbbbbbbbbbbbbbbbbb\"]";

    [Fact]
    public void PurchaseIsImmediatelyVisibleAndSurvivesOlderRefresh()
    {
        var cache = new MarkerCache();
        cache.Replace(0, MarkerSnapshot.Parse(Names, "{}", Restricted));
        var beforePurchase = cache.Revision;
        cache.Apply(new() { [ItemId] = TraderId });
        Assert.True(cache.Snapshot.TryGetTraderName(ItemId, out _));
        Assert.True(cache.Snapshot.IsRestricted(ItemId));
        Assert.False(cache.Replace(beforePurchase, MarkerSnapshot.Parse(Names, "{}", Restricted)));
        Assert.True(cache.Snapshot.TryGetTraderName(ItemId, out _));
        Assert.True(cache.Replace(cache.Revision, MarkerSnapshot.Empty));
        Assert.False(cache.Snapshot.TryGetTraderName(ItemId, out _));
    }

    [Theory]
    [InlineData("{}", false)]
    [InlineData("[]", false)]
    [InlineData("null", false)]
    [InlineData("{\"tradeMarker\":false}", false)]
    [InlineData("{\"tradeMarker\":{\"traderId\":123}}", false)]
    [InlineData("{\"tradeMarker\":{\"traderId\":\"invalid\"}}", false)]
    [InlineData("{\"tradeMarker\":{\"traderId\":\"bbbbbbbbbbbbbbbbbbbbbbbb\"}}", true)]
    public void PurchaseMarkerRequiresAuthoritativeValidData(string json, bool valid)
    {
        var token = Newtonsoft.Json.Linq.JToken.Parse(json);
        Assert.Equal(valid, MarkerSnapshot.TryReadItemMarker(ItemId, token, out var trader));
        if (valid) Assert.Equal(TraderId, trader);
        Assert.False(MarkerSnapshot.TryReadItemMarker("bad", token, out _));
    }

    [Fact]
    public void MultiplePurchasesSurviveStaleRefreshAndEmptyDelta()
    {
        var cache = new MarkerCache();
        cache.Apply(new() { [ItemId] = TraderId });
        var revision = cache.Revision;
        const string second = "cccccccccccccccccccccccc";
        cache.Apply(new() { [second] = TraderId });
        cache.Apply(new());
        Assert.Equal(revision + 1, cache.Revision);
        Assert.True(cache.Replace(revision, MarkerSnapshot.Parse(Names, Markers, Restricted)));
        Assert.True(cache.Snapshot.TryGetTraderName(ItemId, out _));
        Assert.True(cache.Snapshot.TryGetTraderName(second, out _));
    }

    [Fact]
    public void InitialRestrictionIsUnknownUntilACompleteSnapshotArrives()
    {
        var cache = new MarkerCache();
        Assert.Equal(RagfairRestriction.Unknown, cache.Snapshot.GetRestriction(ItemId));
        Assert.False(cache.Snapshot.SameAs(MarkerSnapshot.Empty));
        cache.Apply(new() { [ItemId] = TraderId });
        Assert.Equal(RagfairRestriction.Unknown, cache.Snapshot.GetRestriction(ItemId));
        Assert.True(cache.Replace(0, MarkerSnapshot.Parse(Names, "{}", Restricted)));
        Assert.Equal(RagfairRestriction.Restricted, cache.Snapshot.GetRestriction(ItemId));
        Assert.Equal(RagfairRestriction.Allowed, cache.Snapshot.GetRestriction("cccccccccccccccccccccccc"));
    }

    [Fact]
    public void FirstValidEmptySnapshotStillNotifiesMenusThatLoadingFinished()
    {
        var cache = new MarkerCache();
        Assert.True(cache.Replace(0, MarkerSnapshot.Empty));
        Assert.Equal(RagfairRestriction.Allowed, cache.Snapshot.GetRestriction(ItemId));
        Assert.False(cache.Replace(0, MarkerSnapshot.Empty));
    }

    [Fact]
    public void ConcurrentPurchaseDoesNotDiscardNamesRestrictionsOrOtherItems()
    {
        const string otherItem = "cccccccccccccccccccccccc";
        const string otherTrader = "dddddddddddddddddddddddd";
        var cache = new MarkerCache();
        cache.Replace(0, MarkerSnapshot.Parse(Names, "{}", "[]"));
        var revision = cache.Revision;
        cache.Apply(new() { [ItemId] = TraderId });
        var loaded = MarkerSnapshot.Parse(
            $"{{\"{TraderId}\":\"New name\",\"{otherTrader}\":\"Other trader\"}}",
            $"{{\"{otherItem}\":\"{otherTrader}\"}}", Restricted);
        Assert.True(cache.Replace(revision, loaded));
        Assert.True(cache.Snapshot.TryGetTraderName(ItemId, out var name));
        Assert.Equal("New name", name);
        Assert.True(cache.Snapshot.IsRestricted(ItemId));
        Assert.True(cache.Snapshot.TryGetTraderName(otherItem, out name));
        Assert.Equal("Other trader", name);
        Assert.False(cache.Snapshot.IsRestricted(otherItem));
    }

    [Fact]
    public void OnlyPurchasesAfterFetchStartOverrideAuthoritativeUpdates()
    {
        const string otherItem = "cccccccccccccccccccccccc";
        const string otherTrader = "dddddddddddddddddddddddd";
        var cache = new MarkerCache();
        cache.Apply(new() { [ItemId] = TraderId, [otherItem] = TraderId });
        var revision = cache.Revision;
        cache.Apply(new() { [ItemId] = otherTrader });
        cache.Replace(revision, MarkerSnapshot.Parse("{}", Markers, Restricted));
        Assert.True(cache.Snapshot.TryGetTraderName(ItemId, out var name));
        Assert.Equal(otherTrader, name);
        Assert.False(cache.Snapshot.IsRestricted(ItemId));
        Assert.False(cache.Snapshot.TryGetTraderName(otherItem, out _));
        cache.Replace(cache.Revision, MarkerSnapshot.Empty);
        Assert.False(cache.Snapshot.TryGetTraderName(ItemId, out _));
    }

    [Fact]
    public void RepeatedPurchasesCannotStarveRefreshOfOtherData()
    {
        var cache = new MarkerCache();
        for (var i = 0; i < 20; i++)
        {
            var revision = cache.Revision;
            cache.Apply(new() { [ItemId] = TraderId });
            var name = $"Trader {i}";
            cache.Replace(revision, MarkerSnapshot.Parse($"{{\"{TraderId}\":\"{name}\"}}", "{}", Restricted));
            Assert.True(cache.Snapshot.TryGetTraderName(ItemId, out var actual));
            Assert.Equal(name, actual);
            Assert.True(cache.Snapshot.IsRestricted(ItemId));
        }
    }

    [Fact]
    public void InvalidRefreshPreservesPurchasesUntilRecovery()
    {
        var cache = new MarkerCache();
        cache.Replace(0, MarkerSnapshot.Parse(Names, "{}", Restricted));
        var revision = cache.Revision;
        cache.Apply(new() { [ItemId] = TraderId });
        Assert.ThrowsAny<JsonException>(() => cache.Replace(revision, MarkerSnapshot.Parse("<html>offline</html>", "{}", "[]")));
        Assert.True(cache.Snapshot.IsRestricted(ItemId));
        cache.Replace(revision, MarkerSnapshot.Parse(Names, "{}", Restricted));
        Assert.True(cache.Snapshot.IsRestricted(ItemId));
        cache.Replace(cache.Revision, MarkerSnapshot.Empty);
        Assert.Equal(RagfairRestriction.Allowed, cache.Snapshot.GetRestriction(ItemId));
    }

    [Fact]
    public void ThrottledRequestRunsLaterWithoutAnotherRequest()
    {
        var schedule = new RefreshSchedule(5);
        schedule.Request();
        Assert.True(schedule.TryStart(0));
        schedule.Complete(true);
        schedule.Request();
        Assert.False(schedule.TryStart(1));
        Assert.False(schedule.TryStart(4.99));
        Assert.True(schedule.TryStart(5));
        schedule.Complete(true);
        Assert.False(schedule.TryStart(10));
    }

    [Fact]
    public void RequestsDuringFlightCoalesceAndNeverStartConcurrently()
    {
        var schedule = new RefreshSchedule(5);
        schedule.Request();
        Assert.True(schedule.TryStart(0));
        for (var i = 0; i < 100; i++) schedule.Request();
        Assert.False(schedule.TryStart(10));
        schedule.Complete(true);
        Assert.True(schedule.TryStart(10));
        schedule.Complete(true);
        Assert.False(schedule.TryStart(20));
    }

    [Fact]
    public void FailureRetriesAtLimitAndStopsAfterRecovery()
    {
        var schedule = new RefreshSchedule(5);
        schedule.Request();
        Assert.True(schedule.TryStart(0));
        schedule.Complete(false);
        Assert.False(schedule.TryStart(1));
        Assert.True(schedule.TryStart(5));
        schedule.Complete(true);
        Assert.False(schedule.TryStart(10));
    }

    [Fact]
    public void StopRejectsNewRequestsAndLateCompletion()
    {
        var schedule = new RefreshSchedule(5);
        schedule.Request();
        Assert.True(schedule.TryStart(0));
        schedule.Stop();
        schedule.Request();
        schedule.Complete(false);
        Assert.False(schedule.TryStart(100));
    }

    [Fact]
    public void SnapshotSupportsUnicodeEscapesAndCaseInsensitiveIds()
    {
        var snapshot = MarkerSnapshot.Parse(Names, Markers, Restricted);
        Assert.True(snapshot.TryGetTraderName(ItemId.ToUpperInvariant(), out var name));
        Assert.Equal("商人\"名字", name);
        Assert.True(snapshot.IsRestricted(ItemId));
        Assert.False(snapshot.IsRestricted(TraderId));
    }

    [Theory]
    [InlineData("<html>error</html>")]
    [InlineData("{\"error\":\"offline\"}")]
    [InlineData("[]")]
    [InlineData("null")]
    [InlineData("{\"bbbbbbbbbbbbbbbbbbbbbbbb\":null}")]
    [InlineData("{\"bbbbbbbbbbbbbbbbbbbbbbbb\":123}")]
    [InlineData("{\"bbbbbbbbbbbbbbbbbbbbbbbb\":{\"name\":\"trader\"}}")]
    [InlineData("{\"bbbbbbbbbbbbbbbbbbbbbbbb\":\"a\",\"bbbbbbbbbbbbbbbbbbbbbbbb\":\"b\"}")]
    public void MalformedNamesCannotReplaceLastGoodSnapshot(string json)
    {
        var snapshot = MarkerSnapshot.Parse(Names, Markers, Restricted);
        Assert.ThrowsAny<JsonException>(() => snapshot = MarkerSnapshot.Parse(json, Markers, Restricted));
        Assert.True(snapshot.IsRestricted(ItemId));
    }

    [Theory]
    [InlineData("{}")]
    [InlineData("[null]")]
    [InlineData("[123]")]
    [InlineData("[\"error\"]")]
    [InlineData("{\"error\":\"offline\"}")]
    public void InvalidThirdResponseDoesNotPublishPartialSnapshot(string json)
    {
        var snapshot = MarkerSnapshot.Parse(Names, Markers, Restricted);
        Assert.ThrowsAny<JsonException>(() => snapshot = MarkerSnapshot.Parse("{}", "{}", json));
        Assert.True(snapshot.IsRestricted(ItemId));
    }

    [Fact]
    public void ValidEmptyResponseClearsSnapshotAndEqualitySuppressesRedraw()
    {
        var snapshot = MarkerSnapshot.Parse(Names, Markers, Restricted);
        Assert.True(snapshot.SameAs(MarkerSnapshot.Parse(Names, Markers, Restricted)));
        Assert.False(snapshot.SameAs(MarkerSnapshot.Empty));
        snapshot = MarkerSnapshot.Parse("{}", "{}", "[]");
        Assert.True(snapshot.SameAs(MarkerSnapshot.Empty));
        Assert.False(snapshot.TryGetTraderName(ItemId, out _));
        Assert.False(snapshot.IsRestricted(ItemId));
    }

    [Fact]
    public void MarkerWithoutTraderNameFallsBackToTraderId()
    {
        var snapshot = MarkerSnapshot.Parse("{}", Markers, Restricted);
        Assert.True(snapshot.TryGetTraderName(ItemId, out var name));
        Assert.Equal(TraderId, name);
    }

    [Theory]
    [InlineData(TradeMarkerLanguageMode.English, "English", 1)]
    [InlineData(TradeMarkerLanguageMode.Chinese, "简体中文", 2)]
    [InlineData(TradeMarkerLanguageMode.Czech, "Čeština", 3)]
    [InlineData(TradeMarkerLanguageMode.French, "Français", 4)]
    [InlineData(TradeMarkerLanguageMode.German, "Deutsch", 5)]
    [InlineData(TradeMarkerLanguageMode.Hungarian, "Magyar", 6)]
    [InlineData(TradeMarkerLanguageMode.Italian, "Italiano", 7)]
    [InlineData(TradeMarkerLanguageMode.Japanese, "日本語", 8)]
    [InlineData(TradeMarkerLanguageMode.Korean, "한국어", 9)]
    [InlineData(TradeMarkerLanguageMode.Polish, "Polski", 10)]
    [InlineData(TradeMarkerLanguageMode.Portuguese, "Português", 11)]
    [InlineData(TradeMarkerLanguageMode.Slovak, "Slovenčina", 12)]
    [InlineData(TradeMarkerLanguageMode.Spanish, "Español", 13)]
    [InlineData(TradeMarkerLanguageMode.SpanishMexico, "Español (México)", 14)]
    [InlineData(TradeMarkerLanguageMode.Turkish, "Türkçe", 15)]
    [InlineData(TradeMarkerLanguageMode.Russian, "Русский", 16)]
    [InlineData(TradeMarkerLanguageMode.Romanian, "Română", 17)]
    public void LanguageAutonymsPreserveSerializedValues(TradeMarkerLanguageMode mode, string name, int value)
    {
        var field = typeof(TradeMarkerLanguageMode).GetField(mode.ToString())!;
        var description = Assert.Single(field.GetCustomAttributes(typeof(DescriptionAttribute), false).Cast<DescriptionAttribute>());
        Assert.Equal(name, description.Description);
        Assert.Equal(value, (int)mode);
        Assert.Equal(mode, Enum.Parse<TradeMarkerLanguageMode>(mode.ToString()));
    }

    [Fact]
    public void AutoDescriptionCanChangeWithoutChangingTheEnum()
    {
        var old = AutoLanguageDescriptionAttribute.DisplayText;
        try
        {
            var attribute = new AutoLanguageDescriptionAttribute();
            AutoLanguageDescriptionAttribute.DisplayText = "自动";
            Assert.Equal("自动", attribute.Description);
            AutoLanguageDescriptionAttribute.DisplayText = "Auto";
            Assert.Equal("Auto", attribute.Description);
            Assert.Equal(0, (int)TradeMarkerLanguageMode.Auto);
        }
        finally { AutoLanguageDescriptionAttribute.DisplayText = old; }
    }
}
