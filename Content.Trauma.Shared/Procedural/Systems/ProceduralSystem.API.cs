using System.Diagnostics.CodeAnalysis;
using Content.Trauma.Common.Utilities;
using Content.Trauma.Shared.Procedural.Components;
using Content.Trauma.Shared.Procedural.Prototypes;
using JetBrains.Annotations;

namespace Content.Trauma.Shared.Procedural.Systems;

public sealed partial class ProceduralSystem
{
    [PublicAPI]
    public void EnsureProcedural(EntityUid grid, ProtoId<ProceduralConfigPrototype> config)
    {
        EnsureProcedural(grid, ProtoMan.Index(config).Config);
    }

    [PublicAPI]
    public void EnsureProcedural(EntityUid grid, ProceduralConfig config)
    {
        var comp = EnsureComp<ProceduralGridComponent>(grid);
        comp.Config = config;
        comp.Random = new DeterministicRandom(config.Seed ?? (ulong) _random.NextLong());
    }

    [PublicAPI]
    public bool TryGetDeterministicRandom(
        Entity<ProceduralGridComponent?> grid,
        [NotNullWhen(true)] out DeterministicRandom? random)
    {
        random = null;
        if (!_proceduralGridQuery.Resolve(grid.Owner, ref grid.Comp))
            return false;

        random = grid.Comp.Random;
        return true;
    }
}
