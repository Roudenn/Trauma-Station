using Content.Trauma.Shared.Procedural.Features;

namespace Content.Trauma.Shared.Procedural.Regions;

/// <summary>
/// Chunks are pre-calculated data about which <see cref="Feature"/>s to spawn in bounds of their area.
/// Usually each chunk is a room or a fixed area of open space.
/// <para>
/// Chunks are loaded instantly and on a single thread to be ECS-friendly and ordered.
/// However, there is a limit to how many chunks can be loaded in a single tick.
/// To prevent major lag spikes when going through a procedural grid keep your chunk size small.
/// </para>
/// </summary>
public struct Chunk
{
    /// <summary>
    /// Relative position of this chunk inside a region.
    /// </summary>
    public Vector2 Position;

    /// <summary>
    /// Bounds of the chunk.
    /// </summary>
    public Box2i Bounds;
}
