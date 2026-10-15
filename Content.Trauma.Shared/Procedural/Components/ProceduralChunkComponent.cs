using Content.Shared.EntityEffects;

namespace Content.Trauma.Shared.Procedural.Components;

/// <summary>
/// Component added to every chunk entity of the generated dungeon.
/// The entity with this component is spawned pre-emptively when a region was fully prepared,
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class ProceduralChunkComponent : Component
{
    /// <summary>
    /// Effects that will be applied when this chunk is loaded.
    /// </summary>
    [DataField]
    public List<EntityEffect> Effects = new();
}
