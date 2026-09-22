namespace Content.Trauma.Common.DeviceNetwork;

/// <summary>
/// Suppresses all device network signals on a map it's applied to.
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class DeviceNetworkSuppressionComponent : Component;
