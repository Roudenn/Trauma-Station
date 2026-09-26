// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Lavaland.Server.Biome;
using Content.Lavaland.Server.Procedural.Components;
using Content.Lavaland.Shared.Procedural.Components;
using Content.Lavaland.Shared.Procedural.Prototypes;
using Content.Shared.Atmos.Components;
using Content.Shared.EntityEffects;
using Content.Shared.Gravity;
using Content.Shared.Parallax.Biomes;
using Content.Shared.Salvage;
using Content.Shared.Shuttles.Components;

namespace Content.Lavaland.Server.Procedural.Systems;

public sealed partial class LavalandPlanetSystem
{
    [Dependency] private SharedEntityEffectsSystem _effects = default!;

    public bool SetupLavalandPlanet(
        ProtoId<PlanetPrototype> planetProto,
        out Entity<LavalandMapComponent>? lavaland,
        int? seed = null,
        Entity<LavalandPreloaderComponent>? preloader = null)
    {
        lavaland = null;

        if (!PlanetsEnabled)
            return false;

        if (preloader == null)
        {
            preloader = GetPreloaderEntity();
            if (preloader == null)
            {
                Log.Warning("Failed to find a preloader entity when generating a new planet!");
                return false;
            }
        }

        var prototype = ProtoMan.Index(planetProto);

        // Basic setup.
        var lavalandMap = _map.CreateMap(out var lavalandMapId, runMapInit: false);

        _effects.ApplyEffects(lavalandMap, prototype.StartupEffects);

        var mapComp = EnsureComp<LavalandMapComponent>(lavalandMap);
        lavaland = (lavalandMap, mapComp);

        var loadBox = Box2.CentredAroundZero(new Vector2(prototype.RestrictedRange * 2, prototype.RestrictedRange * 2));

        seed ??= _random.Next();

        mapComp.Seed = seed.Value;
        mapComp.PrototypeId = planetProto;
        mapComp.LoadArea = loadBox;

        EnsureComp<BiomeOptimizeComponent>(lavalandMap).LoadArea = loadBox;

        PlanetBasicSetup(lavalandMap, prototype, seed.Value);
        _map.SetPaused(lavalandMapId, true);

        var ev = new PlanetSetupEvent();
        RaiseLocalEvent(lavalandMap, ref ev);

        // Hide all grids from the mass scanner.
        foreach (var grid in _map.GetAllGrids(lavalandMapId))
        {
            var flag = IFFFlags.HideLabel;

            /*#if DEBUG || TOOLS Uncomment me when GPS is done.
            flag = IFFFlags.HideLabel;
            #endif*/

            _shuttle.AddIFFFlag(grid, flag);
        }

        _map.InitializeMap(lavalandMapId);

        _effects.ApplyEffects(lavalandMap, prototype.MapEffects);

        // Preload here to prevent biome entities from overlaying with everything else
        _biome.Preload(lavalandMap, Comp<BiomeComponent>(lavalandMap), loadBox);

        return true;
    }

    private void PlanetBasicSetup(EntityUid lavalandMap, PlanetPrototype prototype, int seed)
    {
        // Name
        _metaData.SetEntityName(lavalandMap, Loc.GetString(prototype.Name));

        // Biomes
        _biome.EnsurePlanet(lavalandMap, ProtoMan.Index(prototype.BiomePrototype), seed, mapLight: prototype.MapLight);

        // Marker Layers
        var biome = EnsureComp<BiomeComponent>(lavalandMap);
        foreach (var marker in prototype.OreLayers)
        {
            _biome.AddMarkerLayer(lavalandMap, biome, marker);
        }
        Dirty(lavalandMap, biome);

        // Gravity
        var gravity = EnsureComp<GravityComponent>(lavalandMap);
        gravity.Enabled = true;
        Dirty(lavalandMap, gravity);

        var atmos = EnsureComp<MapAtmosphereComponent>(lavalandMap);
        _atmos.SetMapGasMixture(lavalandMap, prototype.Atmosphere, atmos);

        // Restricted Range
        var restricted = EnsureComp<RestrictedRangeComponent>(lavalandMap);
        restricted.Range = prototype.RestrictedRange;
    }
}
