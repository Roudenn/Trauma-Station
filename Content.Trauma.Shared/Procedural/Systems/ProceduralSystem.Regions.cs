using System.Threading;
using Content.Trauma.Common.Utilities;
using Content.Trauma.Shared.Procedural.Components;
using Content.Trauma.Shared.Procedural.Regions;
using Robust.Shared.Map.Components;
using Robust.Shared.Threading;

namespace Content.Trauma.Shared.Procedural.Systems;

public sealed partial class ProceduralSystem
{
    private void GenerateRegions(Entity<ProceduralGridComponent, MapGridComponent> grid, List<Box2> loaders)
    {
        // Generate the regions that must be available before chunks can be generated.
        _regionPosCache.Clear();

        foreach (var box in loaders)
        {
            _regionPosCache.UnionWith(grid.Comp1.Config.RegionGenerator.GetPositions(Vector2i.Zero, box.Center));
        }

        if (_regionPosCache.Count == 0)
            return;

        _regionPosArray.AsSpan().Clear();
        Extensions.EnsureLength(ref _regionPosArray, _regionPosCache.Count);

        var i = 0;
        foreach (var pos in _regionPosCache)
        {
            if (grid.Comp1.Regions.ContainsKey(pos))
                continue;

            _regionPosArray[i] = pos;
            i++;
        }

        Extensions.EnsureLength(ref _regionCache, i + 1);

        _regionsJob.Generator = grid.Comp1.Config.ChunkGenerator;
        _regionsJob.Random = grid.Comp1.Random;
        _regionsJob.Positions = _regionPosArray;
        _regionsJob.Result = _regionCache;
        _parallel.ProcessNow(_regionsJob, _regionPosCache.Count);

        for (int j = 0; j < _regionPosArray.Length; j++)
        {
            var region = _regionCache[j];
            grid.Comp1.Regions.Add(_regionPosArray[j], region);
            var pos = _map.ToCoordinates(grid.Owner, region.GridIndices, grid.Comp2);
            var ent = Spawn(null, pos);
            var comp = EnsureComp<ProceduralRegionComponent>(ent);
            comp.GridOwner = grid.Owner;
            comp.RegionData = region;
            region.RegionEnt = ent;
        }
    }

    private record struct GenerateRegionsJob() : IParallelRobustJob
    {
        public ChunkGenerator Generator = default!;
        public DeterministicRandom Random = default!;
        public Vector2i[] Positions = default!;
        public Region[] Result = default!;
        //public ThreadLocal<ProceduralRegionVisitor> Visitors = default!;

        public int BatchSize => 1;

        public void Execute(int index)
        {
            //Visitors.Value ??= new ProceduralRegionVisitor();
            var pos = Positions[index];
            var region = Generator.Accept(new ProceduralRegionVisitor(), new ProceduralRegionVisitor.Args(Random, pos));
            Result[index] = region;
        }
    }
}
