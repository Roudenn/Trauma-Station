namespace Content.Trauma.Shared.Procedural.Regions;

/// <summary>
/// Defines an algorithm for generating a single <see cref="Region"/>.
/// </summary>
[ImplicitDataDefinitionForInheritors]
public abstract partial class ChunkGenerator
{
    [Access(Other = AccessPermissions.Execute)]
    public abstract Region Accept<TArgs>(IChunkGeneratorVisitor<TArgs> visitor, TArgs args);
}
