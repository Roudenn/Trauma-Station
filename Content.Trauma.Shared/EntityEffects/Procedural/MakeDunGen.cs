using Content.Shared.EntityEffects;
using Content.Shared.Procedural;

namespace Content.Trauma.Shared.EntityEffects.Procedural;

/// <summary>
/// Spawns a dungeon on a map at specified coordinates.
/// </summary>
public sealed partial class MakeDunGen : EntityEffectBase<MakeDunGen>
{
    [DataField(required: true)]
    public ProtoId<DungeonConfigPrototype> Dungeon;

    [DataField]
    public Vector2i Position;
}
