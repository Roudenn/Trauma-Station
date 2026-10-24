using Content.Shared.EntityEffects;
using Content.Trauma.Shared.Procedural.ChunkRoles;
using Content.Trauma.Shared.Procedural.Components;
using Content.Trauma.Shared.Procedural.Systems;

namespace Content.Trauma.Shared.Procedural.Effects;

/// <summary>
/// When applied to a region entity, ensures a <see cref="ProceduralChunkRolesComponent"/>
/// and assigns roles to every chunk.
/// </summary>
public sealed partial class RegionAssignRoles : EntityEffectBase<RegionAssignRoles>
{
    [DataField(required: true)]
    public ChunkRoleSelector Selector;
}

public sealed partial class RegionAssignRolesEffectSystem : EntityEffectSystem<ProceduralRegionComponent, RegionAssignRoles>
{
    [Dependency] private ProceduralSystem _procedural = default!;

    private readonly ProceduralRoleSelectorVisitor _visitor = new();

    protected override void Effect(Entity<ProceduralRegionComponent> ent, ref EntityEffectEvent<RegionAssignRoles> args)
    {
        if (!_procedural.TryGetDeterministicRandom(ent.Comp.GridOwner, out var random))
            return;

        foreach (var chunk in ent.Comp.RegionData.Chunks)
        {
            if (TerminatingOrDeleted(chunk.Owner))
                continue;

            var comp = EnsureComp<ProceduralChunkRolesComponent>(chunk.Owner);

            var roles = _visitor.Visit(
                args.Effect.Selector,
                new ProceduralRoleSelectorVisitor.Args(chunk, random, ProtoMan));

            comp.Roles.IntersectWith(roles);
        }
    }
}
