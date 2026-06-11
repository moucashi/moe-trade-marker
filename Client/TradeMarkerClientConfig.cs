#if SPT_CLIENT
using BepInEx.Configuration;
using MoeTradeMarker.Shared;
using UnityEngine;

namespace MoeTradeMarker.Client;

internal static class TradeMarkerClientConfig
{
    private const string GeneralSection = "通用";
    private const string DisplaySection = "显示";

    private static ConfigEntry<bool>? showTraderMarker;
    private static ConfigEntry<MarkerPosition>? markerPosition;
    private static ConfigEntry<string>? markerColor;

    public static bool ShowTraderMarker => showTraderMarker?.Value ?? true;

    public static MarkerPosition MarkerPosition => markerPosition?.Value ?? MarkerPosition.LeftBottom;

    public static Color MarkerColor
    {
        get
        {
            var value = markerColor?.Value;
            return ColorUtility.TryParseHtmlString(value, out var color)
                ? color
                : new Color(0.46f, 0.96f, 1f, 0.95f);
        }
    }

    public static void Bind(ConfigFile config)
    {
        showTraderMarker = config.Bind(
            GeneralSection,
            "ShowTraderMarker",
            true,
            "是否在带有商人标记的物品图标上显示角标。");

        markerPosition = config.Bind(
            DisplaySection,
            "MarkerPosition",
            MarkerPosition.LeftBottom,
            "商人标记角标位置，可选 LeftTop、RightTop、LeftBottom、RightBottom。");

        markerColor = config.Bind(
            DisplaySection,
            "MarkerColor",
            "#75F4FFFF",
            "商人标记图标颜色，支持 HTML 十六进制颜色，例如 #75F4FFFF。");
    }
}
#endif
