using JetBrains.Annotations;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;

namespace Content.Shared.Procedural.Features.Conditions;

/// <summary>
/// Used for implementing conditional logic for <see cref="Feature"/>s.
/// </summary>
[ImplicitDataDefinitionForInheritors, UsedImplicitly(ImplicitUseTargetFlags.WithInheritors)]
public abstract partial class FeatureCondition
{
    /// <summary>
    /// If true, inverts the result of the condition.
    /// </summary>
    [DataField]
    public bool Invert;

    /// <summary>
    /// Evaluates a condition for a given context.
    /// </summary>
    public bool Evaluate(Feature root, EntityCoordinates position, IEntityManager entMan, IPrototypeManager proto, FeatureContext ctx)
    {
        var res = EvaluateImplementation(root, position, entMan, proto, ctx);

        // XOR eval to invert the result.
        return res ^ Invert;
    }

    protected abstract bool EvaluateImplementation(Feature root, EntityCoordinates position, IEntityManager entMan, IPrototypeManager proto, FeatureContext ctx);

    public override bool Equals(object? obj)
    {
        return obj?.GetType() == GetType()
               && obj is FeatureCondition condition
               && Invert == condition.Invert
               && Equals(condition);
    }

    public override int GetHashCode()
    {
        return Invert.GetHashCode();
    }

    protected abstract bool Equals(FeatureCondition other);
}
