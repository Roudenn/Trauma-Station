using System.Linq;
using System.Threading.Tasks;
using Content.Shared.Procedural;
using Content.Shared.Procedural.DungeonGenerators;
using Content.Shared.ValueSelectors.Numbers;
using Robust.Shared.Collections;
using Robust.Shared.Map;
using Robust.Shared.Random;
using Robust.Shared.Utility;

namespace Content.Server.Procedural.DungeonJob;

public sealed partial class DungeonJob
{
    private async Task<Dungeon> GenerateShapeDunGen(Vector2i position, ShapeRoomDunGen shapeRoom, HashSet<Vector2i> reservedTiles, IRobustRandom random)
    {
        var dungeon = new Dungeon();

        var startBox = (Box2i) Box2.CenteredAround(position, shapeRoom.AreaSize).Rounded(0);
        var minSize = new Vector2i(shapeRoom.MinRoomWidth.Get(random), shapeRoom.MinRoomHeight.Get(random));
        var variation = shapeRoom.Variation.Get(random);
        var curProb = shapeRoom.CutProb.Get(random);

        var tiles = new List<(Vector2i, Tile)>(startBox.Width * startBox.Height);

        var corners = new HashSet<Vector2i>();

        var roomBoxes = SplitBox(startBox, minSize, variation, curProb, random).ToList();
        foreach (var roomBox in roomBoxes)
        {
            var roomTiles = new HashSet<Vector2i>(roomBox.Width * roomBox.Height);
            var exteriorTiles = new HashSet<Vector2i>(roomBox.Width * 2 + roomBox.Height * 2);

            for (int x = roomBox.Left; x < roomBox.Right; x++)
            {
                for (int y = roomBox.Bottom; y < roomBox.Top; y++)
                {
                    var pos = new Vector2i(x, y);

                    if (reservedTiles.Contains(pos))
                        continue;

                    roomTiles.Add(pos);
                    tiles.Add((pos, new Tile(_prototype.Index(shapeRoom.Tile).TileId)));
                }
            }

            // Someone free me from this dungeon slop code
            for (int x = roomBox.Left - 1; x < roomBox.Right + 1; x++)
            {
                for (int y = roomBox.Bottom - 1; y < roomBox.Top + 1; y++)
                {
                    var pos = new Vector2i(x, y);

                    if (reservedTiles.Contains(pos) || roomTiles.Contains(pos))
                        continue;

                    exteriorTiles.Add(pos);
                }
            }

            corners.Add(roomBox.BottomLeft + new Vector2i(-1, -1));
            corners.Add(roomBox.BottomRight + new Vector2i(1, -1));
            corners.Add(roomBox.TopLeft + new Vector2i(-1, 1));
            corners.Add(roomBox.TopRight + new Vector2i(1, 1));

            dungeon.AddRoom(new DungeonRoom(roomTiles, roomBox.Center, roomBox, exteriorTiles));

            await SuspendDungeon();
            if (!ValidateResume())
                return Dungeon.Empty;
        }

        var pickedEntrances = new HashSet<Vector2i>(dungeon.Rooms.Count * 2);

        foreach (var room in dungeon.Rooms)
        {
            ShapePickEntrances(dungeon, room, random, corners, pickedEntrances, shapeRoom.EntranceCount, startBox);
        }

        _maps.SetTiles(_gridUid, _grid, tiles);

        dungeon.Rebuild();

        // Spawn an outer wall
        if (shapeRoom.OuterWall == null)
            return dungeon;

        var outerBox = startBox.Enlarged(2);
        var outerWalls = new List<(Vector2i, Tile)>((outerBox.Height + outerBox.Width) * 2);
        for (int x = -(outerBox.Width / 2) + 1; x < outerBox.Width / 2 - 1; x++) // Start and end 1 tile short to prevent overlapping
        {
            outerWalls.Add((new Vector2i(x, outerBox.Bottom), new Tile(_prototype.Index(shapeRoom.Tile).TileId)));
            outerWalls.Add((new Vector2i(x, outerBox.Top - 1), new Tile(_prototype.Index(shapeRoom.Tile).TileId)));
        }
        for (int y = -(outerBox.Height / 2); y < outerBox.Height / 2; y++)
        {
            outerWalls.Add((new Vector2i(outerBox.Left, y), new Tile(_prototype.Index(shapeRoom.Tile).TileId)));
            outerWalls.Add((new Vector2i(outerBox.Right - 1, y), new Tile(_prototype.Index(shapeRoom.Tile).TileId)));
        }

        _maps.SetTiles(_gridUid, _grid, outerWalls);

        await SuspendDungeon();
        if (!ValidateResume())
            return dungeon;

        foreach (var (pos, _) in outerWalls)
        {
            _entManager.SpawnAtPosition(shapeRoom.OuterWall.Value, _maps.ToCoordinates(_gridUid, pos, _grid));

            await SuspendDungeon();
            if (!ValidateResume())
                return dungeon;
        }

        return dungeon;
    }

