using System.Numerics;
using Content.Shared.Tag;
using Robust.Shared.Prototypes;

namespace Content.Shared.Procedural;

// TODO: Cache center and bounds and shit and don't make the caller deal with it.
public sealed record DungeonRoom(HashSet<Vector2i> Tiles, Vector2 Center, Box2i Bounds, HashSet<Vector2i> Exterior, List<ProtoId<TagPrototype>> Roles)
{
    public readonly List<Vector2i> Entrances = new();

    /// <summary>
    /// Nodes adjacent to tiles, including the corners.
    /// </summary>
    public readonly HashSet<Vector2i> Exterior = Exterior;

    public readonly List<ProtoId<TagPrototype>> Roles = new();
}
