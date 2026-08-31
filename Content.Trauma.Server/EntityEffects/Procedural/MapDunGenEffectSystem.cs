using Content.Server.Procedural;
using Content.Shared.EntityEffects;
using Content.Trauma.Shared.EntityEffects.Procedural;
using Robust.Shared.Map.Components;
using Robust.Shared.Random;

namespace Content.Trauma.Server.EntityEffects.Procedural;

public sealed partial class MapDunGenEffectSystem : EntityEffectSystem<MapGridComponent, MakeDunGen>
{
    [Dependency] private DungeonSystem _dungeon = default!;
    [Dependency] private IRobustRandom _random = default!;

    protected override void Effect(Entity<MapGridComponent> ent, ref EntityEffectEvent<MakeDunGen> args)
    {
        _dungeon.GenerateDungeon(ProtoMan.Index(args.Effect.Dungeon), ent.Owner, ent.Comp, args.Effect.Position, _random.Next());
    }
}
