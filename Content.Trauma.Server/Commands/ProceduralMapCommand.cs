using Content.Server.Administration;
using Content.Shared.Administration;
using Content.Shared.Maps;
using Content.Trauma.Shared.Procedural.Prototypes;
using Content.Trauma.Shared.Procedural.Systems;
using Robust.Shared.Console;
using Robust.Shared.Map;

namespace Content.Trauma.Server.Commands;

/// <summary>
/// Creates a new procedural map and teleports the executor to it. Useful for testing.
/// </summary>
[AdminCommand(AdminFlags.Debug)]
public sealed partial class ProceduralMapCommand : LocalizedCommands
{
    [Dependency] private IEntityManager _entMan = default!;
    [Dependency] private IPrototypeManager _protoMan = default!;

    public override string Command => "proceduralmap";

    private static readonly ProtoId<ContentTileDefinition> BaseTile = "Plating";

    public override void Execute(IConsoleShell shell, string argStr, string[] args)
    {
        if (!_protoMan.TryIndex<ProceduralConfigPrototype>(args[0], out var proto))
        {
            shell.WriteLine(Help);
            return;
        }

        var mapSystem = _entMan.System<SharedMapSystem>();
        var map = mapSystem.CreateMap();
        var grid = mapSystem.CreateGridEntity(map);
        mapSystem.SetTile(grid, new Vector2i(0, 0), new Tile(_protoMan.Index(BaseTile).TileId));
        _entMan.System<ProceduralSystem>().EnsureProcedural(grid, proto);
    }
}
