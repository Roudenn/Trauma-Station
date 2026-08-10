using Content.Shared.Procedural.Features;
using Robust.Shared.GameStates;

namespace Content.Shared.Procedural.Components;

/// <summary>
/// Spawns a <see cref="Feature"/> on map-init.
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class FeatureSpawnerComponent : Component
{
    [DataField(required: true)]
    public Feature Feature;
}
