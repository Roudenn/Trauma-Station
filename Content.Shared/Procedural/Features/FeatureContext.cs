using Content.Shared.Decals;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;

namespace Content.Shared.Procedural.Features;

public sealed class FeatureContext
{
    public readonly HashSet<Vector2i> Obstructed = new();

    public readonly List<EntFeatureEntry> EntFeatures = new();

    public readonly List<TileFeatureEntry> TileFeatures = new();

    public readonly List<DecalFeatureEntry> DecalFeatures = new();
}

public record struct FeatureEntry(Vector2i GridIndices, bool Obstructed = false);

public record struct EntFeatureEntry(EntityUid Ent, Vector2i GridIndices, bool Obstructed = false);

public record struct TileFeatureEntry(Tile Tile, Vector2i GridIndices, bool Obstructed = false);

public record struct DecalFeatureEntry(ProtoId<DecalPrototype> DecalId, Vector2i GridIndices, bool Obstructed = false);
