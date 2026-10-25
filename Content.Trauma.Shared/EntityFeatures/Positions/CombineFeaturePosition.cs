using Robust.Shared.Random;

namespace Content.Trauma.Shared.EntityFeatures.Positions;

/// <summary>
/// A <see cref="FeaturePosition"/> combined from multiple <see cref="FeaturePosition"/>s by summing them up.
/// </summary>
public sealed partial class CombineFeaturePosition : FeaturePosition
{
    public const string CombineFieldName = "combine";

    [DataField]
    public FeaturePosition[] Combine;

    public override Vector2 GetPosition<T>(IRobustRandom rand, T customContext)
    {
        var result = Vector2.Zero;
        foreach (var pos in Combine)
        {
            result += pos.GetPosition(rand, customContext);
        }

        return result;
    }
}
