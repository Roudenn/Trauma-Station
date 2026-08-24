using System.Threading.Tasks;
using Content.Shared.Procedural;
using Content.Shared.Procedural.DungeonLayers;
using Content.Shared.Random.Helpers;
using Robust.Shared.Random;

namespace Content.Server.Procedural.DungeonJob;

public sealed partial class DungeonJob
{
    /// <summary>
    /// <see cref="RoomRoleDunGen"/>
    /// </summary>
    private async Task PostGen(
        RoomRoleDunGen gen,
        Dungeon dungeon,
        HashSet<Vector2i> reservedTiles,
        IRobustRandom random)
    {
        foreach (var room in dungeon.Rooms)
        {
            if (!gen.AllowOther && room.Roles.Count > 0)
                continue;

            if (gen.MinSize != null && gen.MinSize.Value >= room.Bounds.Size)
                continue;

            if (gen.MaxSize != null && gen.MaxSize.Value <= room.Bounds.Size)
                continue;

            if (!random.Prob(gen.Prob))
                continue;

            room.Roles.Add(_prototype.Index(gen.WeightsId).Pick(random));
        }
    }
}
