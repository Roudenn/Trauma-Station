namespace Content.Trauma.Shared.Procedural.Regions;

/// <summary>
/// Generates a rectangular area with smaller chunks with a fixed size inside.
/// Most useful for repetitive locations or open worlds like planets.
/// </summary>
public sealed partial class BoxChunkGenerator : ChunkGenerator
{
    /// <summary>
    /// Amount of chunks on X and Y axis.
    /// </summary>
    [DataField(required: true)]
    public Vector2i RegionSize;

    /// <summary>
    /// Dimensions of every spawned chunk.
    /// </summary>
    [DataField(required: true)]
    public Vector2i ChunkDimensions;

    /// <summary>
    /// Spacings between chunks, 0 by default.
    /// </summary>
    [DataField]
    public int Steps;

    public override Region Accept<TContext>(IChunkGeneratorVisitor<TContext> visitor, TContext args) =>
        visitor.VisitBoxGenerator(this, args);
}
