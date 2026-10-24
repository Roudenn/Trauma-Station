using Content.Shared.Decals;
using Content.Trauma.Shared.Procedural.Effects;
using Content.Trauma.Shared.Procedural.Regions;
using Robust.Shared.Map;

namespace Content.Trauma.Shared.EntityFeatures.Contexts;

/// <summary>
/// Marker interface for different contexts in which a <see cref="Feature"/> can be spawned.
/// </summary>
public interface IFeatureContext;

/// <summary>
/// Dummy feature context with no information.
/// </summary>
public record struct NoneFeatureContext : IFeatureContext;

/// <summary>
/// <see cref="IFeatureContext"/> for features spawned by <see cref="ChunkFeature"/>.
/// Contains the information about the chunk itself and a grid its on.
/// </summary>
public record struct ChunkFeatureContext(Chunk ChunkData) : IFeatureContext;

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
