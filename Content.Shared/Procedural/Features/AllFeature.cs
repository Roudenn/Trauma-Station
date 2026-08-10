namespace Content.Shared.Procedural.Features;

/// <summary>
/// Places all <see cref="Children"/> in its spot.
/// </summary>
public sealed partial class AllFeature : Feature
{
    [DataField(required: true)]
    public List<Feature> Children = new();

    public override void Accept<TContext>(IFeatureVisitor<TContext> visitor, TContext args) =>
        visitor.VisitAllFeature(this, args);
}
