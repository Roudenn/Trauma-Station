namespace Content.Trauma.Shared.EntityFeatures.Features;

/// <summary>
/// Places a single entity.
/// </summary>
public sealed partial class EntFeature : Feature
{
    public const string EntDataFieldTag = "ent";

    [DataField]
    public EntProtoId Ent;

    public override void Accept<TContext>(IFeatureVisitor<TContext> visitor, TContext args) =>
        visitor.VisitEntFeature(this, args);
}
