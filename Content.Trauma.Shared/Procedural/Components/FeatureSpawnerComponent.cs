using Content.Trauma.Shared.Procedural.Features;

namespace Content.Trauma.Shared.Procedural.Components;

/// <summary>
/// Spawns a <see cref="Feature"/> on map-init.
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class FeatureSpawnerComponent : Component
{
    [DataField(required: true)]
    public Feature Feature;
}
