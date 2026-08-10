using System.Numerics;
using Robust.Shared.Random;

namespace Content.Shared.Procedural.Features.Positions;

[ImplicitDataDefinitionForInheritors]
public abstract partial class FeaturePosition
{
    public abstract Vector2 GetPosition(IRobustRandom rand);
}
