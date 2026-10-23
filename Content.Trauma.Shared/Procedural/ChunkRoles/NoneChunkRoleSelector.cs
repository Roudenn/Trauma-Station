namespace Content.Trauma.Shared.Procedural.ChunkRoles;

/// <summary>
/// Does nothing.
/// </summary>
public sealed partial class NoneChunkRoleSelector : ChunkRoleSelector
{
    public override IEnumerable<ProtoId<ChunkRolePrototype>> Accept<TArgs>(IChunkRoleSelectorVisitor<TArgs> visitor, TArgs args)
        => visitor.VisitNoneSelector(this, args);
}
