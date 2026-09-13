using Content.Shared.Procedural.DungeonLayers;
using Robust.Shared.Prototypes;

namespace Content.Shared.Procedural;

/// <summary>
/// A prototype that represents a collection of nested layers used by <see cref="NestedDunGen"/>
/// </summary>
[Prototype]
public sealed partial class DungeonLayersPrototype : IPrototype
{
    [IdDataField]
    public string ID { get; private set; } = default!;

    /// <summary>
    /// Layers that will be executed on the current dungeon.
    /// </summary>
    [DataField]
    public List<IDunGenLayer> Layers = new();
}
