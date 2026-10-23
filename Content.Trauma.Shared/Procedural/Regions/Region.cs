namespace Content.Trauma.Shared.Procedural.Regions;

/// <summary>
/// Regions are "large chunks", areas that contain a fixed amount of smaller chunks.
/// This struct contains basic information about the Chunk layout on a large area,
/// which allows to create large open spaces that feel logical.
/// </summary>
public sealed class Region(Chunk[] chunks, Vector2i gridIndices)
{
    /// <summary>
    /// An array of all generated chunks.
    /// </summary>
    public Chunk[] Chunks = chunks;

    public Vector2i GridIndices = gridIndices;
}
