using Content.Shared.Random;
using Content.Shared.Tag;
using Robust.Shared.Prototypes;

namespace Content.Shared.Procedural.DungeonLayers;

/// <summary>
/// Assigns a dungeon role to a room based on its size.
/// </summary>
public sealed partial class RoomRoleDunGen : IDunGenLayer
{
    /// <summary>
    /// Minimum size of the room.
    /// </summary>
    [DataField]
    public Vector2i? MinSize;

    /// <summary>
    /// Maximum allowed size of the room.
    /// </summary>
    [DataField]
    public Vector2i? MaxSize;

    /// <summary>
    /// After all checks have passed, rolls this probability
    /// in order to finally add a tag on success.
    /// </summary>
    [DataField]
    public float Prob = 1f;

    /// <summary>
    /// Weights of tags to assign.
    /// </summary>
    [DataField(required: true)]
    public ProtoId<WeightedRandomPrototype> WeightsId;

    /// <summary>
    /// Determines whenever the role assigned by this layer should be unique,
    /// or a role can be assigned even to rooms that already have at least 1 role.
    /// </summary>
    [DataField]
    public bool AllowOther;
}
