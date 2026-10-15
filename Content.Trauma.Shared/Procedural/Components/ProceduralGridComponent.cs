namespace Content.Trauma.Shared.Procedural.Components;

/// <summary>
/// Added to a grid that procedurally generates its contents.
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class ProceduralGridComponent : Component
{
    [DataField(required: true)]
    public ProceduralConfig Config = default!;
}
