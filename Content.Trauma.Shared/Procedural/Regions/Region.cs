namespace Content.Trauma.Shared.Procedural.Regions;

/// <summary>
/// Regions are "large chunks", areas that contain a fixed amount of smaller chunks.
/// This struct contains basic information about the Chunk layout on a large area,
/// which allows to create large open spaces that feel logical.
/// </summary>
public sealed record Region(Chunk[] Chunks, Vector2i Bounds, Vector2i GridIndices);
