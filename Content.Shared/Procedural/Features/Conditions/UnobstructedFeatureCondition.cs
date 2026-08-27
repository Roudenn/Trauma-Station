using Robust.Shared.Map;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;

namespace Content.Shared.Procedural.Features.Conditions;

public sealed partial class UnobstructedFeatureCondition : FeatureCondition
{
    /// <summary>
    /// Feature type flags to check.
    /// </summary>
    [DataField(customTypeSerializer: typeof(FlagSerializer<FeatureTypeClass>))]
    public int Type = 1;

    protected override bool EvaluateImplementation(
        Feature root,
        EntityCoordinates position,
        IEntityManager entMan,
        IPrototypeManager proto,
        FeatureContext ctx)
    {
        var intPos = position.ToVector2i(entMan, entMan.System<SharedTransformSystem>());

        if (ctx.Obstructed.Contains(intPos))
            return false;

        var flags = (FeatureType) Type;
        foreach (var type in Enum.GetValues<FeatureType>())
        {
            if (!flags.HasFlag(type))
                continue;

            switch (type)
            {
                case FeatureType.Entity:
                    foreach (var entry in ctx.EntFeatures)
                    {
                        if (entry.Obstructed && entry.GridIndices == intPos)
                            return false;
                    }
                    break;
                case FeatureType.Tile:
                    foreach (var entry in ctx.TileFeatures)
                    {
                        if (entry.Obstructed && entry.GridIndices == intPos)
                            return false;
                    }
                    break;
                case FeatureType.Decal:
                    foreach (var entry in ctx.DecalFeatures)
                    {
                        if (entry.Obstructed && entry.GridIndices == intPos)
                            return false;
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        return true;
    }

    protected override bool Equals(FeatureCondition other)
    {
        return other is UnobstructedFeatureCondition condition && Type == condition.Type;
    }
}

public sealed class FeatureTypeClass;

[Flags, FlagsFor(typeof(FeatureTypeClass))]
public enum FeatureType
{
    Entity = 1 << 0,
    Tile = 1 << 1,
    Decal = 1 << 2,
}
