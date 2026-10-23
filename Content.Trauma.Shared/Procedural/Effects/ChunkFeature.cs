using Content.Shared.EntityEffects;
using Content.Trauma.Shared.Procedural.Components;
using Content.Trauma.Shared.Procedural.Features;
using Content.Trauma.Shared.Procedural.Systems;

namespace Content.Trauma.Shared.Procedural.Effects;

/// <summary>
/// Procedural effect that spawns a feature in a chunk context.
/// </summary>
public sealed partial class ChunkFeature : EntityEffectBase<ChunkFeature>
{
    [DataField(required: true)]
    public Feature Feature;
}

public sealed partial class ChunkFeatureEffectSystem : EntityEffectSystem<ProceduralChunkComponent, ChunkFeature>
{
    [Dependency] private ProceduralSystem _procedural = default!;
    [Dependency] private FeatureSystem _features = default!;

    protected override void Effect(Entity<ProceduralChunkComponent> ent, ref EntityEffectEvent<ChunkFeature> args)
    {
        if (_procedural.TryGetDeterministicRandom(ent.Comp.GridOwner, out var random))
            _features.SpawnFeature(args.Effect.Feature, Transform(ent).Coordinates, random);
    }
}
