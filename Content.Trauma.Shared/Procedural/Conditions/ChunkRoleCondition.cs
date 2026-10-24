using Content.Shared.EntityConditions;
using Content.Trauma.Shared.Procedural.ChunkRoles;
using Content.Trauma.Shared.Procedural.Components;

namespace Content.Trauma.Shared.Procedural.Conditions;

/// <summary>
/// A condition that checks whether a procedural chunk has a specified role.
/// </summary>
public sealed partial class ChunkRoleCondition : EntityConditionBase<ChunkRoleCondition>
{
    [DataField(required: true)]
    public ProtoId<ChunkRolePrototype> Role;

    public override string EntityConditionGuidebookText(IPrototypeManager prototype) => "";
}

public sealed partial class ChunkRoleConditionEntityConditionSystem : EntityConditionSystem<ProceduralChunkRolesComponent, ChunkRoleCondition>
{
    protected override void Condition(Entity<ProceduralChunkRolesComponent> entity, ref EntityConditionEvent<ChunkRoleCondition> args)
    {
        args.Result = entity.Comp.Roles.Contains(args.Condition.Role);
    }
}
