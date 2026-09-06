#if SPT_CLIENT
using System.ComponentModel;
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

    public static bool ShowTraderMarker => showTraderMarker?.Value ?? true;

    public static MarkerPosition MarkerPosition => markerPosition?.Value ?? MarkerPosition.LeftBottom;

    public static Color MarkerColor => markerColor?.Value ?? new Color(1f, 0.5f, 0.5f, 1f);

    public static TradeMarkerLanguageMode LanguageMode => languageMode?.Value ?? TradeMarkerLanguageMode.Auto;

    public static void Bind(ConfigFile config)
    {
        showTraderMarker = config.Bind(
            "General",
            "ShowTraderMarker",
            true,
            CreateDescription(
                TradeMarkerText.ConfigShowTraderMarkerName,
                TradeMarkerText.ConfigGeneralSection,
                TradeMarkerText.ConfigShowTraderMarkerDescription));
        showTraderMarker.SettingChanged += (_, _) => ItemViewTradeMarkerPatch.RefreshTrackedItemViews();

        markerPosition = config.Bind(
            "Display",
            "MarkerPosition",
            MarkerPosition.LeftBottom,
            CreateDescription(
                TradeMarkerText.ConfigMarkerPositionName,
                TradeMarkerText.ConfigDisplaySection,
                TradeMarkerText.ConfigMarkerPositionDescription));
        markerPosition.SettingChanged += (_, _) => TradeMarkerOverlay.ApplyCurrentConfigToVisibleMarkers();

        markerColor = config.Bind(
            "Display",
            "MarkerColor",
            new Color(1f, 0.5f, 0.5f, 1f),
            CreateDescription(
                TradeMarkerText.ConfigMarkerColorName,
                TradeMarkerText.ConfigDisplaySection,
                TradeMarkerText.ConfigMarkerColorDescription));
        markerColor.SettingChanged += (_, _) => TradeMarkerOverlay.ApplyCurrentConfigToVisibleMarkers();

        languageMode = config.Bind(
            "General",
            "LanguageMode",
            TradeMarkerLanguageMode.Auto,
            CreateDescription(
                TradeMarkerText.ConfigLanguageModeName,
                TradeMarkerText.ConfigGeneralSection,
                TradeMarkerText.ConfigLanguageModeDescription));
        languageMode.SettingChanged += (_, _) => TradeMarkerDataLoader.SyncLanguage();
    }

    private static ConfigDescription CreateDescription(
        TradeMarkerText name,
        TradeMarkerText category,
        TradeMarkerText description)
    {
        return new ConfigDescription(
            TradeMarkerLocalization.Text(description),
            null,
            new DisplayNameAttribute(TradeMarkerLocalization.Text(name)),
            new CategoryAttribute(TradeMarkerLocalization.Text(category)));
    }
}
#endif
