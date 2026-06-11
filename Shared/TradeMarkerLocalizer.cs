using System;
using System.Collections.Generic;
using System.Globalization;

namespace MoeTradeMarker.Shared;

public static class TradeMarkerLocalizer
{
    private static readonly Dictionary<TradeMarkerText, string> English = new()
    {
        [TradeMarkerText.ClientLoaded] = "Moe-TradeMarker client loaded.",
        [TradeMarkerText.ClientInitFailed] = "Moe-TradeMarker client initialization failed: {0}",
        [TradeMarkerText.ConfigGeneralSection] = "General",
        [TradeMarkerText.ConfigDisplaySection] = "Display",
        [TradeMarkerText.ConfigShowTraderMarkerDescription] = "Show the trader marker on items that have a trader marker.",
        [TradeMarkerText.ConfigMarkerPositionDescription] = "Trader marker icon position. Available values: LeftTop, RightTop, LeftBottom, RightBottom.",
        [TradeMarkerText.ConfigMarkerColorDescription] = "Trader marker icon color.",
        [TradeMarkerText.ConfigLanguageModeDescription] = "Language used by Moe-TradeMarker. Auto follows the detected game language.",
        [TradeMarkerText.TooltipTraderMarker] = "Trader marker: {0}",
        [TradeMarkerText.ClientItemMarkerMissing] = "Moe-TradeMarker did not find a trader marker for item {0}.",
        [TradeMarkerText.ClientMarkerRefreshFailed] = "Moe-TradeMarker client marker data refresh failed: {0}",
        [TradeMarkerText.ClientItemViewTypeMissing] = "Moe-TradeMarker could not find EFT.UI.DragAndDrop.ItemView; item marker patch will not be installed.",
        [TradeMarkerText.ClientSetQuestItemViewPanelMissing] = "Moe-TradeMarker could not find ItemView.SetQuestItemViewPanel(); item marker patch will not be installed.",
        [TradeMarkerText.ClientItemViewPatchInstalled] = "Moe-TradeMarker installed the ItemView.SetQuestItemViewPanel item marker patch.",
        [TradeMarkerText.ClientLanguageSynced] = "Moe-TradeMarker synced client language to server: {0}.",
        [TradeMarkerText.ClientLanguageSyncFailed] = "Moe-TradeMarker client language sync failed: {0}",
        [TradeMarkerText.ClientItemReadFailed] = "Moe-TradeMarker could not read the item object from ItemView.",
        [TradeMarkerText.ClientMainImageReadFailed] = "Moe-TradeMarker could not read MainImage for item {0}.",
        [TradeMarkerText.ClientItemViewComponentReadFailed] = "Moe-TradeMarker could not convert ItemView for item {0} to a Unity component.",
        [TradeMarkerText.ServerLoaded] = "Moe-TradeMarker server loaded.",
        [TradeMarkerText.ServerConfigLoaded] = "Moe-TradeMarker config loaded.",
        [TradeMarkerText.ServerConfigLoadFailed] = "Moe-TradeMarker config load failed; default config is being used: {0}",
        [TradeMarkerText.RagfairMarkedItemBlocked] = "This item has a trader marker ({0}) and cannot be listed on the flea market.",
    };

