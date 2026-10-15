using Content.Shared.ValueSelectors.Floats;
using Content.Shared.ValueSelectors.Numbers;

namespace Content.Trauma.Shared.Procedural.Regions;

/// <summary>
/// Generates a rectangular area that is split using the BSP algorithm.
/// Allows to make simple room-like dungeons.
/// </summary>
public sealed partial class BSPChunkGenerator : ChunkGenerator
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
    /// Specifies the chance of a cut when it will lead to making a box that's smaller than the minimal width or height.
    /// 0 will make all rooms bigger than the minimum and 1 will make them all smaller than the minimum.
    /// </summary>
    [DataField]
    public FloatSelector CutProb = new ConstantFloatSelector(0.5f);

    /// <summary>
    /// Determines the percentage of the aspect ratio of a box that gets split that when surpassed
    /// forces a split in a more balanced direction instead of randomly choosing one.
    /// The lower this value is, the more rooms will try to look like accurate boxes.
    /// </summary>
    [DataField]
    public FloatSelector Variation = new RangeFloatSelector(new Vector2(0.2f, 0.4f));

    public override Region Accept<TContext>(IChunkGeneratorVisitor<TContext> visitor, TContext args) =>
        visitor.VisitBSPGenerator(this, args);
}
