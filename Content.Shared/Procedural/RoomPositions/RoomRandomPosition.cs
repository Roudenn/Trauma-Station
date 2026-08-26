using System.Numerics;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Shared.Procedural.RoomPositions;

public sealed partial class RoomRandomPosition : RoomPosition
{
    protected override Vector2 GetPositionImpl(IEntityManager entMan, IPrototypeManager proto, IRobustRandom rand, DungeonRoom room)
    {
        return rand.Pick(room.Tiles) + new Vector2(0.5f, 0.5f);
    }
}
