using System.Numerics;
using Robust.Shared.Random;

namespace Content.Shared.Procedural.Features.Positions;

public sealed partial class RangeFeaturePosition : FeaturePosition
{
    [DataField]
    public Vector2 MinPos;

    [DataField]
    public Vector2 MaxPos;

    public override Vector2 GetPosition(IRobustRandom rand)
    {
        return new Vector2(rand.NextFloat(MinPos.X, MaxPos.X), rand.NextFloat(MinPos.Y, MaxPos.Y));
    }
}
