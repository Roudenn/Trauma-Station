namespace Content.Trauma.Shared.Procedural.ChunkRoles;

/// <summary>
/// Wrapper for another <see cref="ChunkRoleSelector"/>.
/// </summary>
public sealed partial class SingleChunkRoleSelector : ChunkRoleSelector
{
    [DataField(required: true)]
    public ProtoId<ChunkRolePrototype> Role;

    public override IEnumerable<ProtoId<ChunkRolePrototype>> Accept<TArgs>(IChunkRoleSelectorVisitor<TArgs> visitor, TArgs args)
        => visitor.VisitSingleSelector(this, args);
}
