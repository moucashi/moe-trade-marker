using System;
using System.Collections.Generic;
using System.Linq;

namespace MoeTradeMarker.Client.Data;

// The caller serializes access and allows only one refresh at a time.
internal sealed class MarkerCache
{
    private readonly Dictionary<string, (string TraderId, long Revision)> pendingMarkers = new(StringComparer.OrdinalIgnoreCase);
    public MarkerSnapshot Snapshot { get; private set; } = MarkerSnapshot.Uninitialized;
    public long Revision { get; private set; }

    public void Apply(Dictionary<string, string> additions)
    {
        if (additions.Count == 0) return;
        Snapshot = Snapshot.WithMarkers(additions);
        Revision++;
        foreach (var pair in additions) pendingMarkers[pair.Key] = (pair.Value, Revision);
    }

    public bool Replace(long startedRevision, MarkerSnapshot loaded)
    {
        // Only purchases newer than this fetch override it; older entries are authoritative again.
        foreach (var key in pendingMarkers.Where(pair => pair.Value.Revision <= startedRevision).Select(pair => pair.Key).ToArray())
            pendingMarkers.Remove(key);
        if (pendingMarkers.Count > 0)
            loaded = loaded.WithMarkers(pendingMarkers.ToDictionary(pair => pair.Key, pair => pair.Value.TraderId, StringComparer.OrdinalIgnoreCase));
        var changed = !Snapshot.SameAs(loaded);
        Snapshot = loaded;
        return changed;
    }
}
