using System.Linq;
using Content.Shared.Decals;
using Content.Shared.EntityShapes;
using Content.Shared.Procedural.Components;
using Content.Shared.Procedural.Features;
using Content.Shared.Random.Helpers;
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
        if (!args.Rand.Prob(feature.Prob))
            return;

        var validWeightedChildren = feature.Children
            .Where(child => child.Weight >= float.Epsilon)
            .ToDictionary(child => child, child => child.Weight);

        if (validWeightedChildren.Count == 0)
            return;

        var child = SharedRandomExtensions.Pick(validWeightedChildren, args.Rand);
        if (feature.AlignTile)
            child.AlignTile = true;
        child.Accept(this, args);
    }
}
