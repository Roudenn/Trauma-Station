namespace Content.Trauma.Common.GPS;

/// <summary>
/// Component applied to a map that makes GPS not work here.
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class GPSSuppressionComponent : Component;
