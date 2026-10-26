namespace Content.Trauma.Shared.Procedural.Regions;

/// <summary>
/// Generates a single region in the center of a grid.
/// </summary>
public sealed partial class GridRegionGenerator : RegionGenerator
{
    [DataField(required: true)]
    public Vector2i RegionInterval;

    protected override IEnumerable<Vector2i> GetPositionsInternal(Vector2i gridCenter, Box2 loadingArea)
    {
        loadingArea.Top / RegionInterval.Y

        if (loadingArea.Contains(gridCenter))
            yield return gridCenter;
    }
}