    private static readonly Dictionary<TradeMarkerText, string> Chinese = new()
    {
        [TradeMarkerText.ClientLoaded] = "Moe-TradeMarker 客户端已加载。",
        [TradeMarkerText.ClientInitFailed] = "Moe-TradeMarker 客户端初始化失败：{0}",
        [TradeMarkerText.ConfigGeneralSection] = "通用",
        [TradeMarkerText.ConfigDisplaySection] = "显示",
        [TradeMarkerText.ConfigShowTraderMarkerDescription] = "是否在带有商人标记的物品图标上显示角标。",
        [TradeMarkerText.ConfigMarkerPositionDescription] = "商人标记角标位置，可选 LeftTop、RightTop、LeftBottom、RightBottom。",
        [TradeMarkerText.ConfigMarkerColorDescription] = "商人标记图标颜色。",
        [TradeMarkerText.ConfigLanguageModeDescription] = "Moe-TradeMarker 使用的语言。Auto 会跟随检测到的游戏语言。",
        [TradeMarkerText.TooltipTraderMarker] = "商人标记：{0}",
        [TradeMarkerText.ClientItemMarkerMissing] = "Moe-TradeMarker 未找到物品 {0} 的商人标记。",
        [TradeMarkerText.ClientMarkerRefreshFailed] = "Moe-TradeMarker 客户端标记数据刷新失败：{0}",
        [TradeMarkerText.ClientItemViewTypeMissing] = "Moe-TradeMarker 未找到 EFT.UI.DragAndDrop.ItemView，无法安装物品角标补丁。",
        [TradeMarkerText.ClientSetQuestItemViewPanelMissing] = "Moe-TradeMarker 未找到 ItemView.SetQuestItemViewPanel()，无法安装物品角标补丁。",
        [TradeMarkerText.ClientItemViewPatchInstalled] = "Moe-TradeMarker 已安装 ItemView.SetQuestItemViewPanel 物品角标补丁。",
        [TradeMarkerText.ClientLanguageSynced] = "Moe-TradeMarker 已将客户端语言同步到服务端：{0}。",
        [TradeMarkerText.ClientLanguageSyncFailed] = "Moe-TradeMarker 客户端语言同步失败：{0}",
        [TradeMarkerText.ClientItemReadFailed] = "Moe-TradeMarker 未能从 ItemView 读取物品对象。",
        [TradeMarkerText.ClientMainImageReadFailed] = "Moe-TradeMarker 未能从 ItemView 读取物品 {0} 的 MainImage。",
        [TradeMarkerText.ClientItemViewComponentReadFailed] = "Moe-TradeMarker 未能将物品 {0} 的 ItemView 转换为 Unity 组件。",
        [TradeMarkerText.ServerLoaded] = "Moe-TradeMarker 服务端已加载。",
        [TradeMarkerText.ServerConfigLoaded] = "Moe-TradeMarker 配置已加载。",
        [TradeMarkerText.ServerConfigLoadFailed] = "Moe-TradeMarker 配置读取失败，已使用默认配置：{0}",
        [TradeMarkerText.RagfairMarkedItemBlocked] = "物品带有商人标记（{0}），无法上架跳蚤市场。",
    };

    public static TradeMarkerLanguage DetectLanguage(string? language)
    {
        if (string.IsNullOrWhiteSpace(language))
        {
            return TradeMarkerLanguage.English;
        }

        var normalized = (language ?? string.Empty).Trim().Replace('_', '-').ToLowerInvariant();
        return normalized is "ch" or "chs" or "cht" or "cn" or "zh" or "zh-cn" or "zh-hans" or "zh-hant" or "chinese" or "chinesesimplified" or "chinesetraditional"
            || normalized.StartsWith("zh-", StringComparison.Ordinal)
            || normalized.StartsWith("cn-", StringComparison.Ordinal)
            || normalized.StartsWith("ch-", StringComparison.Ordinal)
            || normalized.StartsWith("chinese", StringComparison.Ordinal)
            ? TradeMarkerLanguage.Chinese
            : TradeMarkerLanguage.English;
    }

    public static string Text(TradeMarkerText key, TradeMarkerLanguage language)
    {
        var table = language == TradeMarkerLanguage.Chinese ? Chinese : English;
        return table.TryGetValue(key, out var value)
            ? value
            : English.TryGetValue(key, out var fallback) ? fallback : key.ToString();
    }

    public static string Format(TradeMarkerText key, TradeMarkerLanguage language, params object?[] args)
    {
        return string.Format(CultureInfo.InvariantCulture, Text(key, language), args);
    }
}
