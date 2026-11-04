using Content.Shared.Tag;
using Robust.Shared.Map;

namespace Content.Trauma.Shared.Teleportation;

/// <summary>
/// A portal that's automatically linked to all other portals with the same tag on a grid or map.
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class AutoPortalComponent : Component
{
    [DataField(required: true)]
    public ProtoId<TagPrototype> Tag;

    [DataField]
    public HashSet<EntityUid> LinkedGrids = new();

    [DataField]
    public HashSet<MapId> LinkedMaps = new();

    /// <summary>
    /// A of a <see cref="AutoPortalMapGridComponent"/> that this portal
    /// will try to link to when the portal is first created.
    /// </summary>
    [DataField]
    public ProtoId<TagPrototype>? AutoLinkTag;
}
