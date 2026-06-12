using MoeTradeMarker.Shared;
using SPTarkov.Server.Core.Models.Spt.Mod;
using SPTarkov.Server.Core.Models.Utils;

namespace MoeTradeMarker.Server;

public record ModMetadata : AbstractModMetadata
{
    public override string ModGuid { get; init; } = TradeMarkerConstants.ServerGuid;
    public override string Name { get; init; } = TradeMarkerConstants.ModName;
    public override string Author { get; init; } = "moe";
    public override List<string>? Contributors { get; init; }
    public override SemanticVersioning.Version Version { get; init; } = new("0.5.0");
    public override SemanticVersioning.Range SptVersion { get; init; } = new("~4.0.0");
    public override List<string>? Incompatibilities { get; init; }
    public override Dictionary<string, SemanticVersioning.Range>? ModDependencies { get; init; }
    public override string? Url { get; init; }
    public override bool? IsBundleMod { get; init; }
    public override string License { get; init; } = "MIT";
}
