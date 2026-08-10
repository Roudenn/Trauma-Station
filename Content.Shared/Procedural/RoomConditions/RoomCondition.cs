using Content.Shared.Procedural.DungeonLayers;
using JetBrains.Annotations;
using Robust.Shared.Prototypes;

namespace Content.Shared.Procedural.RoomConditions;

[ImplicitDataDefinitionForInheritors, UsedImplicitly(ImplicitUseTargetFlags.WithInheritors)]
public abstract partial class RoomCondition
{
    /// <summary>
    /// If true, inverts the result of the condition.
    /// </summary>
    [DataField]
    public bool Invert;

    public bool Evaluate(FeatureDunGen root, DungeonRoom room, IEntityManager entMan, IPrototypeManager proto)
    {
        var res = EvaluateImplementation(root, room, entMan, proto);

        // XOR eval to invert the result.
        return res ^ Invert;
    }

    protected abstract bool EvaluateImplementation(FeatureDunGen root, DungeonRoom room, IEntityManager entMan, IPrototypeManager proto);
}
