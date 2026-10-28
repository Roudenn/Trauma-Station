using Content.Trauma.Shared.Procedural.Components;
using Robust.Shared.Map.Components;

namespace Content.Trauma.Shared.Procedural.Systems;

public sealed partial class ProceduralSystem
{
    private void GenerateChunks(Entity<ProceduralGridComponent, MapGridComponent> grid, List<Box2> loaders)
    {
        foreach (var (regionIndices, region) in grid.Comp1.Regions)
        {
            foreach (var chunk in region.Chunks)
            {
                // Drop the task when out of time.
                if (_timing.RealTime - _timing.LastTick > _maxChunkProcessTime)
                    return;

                var chunkBox = Box2.CenteredAround(
                    _map.GridTileToWorldPos(grid.Owner, grid.Comp2, regionIndices) + chunk.Position,
                    chunk.Bounds.Size);
                bool doLoad = false;
                foreach (var loadBox in loaders)
                {
                    if (loadBox.Intersects(chunkBox))
                        doLoad = true;
                }

                if (!doLoad)
                    continue;

                var pos = _map.GridTileToLocal(grid.Owner, grid.Comp2, region.GridIndices).Offset(chunk.Position);
                var chunkEnt = Spawn(null, pos);
                var chunkComp = EnsureComp<ProceduralChunkComponent>(chunkEnt);
                chunkComp.GridOwner = grid.Owner;
                chunkComp.ChunkData = chunk;


            }
        }
    }
}
