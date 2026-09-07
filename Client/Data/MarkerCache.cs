using System.Collections.Generic;

namespace MoeTradeMarker.Client.Data;

// The caller serializes access. A response received during a fetch invalidates that fetch.
internal sealed class MarkerCache
{
    public MarkerSnapshot Snapshot { get; private set; } = MarkerSnapshot.Empty;
    public long Revision { get; private set; }

    public void Apply(Dictionary<string, string> additions)
    {
        if (additions.Count == 0) return;
        Snapshot = Snapshot.WithMarkers(additions);
        Revision++;
    }

    public bool TryReplace(long startedRevision, MarkerSnapshot loaded, out bool changed)
    {
        changed = false;
        if (startedRevision != Revision) return false;
        changed = !Snapshot.SameAs(loaded);
        Snapshot = loaded;
        return true;
    }
}
