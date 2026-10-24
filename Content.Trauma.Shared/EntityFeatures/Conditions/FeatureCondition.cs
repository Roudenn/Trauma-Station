using Content.Trauma.Shared.EntityFeatures.Contexts;
using Content.Trauma.Shared.EntityFeatures.Features;
using JetBrains.Annotations;
using Robust.Shared.Map;

namespace Content.Trauma.Shared.EntityFeatures.Conditions;

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
    public readonly bool Invert;

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

    protected abstract bool Equals(FeatureCondition other);
}
