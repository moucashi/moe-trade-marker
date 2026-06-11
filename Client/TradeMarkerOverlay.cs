#if SPT_CLIENT
using System;
using System.Linq;
using System.Reflection;
using MoeTradeMarker.Shared;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MoeTradeMarker.Client;

internal static class TradeMarkerOverlay
{
    private static Sprite? iconSprite;

    public static void Show(Image iconImage, string overlayName, string traderName, object? tooltip, MarkerPosition position, Color color)
    {
        var overlay = GetOrCreateOverlay(iconImage, overlayName);
        overlay.color = color;
        overlay.sprite = iconSprite ??= CreateMarkerSprite();
        overlay.raycastTarget = true;

        Position(overlay.rectTransform, position);

        var trigger = overlay.GetComponent<TradeMarkerTooltipTrigger>()
            ?? overlay.gameObject.AddComponent<TradeMarkerTooltipTrigger>();
        trigger.Tooltip = tooltip;
        trigger.Text = $"商人标记：{traderName}";

        overlay.gameObject.SetActive(true);
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
        var fromArgs = args.FirstOrDefault(arg => arg?.GetType().Name.Contains("Tooltip", StringComparison.OrdinalIgnoreCase) == true);
        if (fromArgs is not null)
        {
            return fromArgs;
        }

        var type = instance.GetType();
        foreach (var field in type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
        {
            if (field.FieldType.Name.Contains("Tooltip", StringComparison.OrdinalIgnoreCase))
            {
                return field.GetValue(instance);
            }
        }

        return null;
    }

    private static Image GetOrCreateOverlay(Image iconImage, string overlayName)
    {
        var existing = iconImage.transform.Find(overlayName)?.GetComponent<Image>();
        if (existing is not null)
        {
            return existing;
        }

        var gameObject = new GameObject(overlayName, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        gameObject.transform.SetParent(iconImage.transform, worldPositionStays: false);
        return gameObject.GetComponent<Image>();
    }

    private static void Position(RectTransform rectTransform, MarkerPosition position)
    {
        const float size = 17f;
        const float inset = 2f;

        rectTransform.sizeDelta = new Vector2(size, size);
        rectTransform.localScale = Vector3.one;

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
                var inside = x >= 4 && x <= 25 && y >= 6 && y <= 27 && x + y >= 14;
                var hole = (x - 10) * (x - 10) + (y - 22) * (y - 22) <= 9;
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
