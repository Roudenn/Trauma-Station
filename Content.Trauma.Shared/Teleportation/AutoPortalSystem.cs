using Content.Trauma.Common.Teleportation;
using JetBrains.Annotations;
using Robust.Shared.Map.Components;
using Robust.Shared.Random;

namespace Content.Trauma.Shared.Teleportation;

public sealed partial class AutoPortalSystem : EntitySystem
{
    [Dependency] private EntityLookupSystem _lookup = default!;
    [Dependency] private IRobustRandom _random = default!;

    [Dependency] private EntityQuery<MapGridComponent> _mapGridQuery;
    [Dependency] private EntityQuery<MapComponent> _mapQuery;
    [Dependency] private EntityQuery<AutoPortalComponent> _autoPortalQuery;

    private readonly HashSet<Entity<AutoPortalComponent>> _cache = new();

    [SubscribeLocalEvent]
    private void OnOverrideTarget(Entity<AutoPortalComponent> ent, ref PortalOverrideTargetEvent args)
    {
        _cache.Clear();

        foreach (var grid in ent.Comp.LinkedGrids)
        {
            _lookup.GetGridEntities(grid, _cache);
        }

        foreach (var map in ent.Comp.LinkedMaps)
        {
            _lookup.GetEntitiesOnMap(map, _cache);
        }

        if (_cache.Count > 0)
            args.Target = _random.Pick(_cache).Owner;
    }

    [SubscribeLocalEvent]
    public void OnPortalInit(Entity<AutoPortalComponent> ent, ref MapInitEvent args)
    {
        if (ent.Comp.AutoLinkTag == null)
            return;

        var query = EntityQueryEnumerator<AutoPortalMapGridComponent>();
        while (query.MoveNext(out var uid, out var comp))
        {
            if (comp.LinkTag != ent.Comp.AutoLinkTag.Value)
                continue;

            LinkTo(ent.AsNullable(), uid);
        }
    }

    /// <summary>
    /// Links an <see cref="AutoPortalComponent"/> to a grid or map.
    /// </summary>
    [PublicAPI]
    public bool LinkTo(Entity<AutoPortalComponent?> ent, EntityUid location)
    {
        if (_mapGridQuery.HasComp(location))
            return LinkToGrid(ent, location);

        if (_mapQuery.TryComp(location, out var mapComp))
            return LinkToMap(ent, (location, mapComp));

        return false;
    }

    /// <summary>
    /// Links an <see cref="AutoPortalComponent"/> to a map.
    /// </summary>
    [PublicAPI]
    public bool LinkToMap(Entity<AutoPortalComponent?> ent, Entity<MapComponent?> map)
    {
        if (!_mapQuery.Resolve(map.Owner, ref map.Comp))
            return false;

        EnsureComp(ref ent);
        return ent.Comp!.LinkedMaps.Add(map.Comp.MapId);
    }

    /// <summary>
    /// Links an <see cref="AutoPortalComponent"/> to a grid.
    /// </summary>
    [PublicAPI]
    public bool LinkToGrid(Entity<AutoPortalComponent?> ent, EntityUid grid)
    {
        EnsureComp(ref ent);
        return ent.Comp!.LinkedGrids.Add(grid);
    }

    /// <summary>
    /// Unlinks a grid or map from an <see cref="AutoPortalComponent"/> entity.
    /// </summary>
    [PublicAPI]
    public bool Unlink(Entity<AutoPortalComponent?> ent, EntityUid location)
    {
        if (!_autoPortalQuery.Resolve(ent.Owner, ref ent.Comp, false))
            return false;

        if (ent.Comp.LinkedGrids.Remove(location))
            return true;

        return _mapQuery.TryComp(location, out var mapComp)
               && ent.Comp.LinkedMaps.Remove(mapComp.MapId);
    }
}
