using Robust.Shared.Prototypes;

namespace Content.Shared.Procedural.Features;

/// <summary>
/// Spawns a feature from a provided feature prototype ID.
/// Can be used to reuse common features.
/// </summary>
public sealed partial class NestedFeature : Feature
{
    public const string IdDataFieldTag = "id";

    [DataField(required: true)]
    public ProtoId<FeaturePrototype> Id;

    public override void Accept<TContext>(IFeatureVisitor<TContext> visitor, TContext args) =>
        visitor.VisitNestedFeature(this, args);
}
