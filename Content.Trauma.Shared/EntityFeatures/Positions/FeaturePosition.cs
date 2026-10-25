using Content.Trauma.Shared.EntityFeatures.Contexts;
using JetBrains.Annotations;
using Robust.Shared.Random;

namespace Content.Trauma.Shared.EntityFeatures.Positions;

[ImplicitDataDefinitionForInheritors, UsedImplicitly(ImplicitUseTargetFlags.WithInheritors)]
public abstract partial class FeaturePosition
{
    [DataField]
    public float Weight = 1f;

    public abstract Vector2 GetPosition<T>(IRobustRandom rand, T customContext) where T : IFeatureContext;
}
