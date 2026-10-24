using Content.Trauma.Shared.Procedural.ChunkRoles;

namespace Content.Trauma.Shared.Procedural.Components;

/// <summary>
/// Assigned to chunk entities that have some roles which are allowed to be used in the
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class ProceduralChunkRolesComponent : Component
{
    [DataField]
    public HashSet<ProtoId<ChunkRolePrototype>> Roles = new();
}
