using System.Threading;
using Content.Trauma.Common.Utilities;
using Content.Trauma.Shared.Procedural.Components;
using Content.Trauma.Shared.Procedural.Regions;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Player;
using Robust.Shared.Random;
using Robust.Shared.Threading;
using Robust.Shared.Timing;

namespace Content.Trauma.Shared.Procedural.Systems;

/// <summary>
/// Handles procedural generation of entities and features.
/// Instead of generating everything all at once, layouts of large <see cref="Region"/>s are precalculated,
/// and small parts called <see cref="Chunk"/>s are generated dynamically when a player comes close.
/// <para>
/// Useful both for infinite dimensions and for large in-world structures.
/// </para>
/// </summary>
public sealed partial class ProceduralSystem : EntitySystem
{
    [Dependency] private IGameTiming _timing = default!;
    [Dependency] private IRobustRandom _random = default!;
    [Dependency] private IParallelManager _parallel = default!;
    [Dependency] private SharedTransformSystem _xform = default!;
    [Dependency] private SharedMapSystem _map = default!;

    [Dependency] private EntityQuery<ProceduralGridComponent> _proceduralGridQuery;

    private GenerateRegionsJob _regionsJob;

    /// <summary>
    /// Dictionary cache for all loading areas on all maps.
    /// </summary>
    private readonly Dictionary<MapId, List<Box2>> _loadAreasCache = new();

    private readonly HashSet<Vector2i> _regionPosCache = new();

    private Vector2i[] _regionPosArray = [];

    private Region[] _regionCache = [];

    /// <summary>
    /// Maximum time that can be spent on procedural loading.
    /// </summary>
    private TimeSpan _maxChunkProcessTime = TimeSpan.FromMilliseconds(30);

    private const float DefaultLoadingArea = 16f;

    private ThreadLocal<ProceduralRegionVisitor> _regionVisitors = new();

    public override void Initialize()
    {
        base.Initialize();
        _regionVisitors = new ThreadLocal<ProceduralRegionVisitor>(() => new ProceduralRegionVisitor());
        _regionsJob = new GenerateRegionsJob { Visitors = _regionVisitors };
    }

    public override void Shutdown()
    {
        base.Shutdown();
        _regionVisitors.Dispose();
    }

    // TODO PROCEDURAL FUTURE:
    // - Add support for chunk unloading
    // - Reduce the real loading radius to somewhat bigger than the interaction range, and load the rest client-only
    // (requires predicted spawning V2 or even a specific engine PR to allow that)
    // - Optimizations for tile spawning
    // Currently there are too many entities and tile changes made at once which are server-only,
    // then networks everything one-by-one. Instead it's better to send the seed and generate everything on the client deterministically.
    // This is a fundamental flaw that makes procedural generation very slow.
    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        // TODO PROCEDURAL:
        // Consider not recalculating the chunks that have to be loaded
        // This requires a deep rethinking and reorganization of the code below

        // Find all players on all maps and get their center positions and loading areas.
        _loadAreasCache.Clear();
        foreach (var area in _loadAreasCache.Values)
        {
            area.Clear();
        }

        var playerQuery = EntityQueryEnumerator<ActorComponent, TransformComponent>();
        while (playerQuery.MoveNext(out _, out _, out var playerXform))
        {
            if (!_loadAreasCache.ContainsKey(playerXform.MapID))
                _loadAreasCache.TryAdd(playerXform.MapID, new List<Box2>());

            _loadAreasCache[playerXform.MapID]
                .Add(
                Box2.CenteredAround(
                _xform.GetWorldPosition(playerXform),
                new Vector2(DefaultLoadingArea)));
        }

        // Get all regions and chunks that have to be generated based on which areas are currently loaded.
        var gridQuery = EntityQueryEnumerator<ProceduralGridComponent, MapGridComponent, TransformComponent>();
        while (gridQuery.MoveNext(out var gridUid, out var gridComp, out var mapGridComp, out var gridXform))
        {
            if (!_loadAreasCache.TryGetValue(gridXform.MapID, out var loaders))
                continue; // No active loaders on the map

            // Generate the regions that must be available before chunks can be generated.
            _regionPosCache.Clear();

            foreach (var box in loaders)
            {
                _regionPosCache.UnionWith(gridComp.Config.RegionGenerator.GetPositions(Vector2i.Zero, box.Center));
            }

            if (_regionPosCache.Count == 0)
                continue;

            _regionPosArray.AsSpan().Clear();
            Extensions.EnsureLength(ref _regionPosArray, _regionPosCache.Count);

            var i = 0;
            foreach (var pos in _regionPosCache)
            {
                if (gridComp.Regions.ContainsKey(pos))
                    continue;

                _regionPosArray[i] = pos;
                i++;
            }

            Extensions.EnsureLength(ref _regionCache, i + 1);

            _regionsJob.Generator = gridComp.Config.ChunkGenerator;
            _regionsJob.Random = gridComp.Random;
            _regionsJob.Positions = _regionPosArray;
            _regionsJob.Result = _regionCache;
            _parallel.ProcessNow(_regionsJob, _regionPosCache.Count);

            for (int j = 0; j < _regionPosArray.Length; j++)
            {
                var region = _regionCache[j];
                gridComp.Regions.Add(_regionPosArray[j], region);
                var pos = _map.ToCoordinates(gridUid, region.GridIndices, mapGridComp);
                var ent = Spawn("", pos);
                region.RegionEnt = ent;
            }

            // Generate chunks
            foreach (var (regionIndices, region) in gridComp.Regions)
            {
                foreach (var chunk in region.Chunks)
                {
                    // Drop the task when out of time.
                    if (_timing.RealTime - _timing.LastTick > _maxChunkProcessTime)
                        return;

                    var chunkBox = Box2.CenteredAround(
                        _map.GridTileToWorldPos(gridUid, mapGridComp, regionIndices) + chunk.Position,
                        chunk.Bounds.Size);
                    bool doLoad = false;
                    foreach (var loadBox in loaders)
                    {
                        if (loadBox.Intersects(chunkBox))
                            doLoad = true;
                    }

                    if (!doLoad)
                        continue;

                    var pos = _map.GridTileToLocal(gridUid, mapGridComp, region.GridIndices).Offset(chunk.Position);
                    var chunkEnt = Spawn(null, pos);
                    var chunkComp = EnsureComp<ProceduralChunkComponent>(chunkEnt);
                    chunkComp.GridOwner = gridUid;
                    chunkComp.ChunkData = chunk;


                }
            }
        }
    }

    private record struct GenerateRegionsJob() : IParallelRobustJob
    {
        public ChunkGenerator Generator = default!;
        public DeterministicRandom Random = default!;
        public Vector2i[] Positions = default!;
        public Region[] Result = default!;
        public ThreadLocal<ProceduralRegionVisitor> Visitors = default!;

        public int BatchSize => 1;

        public void Execute(int index)
        {
            Visitors.Value ??= new ProceduralRegionVisitor();
            var pos = Positions[index];
            var region = Generator.Accept(Visitors.Value, new ProceduralRegionVisitor.Args(Random, pos));
            Result[index] = region;
        }
    }
}
