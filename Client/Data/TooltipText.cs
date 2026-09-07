using System;

namespace MoeTradeMarker.Client.Data;

internal sealed class TooltipText
{
    public string? ItemId { get; private set; }
    public string OriginalText { get; private set; } = string.Empty;
    public string RenderedText { get; private set; } = string.Empty;

    public void Reset(string? itemId)
    {
        ItemId = itemId;
        OriginalText = RenderedText = string.Empty;
    }

    public string SetText(string? text, string marker)
    {
        OriginalText = text ?? string.Empty;
        RenderedText = string.IsNullOrWhiteSpace(marker) || OriginalText.IndexOf(marker, StringComparison.Ordinal) >= 0
            ? OriginalText
            : string.IsNullOrWhiteSpace(OriginalText) ? marker : $"{OriginalText}\n\n{marker}";
        return RenderedText;
    }
}
