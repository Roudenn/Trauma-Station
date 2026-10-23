using Content.Shared.EntityEffects;
using Content.Shared.Maps;
using Content.Trauma.Shared.Procedural.Components;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;

namespace Content.Trauma.Shared.Procedural.Effects;

/// <summary>
/// When applied to a region entity, fills everything inside with a specific tile.
/// </summary>
public sealed partial class RegionTileFill : EntityEffectBase<RegionTileFill>
{
    [DataField(required: true)]
    public ProtoId<ContentTileDefinition> Tile;
}

public sealed partial class RegionTileFillEffectSystem : EntityEffectSystem<ProceduralRegionComponent, RegionTileFill>
{
    [Dependency] private SharedMapSystem _map = default!;

    [Dependency] private EntityQuery<MapGridComponent> _gridQuery;

    private readonly List<(Vector2i, Tile)> _cache = new();

    protected override void Effect(Entity<ProceduralRegionComponent> ent, ref EntityEffectEvent<RegionTileFill> args)
    {
        _cache.Clear();
        var tile = new Tile(ProtoMan.Index(args.Effect.Tile).TileId);

        foreach (var chunk in ent.Comp.RegionData.Chunks)
        {
            for (int y = 0; y < chunk.Bounds.Height; y++)
            {
                for (int x = 0; x < chunk.Bounds.Height; x++)
                {
                    _cache.Add((new Vector2i(x, y), tile));
                }
            }
        }

        if (_gridQuery.TryComp(ent.Comp.GridOwner, out var gridComp))
            _map.SetTiles((ent.Comp.GridOwner, gridComp), _cache);
    }
}
