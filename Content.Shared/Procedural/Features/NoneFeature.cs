namespace Content.Shared.Procedural.Features;

/// <summary>
/// A dummy feature that does nothing.
/// </summary>
public sealed partial class NoneFeature : Feature
{
    public override void Accept<TContext>(IFeatureVisitor<TContext> visitor, TContext args) =>
        visitor.VisitNoneFeature(this, args);
}
