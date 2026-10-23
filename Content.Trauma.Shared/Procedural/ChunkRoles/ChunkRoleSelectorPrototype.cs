namespace Content.Trauma.Shared.Procedural.ChunkRoles;

[Prototype]
public sealed partial class ChunkRoleSelectorPrototype : IPrototype
{
    /// <inheritdoc/>
    [IdDataField]
    public string ID { get; private set; } = default!;

    [DataField(required: true)]
    public ChunkRoleSelector Selector;
}
