using System.Linq;
using Content.Shared.Decals;
using Content.Shared.EntityShapes;
using Content.Shared.EntityTable;
using Content.Shared.Maps;
using Content.Shared.Physics;
using Content.Shared.Procedural.Components;
using Content.Shared.Procedural.Features;
using Content.Shared.Random.Helpers;
using Robust.Shared.Collections;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Shared.Procedural;

/// <summary>
/// A system that allows to place <see cref="Feature"/>s at specified positions.
/// </summary>
public sealed partial class FeatureSystem : EntitySystem
{
    [Dependency] private IRobustRandom _random = default!;

    [SubscribeLocalEvent]
    private void OnMapInit(Entity<FeatureSpawnerComponent> ent, ref MapInitEvent args)
    {
        SpawnFeature(ent.Comp.Feature, Transform(ent).Coordinates);
    }

    /// <summary>
    /// Spawns a <see cref="Feature"/> on specified coordinates.
    /// </summary>
    /// <param name="table">The feature to spawn.</param>
    /// <param name="centerCoords">Center coordinates of the feature.</param>
    /// <param name="rand">The randomizer to use. <br/> Defaults to the instance <see cref="IoCManager"/> provides.</param>
    /// <param name="ctx">The context used for evaluating conditions.</param>
    public void SpawnFeature(
        Feature? table,
        EntityCoordinates centerCoords,
        IRobustRandom? rand = null,
        FeatureContext? ctx = null)
    {
        if (table == null)
            return;

        if (!centerCoords.IsValid(EntityManager))
            return;

        rand ??= _random;
        ctx ??= new FeatureContext();
        SpawnFeatureVisitor.Instance.Visit(table, new SpawnFeatureVisitor.Args(EntityManager, ProtoMan, rand, ctx, centerCoords));
    }
}

