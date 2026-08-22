using System.Linq;
using System.Threading.Tasks;
using Content.Shared.Procedural;
using Content.Shared.Procedural.DungeonGenerators;
using Content.Shared.Tag;
using Robust.Shared.Collections;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;
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

        var tiles = new List<(Vector2i, Tile)>(startBox.Width * startBox.Height);

        var corners = new HashSet<Vector2i>();

        var roomBoxes = SplitRecursiveBox(shapeRoom, startBox, minSize, variation, random).ToList();
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

            dungeon.AddRoom(new DungeonRoom(roomTiles, roomBox.Center, roomBox, exteriorTiles, new List<ProtoId<TagPrototype>>()));
        }

        var pickedEntrances = new HashSet<Vector2i>(dungeon.Rooms.Count * 2);

        foreach (var room in dungeon.Rooms)
        {
            ShapePickEntrances(room, random, corners, pickedEntrances, startBox);
        }

        _maps.SetTiles(_gridUid, _grid, tiles);

        dungeon.Rebuild();

        return dungeon;
    }

    private static IEnumerable<Box2i> SplitRecursiveBox(ShapeRoomDunGen shapeRoom, Box2i box, Vector2i minSize, float variation, IRobustRandom random)
    {
        DebugTools.Assert(box.IsValid());

        bool isHorizontalSplit;

        if ((float) box.Width / box.Height > 1f + variation)
            isHorizontalSplit = false;
        else if ((float) box.Height / box.Width > 1f + variation)
            isHorizontalSplit = true;
        else
            isHorizontalSplit = random.Prob(0.5f);

        if (isHorizontalSplit)
        {
            var forbiddenBoxHeight = box.Height / 2;

            // This height is excluded from both boxes and gets replaced by a wall
            var cutHeight = random.Next(box.Bottom + forbiddenBoxHeight, box.Top - forbiddenBoxHeight);

            var bottom = new Box2i(box.BottomLeft, new Vector2i(box.Right, cutHeight - 1));
            var top = new Box2i(new Vector2i(box.Left, cutHeight), box.TopRight);

            if (bottom.Width < minSize.X || bottom.Height < minSize.Y)
                yield return bottom;
            else
            {
                foreach (var recursiveBox in SplitRecursiveBox(shapeRoom, bottom, minSize, variation, random))
                {
                    yield return recursiveBox;
                }
            }

            if (top.Width < minSize.X || top.Height < minSize.Y)
                yield return top;
            else
            {
                foreach (var recursiveBox in SplitRecursiveBox(shapeRoom, top, minSize, variation, random))
                {
                    yield return recursiveBox;
                }
            }
        }
        else
        {
            var forbiddenBoxWidth = box.Width / 2;

            // This height is excluded from both boxes and gets replaced by a wall
            var cutWidth = random.Next(box.Left + forbiddenBoxWidth, box.Right - forbiddenBoxWidth);

            var left = new Box2i(box.BottomLeft, new Vector2i(cutWidth - 1, box.Top));
            var right = new Box2i(new Vector2i(cutWidth, box.Bottom), box.TopRight);

            if (left.Width < minSize.X || left.Height < minSize.Y)
                yield return left;
            else
            {
                foreach (var recursiveBox in SplitRecursiveBox(shapeRoom, left, minSize, variation, random))
                {
                    yield return recursiveBox;
                }
            }

            if (right.Width < minSize.X || right.Height < minSize.Y)
                yield return right;
            else
            {
                foreach (var recursiveBox in SplitRecursiveBox(shapeRoom, right, minSize, variation, random))
                {
                    yield return recursiveBox;
                }
            }
        }
    }

    /// <summary>
    /// Picks multiple entrances for a dungeon room.
    /// </summary>
    private static void ShapePickEntrances(DungeonRoom room, IRobustRandom random, HashSet<Vector2i> corners, HashSet<Vector2i> pickedEntrances, Box2i? shapeBounds = null)
    {
        for (int i = 1; i < 5; i++)
        {
            var j = 0;
            Vector2i? found = null;
            while (j < 30)
            {
                if (AttemptPick(i, out var candidate))
                {
                    found = candidate;
                    break;
                }

                j++;
            }

            if (found != null)
                room.Entrances.Add(found.Value);
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
