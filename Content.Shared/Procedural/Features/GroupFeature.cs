namespace Content.Shared.Procedural.Features;

/// <summary>
/// Places a feature from one of the <see cref="Children"/>, based on their <see cref="Feature.Weight"/>.
/// </summary>
public sealed partial class GroupFeature : Feature
{
    [DataField(required: true)]
    public List<Feature> Children = new();

    public override void Accept<TContext>(IFeatureVisitor<TContext> visitor, TContext args) =>
        visitor.VisitGroupFeature(this, args);
}
