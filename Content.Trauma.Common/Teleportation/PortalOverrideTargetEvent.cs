namespace Content.Trauma.Common.Teleportation;

/// <summary>
/// Overrides the target destination of a portal.
/// </summary>
[ByRefEvent]
public record struct PortalOverrideTargetEvent(EntityUid? Target = null);
