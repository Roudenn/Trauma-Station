using Content.Shared.EntityEffects;
using Content.Trauma.Shared.EntityFeatures;
using Content.Trauma.Shared.EntityFeatures.Contexts;
using Content.Trauma.Shared.EntityFeatures.Features;
using Content.Trauma.Shared.Procedural.Components;
using Content.Trauma.Shared.Procedural.Systems;
using Robust.Shared.Map.Components;

namespace Content.Trauma.Shared.Procedural.Effects;

/// <summary>
/// Procedural effect that spawns a feature in a chunk context on every available tile.
/// </summary>
public sealed partial class ChunkFeatureFill : EntityEffectBase<ChunkFeatureFill>
{
    [DataField(required: true)]
    public Feature Feature;
}

public sealed partial class ChunkFeatureFillEffectSystem : EntityEffectSystem<ProceduralChunkComponent, ChunkFeatureFill>
{
    [Dependency] private ProceduralSystem _procedural = default!;
    [Dependency] private FeatureSystem _features = default!;
    [Dependency] private SharedMapSystem _map = default!;

    [Dependency] private EntityQuery<MapGridComponent> _gridQuery;

    protected override void Effect(Entity<ProceduralChunkComponent> ent, ref EntityEffectEvent<ChunkFeatureFill> args)
    {
        if (!_gridQuery.TryComp(ent.Comp.GridOwner, out var gridComp)
            || !_procedural.TryGetDeterministicRandom(ent.Comp.GridOwner, out var random))
            return;

        for (int x = 0; x < ent.Comp.ChunkData.Bounds.Right; x++)
        {
            for (int y = 0; y < ent.Comp.ChunkData.Bounds.Top; y++)
            {
                var coords = _map.GridTileToLocal(
                    ent.Comp.GridOwner,
                    gridComp,
                    new Vector2i(x, y) + ent.Comp.ChunkData.Position);
                _features.SpawnFeature(args.Effect.Feature, coords, new ChunkFeatureContext(ent.Comp.ChunkData), random);
            }
        }
    }
}
