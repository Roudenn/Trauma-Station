using Content.Trauma.Common.Utilities;
using Content.Trauma.Shared.Procedural.Regions;

namespace Content.Trauma.Shared.Procedural.Components;

/// <summary>
/// Added to a grid that procedurally generates its contents.
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class ProceduralGridComponent : Component
{
    [DataField(required: true)]
    public ProceduralConfig Config = default!;

    [DataField]
    public Dictionary<Vector2i, Region> Regions = new();

    [DataField]
    public DeterministicRandom Random = new();
}
