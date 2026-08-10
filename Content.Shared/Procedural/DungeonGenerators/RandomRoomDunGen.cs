using Content.Shared.Maps;
using Content.Shared.ValueSelectors.Numbers;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Shared.Procedural.DungeonGenerators;

/// <summary>
/// Places rooms randomly over an area, and procedurally generates their contents.
/// </summary>
/// <remarks>
/// This dungeon generator allows to strictly define the possible sizes of the rooms,
/// but has loose control over the resulting shape of a dungeon.
/// </remarks>
public sealed partial class RandomRoomDunGen : IDunGenLayer
{
    /// <summary>
    /// Amount of rooms to generate.
    /// </summary>
    [DataField]
    public NumberSelector RoomCount = new ConstantNumberSelector(10);

    /// <summary>
    /// Specifies a size of each random room.
    /// </summary>
    [DataField(required: true)]
    public RoomSize RoomSize;

    /// <summary>
    /// Size of the area in which the rooms can be spawned.
    /// </summary>
    [DataField(required: true)]
    public Vector2i AreaSize;

    [DataField(required: true)]
    public ProtoId<ContentTileDefinition> Tile;
}

/// <summary>
/// A helper interface to specify different methods of getting a size of a room.
/// </summary>
[ImplicitDataDefinitionForInheritors]
public abstract partial class RoomSize
{
    /// <summary>
    /// Gets the size of a single room when called.
    /// </summary>
    public abstract Vector2i GetSize(IRobustRandom random);
}

/// <summary>
/// An implementation of <see cref="RoomSize"/> that lists all possible room sizes as vectors.
/// </summary>
public sealed partial class RoomSizeList : RoomSize
{
    /// <summary>
    /// Available room sizes.
    /// </summary>
    [DataField(required: true)]
    public HashSet<Vector2i> RoomSizes = new();

    public override Vector2i GetSize(IRobustRandom random)
    {
        return random.Pick(RoomSizes);
    }
}

/// <summary>
/// An implementation of <see cref="RoomSize"/> that uses
/// <see cref="NumberSelector"/>s to get the width and the height of a room.
/// </summary>
public sealed partial class RoomSizeSelector : RoomSize
{
    /// <summary>
    /// Determines the width of all rooms.
    /// </summary>
    [DataField(required: true)]
    public NumberSelector Width = new ConstantNumberSelector(5);

    /// <summary>
    /// Determines the height of all rooms.
    /// </summary>
    [DataField(required: true)]
    public NumberSelector Height = new ConstantNumberSelector(5);

    public override Vector2i GetSize(IRobustRandom random)
    {
        return new Vector2i(Width.Get(random), Height.Get(random));
    }
}
