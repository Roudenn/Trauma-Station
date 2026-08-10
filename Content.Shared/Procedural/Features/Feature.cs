using System.Numerics;
using Content.Shared.Procedural.Features.Positions;

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

    /// <summary>
    /// Local offset of this feature.
    /// </summary>
    [DataField]
    public FeaturePosition Offset = new ConstantFeaturePosition(Vector2.Zero);

    [DataField]
    public bool Obstruct;

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
}
