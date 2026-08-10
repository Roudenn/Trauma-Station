using System.Numerics;
using Robust.Shared.Random;

namespace Content.Shared.Procedural.Features.Positions;

public sealed partial class ConstantFeaturePosition : FeaturePosition
{
    /// <summary>
    /// Local offset of this feature.
    /// </summary>
    [DataField]
    public Vector2 Offset;

    public ConstantFeaturePosition(Vector2 value)
    {
        Offset = value;
    }

    public override Vector2 GetPosition(IRobustRandom rand)
    {
        return Offset;
    }
}
