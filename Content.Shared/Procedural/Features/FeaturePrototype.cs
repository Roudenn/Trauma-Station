using Robust.Shared.Prototypes;

namespace Content.Shared.Procedural.Features;

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
