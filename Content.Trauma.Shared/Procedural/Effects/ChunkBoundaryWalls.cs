using Content.Shared.EntityEffects;
using Content.Shared.EntityTable;
using Content.Shared.EntityTable.EntitySelectors;
using Content.Trauma.Shared.Procedural.Components;
using Content.Trauma.Shared.Procedural.Systems;
using Robust.Shared.Map.Components;
using Robust.Shared.Random;

namespace Content.Trauma.Shared.Procedural.Effects;

/// <summary>
/// Procedural effect that spawns a boundary wall around a chunk.
/// </summary>
public sealed partial class ChunkBoundaryWalls : EntityEffectBase<ChunkBoundaryWalls>
{
    /// <summary>
    /// Entity table for each wall entity.
    /// </summary>
    [DataField(required: true)]
    public EntityTableSelector Table;

    /// <summary>
    /// How much to enlarge the original chunk boundaries.
    /// </summary>
    [DataField]
    public int Enlargement;
}

public sealed partial class ChunkBoundaryWallsEffectSystem : EntityEffectSystem<ProceduralChunkComponent, ChunkBoundaryWalls>
{
    [Dependency] private SharedMapSystem _map = default!;
    [Dependency] private EntityTableSystem _table = default!;
    [Dependency] private ProceduralSystem _procedural = default!;

    [Dependency] private EntityQuery<MapGridComponent> _gridQuery;

    private readonly List<EntityUid> _cache = new();

    private readonly List<EntProtoId> _tableCache = new();

    protected override void Effect(Entity<ProceduralChunkComponent> ent, ref EntityEffectEvent<ChunkBoundaryWalls> args)
    {
        if (!_gridQuery.TryComp(ent.Comp.GridOwner, out var gridComp)
            || !_procedural.TryGetDeterministicRandom(ent.Comp.GridOwner, out var random))
            return;

        var box = ent.Comp.ChunkData.Bounds.Enlarged(args.Effect.Enlargement);
        var chunkGridCenter = ent.Comp.ChunkData.Position;

        for (int x = 0; x < box.Right; x++)
        {
            SpawnWall((ent.Comp.GridOwner, gridComp), new Vector2i(x, 0) + chunkGridCenter, args.Effect.Table, random);
        }
        for (int y = 0; y < box.Top; y++)
        {
            SpawnWall((ent.Comp.GridOwner, gridComp), new Vector2i(0, y) + chunkGridCenter, args.Effect.Table, random);
        }

        for (int x = box.Right - 1; x >= 0; x--)
        {
            SpawnWall((ent.Comp.GridOwner, gridComp), new Vector2i(x, box.Top) + chunkGridCenter, args.Effect.Table, random);
        }

        for (int y = box.Top - 1; y >= 0; y--)
        {
            SpawnWall((ent.Comp.GridOwner, gridComp), new Vector2i(box.Right, y) + chunkGridCenter, args.Effect.Table, random);
        }
    }

    private void SpawnWall(Entity<MapGridComponent> grid, Vector2i gridIndices, EntityTableSelector table, IRobustRandom random)
    {
        _cache.Clear();
        _map.GetAnchoredEntities(grid, gridIndices, _cache);
        if (_cache.Count != 0)
            return;

        _tableCache.Clear();
        _tableCache.AddRange(_table.GetSpawns(table, random));
        foreach (var entId in _tableCache)
        {
            var coords = _map.GridTileToLocal(grid.Owner, grid.Comp, gridIndices);
            Spawn(entId, coords);
        }
    }
}
