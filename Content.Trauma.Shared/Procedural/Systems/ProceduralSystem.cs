using Content.Trauma.Shared.Procedural.Components;
using Content.Trauma.Shared.Procedural.Prototypes;
using Content.Trauma.Shared.Procedural.Regions;
using JetBrains.Annotations;
using Robust.Shared.Map;
using Robust.Shared.Player;
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
public sealed partial class ProceduralSystem : EntitySystem, IChunkGeneratorVisitor<ProceduralSystem.Args>
{
    [Dependency] private IGameTiming _timing = default!;
    [Dependency] private SharedTransformSystem _xform = default!;

    /// <summary>
    /// Dictionary cache for all loading areas on all maps.
    /// </summary>
    private readonly Dictionary<MapId, List<Box2>> _loadAreasCache = new();

    /// <summary>
    /// A priority queue for all chunks. Priority is the distance from the nearest player to the chunk.
    /// </summary>
    private readonly PriorityQueue<Chunk, float> _loadQueue = new();

    /// <summary>
    /// Maximum time that can be spent on procedural loading.
    /// </summary>
    private TimeSpan _maxChunkProcessTime = TimeSpan.FromMilliseconds(30);

    private const float DefaultLoadingArea = 16f;

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        // TODO consider not recalculating the chunks that have to be loaded
        // and instead somehow update an already existing queue instead

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
        var gridQuery = EntityQueryEnumerator<ProceduralGridComponent, TransformComponent>();
        while (gridQuery.MoveNext(out var gridUid, out var gridComp, out var gridXform))
        {
            gridComp.Config.RegionGenerator.GetPositions(Vector2i.Zero, );
        }

        // First the region layouts are generated in parallel.
        // They are necessary before we can get to generating actual chunks.

        var startTime = _timing.RealTime;
        while (_loadQueue.Count > 0)
        {
            // Drop loading chunks when out of time.
            if (_timing.RealTime - startTime > _maxChunkProcessTime)
                return;
        }
    }

    [PublicAPI]
    public void EnsureProceduralGrid(EntityUid grid, ProtoId<ProceduralConfigPrototype> config)
    {
        EnsureProceduralGrid(grid, ProtoMan.Index(config).Config);
    }

    [PublicAPI]
    public void EnsureProceduralGrid(EntityUid grid, ProceduralConfig config)
    {

    }
}
