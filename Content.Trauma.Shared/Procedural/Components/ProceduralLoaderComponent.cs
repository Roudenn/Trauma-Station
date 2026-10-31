namespace Content.Trauma.Shared.Procedural.Components;

/// <summary>
/// Allows the entity to procedurally load worlds.
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class ProceduralLoaderComponent : Component
{
    [DataField]
    public float Range = 16f;
}
