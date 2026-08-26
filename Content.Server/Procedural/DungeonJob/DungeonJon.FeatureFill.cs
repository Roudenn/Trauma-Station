using System.Threading.Tasks;
using Content.Shared.Procedural;
using Content.Shared.Procedural.DungeonLayers;
using Robust.Shared.Random;

namespace Content.Server.Procedural.DungeonJob;

public sealed partial class DungeonJob
{
    /// <summary>
    /// <see cref="FeatureFillDunGen"/>
    /// </summary>
    private async Task PostGen(
        FeatureFillDunGen gen,
        Dungeon dungeon,
        HashSet<Vector2i> reservedTiles,
        IRobustRandom random)
    {
        var featureSystem = _entManager.System<FeatureSystem>();

        foreach (var room in dungeon.Rooms)
        {
            foreach (var tile in room.Tiles)
            {
                if (reservedTiles.Contains(tile))
                    continue;

                if (!_maps.TryGetTileDef(_grid, tile, out _))
                    continue;

                if (!gen.CheckConditions(_entManager, _prototype, room))
                    continue;

                if (!_anchorable.TileFree((_gridUid, _grid), tile, DungeonSystem.CollisionLayer, DungeonSystem.CollisionMask))
                    continue;

                var gridPos = _maps.GridTileToLocal(_gridUid, _grid, tile);
                featureSystem.SpawnFeature(gen.Feature, gridPos, random, dungeon.FeatureContext);
            }

            await SuspendDungeon();
            if (!ValidateResume())
                break;
        }
    }
}
