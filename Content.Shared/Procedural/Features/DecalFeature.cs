using Content.Shared.Decals;
using Robust.Shared.Prototypes;

namespace Content.Shared.Procedural.Features;

/// <summary>
/// Places a single decal.
/// </summary>
public sealed partial class DecalFeature : Feature
{
    public const string DecalDataFieldTag = "decal";

    [DataField]
    public ProtoId<DecalPrototype> Decal;

    [DataField]
    public Color Color;

    [DataField]
    public Angle Angle;

    [DataField]
    public bool Clearable;

    public override void Accept<TContext>(IFeatureVisitor<TContext> visitor, TContext args) =>
        visitor.VisitDecalFeature(this, args);
}
