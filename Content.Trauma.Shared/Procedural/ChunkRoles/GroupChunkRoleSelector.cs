using Content.Trauma.Shared.Procedural.Regions;

namespace Content.Trauma.Shared.Procedural.ChunkRoles;

/// <summary>
/// Selects a <see cref="ChunkRolePrototype"/> for a <see cref="Chunk"/> based on its dimensions.
/// </summary>
public sealed partial class GroupChunkRoleSelector : ChunkRoleSelector
{
    /// <summary>
    /// Dictionary of chunk sizes and corresponding chunk roles.
    /// If a chunk's dimensions are equal to or lower than an entry, it assigns the corresponding role.
    /// </summary>
    [DataField(required: true)]
    public List<ChunkRoleSelector> Children;

    public override IEnumerable<ProtoId<ChunkRolePrototype>> Accept<TArgs>(IChunkRoleSelectorVisitor<TArgs> visitor, TArgs args)
        => visitor.VisitGroupSelector(this, args);
}