    /// <summary>
    /// Picks multiple entrances for a dungeon room.
    /// </summary>
    private static void ShapePickEntrances(
        Dungeon dungeon,
        DungeonRoom room,
        IRobustRandom random,
        HashSet<Vector2i> corners,
        HashSet<Vector2i> pickedEntrances,
        NumberSelector count,
        Box2i? shapeBounds = null)
    {
        for (int i = 0; i < count.Get(random); i++)
        {
            var dir = random.Next(1, 4);
            var j = 0;
            Vector2i? found = null;
            while (j < 30)
            {
                if (AttemptPick(dir, out var candidate))
                {
                    found = candidate;
                    break;
                }

                j++;
            }

            if (found == null)
                continue;

            room.Entrances.Add(found.Value);

            dungeon.FeatureContext.Obstructed.Add(found.Value);
            dungeon.FeatureContext.Obstructed.Add(found.Value + new Vector2i(1, 0));
            dungeon.FeatureContext.Obstructed.Add(found.Value + new Vector2i(0, 1));
            dungeon.FeatureContext.Obstructed.Add(found.Value + new Vector2i(-1, 0));
            dungeon.FeatureContext.Obstructed.Add(found.Value + new Vector2i(0, -1));
        }

        return;

        bool AttemptPick(int index, out Vector2i candidate)
        {
            switch (index)
            {
                case 1:
                    candidate = random.Pick(new ValueList<Vector2i>(room.Exterior.Where(p => p.Y == room.Bounds.Top)));
                    break;
                case 2:
                    candidate = random.Pick(new ValueList<Vector2i>(room.Exterior.Where(p => p.Y == room.Bounds.Bottom)));
                    break;
                case 3:
                    candidate = random.Pick(new ValueList<Vector2i>(room.Exterior.Where(p => p.X == room.Bounds.Left)));
                    break;
                case 4:
                    candidate = random.Pick(new ValueList<Vector2i>(room.Exterior.Where(p => p.X == room.Bounds.Right)));
                    break;
                default:
                    candidate = random.Pick(room.Exterior);
                    return false;
            }

            // Check for corners
            if (corners.Contains(candidate))
                return false;

            // Check for other entrances (and 1 tile in all directions)
            if (pickedEntrances.Contains(candidate)
                || pickedEntrances.Contains(candidate + new Vector2i(0, 1))
                || pickedEntrances.Contains(candidate + new Vector2i(1, 0))
                || pickedEntrances.Contains(candidate + new Vector2i(0, -1))
                || pickedEntrances.Contains(candidate + new Vector2i(-1, 0)))
                return false;

            // Check for the edge of the map
            if (shapeBounds != null
                && (shapeBounds.Value.Left - 1 == candidate.X
                    || shapeBounds.Value.Right == candidate.X
                    || shapeBounds.Value.Top == candidate.Y
                    || shapeBounds.Value.Bottom - 1 == candidate.Y))
                return false;

            pickedEntrances.Add(candidate);
            return true;
        }
    }
}
