using System.Numerics;
using System.Threading.Tasks;
using Content.Shared.Procedural;
using Content.Shared.Procedural.DungeonGenerators;
using Content.Shared.Tag;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Server.Procedural.DungeonJob;

public sealed partial class DungeonJob
{
    private async Task<Dungeon> GenerateRandomDunGen(Vector2i position, RandomRoomDunGen randomRoom, HashSet<Vector2i> reservedTiles, IRobustRandom random)
    {
        var dungeon = new Dungeon();
        var tiles = new List<(Vector2i, Tile)>();

        // Pick the rooms to generate
        var count = randomRoom.RoomCount.Get(random);
        var roomDimensions = new Vector2i[count];

        for (int i = 0; i < count; i++)
        {
            var size = randomRoom.RoomSize.GetSize(random);
            roomDimensions[i] = size;
        }

        // Place the rooms down on random positions inside the area
        var areaBox = (Box2i) Box2.CenteredAround(position, randomRoom.AreaSize).Rounded(0);
        var roomSizes = new List<Box2i>(count);

        const int maxAttempts = 50;
        for (int i = 0; i < count; i++)
        {
            var dimensions = roomDimensions[i];
            Box2i? foundBox = null;
            for (int j = 0; j < maxAttempts; j++)
            {
                var x = random.Next(areaBox.Left, areaBox.Right);
                var y = random.Next(areaBox.Bottom, areaBox.Top);
                var pos = new Vector2i(x, y);
                var box = new Box2i(Vector2i.Zero, dimensions).Translated(pos);
                if (!areaBox.Contains(box))
                    continue;

                var isIntersect = false;
                foreach (var other in roomSizes)
                {
                    if (!other.Intersects(box))
                        continue;

                    isIntersect = true;
                    break;
                }

                if (isIntersect)
                    continue;

                foundBox = box;
                break;
            }

            if (foundBox != null)
                roomSizes.Add(foundBox.Value);
        }

        // Place the actual tiles and add the room
        foreach (var roomSize in roomSizes)
        {
            for (var x = roomSize.Left; x < roomSize.Right; x++)
            {
                for (var y = roomSize.Bottom; y < roomSize.Top; y++)
                {
                    var index = (new Vector2(x, y) + _grid.TileSizeHalfVector + position).Floored();

                    if (reservedTiles.Contains(index))
                        continue;

                    tiles.Add((index, new Tile(_tileDefManager[randomRoom.Tile].TileId)));
                }
            }

            _maps.SetTiles(_gridUid, _grid, tiles);
            tiles.Clear();

            // TODO move the code below to DungeonSystem (and replace it in the prefab dungen)
            var roomCenter = (roomSize.BottomLeft + roomSize.Size / 2f) * _grid.TileSize;
            var roomTiles = new HashSet<Vector2i>(roomSize.Width * roomSize.Height);
            var exterior = new HashSet<Vector2i>(roomSize.Width * 2 + roomSize.Height * 2);
            var tileOffset = -roomCenter + _grid.TileSizeHalfVector;
            Box2i? mapBounds = null;

            for (var x = -1; x <= roomSize.Width; x++)
            {
                for (var y = -1; y <= roomSize.Height; y++)
                {
                    if (x != -1 && y != -1 && x != roomSize.Width && y != roomSize.Height)
                    {
                        continue;
                    }

                    var tilePos = (new Vector2i(x + roomSize.Width, y + roomSize.Height) + tileOffset + position).Floored();

                    if (reservedTiles.Contains(tilePos))
                        continue;

                    exterior.Add(tilePos);
                }
            }

            var center = Vector2.Zero;

            for (var x = 0; x < roomSize.Width; x++)
            {
                for (var y = 0; y < roomSize.Height; y++)
                {
                    var roomTile = new Vector2i(x + roomSize.Width, y + roomSize.Height);
                    var tilePos = roomTile + tileOffset + position;
                    var tileIndex = tilePos.Floored();
                    roomTiles.Add(tileIndex);

                    mapBounds = mapBounds?.Union(tileIndex) ?? new Box2i(tileIndex, tileIndex);
                    center += tilePos + _grid.TileSizeHalfVector;
                }
            }

            center /= roomTiles.Count;

            dungeon.AddRoom(new DungeonRoom(roomTiles, center, mapBounds!.Value, exterior, new List<ProtoId<TagPrototype>>()));

            await SuspendDungeon();

            if (!ValidateResume())
                return Dungeon.Empty;
        }

        foreach (var room in dungeon.Rooms)
        {
            SetDungeonEntrance(dungeon, room, reservedTiles, random);
        }

        dungeon.Rebuild();

        return dungeon;
    }
}
