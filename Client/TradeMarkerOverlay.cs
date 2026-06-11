#if SPT_CLIENT
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MoeTradeMarker.Shared;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MoeTradeMarker.Client;

internal static class TradeMarkerOverlay
{
    private static readonly List<WeakReference<Image>> ActiveMarkers = [];
    private static Sprite? iconSprite;

    public static void ShowOnItemView(Component itemView, string overlayName, string traderName, object? tooltip, MarkerPosition position, Color color)
    {
        var overlay = GetOrCreateOverlay(itemView, overlayName);
        overlay.color = color;
        iconSprite ??= CreateMarkerSprite();
        overlay.sprite = iconSprite;
        overlay.raycastTarget = true;

        Position(overlay.rectTransform, position);
        TrackMarker(overlay);

        var trigger = overlay.GetComponent<TradeMarkerTooltipTrigger>()
            ?? overlay.gameObject.AddComponent<TradeMarkerTooltipTrigger>();
        trigger.Tooltip = tooltip;
        trigger.Text = TradeMarkerLocalization.Format(TradeMarkerText.TooltipTraderMarker, traderName);

        overlay.gameObject.SetActive(true);
    }

    public static void HideFromItemView(object itemView, string overlayName)
    {
        var child = (itemView as Component)?.transform.Find(overlayName);
        if (child is not null)
        {
            child.gameObject.SetActive(false);
        }
    }

    public static void ApplyCurrentConfigToVisibleMarkers()
    {
        ActiveMarkers.RemoveAll(reference => !reference.TryGetTarget(out var image) || image is null);
        foreach (var reference in ActiveMarkers)
        {
            if (!reference.TryGetTarget(out var marker) || marker is null)
            {
                continue;
            }

            marker.color = TradeMarkerClientConfig.MarkerColor;
            Position(marker.rectTransform, TradeMarkerClientConfig.MarkerPosition);
        }
    }

    public static void Hide(object instance, string overlayName)
    {
        var component = instance as Component;
        var child = component?.transform.Find(overlayName);
        if (child is not null)
        {
            child.gameObject.SetActive(false);
        }
    }

    public static Image? FindIconImage(object instance)
    {
        foreach (var fieldName in new[] { "_questIconImage", "questIconImage", "QuestIconImage", "____questIconImage" })
        {
            var field = AccessField(instance.GetType(), fieldName);
            if (field?.GetValue(instance) is Image image)
            {
                return image;
            }
        }

        return (instance as Component)?.GetComponentsInChildren<Image>(true)
            .FirstOrDefault(image => image.gameObject.name != "MoeTradeMarkerIcon");
    }

    public static object? FindTooltip(object instance, object[] args)
    {
        var fromArgs = args.FirstOrDefault(arg => Contains(arg?.GetType().Name, "Tooltip", StringComparison.OrdinalIgnoreCase));
        if (fromArgs is not null)
        {
            return fromArgs;
        }

        var type = instance.GetType();
        foreach (var field in type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
        {
            if (Contains(field.FieldType.Name, "Tooltip", StringComparison.OrdinalIgnoreCase))
            {
                return field.GetValue(instance);
            }
        }

        return null;
    }

    private static Image GetOrCreateOverlay(Component parent, string overlayName)
    {
        var existing = parent.transform.Find(overlayName)?.GetComponent<Image>();
        if (existing is not null)
        {
            return existing;
        }

        var gameObject = new GameObject(overlayName, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        gameObject.transform.SetParent(parent.transform, worldPositionStays: false);
        gameObject.transform.SetAsLastSibling();
        return gameObject.GetComponent<Image>();
    }

    private static void TrackMarker(Image marker)
    {
        if (ActiveMarkers.Any(reference => reference.TryGetTarget(out var image) && image == marker))
        {
            return;
        }

        ActiveMarkers.Add(new WeakReference<Image>(marker));
    }

    private static void Position(RectTransform rectTransform, MarkerPosition position)
    {
        const float size = 17f;
        const float inset = 2f;

        rectTransform.sizeDelta = new Vector2(size, size);
        rectTransform.localScale = Vector3.one;
        rectTransform.localRotation = Quaternion.identity;

        switch (position)
        {
            case MarkerPosition.LeftTop:
                rectTransform.anchorMin = rectTransform.anchorMax = rectTransform.pivot = new Vector2(0f, 1f);
                rectTransform.anchoredPosition = new Vector2(inset, -inset);
                break;
            case MarkerPosition.RightTop:
                rectTransform.anchorMin = rectTransform.anchorMax = rectTransform.pivot = new Vector2(1f, 1f);
                rectTransform.anchoredPosition = new Vector2(-inset, -inset);
                break;
            case MarkerPosition.RightBottom:
                rectTransform.anchorMin = rectTransform.anchorMax = rectTransform.pivot = new Vector2(1f, 0f);
                rectTransform.anchoredPosition = new Vector2(-inset, inset);
                break;
            default:
                rectTransform.anchorMin = rectTransform.anchorMax = rectTransform.pivot = new Vector2(0f, 0f);
                rectTransform.anchoredPosition = new Vector2(inset, inset);
                break;
        }
    }

    private static Sprite CreateMarkerSprite()
    {
        const int size = 32;
        var texture = new Texture2D(size, size, TextureFormat.ARGB32, false);
        var transparent = new Color(1f, 1f, 1f, 0f);
        var solid = Color.white;

        for (var y = 0; y < size; y++)
        {
            for (var x = 0; x < size; x++)
            {
                var body = x >= 5 && x <= 26 && y >= 7 && y <= 24;
                var point = x >= 18 && x <= 29 && y >= 12 && y <= 19 && x - y <= 12 && y - x <= 1;
                var hole = (x - 10) * (x - 10) + (y - 20) * (y - 20) <= 6;
                var inside = body || point;
                texture.SetPixel(x, y, inside && !hole ? solid : transparent);
            }
        }

        texture.Apply(updateMipmaps: false, makeNoLongerReadable: true);
        return Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
    }

    private static FieldInfo? AccessField(Type type, string fieldName)
    {
        return type.GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
    }

    private static bool Contains(string? value, string part, StringComparison comparison)
    {
        return value?.IndexOf(part, comparison) >= 0;
    }
}

internal sealed class TradeMarkerTooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public object? Tooltip { get; set; }

    public string Text { get; set; } = string.Empty;

    public void OnPointerEnter(PointerEventData eventData)
    {
        InvokeTooltip(new[] { "Show", "ShowTooltip", "ShowMessage" }, Text);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        InvokeTooltip(new[] { "Close", "Hide", "CloseTooltip", "HideTooltip" });
    }

    private void InvokeTooltip(string[] methodNames, params object[] args)
    {
        if (Tooltip is null)
        {
            return;
        }

        var method = Tooltip.GetType()
            .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            .FirstOrDefault(info => methodNames.Contains(info.Name)
                && info.GetParameters().Length == args.Length
                && info.GetParameters().Zip(args, (parameter, arg) => parameter.ParameterType.IsInstanceOfType(arg) || parameter.ParameterType == typeof(string)).All(match => match));

        method?.Invoke(Tooltip, args);
    }
}
#endif
