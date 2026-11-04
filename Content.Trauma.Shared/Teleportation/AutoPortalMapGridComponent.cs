using Content.Shared.Tag;

namespace Content.Trauma.Shared.Teleportation;

/// <summary>
/// Specifies this map or grid as a target for
/// <see cref="AutoPortalComponent"/> teleportation.
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class AutoPortalMapGridComponent : Component
{
    /// <summary>
    /// The tag which other portals link to.
    /// </summary>
    [DataField(required: true)]
    public ProtoId<TagPrototype> LinkTag;
}
