using Content.Shared.EntityShapes.Shapes;

namespace Content.Shared.Procedural.Features;

/// <summary>
/// Spawns a <see cref="Feature"/> at each position given by an <see cref="EntityShape"/>.
/// </summary>
public sealed partial class ShapeFeature : Feature
{
    [DataField(required: true)]
    public Feature Feature;

    [DataField(required: true)]
    public EntityShape Shape;

    public override void Accept<TArgs>(IFeatureVisitor<TArgs> visitor, TArgs args)
        => visitor.VisitShapeFeature(this, args);
}
