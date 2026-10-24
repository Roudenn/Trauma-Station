using Content.Trauma.Shared.EntityFeatures.Features;

namespace Content.Trauma.Shared.EntityFeatures.Components;

/// <summary>
/// Spawns a <see cref="Feature"/> on map-init.
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class FeatureSpawnerComponent : Component
{
    [DataField(required: true)]
    public Feature Feature;
}
