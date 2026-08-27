using Content.Shared.EntityTable;
using Robust.Shared.Prototypes;

namespace Content.Shared.Procedural.Features;

/// <summary>
/// Spawns an entity table prototype directly.
/// Useful for reusing already existing tables instead of using spawners,
/// which depend on a general rando function and therefore different with the same dungeon seed.
/// </summary>
public sealed partial class EntityTableFeature : Feature
{
    public const string TableDataFieldTag = "table";

    [DataField]
    public ProtoId<EntityTablePrototype> Table;

    public override void Accept<TContext>(IFeatureVisitor<TContext> visitor, TContext args) =>
        visitor.VisitTableFeature(this, args);
}
