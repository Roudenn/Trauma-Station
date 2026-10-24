using System.Linq;
using Content.Shared.Random.Helpers;
using Robust.Shared.Random;

namespace Content.Trauma.Shared.EntityFeatures.Positions;

/// <summary>
/// Selects a room position from a group randomly based on their weight.
/// </summary>
public sealed partial class GroupFeaturePosition : FeaturePosition
{
    [DataField(required: true)]
    public List<FeaturePosition> Children = new();

    public override Vector2 GetPosition<T>(IRobustRandom rand, T customContext)
    {
        var validWeightedChildren = Children
            .Where(child => child.Weight >= float.Epsilon)
            .ToDictionary(child => child, child => child.Weight);

        return validWeightedChildren.Count == 0
            ? Vector2.Zero
            : SharedRandomExtensions.Pick(validWeightedChildren, rand).GetPosition(rand, customContext);
    }
}
