using Content.Trauma.Shared.EntityFeatures.Features;

namespace Content.Trauma.Shared.EntityFeatures;

/// <summary>
/// A prototype for <see cref="Feature"/>s.
/// </summary>
[Prototype]
public sealed partial class FeaturePrototype : IPrototype
{
    /// <inheritdoc/>
    [IdDataField]
    public string ID { get; private set; } = default!;

    [DataField(required: true)]
    public Feature Feature;
}
