namespace Content.Trauma.Shared.Procedural.ChunkRoles;

/// <summary>
/// Selects a role for a chunk based on its position in the region,
/// its dimensions, or other internal data.
/// </summary>
[ImplicitDataDefinitionForInheritors]
public abstract partial class ChunkRoleSelector
{
    [DataField]
    public float Weight;

    [Access(Other = AccessPermissions.Execute)]
    public abstract IEnumerable<ProtoId<ChunkRolePrototype>> Accept<TArgs>(IChunkRoleSelectorVisitor<TArgs> visitor, TArgs args);
}
