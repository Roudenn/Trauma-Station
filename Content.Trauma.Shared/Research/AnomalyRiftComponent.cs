namespace Content.Trauma.Shared.Research;

/// <summary>
/// A tag component that marks all entities that can be detected with a rift locator.
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class AnomalyRiftComponent : Component;
