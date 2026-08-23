namespace Content.Shared.Procedural.Features;

/// <summary>
/// Feature that spawns one of the children features in
/// the longest line possible until it hits an obstructed tile.
/// </summary>
public sealed partial class LineFeature : Feature
{
    [DataField(required: true)]
    public Feature Feature;

    /// <summary>
    /// Maximum amount of allowed spawns of this feature.
    /// </summary>
    [DataField]
    public int MaxSpawns = 15;

    public override void Accept<TArgs>(IFeatureVisitor<TArgs> visitor, TArgs args)
        => visitor.VisitLineFeature(this, args);
}
