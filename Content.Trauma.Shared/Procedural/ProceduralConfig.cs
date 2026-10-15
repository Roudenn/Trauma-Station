using Content.Shared.EntityEffects;
using Content.Trauma.Shared.Procedural.Components;
using Content.Trauma.Shared.Procedural.Regions;

namespace Content.Trauma.Shared.Procedural;

/// <summary>
/// Configuration that can be applied to any grid to make it procedural.
/// Can be either finite (single region) or infinite (regions are also procedurally generated).
/// </summary>
[DataDefinition]
public sealed partial class ProceduralConfig
{
    /// <summary>
    /// The main <see cref="ChunkGenerator"/> for this procedural grid.
    /// Defines how the chunks will be positioned in every region.
    /// </summary>
    [DataField(required: true)]
    public ChunkGenerator ChunkGenerator;

    /// <summary>
    /// The main <see cref="ChunkGenerator"/> for this procedural grid.
    /// Defines how many regions there will be and how will they be positioned on a grid.
    /// </summary>
    [DataField]
    public RegionGenerator RegionGenerator = new SingleRegionGenerator();

    /// <summary>
    /// Effects applied to the grid entity with a <see cref="ProceduralGridComponent"/>
    /// when the config is first applied.
    /// </summary>
    [DataField]
    public List<EntityEffect> GridEffects = new();

    /// <summary>
    /// Effects applied to every chunk entity with a <see cref="ProceduralChunkComponent"/> when they are loaded.
    /// </summary>
    [DataField]
    public List<EntityEffect> ChunkEffects = new();
}
