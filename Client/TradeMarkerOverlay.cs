#if SPT_CLIENT
using System.Runtime.CompilerServices;
using MoeTradeMarker.Shared;
using UnityEngine;
using UnityEngine.UI;

namespace MoeTradeMarker.Client;

internal static class TradeMarkerOverlay
{
    private static readonly List<WeakReference<Image>> ActiveMarkers = new();
    private static readonly ConditionalWeakTable<Image, object> TrackedMarkers = new();
    private static readonly object TrackedMarker = new();
    private static Sprite? iconSprite;

    public static void ShowOnItemView(Component itemView, string overlayName, MarkerPosition position, Color color)
    {
        if (itemView == null) return;
        var child = itemView.transform.Find(overlayName);
        var overlay = child != null ? child.GetComponent<Image>() : null;
        if (overlay == null)
        {
            var go = new GameObject(overlayName, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            go.transform.SetParent(itemView.transform, false);
            go.transform.SetAsLastSibling();
            overlay = go.GetComponent<Image>();
        }
        overlay.color = color;
        if (iconSprite == null) iconSprite = CreateMarkerSprite();
        overlay.sprite = iconSprite;
        overlay.raycastTarget = false;
        Position(overlay.rectTransform, position);
        if (!TrackedMarkers.TryGetValue(overlay, out _))
        {
            TrackedMarkers.Add(overlay, TrackedMarker);
            ActiveMarkers.Add(new WeakReference<Image>(overlay));
        }
        overlay.gameObject.SetActive(true);
    }

    public static void HideFromItemView(Component itemView, string overlayName)
    {
        if (itemView == null) return;
        var child = itemView.transform.Find(overlayName);
        if (child != null) child.gameObject.SetActive(false);
    }

    public static void ApplyCurrentConfigToVisibleMarkers()
    {
        ActiveMarkers.RemoveAll(reference => !reference.TryGetTarget(out var image) || image == null);
        foreach (var reference in ActiveMarkers)
        {
            if (!reference.TryGetTarget(out var marker) || marker == null) continue;
            marker.color = TradeMarkerClientConfig.MarkerColor;
            Position(marker.rectTransform, TradeMarkerClientConfig.MarkerPosition);
        }
    }

    public static void Clear()
    {
        foreach (var reference in ActiveMarkers)
        {
            if (reference.TryGetTarget(out var marker) && marker != null)
            {
                TrackedMarkers.Remove(marker);
                UnityEngine.Object.Destroy(marker.gameObject);
            }
        }
        ActiveMarkers.Clear();
        if (iconSprite != null)
        {
            UnityEngine.Object.Destroy(iconSprite.texture);
            UnityEngine.Object.Destroy(iconSprite);
            iconSprite = null;
        }
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

}
#endif
