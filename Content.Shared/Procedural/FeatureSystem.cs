using System.Linq;
using Content.Shared.Decals;
using Content.Shared.EntityShapes;
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
        table.Accept(
            SpawnFeatureVisitor.Instance,
            new SpawnFeatureVisitor.Args(EntityManager, ProtoMan, rand, ctx, centerCoords)
        );
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
        EntityCoordinates CenterCoordinates
    );

    public void VisitAllFeature(AllFeature feature, Args args)
    {
        if (!args.Rand.Prob(feature.Prob))
            return;

        foreach (var child in feature.Children)
        {
            if (feature.AlignTile)
                child.AlignTile = true;

            child.Accept(this, args);
        }
    }

    public void VisitEntFeature(EntFeature feature, Args args)
    {
        if (!args.Rand.Prob(feature.Prob))
            return;

        var pos = args.CenterCoordinates
            .Offset(feature.Offset.GetPosition(args.Rand))
            .AlignWithClosestGridTile(entityManager: args.EntMan);

        var ent = args.EntMan.PredictedSpawnAtPosition(feature.Ent, pos);
        var transformSys = args.EntMan.System<SharedTransformSystem>();
        transformSys.SetLocalRotation(ent, feature.Rotation);

        var entry = new EntFeatureEntry(
            ent,
            pos.ToVector2i(args.EntMan, args.EntMan.System<SharedTransformSystem>()),
            feature.Obstruct);
        args.Context.EntFeatures.Add(entry);
    }

    public void VisitTileFeature(TileFeature feature, Args args)
    {
        if (!args.Rand.Prob(feature.Prob)
            || !args.EntMan.TryGetComponent(args.CenterCoordinates.EntityId, out MapGridComponent? gridComp))
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
        if (!args.Rand.Prob(feature.Prob))
            return;

        var decalSystem = args.EntMan.System<SharedDecalSystem>();
        var pos = args.CenterCoordinates
            .Offset(feature.Offset.GetPosition(args.Rand))
            .AlignWithClosestGridTile(entityManager: args.EntMan);

        decalSystem.TryAddDecal(
            feature.Decal,
            pos,
            out _,
            feature.Color,
            feature.Angle,
            cleanable: feature.Clearable);

        var entry = new DecalFeatureEntry(
            feature.Decal,
            pos.ToVector2i(args.EntMan, args.EntMan.System<SharedTransformSystem>()),
            feature.Obstruct);
        args.Context.DecalFeatures.Add(entry);
    }

    public void VisitNestedFeature(NestedFeature feature, Args args)
    {
        if (!args.Rand.Prob(feature.Prob))
            return;

        args.ProtoMan.Index(feature.Id).Feature.Accept(this, args);
    }

    public void VisitShapeFeature(ShapeFeature feature, Args args)
    {
        if (!args.Rand.Prob(feature.Prob))
            return;

        var entShape = args.EntMan.System<EntityShapeSystem>();
        foreach (var pos in entShape.GetShape(feature.Shape, null, args.Rand))
        {
            var copyArgs = args;
            copyArgs.CenterCoordinates = copyArgs.CenterCoordinates.Offset(pos);
            feature.Feature.Accept(this, copyArgs);
        }
    }

    public void VisitNoneFeature(NoneFeature feature, Args args) { }

    public void VisitGroupFeature(GroupFeature feature, Args args)
    {
        PickFeatureGroup(this, feature.Children, feature.Prob, feature.AlignTile, args);
    }

    public void VisitLineFeature(LineFeature feature, Args args)
    {
        if (!args.Rand.Prob(feature.Prob))
            return;

        var turf = args.EntMan.System<TurfSystem>();
        var center = turf.GetTileRef(args.CenterCoordinates);
        if (center == null || turf.IsTileBlocked(center.Value, CollisionGroup.Impassable))
            return;

        var horizPos = new ValueList<EntityCoordinates>(feature.MaxSpawns + 1);
        horizPos.Add(new EntityCoordinates(center.Value.GridUid, center.Value.GridIndices));
        var leftEnd = false;
        var rightEnd = false;
        for (int i = 1; !leftEnd && !rightEnd; i++)
        {
            var left = turf.GetTileRef(new EntityCoordinates(center.Value.GridUid, center.Value.GridIndices + new Vector2i(-i, 0)));
            var right = turf.GetTileRef(new EntityCoordinates(center.Value.GridUid, center.Value.GridIndices + new Vector2i(i, 0)));

            if (left == null || turf.IsTileBlocked(left.Value, CollisionGroup.Impassable))
                leftEnd = true;
            else
                horizPos.Add(new EntityCoordinates(left.Value.GridUid, left.Value.GridIndices));

            if (right == null || turf.IsTileBlocked(right.Value, CollisionGroup.Impassable))
                rightEnd = true;
            else
                horizPos.Add(new EntityCoordinates(right.Value.GridUid, right.Value.GridIndices));

            if (horizPos.Count >= feature.MaxSpawns)
                break;
        }

        var vertPos = new ValueList<EntityCoordinates>(feature.MaxSpawns + 1);
        vertPos.Add(new EntityCoordinates(center.Value.GridUid, center.Value.GridIndices));
        var topEnd = false;
        var bottomEnd = false;
        for (int i = 1; !topEnd && !bottomEnd; i++)
        {
            var bottom = turf.GetTileRef(new EntityCoordinates(center.Value.GridUid, center.Value.GridIndices + new Vector2i(0, -i)));
            var top = turf.GetTileRef(new EntityCoordinates(center.Value.GridUid, center.Value.GridIndices + new Vector2i(0, i)));

            if (bottom == null || turf.IsTileBlocked(bottom.Value, CollisionGroup.Impassable))
                bottomEnd = true;
            else
                vertPos.Add(new EntityCoordinates(bottom.Value.GridUid, bottom.Value.GridIndices));

            if (top == null || turf.IsTileBlocked(top.Value, CollisionGroup.Impassable))
                topEnd = true;
            else
                vertPos.Add(new EntityCoordinates(top.Value.GridUid, top.Value.GridIndices));

            if (vertPos.Count >= feature.MaxSpawns)
                break;
        }

        SpawnFeatures(vertPos.Count > horizPos.Count ? vertPos : horizPos);
        return;

        void SpawnFeatures(ValueList<EntityCoordinates> positions)
        {
            foreach (var pos in positions)
            {
                var newArgs = args with { CenterCoordinates = pos };
                PickFeatureGroup(this, feature.Children, feature.Prob, feature.AlignTile, newArgs);
            }
        }
    }

    private static void PickFeatureGroup(SpawnFeatureVisitor visitor, List<Feature> features, float prob, bool alignTile, Args args)
    {
        if (!args.Rand.Prob(prob))
            return;

        var validWeightedChildren = features
            .Where(child => child.Weight >= float.Epsilon)
            .ToDictionary(child => child, child => child.Weight);

        if (validWeightedChildren.Count == 0)
            return;

        var child = SharedRandomExtensions.Pick(validWeightedChildren, args.Rand);
        if (alignTile)
            child.AlignTile = true;
        child.Accept(visitor, args);
    }
}
