// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Lavaland.Server.Procedural.Components;
using Content.Lavaland.Shared.CCVar;
using Content.Lavaland.Shared.Procedural.Components;
using Content.Server.Atmos.EntitySystems;
using Content.Server.GameTicking;
using Content.Server.Parallax;
using Content.Server.Shuttles.Systems;
using Content.Shared.GameTicking;
using Content.Shared.Mobs.Components;
using Robust.Shared.Configuration;
using Robust.Shared.Random;

// ReSharper disable EnforceForeachStatementBraces
namespace Content.Lavaland.Server.Procedural.Systems;

public sealed partial class LavalandPlanetSystem : EntitySystem
{
    public bool PlanetsEnabled = true;

    [Dependency] private SharedMapSystem _map = default!;
    [Dependency] private IRobustRandom _random = default!;
    [Dependency] private INetConfigurationManager _config = default!;
    [Dependency] private AtmosphereSystem _atmos = default!;
    [Dependency] private BiomeSystem _biome = default!;
    [Dependency] private MetaDataSystem _metaData = default!;
    [Dependency] private ShuttleSystem _shuttle = default!;

    public override void Initialize()
    {
        base.Initialize();
        Subs.CVar(_config, LavalandCVars.PlanetsEnabled, value => PlanetsEnabled = value, true);
    }

    [SubscribeLocalEvent]
    private void OnLoadingMaps(LoadingMapsEvent ev)
    {
        EnsurePreloaderMap();
        foreach (var gameMap in ev.Maps)
        {
            foreach (var planetEntry in gameMap.Planets)
            {
                SetupLavalandPlanet(planetEntry, out _);
            }
        }
    }

    [SubscribeLocalEvent]
    private void OnRoundRestart(RoundRestartCleanupEvent ev)
    {
        var ent = GetPreloaderEntity();
        if (ent == null)
            return;

        Del(ent.Value.Owner);
    }

    public void EnsurePreloaderMap()
    {
        // Already have a preloader?
        if (GetPreloaderEntity() != null
            || !PlanetsEnabled)
            return;

        var mapUid = _map.CreateMap(out var mapId, false);
        EnsureComp<LavalandPreloaderComponent>(mapUid);
        _metaData.SetEntityName(mapUid, "Lavaland Preloader Map");
        _map.SetPaused(mapId, true);
    }

    [SubscribeLocalEvent]
    private void OnPlayerParentChange(Entity<MobStateComponent> ent, ref EntParentChangedMessage args)
    {
        if (TerminatingOrDeleted(ent.Owner))
            return;

        if (args.OldParent != null
            && TryComp<LavalandGridGrantComponent>(args.OldParent.Value, out var toRemove))
            EntityManager.RemoveComponents(ent.Owner, toRemove.ComponentsToGrant);
        else if (TryComp<LavalandGridGrantComponent>(Transform(ent.Owner).GridUid, out var toGrant))
            EntityManager.AddComponents(ent.Owner, toGrant.ComponentsToGrant);
    }

    public Entity<LavalandPreloaderComponent>? GetPreloaderEntity()
    {
        var query = AllEntityQuery<LavalandPreloaderComponent>();
        while (query.MoveNext(out var uid, out var comp))
        {
            return (uid, comp);
        }

        return null;
    }

    public List<Entity<LavalandMapComponent>> GetLavalands()
    {
        var lavalandsQuery = EntityQueryEnumerator<LavalandMapComponent>();
        var lavalands = new List<Entity<LavalandMapComponent>>();
        while (lavalandsQuery.MoveNext(out var uid, out var comp))
        {
            lavalands.Add((uid, comp));
        }

        return lavalands;
    }
}
