using Content.Shared.EntityTable.EntitySelectors;
using JetBrains.Annotations;

namespace Content.Shared.Procedural.Features;

/// <summary>
/// <a href="https://en.wikipedia.org/wiki/Visitor_pattern">Visitor</a> for <see cref="Feature"/>s.
/// </summary>
/// <typeparam name="TArgs">The type of arguments passed to visitation</typeparam>
[PublicAPI]
public interface IFeatureVisitor<in TArgs>
{
    /// <summary>
    /// Alias of <see cref="Feature.Accept{TContext}(IFeatureVisitor{TContext}, TContext)"/>.
    /// </summary>
    [PublicAPI]
    void Visit(Feature selector, TArgs args) => selector.Accept(this, args);

    /// <summary>
    /// Visit an <see cref="AllSelector"/>.
    /// </summary>
    [PublicAPI]
    void VisitAllFeature(AllFeature feature, TArgs args);

    /// <summary>
    /// Visit a <see cref="GroupFeature"/>.
    /// </summary>
    [PublicAPI]
    void VisitGroupFeature(GroupFeature feature, TArgs args);

    /// <summary>
    /// Visit an <see cref="EntFeature"/>.
    /// </summary>
    [PublicAPI]
    void VisitEntFeature(EntFeature feature, TArgs args);

    /// <summary>
    /// Visit an <see cref="TileFeature"/>.
    /// </summary>
    [PublicAPI]
    void VisitTileFeature(TileFeature feature, TArgs args);

    /// <summary>
    /// Visit an <see cref="DecalFeature"/>.
    /// </summary>
    [PublicAPI]
    void VisitDecalFeature(DecalFeature feature, TArgs args);

    /// <summary>
    /// Visit a <see cref="NestedFeature"/>.
    /// </summary>
    [PublicAPI]
    void VisitNestedFeature(NestedFeature feature, TArgs args);

    /// <summary>
    /// Visit a <see cref="ShapeFeature"/>.
    /// </summary>
    [PublicAPI]
    void VisitShapeFeature(ShapeFeature feature, TArgs args);

    /// <summary>
    /// Visit a <see cref="NoneFeature"/>.
    /// </summary>
    [PublicAPI]
    void VisitNoneFeature(NoneFeature feature, TArgs args);

    /// <summary>
    /// Visit a <see cref="LineFeature"/>.
    /// </summary>
    [PublicAPI]
    void VisitLineFeature(LineFeature feature, TArgs args);
}
