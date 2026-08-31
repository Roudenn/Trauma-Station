using Content.Shared.EntityEffects;
using Content.Shared.Parallax.Biomes;

namespace Content.Trauma.Shared.EntityEffects.Procedural;

/// <summary>
/// Turns the affected map into a planet from a provided <see cref="BiomeTemplatePrototype"/>.
/// </summary>
public sealed partial class MakePlanet : EntityEffectBase<MakePlanet>
{
    [DataField(required: true)]
    public ProtoId<BiomeTemplatePrototype> Planet;

    [DataField]
    public Color? MapLight;
}
