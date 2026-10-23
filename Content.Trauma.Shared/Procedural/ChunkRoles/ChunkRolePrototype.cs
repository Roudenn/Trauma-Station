using Content.Trauma.Shared.Procedural.Components;

namespace Content.Trauma.Shared.Procedural.ChunkRoles;

/// <summary>
/// A tag prototype that specifies a certain role for a chunk on a <see cref="ProceduralGridComponent"/>.
/// </summary>
[Prototype]
public sealed partial class ChunkRolePrototype : IPrototype
{
    /// <inheritdoc/>
    [IdDataField]
    public string ID { get; private set; } = default!;
}
