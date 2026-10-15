using JetBrains.Annotations;

namespace Content.Trauma.Shared.Procedural.Regions;

public interface IChunkGeneratorVisitor<in TArgs>
{
    /// <summary>
    /// Alias of <see cref="ChunkGenerator.Accept{TArgs}"/>.
    /// </summary>
    [PublicAPI]
    Region Visit(ChunkGenerator chunk, TArgs args) => chunk.Accept(this, args);

    /// <summary>
    /// Visit an <see cref="BSPChunkGenerator"/>.
    /// </summary>
    [PublicAPI]
    Region VisitBSPGenerator(BSPChunkGenerator generator, TArgs args);
}
