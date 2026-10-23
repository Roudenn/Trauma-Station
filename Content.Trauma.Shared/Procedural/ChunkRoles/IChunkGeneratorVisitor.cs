using JetBrains.Annotations;

namespace Content.Trauma.Shared.Procedural.ChunkRoles;

public interface IChunkRoleSelectorVisitor<in TArgs>
{
    /// <summary>
    /// Alias of <see cref="ChunkRoleSelector.Accept{TArgs}"/>.
    /// </summary>
    [PublicAPI]
    IEnumerable<ProtoId<ChunkRolePrototype>> Visit(ChunkRoleSelector selector, TArgs args) => selector.Accept(this, args);

    /// <summary>
    /// Visit an <see cref="SizeChunkRoleSelector"/>.
    /// </summary>
    [PublicAPI]
    IEnumerable<ProtoId<ChunkRolePrototype>> VisitSizeSelector(SizeChunkRoleSelector size, TArgs args);

    /// <summary>
    /// Visit an <see cref="AllChunkRoleSelector"/>.
    /// </summary>
    [PublicAPI]
    IEnumerable<ProtoId<ChunkRolePrototype>> VisitAllSelector(AllChunkRoleSelector size, TArgs args);

    /// <summary>
    /// Visit an <see cref="GroupChunkRoleSelector"/>.
    /// </summary>
    [PublicAPI]
    IEnumerable<ProtoId<ChunkRolePrototype>> VisitGroupSelector(GroupChunkRoleSelector size, TArgs args);

    /// <summary>
    /// Visit an <see cref="NestedChunkRoleSelector"/>.
    /// </summary>
    [PublicAPI]
    IEnumerable<ProtoId<ChunkRolePrototype>> VisitNestedSelector(NestedChunkRoleSelector size, TArgs args);

    /// <summary>
    /// Visit an <see cref="SingleChunkRoleSelector"/>.
    /// </summary>
    [PublicAPI]
    IEnumerable<ProtoId<ChunkRolePrototype>> VisitSingleSelector(SingleChunkRoleSelector size, TArgs args);

    /// <summary>
    /// Visit an <see cref="NoneChunkRoleSelector"/>.
    /// </summary>
    [PublicAPI]
    IEnumerable<ProtoId<ChunkRolePrototype>> VisitNoneSelector(NoneChunkRoleSelector size, TArgs args);
}
