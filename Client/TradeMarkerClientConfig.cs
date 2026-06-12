#if SPT_CLIENT
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

    public static MarkerPosition MarkerPosition => markerPosition?.Value ?? MarkerPosition.LeftTop;

    public static Color MarkerColor => markerColor?.Value ?? new Color(0.46f, 0.96f, 1f, 0.95f);

    public static TradeMarkerLanguageMode LanguageMode => languageMode?.Value ?? TradeMarkerLanguageMode.Auto;

    public static void Bind(ConfigFile config)
    {
        showTraderMarker = config.Bind(
            TradeMarkerLocalization.Text(TradeMarkerText.ConfigGeneralSection),
            "ShowTraderMarker",
            true,
            TradeMarkerLocalization.Text(TradeMarkerText.ConfigShowTraderMarkerDescription));
        showTraderMarker.SettingChanged += (_, _) => ItemViewTradeMarkerPatch.RefreshTrackedItemViews();

        markerPosition = config.Bind(
            TradeMarkerLocalization.Text(TradeMarkerText.ConfigDisplaySection),
            "MarkerPosition",
            MarkerPosition.LeftTop,
            TradeMarkerLocalization.Text(TradeMarkerText.ConfigMarkerPositionDescription));
        markerPosition.SettingChanged += (_, _) => TradeMarkerOverlay.ApplyCurrentConfigToVisibleMarkers();

        markerColor = config.Bind(
            TradeMarkerLocalization.Text(TradeMarkerText.ConfigDisplaySection),
            "MarkerColor",
            new Color(0.46f, 0.96f, 1f, 0.95f),
            TradeMarkerLocalization.Text(TradeMarkerText.ConfigMarkerColorDescription));
        markerColor.SettingChanged += (_, _) => TradeMarkerOverlay.ApplyCurrentConfigToVisibleMarkers();

        languageMode = config.Bind(
            TradeMarkerLocalization.Text(TradeMarkerText.ConfigGeneralSection),
            "LanguageMode",
            TradeMarkerLanguageMode.Auto,
            TradeMarkerLocalization.Text(TradeMarkerText.ConfigLanguageModeDescription));
        languageMode.SettingChanged += (_, _) => TradeMarkerDataLoader.SyncLanguage();
    }
}
#endif
