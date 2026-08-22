using Content.Shared.Procedural.Features;
using Content.Shared.Procedural.RoomConditions;
using Content.Shared.Procedural.RoomPositions;
using Robust.Shared.Prototypes;

namespace Content.Shared.Procedural.DungeonLayers;

/// <summary>
/// Generates a single <see cref="Features.Feature"/> in each room of the current dungeon.
/// </summary>
public sealed partial class FeatureDunGen : IDunGenLayer
{
    [DataField(required: true)]
    public Feature Feature;

    [DataField(required: true)]
    public RoomPosition Position;

    [DataField]
    public List<RoomCondition> Conditions = new();

    /// <summary>
    /// If true, all the conditions must be successful in order for the selector to process.
    /// Otherwise, only one of them must be.
    /// </summary>
    [DataField]
    public bool RequireAll = true;

    /// <summary>
    /// Check if the condition for this selector are met.
    /// </summary>
    public bool CheckConditions(IEntityManager entMan, IPrototypeManager proto, DungeonRoom room)
    {
        if (Conditions.Count == 0)
            return true;

        var success = false;
        foreach (var condition in Conditions)
        {
            var res = condition.Evaluate(this, room, entMan, proto);

            if (RequireAll && !res)
                return false; // intentional break out of loop and function

            success |= res;
        }

        if (RequireAll)
            return true;

        return success;
    }
}
