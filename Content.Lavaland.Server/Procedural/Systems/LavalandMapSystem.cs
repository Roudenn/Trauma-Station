using Content.Lavaland.Shared.Procedural.Components;
using Content.Lavaland.Shared.Procedural.Prototypes;
using Content.Server.Decals;
using Content.Shared.Maps;
using Robust.Shared.EntitySerialization.Systems;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Random;

namespace Content.Lavaland.Server.Procedural.Systems;

/// <summary>
/// Handles <see cref="LavalandMapComponent"/> startup.
/// </summary>
public sealed partial class LavalandMapSystem : EntitySystem
{
    [Dependency] private LavalandPlanetSystem _lavalandPlanet = default!;
    [Dependency] private SharedMapSystem _map = default!;
    [Dependency] private SharedTransformSystem _transform = default!;
    [Dependency] private TileSystem _tile = default!;
    [Dependency] private ITileDefinitionManager _tiledef = default!;
    [Dependency] private IRobustRandom _random = default!;
    [Dependency] private DecalSystem _decals = default!;
    [Dependency] private EntityLookupSystem _lookup = default!;
    [Dependency] private MetaDataSystem _metaData = default!;
    [Dependency] private MapLoaderSystem _mapLoader = default!;
    [Dependency] private SharedPhysicsSystem _physics = default!;

    [Dependency] private EntityQuery<MapGridComponent> _gridQuery = default!;
    [Dependency] private EntityQuery<TransformComponent> _xformQuery = default!;
    [Dependency] private EntityQuery<FixturesComponent> _fixtureQuery = default!;

    [SubscribeLocalEvent]
    private void OnPlanetSetup(Entity<LavalandMapComponent> ent, ref PlanetSetupEvent args)
    {
        var layout = ProtoMan.Index(ent.Comp.Layout);
        var pool = ProtoMan.Index(ent.Comp.Ruins);

        _lavalandPlanet.EnsurePreloaderMap();
        var preloader = _lavalandPlanet.GetPreloaderEntity();
        if (preloader == null)
            return;

        SetupLayout(ent.Owner, Transform(ent.Owner).MapID, layout, out ent.Comp.SpawnedGrids);
        SetupRuins(pool, ent, preloader.Value);
    }

    private void SetupLayout(EntityUid lavaland, MapId lavalandMapId, LavalandLayoutPrototype? proto, out List<EntityUid> spawned)
    {
        spawned = new();

        if (proto == null)
            return; // nothing to spawn

        foreach (var layout in proto.Layouts)
        {
            if (!_mapLoader.TryLoadGrid(lavalandMapId, layout.GridPath, out var result))
            {
                Log.Error($"Failed to load grid {layout.GridPath} on planet {ToPrettyString(lavaland)}!");
                continue;
            }

            _transform.SetCoordinates(result.Value, new EntityCoordinates(lavaland, layout.Position));
            _metaData.SetEntityName(result.Value, Loc.GetString(layout.Name));

            Log.Debug($"Spawned {ToPrettyString(result.Value)} grid on planet {ToPrettyString(lavaland)}.");
            spawned.Add(result.Value);
        }
    }
}
