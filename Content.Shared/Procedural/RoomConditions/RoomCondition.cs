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

    public bool Evaluate(DungeonRoom room, IEntityManager entMan, IPrototypeManager proto)
    {
        var res = EvaluateImplementation(room, entMan, proto);

        // XOR eval to invert the result.
        return res ^ Invert;
    }

    protected abstract bool EvaluateImplementation(DungeonRoom room, IEntityManager entMan, IPrototypeManager proto);
}
