namespace Content.Trauma.Shared.Procedural.Regions;

/// <summary>
/// Generates a single region in the center of a grid.
/// </summary>
public sealed partial class GridRegionGenerator : RegionGenerator
{
    [DataField(required: true)]
    public Vector2i RegionInterval;

    private readonly List<int> _cacheX = new();

    private readonly List<int> _cacheY = new();

    protected override IEnumerable<Vector2i> GetPositionsInternal(Vector2i gridCenter, Box2 loadingArea)
    {
        _cacheX.Clear();
        _cacheY.Clear();

        GetDivisibleValues((int) MathF.Round(loadingArea.Left), (int) MathF.Round(loadingArea.Right), RegionInterval.X, _cacheX);
        GetDivisibleValues((int) MathF.Round(loadingArea.Bottom), (int) MathF.Round(loadingArea.Top), RegionInterval.Y, _cacheY);

        foreach (var x in _cacheX)
        {
            foreach (var y in _cacheY)
            {
                yield return new Vector2i(x, y);
            }
        }
    }

    private static void GetDivisibleValues(int min, int max, int interval, List<int> result)
    {
        if (min > max)
            (min, max) = (max, min);

        var first = min - min % interval;
        if (first < min)
            first += interval;

        for (int i = first; i < max; i += interval)
        {
            result.Add(i);
        }
    }
}
