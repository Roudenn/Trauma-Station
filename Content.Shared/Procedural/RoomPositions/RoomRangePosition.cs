using System.Numerics;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Shared.Procedural.RoomPositions;

public sealed partial class RoomRangePosition : RoomPosition
{
    /// <summary>
    /// A vector that represents percentages from the bottom-left towards the top-right of the room
    /// when width and height are in the range of 0-1.
    /// <br/>
    /// This is the furthest bottom-left point of the box where the position can be placed.
    /// </summary>
    [DataField]
    public Vector2 MinPos;

    /// <summary>
    /// A vector that represents percentages from the bottom-left towards the top-right of the room
    /// when width and height are in the range of 0-1.
    /// <br/>
    /// This is the furthest top-right point of the box where the position can be placed.
    /// </summary>
    [DataField]
    public Vector2 MaxPos;

    protected override Vector2 GetPositionImpl(IEntityManager entMan, IPrototypeManager proto, IRobustRandom rand, DungeonRoom room)
    {
        return new Vector2(
            rand.NextFloat(MinPos.X, MaxPos.X) * room.Bounds.Width,
            rand.NextFloat(MinPos.Y, MaxPos.Y) * room.Bounds.Height)
               + room.Bounds.BottomLeft;
    }
}
