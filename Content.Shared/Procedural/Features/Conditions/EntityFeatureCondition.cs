using Content.Shared.Maps;
using Content.Shared.Whitelist;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;

namespace Content.Shared.Procedural.Features.Conditions;

/// <summary>
/// Condition that returns true if the target tile has at least one entity that passes a whitelist.
/// </summary>
public sealed partial class EntityFeatureCondition : FeatureCondition
{
    [DataField(required: true)]
    public EntityWhitelist Whitelist;

    private readonly HashSet<EntityUid> _cache = new();

    protected override bool EvaluateImplementation(Feature root,
        EntityCoordinates position,
        IEntityManager entMan,
        IPrototypeManager proto,
        FeatureContext ctx)
    {
        var whitelistSystem = entMan.System<EntityWhitelistSystem>();
        var turfSystem = entMan.System<TurfSystem>();

        _cache.Clear();
        turfSystem.GetEntitiesInTile(position, _cache, LookupFlags.All);

        foreach (var entity in _cache)
        {
            if (whitelistSystem.IsWhitelistPass(Whitelist, entity))
                return true;
        }

        return false;
    }

    protected override bool Equals(FeatureCondition other)
    {
        return other is EntityFeatureCondition;
    }
}
