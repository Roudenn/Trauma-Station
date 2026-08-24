using System.Linq;
using Content.Shared.Maps;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;

namespace Content.Shared.Procedural.Features.Conditions;

public sealed partial class TileFeatureCondition : FeatureCondition
{
    [DataField(required: true)]
    public HashSet<ProtoId<ContentTileDefinition>> Tiles = new();

    protected override bool EvaluateImplementation(Feature root,
        EntityCoordinates position,
        IEntityManager entMan,
        IPrototypeManager proto,
        FeatureContext ctx)
    {
        var turfSystem = entMan.System<TurfSystem>();
        var tile = turfSystem.GetTileRef(position);
        if (tile == null)
            return false;

        foreach (var tileDefinition in Tiles)
        {
            var id = proto.Index(tileDefinition).TileId;
            if (tile.Value.Tile.TypeId == id)
                return true;
        }

        return false;
    }

    protected override bool Equals(FeatureCondition other)
    {
        return other is TileFeatureCondition tile && tile.Tiles.SequenceEqual(Tiles);
    }
}
