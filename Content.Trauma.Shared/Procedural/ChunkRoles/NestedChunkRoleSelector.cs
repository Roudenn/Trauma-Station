namespace Content.Trauma.Shared.Procedural.ChunkRoles;

/// <summary>
/// Wrapper for another <see cref="ChunkRoleSelector"/>.
/// </summary>
public sealed partial class NestedChunkRoleSelector : ChunkRoleSelector
{
    public const string IdFieldName = "id";

    [DataField(required: true)]
    public ProtoId<ChunkRoleSelectorPrototype> Id;

    public override IEnumerable<ProtoId<ChunkRolePrototype>> Accept<TArgs>(IChunkRoleSelectorVisitor<TArgs> visitor, TArgs args)
        => visitor.VisitNestedSelector(this, args);
}
