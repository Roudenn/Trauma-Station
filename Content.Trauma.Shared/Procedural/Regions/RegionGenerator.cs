namespace Content.Trauma.Shared.Procedural.Regions;

/// <summary>
/// Returns a position of new regions when the passed position is close enough to any of them.
/// </summary>
[ImplicitDataDefinitionForInheritors]
public abstract partial class RegionGenerator
{
    /// <summary>
    /// Distance around each region that's required to start calculating its layout.
    /// </summary>
    /// <remarks>
    /// The default value is a middle ground between planetary and space dungeons.
    /// If you're making a planet dungeon, it's likely better to reduce this value.
    /// If you're making a large space dungeon, it's better to increase this value.
    /// </remarks>
    [DataField]
    public float LoadRadius = 128f;

    public IEnumerable<Vector2i> GetPositions(Vector2i gridCenter, Vector2 loaderPos)
    {
        return GetPositionsInternal(gridCenter, Box2.CenteredAround(loaderPos, new Vector2(LoadRadius * 2f)));
    }

    protected abstract IEnumerable<Vector2i> GetPositionsInternal(Vector2i gridCenter, Box2 loadingArea);
}
