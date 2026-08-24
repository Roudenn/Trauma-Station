using Robust.Shared.Prototypes;

namespace Content.Shared.Procedural.RoomConditions;

/// <summary>
/// Checks if a room has a certain role tag.
/// </summary>
public sealed partial class RoomRoleCondition : RoomCondition
{
    /// <summary>
    /// A role tag to check on a room.
    /// </summary>
    [DataField]
    public string Role;

    protected override bool EvaluateImplementation(DungeonRoom room, IEntityManager entMan, IPrototypeManager proto)
    {
        return room.Roles.Contains(Role);
    }
}
