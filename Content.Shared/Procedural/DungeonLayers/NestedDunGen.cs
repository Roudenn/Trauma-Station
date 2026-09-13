using Robust.Shared.Prototypes;

namespace Content.Shared.Procedural.DungeonLayers;

/// <summary>
/// Executes a prototyped nested list of layers on the current dungeon.
/// Useful for organizing large configs into smaller files.
/// </summary>
public sealed partial class NestedDunGen : IDunGenLayer
{
    [DataField(required: true)]
    public ProtoId<DungeonLayersPrototype> Proto;
}
