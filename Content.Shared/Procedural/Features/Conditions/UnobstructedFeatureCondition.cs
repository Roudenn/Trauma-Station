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
    public int Type;

    protected override bool EvaluateImplementation(
        Feature root,
        EntityCoordinates position,
        IEntityManager entMan,
        IPrototypeManager proto,
        FeatureContext ctx)
    {
        var flags = (FeatureType) Type;
        foreach (var type in Enum.GetValues<FeatureType>())
        {
            if (!flags.HasFlag(type))
                continue;

            var intPos = position.ToVector2i(entMan, entMan.System<SharedTransformSystem>());
            switch (type)
            {
                case FeatureType.Entity:
                    foreach (var entry in ctx.EntFeatures)
                    {
                        if (entry.Obstructed && entry.GridIndices == intPos)
                            return true;
                    }
                    break;
                case FeatureType.Tile:
                    foreach (var entry in ctx.TileFeatures)
                    {
                        if (entry.Obstructed && entry.GridIndices == intPos)
                            return true;
                    }
                    break;
                case FeatureType.Decal:
                    foreach (var entry in ctx.DecalFeatures)
                    {
                        if (entry.Obstructed && entry.GridIndices == intPos)
                            return true;
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        return false;
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
