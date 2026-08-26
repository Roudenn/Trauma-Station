namespace Content.Shared.Procedural.Features;

/// <summary>
/// A feature that will look in all 4 cardinal directions in order to be placed on a wall.
/// Properly rotates it to face in the direction of the feature's origin.
/// </summary>
public sealed partial class WallMountFeature : Feature
{
    [DataField(required: true)]
    public Feature Feature;

    /// <summary>
    /// Max distance to search for a wall before giving up.
    /// </summary>
    [DataField]
    public int MaxDistance = 16;

    /// <summary>
    /// If true, will offset the entity by 1 tile towards the center.
    /// Useful for wallmounts that aren't actually wallmount, like lights or cameras.
    /// </summary>
    [DataField]
    public bool WallOffset;

    public override void Accept<TContext>(IFeatureVisitor<TContext> visitor, TContext args) =>
        visitor.VisitWallMountFeature(this, args);
}
