using Content.Server.Parallax;
using Content.Shared.EntityEffects;
using Content.Trauma.Shared.EntityEffects.Procedural;
using Robust.Shared.Map.Components;

namespace Content.Trauma.Server.EntityEffects.Procedural;

public sealed partial class MapPlanetEffectSystem : EntityEffectSystem<MapComponent, MakePlanet>
{
    [Dependency] private BiomeSystem _biome = default!;

    protected override void Effect(Entity<MapComponent> ent, ref EntityEffectEvent<MakePlanet> args)
    {
        _biome.EnsurePlanet(ent.Owner, ProtoMan.Index(args.Effect.Planet), mapLight: args.Effect.MapLight);
    }
}
