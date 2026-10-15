namespace Content.Trauma.Shared.Procedural.Regions;

/// <summary>
/// Generates a single region in the center of a grid.
/// </summary>
public sealed partial class SingleRegionGenerator : RegionGenerator
{
    protected override IEnumerable<Vector2i> GetPositionsInternal(Vector2i gridCenter, Box2 loadingArea)
    {
        if (loadingArea.Contains(gridCenter))
            yield return gridCenter;
    }
}
