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
public record struct Chunk(Vector2i Position, Box2i Bounds, EntityUid Owner);
