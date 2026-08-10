using Content.Shared.Maps;
using Robust.Shared.Prototypes;

namespace Content.Shared.Procedural.Features;

/// <summary>
/// Places a single tile.
/// </summary>
public sealed partial class TileFeature : Feature
{
    public const string TileDataFieldTag = "tile";

    [DataField]
    public ProtoId<ContentTileDefinition> Tile;

    public override void Accept<TContext>(IFeatureVisitor<TContext> visitor, TContext args) =>
        visitor.VisitTileFeature(this, args);
}
