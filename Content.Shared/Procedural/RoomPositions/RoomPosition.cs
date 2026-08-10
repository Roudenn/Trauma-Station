using System.Numerics;
using Content.Shared.Procedural.DungeonLayers;
using Content.Shared.Procedural.Features;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Shared.Procedural.RoomPositions;

/// <summary>
/// A position inside a <see cref="DungeonRoom"/>.
/// Used by <see cref="FeatureDunGen"/> to generate <see cref="Feature"/>s at specific positions.
/// </summary>
[ImplicitDataDefinitionForInheritors]
public abstract partial class RoomPosition
{
    /// <summary>
    /// Weight used by <see cref="GroupRoomPosition"/> to select a position.
    /// </summary>
    [DataField]
    public float Weight = 1f;

    /// <summary>
    /// Offset from the resulting position.
    /// </summary>
    /// <remarks>This potentially can clip the feature out of bounds of the room.</remarks>
    [DataField]
    public Vector2 Offset;

    public Vector2 GetPosition(IEntityManager entMan, IPrototypeManager proto, IRobustRandom rand, DungeonRoom room)
    {
        return GetPositionImpl(entMan, proto, rand, room) + Offset;
    }

    protected abstract Vector2 GetPositionImpl(IEntityManager entMan, IPrototypeManager proto, IRobustRandom rand, DungeonRoom room);
}
