using Robust.Shared.Random;

namespace Content.Trauma.Shared.Procedural.Features.Positions;

public sealed partial class ChunkFeaturePosition : FeaturePosition
{
    [DataField]
    public RoomSides? Side;

    [DataField]
    public RoomSides[]? AvailableSides;

    public override Vector2 GetPosition<T>(IRobustRandom rand, T customContext)
    {
        if (customContext is not ChunkFeatureContext chunkCtx)
            return Vector2.Zero;

        var room = chunkCtx.ChunkData.Bounds;
        var side = Side ?? (AvailableSides != null ? rand.Pick(AvailableSides) : RoomSides.None);
        switch (side)
        {
            case RoomSides.Center:
                return room.Center;
            case RoomSides.Left:
                return room.Center with { X = room.Left };
            case RoomSides.Right:
                return room.Center with { X = room.Right };
            case RoomSides.Top:
                return room.Center with { Y = room.Top - 1 };
            case RoomSides.Bottom:
                return room.Center with { Y = room.Bottom - 1 };
            case RoomSides.TopLeft:
                return room.TopLeft;
            case RoomSides.TopRight:
                return room.TopRight - Vector2.One;
            case RoomSides.BottomLeft:
                return room.BottomLeft;
            case RoomSides.BottomRight:
                return room.BottomRight;
            case RoomSides.None:
                DebugTools.Assert($"Tried to get coordinates of a {nameof(ChunkFeaturePosition)} when the side wasn't specified.");
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
