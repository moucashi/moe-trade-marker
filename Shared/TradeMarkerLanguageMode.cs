using System.ComponentModel;

namespace MoeTradeMarker.Shared;

public enum TradeMarkerLanguageMode
{
    [AutoLanguageDescription]
    Auto,
    [Description("English")]
    English,
    [Description("简体中文")]
    Chinese,
    [Description("Čeština")]
    Czech,
    [Description("Français")]
    French,
    [Description("Deutsch")]
    German,
    [Description("Magyar")]
    Hungarian,
    [Description("Italiano")]
    Italian,
    [Description("日本語")]
    Japanese,
    [Description("한국어")]
    Korean,
    [Description("Polski")]
    Polish,
    [Description("Português")]
    Portuguese,
    [Description("Slovenčina")]
    Slovak,
    [Description("Español")]
    Spanish,
    [Description("Español (México)")]
    SpanishMexico,
    [Description("Türkçe")]
    Turkish,
    [Description("Русский")]
    Russian,
    [Description("Română")]
    Romanian,
}

public sealed class AutoLanguageDescriptionAttribute : DescriptionAttribute
{
    public static string DisplayText { get; set; } = "Auto";
    public override string Description => DisplayText;
}
