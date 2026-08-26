using System.Threading.Tasks;
using Content.Shared.Procedural;
using Content.Shared.Procedural.DungeonLayers;
using Robust.Shared.Map;
using Robust.Shared.Random;

namespace Content.Server.Procedural.DungeonJob;

public sealed partial class DungeonJob
{
    /// <summary>
    /// <see cref="FeatureDunGen"/>
    /// </summary>
    private async Task PostGen(
        FeatureDunGen gen,
        Dungeon dungeon,
        HashSet<Vector2i> reservedTiles,
        IRobustRandom random)
    {
        var featureSystem = _entManager.System<FeatureSystem>();

        foreach (var room in dungeon.Rooms)
        {
            if (!gen.CheckConditions(_entManager, _prototype, room))
                continue;

            var pos = gen.Position.GetPosition(_entManager, _prototype, random, room);
            featureSystem.SpawnFeature(gen.Feature, new EntityCoordinates(_gridUid, pos), random, dungeon.FeatureContext);

            await SuspendDungeon();

            if (!ValidateResume())
                return;
        }
    }
}
