using Robust.Shared.Random;

namespace Content.Trauma.Shared.EntityFeatures.Positions;

public sealed partial class RangeFeaturePosition : FeaturePosition
{
    [DataField]
    public Vector2 MinPos;

    [DataField]
    public Vector2 MaxPos;

    public override Vector2 GetPosition<T>(IRobustRandom rand, T customContext)
    {
        return new Vector2(rand.NextFloat(MinPos.X, MaxPos.X), rand.NextFloat(MinPos.Y, MaxPos.Y));
    }

    public RangeFeaturePosition(Vector2 minPos, Vector2 maxPos)
    {
        MinPos = minPos;
        MaxPos = maxPos;
    }
}
