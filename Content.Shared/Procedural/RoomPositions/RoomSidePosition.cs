using System.Numerics;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Utility;

namespace Content.Shared.Procedural.RoomPositions;

/// <summary>
/// Places a position on one of the sides of the room or in the center.
/// </summary>
public sealed partial class RoomSidePosition : RoomPosition
{
    [DataField(required: true)]
    public RoomSides Side;

    protected override Vector2 GetPositionImpl(
        IEntityManager entMan,
        IPrototypeManager proto,
        IRobustRandom rand,
        DungeonRoom room)
    {
        switch (Side)
        {
            case RoomSides.Center:
                return room.Center;
            case RoomSides.Left:
                return room.Center with { X = room.Bounds.Left };
            case RoomSides.Right:
                return room.Center with { X = room.Bounds.Right };
            case RoomSides.Top:
                return room.Center with { Y = room.Bounds.Top };
            case RoomSides.Bottom:
                return room.Center with { Y = room.Bounds.Bottom };
            case RoomSides.TopLeft:
                return room.Bounds.TopLeft;
            case RoomSides.TopRight:
                return room.Bounds.TopRight;
            case RoomSides.BottomLeft:
                return room.Bounds.BottomLeft;
            case RoomSides.BottomRight:
                return room.Bounds.BottomRight;
            case RoomSides.None:
                DebugTools.Assert($"Tried to get coordinates of a {nameof(RoomSidePosition)} when the side wasn't specified.");
                return room.Center;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}

public enum RoomSides : byte
{
    None = 0,
    Center,
    Left,
    Right,
    Top,
    Bottom,
    TopLeft,
    TopRight,
    BottomLeft,
    BottomRight,
}
