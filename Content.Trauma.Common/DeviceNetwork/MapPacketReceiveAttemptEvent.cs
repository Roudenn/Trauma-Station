namespace Content.Trauma.Common.DeviceNetwork;

[ByRefEvent]
public record struct MapPacketReceiveAttemptEvent(bool Cancelled = false);
