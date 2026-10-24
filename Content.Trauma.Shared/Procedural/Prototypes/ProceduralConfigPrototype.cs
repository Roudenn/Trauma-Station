namespace Content.Trauma.Shared.Procedural.Prototypes;

/// <summary>
/// Defines a <see cref="ProceduralConfig"/>.
/// </summary>
[Prototype]
public sealed partial class ProceduralConfigPrototype : IPrototype
{
    [IdDataField]
    public string ID { get; private set; } = default!;

    [DataField, IncludeDataField]
    public ProceduralConfig Config;
}
