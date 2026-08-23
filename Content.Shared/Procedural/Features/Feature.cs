using System.Numerics;
using Content.Shared.Procedural.Features.Conditions;
using Content.Shared.Procedural.Features.Positions;
using Content.Shared.ValueSelectors.Numbers;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;

namespace Content.Shared.Procedural.Features;

/// <summary>
/// A "feature" is a way to represent any placeable object - a combination of entities, tiles and decals.
/// This allows to make and reuse certain combinations of objects as YAML prototypes.
/// </summary>
[ImplicitDataDefinitionForInheritors]
public abstract partial class Feature
{
    /// <summary>
    /// A weight used to pick between features.
    /// </summary>
    [DataField]
    public float Weight = 1f;

    /// <summary>
    /// A simple chance that the feature will place.
    /// </summary>
    [DataField]
    public float Prob = 1f;

    [DataField]
    public NumberSelector Rolls = new ConstantNumberSelector(1);

    /// <summary>
    /// Local offset of this feature.
    /// </summary>
    [DataField]
    public FeaturePosition Offset = new ConstantFeaturePosition(Vector2.Zero);

    /// <summary>
    /// Conditions that are checked before spawning this feature.
    /// </summary>
    [DataField]
    public HashSet<FeatureCondition> Conditions = new();

    /// <summary>
    /// If true on a condition that executes other nested conditions,
    /// all recursive children will concat their own conditions and conditions of this feature.
    /// Useful for reducing copy-paste with feature conditions that have to be applied to all children.
    /// </summary>
    [DataField]
    public bool ConditionInheritance;

    [DataField]
    public bool ConditionsApplySelf = true;

    [DataField]
    public bool RequireAll = true;

    [DataField]
    public bool Obstruct = true;

    /// <summary>
    /// If true, this feature gets aligned to the grid.
    /// This gets inherited if passed through <see cref="AllFeature"/> or <see cref="GroupFeature"/>.
    /// </summary>
    [DataField]
    public bool AlignTile;

    /// <summary>
    /// Accepts <paramref name="visitor"/>, passing <paramref name="args"/> to it, and returning the result. Basically
    /// an alias for invoking <c>visitor.Visit(this, args)</c>.
    /// <br/>
    /// </summary>
    /// <seealso cref="IFeatureVisitor{TArgs}"/>
    [Access(Other = AccessPermissions.Execute)]
    public abstract void Accept<TArgs>(IFeatureVisitor<TArgs> visitor, TArgs args);

    /// <summary>
    /// Check if the condition for this selector are met.
    /// </summary>
    public bool CheckConditions(EntityCoordinates pos, IEntityManager entMan, IPrototypeManager proto, FeatureContext ctx)
    {
        if (Conditions.Count == 0)
            return true;

        if (!ConditionsApplySelf && ConditionInheritance)
            return true;

        var success = false;
        foreach (var condition in Conditions)
        {
            var res = condition.Evaluate(this, pos, entMan, proto, ctx);

            if (RequireAll && !res)
                return false; // intentional break out of loop and function

            success |= res;
        }

        return RequireAll || success;
    }
}