/// <summary>
/// The implementation of <see cref="FeatureSystem.SpawnFeature(Feature?, EntityCoordinates, IRobustRandom?, FeatureContext?)"/>.
/// </summary>
sealed file class SpawnFeatureVisitor : IFeatureVisitor<SpawnFeatureVisitor.Args>
{
    private SpawnFeatureVisitor() { }
    public static readonly SpawnFeatureVisitor Instance = new();

    public record struct Args(
        EntityManager EntMan,
        IPrototypeManager ProtoMan,
        IRobustRandom Rand,
        FeatureContext Context,
        EntityCoordinates CenterCoordinates,
        Feature? LastFeature = null,
        Angle? OverrideRotation = null
    );

    private static bool Check(
        Feature feature,
        ref Args args
    )
    {
        var value = feature.CheckConditions(args.CenterCoordinates, args.EntMan, args.ProtoMan, args.Context)
                    && args.Rand.Prob(feature.Prob);

        if (!value)
            return false;

        args.LastFeature = feature;
        return true;
    }

    public void Visit(Feature feature, Args args)
    {
        if (args.LastFeature != null)
        {
            if (args.LastFeature.AlignTile)
                feature.AlignTile = true;

            if (args.LastFeature.ConditionInheritance)
            {
                feature.ConditionInheritance = true;
                feature.Conditions.UnionWith(args.LastFeature.Conditions);
            }

            feature.Rotation ??= args.LastFeature.Rotation;
        }

        if (args.OverrideRotation != null)
            feature.Rotation = args.OverrideRotation.Value;

        var amount = feature.Rolls.Get(args.Rand);
        for (int i = 0; i < amount; i++)
        {
            if (!Check(feature, ref args))
                return;

            feature.Accept(this, args);
        }
    }

    public void VisitAllFeature(AllFeature feature, Args args)
    {
        foreach (var child in feature.Children)
        {
            Visit(child, args);
        }
    }

    public void VisitEntFeature(EntFeature feature, Args args)
    {
        SpawnEnt(feature.Ent, feature, args);
    }

    public void VisitTableFeature(EntityTableFeature feature, Args args)
    {
        var tableSystem = args.EntMan.System<EntityTableSystem>();
        foreach (var protoId in tableSystem.GetSpawns(args.ProtoMan.Index(feature.Table)))
        {
            SpawnEnt(protoId, feature, args);
        }
    }

    private static void SpawnEnt(EntProtoId entId, Feature feature, Args args)
    {
        var pos = args.CenterCoordinates
            .Offset(feature.Offset.GetPosition(args.Rand));

        if (feature.AlignTile)
            pos = pos.AlignWithClosestGridTile(entityManager: args.EntMan);

        var ent = args.EntMan.PredictedSpawnAtPosition(entId, pos);
        var transformSys = args.EntMan.System<SharedTransformSystem>();
        transformSys.SetLocalRotation(ent, feature.Rotation ?? Angle.Zero);

        var entry = new EntFeatureEntry(
            ent,
            pos.ToVector2i(args.EntMan, args.EntMan.System<SharedTransformSystem>()),
            feature.Obstruct);
        args.Context.EntFeatures.Add(entry);
    }

    public void VisitTileFeature(TileFeature feature, Args args)
    {
        if (!args.EntMan.TryGetComponent(args.CenterCoordinates.EntityId, out MapGridComponent? gridComp))
            return;

        var mapSystem = args.EntMan.System<SharedMapSystem>();
        var pos = args.CenterCoordinates
            .Offset(feature.Offset.GetPosition(args.Rand))
            .AlignWithClosestGridTile(entityManager: args.EntMan);
        var tile = new Tile(args.ProtoMan.Index(feature.Tile).TileId);

        mapSystem.SetTile((args.CenterCoordinates.EntityId, gridComp), pos, tile);
        var entry = new TileFeatureEntry(
            tile,
            pos.ToVector2i(args.EntMan, args.EntMan.System<SharedTransformSystem>()),
            feature.Obstruct);
        args.Context.TileFeatures.Add(entry);
    }

    public void VisitDecalFeature(DecalFeature feature, Args args)
    {
        var decalSystem = args.EntMan.System<SharedDecalSystem>();
        var pos = args.CenterCoordinates
            .Offset(feature.Offset.GetPosition(args.Rand))
            .AlignWithClosestGridTile(entityManager: args.EntMan);

        decalSystem.TryAddDecal(
            feature.Decal,
            pos,
            out _,
            feature.Color,
            feature.Rotation,
            cleanable: feature.Clearable);

        var entry = new DecalFeatureEntry(
            feature.Decal,
            pos.ToVector2i(args.EntMan, args.EntMan.System<SharedTransformSystem>()),
            feature.Obstruct);
        args.Context.DecalFeatures.Add(entry);
    }

    public void VisitNestedFeature(NestedFeature feature, Args args)
    {
        Visit(args.ProtoMan.Index(feature.Id).Feature, args);
    }

    public void VisitShapeFeature(ShapeFeature feature, Args args)
    {
        var entShape = args.EntMan.System<EntityShapeSystem>();
        foreach (var pos in entShape.GetShape(feature.Shape, null, args.Rand))
        {
            var copyArgs = args;
            copyArgs.CenterCoordinates = copyArgs.CenterCoordinates.Offset(pos);
            Visit(feature.Feature, copyArgs);
        }
    }

    public void VisitNoneFeature(NoneFeature feature, Args args) { }

    public void VisitGroupFeature(GroupFeature feature, Args args)
    {
        var validWeightedChildren = feature.Children
            .Where(child => child.Weight >= float.Epsilon)
            .ToDictionary(child => child, child => child.Weight);

        if (validWeightedChildren.Count == 0)
            return;

        var child = SharedRandomExtensions.Pick(validWeightedChildren, args.Rand);

        Visit(child, args);
    }

    private static readonly Vector2i[] Axes = [new(1, 0), new(0, 1)];

    public void VisitLineFeature(LineFeature feature, Args args)
    {
        var turfSystem = args.EntMan.System<TurfSystem>();
        var mapSystem = args.EntMan.System<SharedMapSystem>();
        var gridUid = args.CenterCoordinates.EntityId;
        if (!args.EntMan.TryGetComponent(gridUid, out MapGridComponent? gridComp))
            return;

        var center = turfSystem.GetTileRef(args.CenterCoordinates);
        if (center == null || turfSystem.IsTileBlocked(center.Value, CollisionGroup.MobMask))
            return;

        var maxSpawns = feature.MaxSpawns.Get(args.Rand);
        var centerTile = center.Value.GridIndices;

        ValueList<Vector2i> bestPositions = default;

        foreach (var axis in Axes)
        {
            var positions = GetAxisPositions(axis);
            if (positions.Count > bestPositions.Count)
                bestPositions = positions;
        }

        SpawnFeatures(bestPositions);
        return;

        ValueList<Vector2i> GetAxisPositions(Vector2i axis)
        {
            var positions = new ValueList<Vector2i>(maxSpawns + 1) { centerTile };
            var negEnd = false;
            var posEnd = false;

            for (int i = 1; (!negEnd || !posEnd) && positions.Count < maxSpawns; i++)
            {
                if (!negEnd)
                {
                    var negTile = centerTile - axis * i;
                    var neg = turfSystem.GetTileRef(mapSystem.GridTileToLocal(gridUid, gridComp, negTile));
                    if (neg == null || turfSystem.IsTileBlocked(neg.Value, CollisionGroup.MobMask))
                        negEnd = true;
                    else
                        positions.Add(negTile);
                }

                if (!posEnd && positions.Count < maxSpawns)
                {
                    var posTile = centerTile + axis * i;
                    var pos = turfSystem.GetTileRef(mapSystem.GridTileToLocal(gridUid, gridComp, posTile));
                    if (pos == null || turfSystem.IsTileBlocked(pos.Value, CollisionGroup.MobMask))
                        posEnd = true;
                    else
                        positions.Add(posTile);
                }
            }

            return positions;
        }

        void SpawnFeatures(ValueList<Vector2i> positions)
        {
            foreach (var pos in positions)
            {
                var newArgs = args with
                {
                    CenterCoordinates = mapSystem.GridTileToLocal(gridUid, gridComp, pos),
                };
                Visit(feature.Feature, newArgs);
            }
        }
    }

    private static readonly Direction[] CardinalDirections =
    [
        Direction.North,
        Direction.East,
        Direction.South,
        Direction.West,
    ];

    public void VisitWallMountFeature(WallMountFeature feature, Args args)
    {
        var grid = args.CenterCoordinates.EntityId;
        if (!args.EntMan.TryGetComponent(grid, out MapGridComponent? gridComp))
            return;

        var anchor = args.EntMan.System<TurfSystem>();
        var mapSystem = args.EntMan.System<SharedMapSystem>();
        var center = args.CenterCoordinates.ToVector2i(args.EntMan, args.EntMan.System<SharedTransformSystem>());
        var i = 0;
        var isFound = false;
        Vector2i? foundPos = null;
        var direction = Direction.South;
        while (!isFound && i <= feature.MaxDistance)
        {
            foreach (var dir in CardinalDirections)
            {
                var newPos = center + dir.ToIntVec() * i;
                if (!anchor.IsTileBlocked(args.CenterCoordinates.EntityId, newPos, CollisionGroup.Impassable, gridComp))
                    continue;

                isFound = true;
                foundPos = newPos;
                direction = dir.GetOpposite();
                break;
            }

            i++;
        }

        if (foundPos == null)
            return;

        feature.Rotation = direction.ToAngle() + (feature.Rotation ?? Angle.Zero);
        var coords = mapSystem.GridTileToLocal(grid, gridComp, foundPos.Value);
        if (feature.WallOffset)
            coords = coords.Offset(direction.ToVec());

        var newArgs = args with { CenterCoordinates = coords, OverrideRotation = feature.Rotation };
        Visit(feature.Feature, newArgs);
    }
}
