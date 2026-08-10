using Content.Shared.EntityTable.ValueSelector;
using Content.Shared.Maps;
using Robust.Shared.Prototypes;

namespace Content.Shared.Procedural.DungeonGenerators;

/// <summary>
/// Divides a provided shape into rooms, and procedurally generates room contents.
/// </summary>
/// <remarks>
/// This dungeon generator allows to make more vast and cluttered dungeons,
/// but has loose control over the size of the generated rooms.
/// </remarks>
public sealed partial class ShapeRoomDunGen : IDunGenLayer
{
    /// <summary>
    /// Size of the starting box shape which will be divided into rooms.
    /// </summary>
    [DataField(required: true)]
    public Vector2i AreaSize;

    /// <summary>
    /// Determines the minimal width of all rooms.
    /// </summary>
    [DataField(required: true)]
    public NumberSelector MinRoomWidth;

    /// <summary>
    /// Determines the minimal height of all rooms.
    /// </summary>
    [DataField(required: true)]
    public NumberSelector MinRoomHeight;

    /// <summary>
    /// Determines the percentage of the aspect ratio of a box that gets split that when surpassed
    /// forces a split in a more balanced direction instead of randomly choosing one.
    /// The lower this value is, the more rooms will try to look like accurate boxes.
    /// </summary>
    [DataField]
    public float Variation = 0.25f;

    [DataField(required: true)]
    public ProtoId<ContentTileDefinition> Tile;
}
