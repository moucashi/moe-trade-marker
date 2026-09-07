#if SPT_CLIENT
using System.Diagnostics;
using System.Reflection;
using BepInEx.Bootstrap;
using BepInEx.Configuration;
using MoeTradeMarker.Client.Patches;
using MoeTradeMarker.Shared;
using UnityEngine;

namespace MoeTradeMarker.Client;

internal static class TradeMarkerClientConfig
{
    private static ConfigEntry<bool>? showTraderMarker;
    private static ConfigEntry<MarkerPosition>? markerPosition;
    private static ConfigEntry<Color>? markerColor;
    private static ConfigEntry<TradeMarkerLanguageMode>? languageMode;
    private static readonly List<ConfigurationManagerAttributes> Metadata = new();
    private static readonly Stopwatch Clock = Stopwatch.StartNew();
    private static double nextLanguageCheck;
    private static bool languageChanged;
    private static bool rebuildPending;
    private static object? manager;
    private static MethodInfo? rebuildSettings;

    public static bool ShowTraderMarker => showTraderMarker?.Value ?? true;
    public static MarkerPosition MarkerPosition => markerPosition?.Value ?? MarkerPosition.LeftBottom;
    public static Color MarkerColor => markerColor?.Value ?? new Color(1f, 0.5f, 0.5f, 1f);
    public static TradeMarkerLanguageMode LanguageMode => languageMode?.Value ?? TradeMarkerLanguageMode.Auto;

    public static void Bind(ConfigFile config)
    {
        Metadata.Clear();
        languageMode = config.Bind("General", "LanguageMode", TradeMarkerLanguageMode.Auto,
            CreateDescription(TradeMarkerText.ConfigLanguageModeName, TradeMarkerText.ConfigGeneralSection,
                TradeMarkerText.ConfigLanguageModeDescription));
        TradeMarkerLocalization.Refresh();

        showTraderMarker = config.Bind("General", "ShowTraderMarker", true,
            CreateDescription(TradeMarkerText.ConfigShowTraderMarkerName, TradeMarkerText.ConfigGeneralSection,
                TradeMarkerText.ConfigShowTraderMarkerDescription));
        showTraderMarker.SettingChanged += (_, _) =>
        {
            TradeMarkerDataLoader.RequestRefresh();
            ItemViewTradeMarkerPatch.RefreshTrackedItemViews();
        };
        markerPosition = config.Bind("Display", "MarkerPosition", MarkerPosition.LeftBottom,
            CreateDescription(TradeMarkerText.ConfigMarkerPositionName, TradeMarkerText.ConfigDisplaySection,
                TradeMarkerText.ConfigMarkerPositionDescription));
        markerPosition.SettingChanged += (_, _) => TradeMarkerOverlay.ApplyCurrentConfigToVisibleMarkers();
        markerColor = config.Bind("Display", "MarkerColor", new Color(1f, 0.5f, 0.5f, 1f),
            CreateDescription(TradeMarkerText.ConfigMarkerColorName, TradeMarkerText.ConfigDisplaySection,
                TradeMarkerText.ConfigMarkerColorDescription));
        markerColor.SettingChanged += (_, _) => TradeMarkerOverlay.ApplyCurrentConfigToVisibleMarkers();
        languageMode.SettingChanged += (_, _) => languageChanged = true;
        UpdateMetadata();
        rebuildPending = true;
    }

    public static void Tick()
    {
        // Rebuild on Update, never from ConfigurationManager's OnGUI enumeration.
        if (rebuildPending)
        {
            rebuildPending = false;
            RebuildSettings();
        }
        if (!languageChanged && Clock.Elapsed.TotalSeconds < nextLanguageCheck) return;
        nextLanguageCheck = Clock.Elapsed.TotalSeconds + 1;
        var previous = TradeMarkerLocalization.Language;
        if (languageChanged || LanguageMode == TradeMarkerLanguageMode.Auto) TradeMarkerLocalization.Refresh();
        if (languageChanged || previous != TradeMarkerLocalization.Language)
        {
            languageChanged = false;
            UpdateMetadata();
            rebuildPending = true;
            TradeMarkerDataLoader.QueueLanguage(TradeMarkerLocalization.LanguageCode);
            TradeMarkerTooltipContext.RefreshVisibleTooltips();
        }
    }

    private static void UpdateMetadata()
    {
        foreach (var metadata in Metadata) metadata.Refresh();
        AutoLanguageDescriptionAttribute.DisplayText = TradeMarkerLocalization.Language switch
        {
            TradeMarkerLanguage.Chinese => "自动",
            TradeMarkerLanguage.Czech => "Automaticky",
            TradeMarkerLanguage.French => "Automatique",
            TradeMarkerLanguage.German => "Automatisch",
            TradeMarkerLanguage.Hungarian => "Automatikus",
            TradeMarkerLanguage.Italian => "Automatico",
            TradeMarkerLanguage.Japanese => "自動",
            TradeMarkerLanguage.Korean => "자동",
            TradeMarkerLanguage.Polish => "Automatycznie",
            TradeMarkerLanguage.Portuguese => "Automático",
            TradeMarkerLanguage.Slovak => "Automaticky",
            TradeMarkerLanguage.Spanish or TradeMarkerLanguage.SpanishMexico => "Automático",
            TradeMarkerLanguage.Turkish => "Otomatik",
            TradeMarkerLanguage.Russian => "Автоматически",
            TradeMarkerLanguage.Romanian => "Automat",
            _ => "Auto"
        };
    }

    private static void RebuildSettings()
    {
        if (!Chainloader.PluginInfos.TryGetValue("com.bepis.bepinex.configurationmanager", out var plugin)) return;
        if (!ReferenceEquals(manager, plugin.Instance))
        {
            manager = plugin.Instance;
            rebuildSettings = manager.GetType().GetMethod("BuildSettingList", Type.EmptyTypes);
        }
        try { rebuildSettings?.Invoke(manager, null); }
        catch (Exception exception) { Plugin.Log.LogWarning($"Could not refresh configuration labels: {exception.Message}"); }
    }

    private static ConfigDescription CreateDescription(TradeMarkerText name, TradeMarkerText category, TradeMarkerText description)
    {
        var metadata = new ConfigurationManagerAttributes(name, category, description);
        Metadata.Add(metadata);
        return new ConfigDescription(TradeMarkerLocalization.Text(description), null, metadata);
    }

    // ConfigurationManager's supported tag contract uses these public fields by name.
    private sealed class ConfigurationManagerAttributes
    {
        private readonly TradeMarkerText nameKey;
        private readonly TradeMarkerText categoryKey;
        private readonly TradeMarkerText descriptionKey;
        public string DispName = string.Empty;
        public string Category = string.Empty;
        public string Description = string.Empty;

        public ConfigurationManagerAttributes(TradeMarkerText name, TradeMarkerText category, TradeMarkerText description)
        {
            nameKey = name;
            categoryKey = category;
            descriptionKey = description;
            Refresh();
        }

        public void Refresh()
        {
            DispName = TradeMarkerLocalization.Text(nameKey);
            Category = TradeMarkerLocalization.Text(categoryKey);
            Description = TradeMarkerLocalization.Text(descriptionKey);
        }
    }
}
#endif
