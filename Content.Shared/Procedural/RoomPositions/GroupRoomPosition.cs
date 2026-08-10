using System.Linq;
using System.Numerics;
using Content.Shared.Random.Helpers;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Shared.Procedural.RoomPositions;

/// <summary>
/// Selects a room position from a group randomly based on their weight.
/// </summary>
public sealed partial class GroupRoomPosition : RoomPosition
{
    [DataField(required: true)]
    public List<RoomPosition> Children = new();

    protected override Vector2 GetPositionImpl(IEntityManager entMan, IPrototypeManager proto, IRobustRandom rand, DungeonRoom room)
    {
        var validWeightedChildren = Children
            .Where(child => child.Weight >= float.Epsilon)
            .ToDictionary(child => child, child => child.Weight);

        return validWeightedChildren.Count == 0
            ? room.Center // Center as a fallback
            : SharedRandomExtensions.Pick(validWeightedChildren, rand).GetPosition(entMan, proto, rand, room);
    }
}
