using Content.Trauma.Shared.EntityFeatures.Contexts;
using Robust.Shared.Random;

namespace Content.Trauma.Shared.EntityFeatures.Positions.Procedural;

/// <summary>
/// Spawns a feature in a random tile inside the chunk.
/// </summary>
public sealed partial class ChunkRandomFeaturePosition : FeaturePosition
{
    public override Vector2 GetPosition<T>(IRobustRandom rand, T customContext)
    {
        if (customContext is not ChunkFeatureContext chunkCtx)
            return Vector2.Zero;

        var room = chunkCtx.ChunkData.GridBounds;
        return rand.NextVector2Box(room.Left, room.Bottom, room.Right, room.Top);
    }
}
