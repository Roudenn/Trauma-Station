using Content.Shared.EntityEffects;
using Content.Trauma.Shared.Procedural.Features;

namespace Content.Trauma.Shared.Procedural.Effects;

/// <summary>
/// Procedural effect that spawns a feature in a chunk context.
/// </summary>
public sealed partial class ChunkFeature : EntityEffectBase<ChunkFeature>
{
    [DataField(required: true)]
    public Feature Feature;
}
