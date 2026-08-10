using System.Numerics;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Shared.Procedural.RoomPositions;

public sealed partial class RoomConstPosition : RoomPosition
{
    /// <summary>
    /// A vector that represents percentages from the bottom-left towards the top-right of the room
    /// when width and height are in the range of 0-1.
    /// </summary>
    [DataField]
    public Vector2 Position;

    public RoomConstPosition(Vector2 value)
    {
        Position = value;
    }

    protected override Vector2 GetPositionImpl(IEntityManager entMan, IPrototypeManager proto, IRobustRandom rand, DungeonRoom room)
    {
        Position = Vector2.Clamp(Position, Vector2.Zero, Vector2.One);
        return new Vector2(room.Bounds.Width * Position.X, room.Bounds.Height * Position.Y) + room.Bounds.BottomLeft;
    }
}
