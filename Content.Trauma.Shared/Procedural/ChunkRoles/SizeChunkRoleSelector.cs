using Content.Trauma.Shared.Procedural.Regions;

namespace Content.Trauma.Shared.Procedural.ChunkRoles;

/// <summary>
/// Selects a <see cref="ChunkRolePrototype"/> for a <see cref="Chunk"/> based on its dimensions.
/// </summary>
public sealed partial class SizeChunkRoleSelector : ChunkRoleSelector
{
    /// <summary>
    /// Dictionary of chunk sizes and corresponding chunk roles.
    /// If a chunk's dimensions are equal to or lower than an entry, it assigns the corresponding role.
    /// </summary>
    [DataField(required: true)]
    public Dictionary<Vector2i, ProtoId<ChunkRolePrototype>> SizeDict;

    public override IEnumerable<ProtoId<ChunkRolePrototype>> Accept<TArgs>(IChunkRoleSelectorVisitor<TArgs> visitor, TArgs args)
        => visitor.VisitSizeSelector(this, args);
}
