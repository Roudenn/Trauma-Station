// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Lavaland.Shared.Procedural.Prototypes;

namespace Content.Lavaland.Shared.Procedural.Components;

[RegisterComponent, NetworkedComponent]
public sealed partial class LavalandMapComponent : Component
{
    [ViewVariables]
    public List<EntityUid> SpawnedGrids;

    [ViewVariables]
    public int Seed;

    [ViewVariables]
    public ProtoId<PlanetPrototype>? PrototypeId;

    /// <summary>
    /// Chunks in this area are always loaded
    /// </summary>
    [ViewVariables]
    public Box2 LoadArea;

    /// <summary>
    /// Currently active chunks
    /// </summary>
    [DataField("loadedChunks")]
    public HashSet<Vector2i> LoadedChunks = new();

    [DataField]
    public ProtoId<LavalandLayoutPrototype>? Layout;

    [DataField]
    public ProtoId<LavalandRuinPoolPrototype>? Ruins;
}
