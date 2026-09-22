using Content.Trauma.Common.DeviceNetwork;

namespace Content.Trauma.Shared.DeviceNetwork;

public sealed partial class TraumaDeviceNetworkSystem : EntitySystem
{
    [SubscribeLocalEvent]
    private static void OnPacketAttempt(Entity<DeviceNetworkSuppressionComponent> ent, ref MapPacketReceiveAttemptEvent packet)
    {
        packet.Cancelled = true;
    }
}
