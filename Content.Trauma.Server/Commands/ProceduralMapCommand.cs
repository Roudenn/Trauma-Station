using Content.Server.Administration;
using Content.Shared.Administration;
using Content.Trauma.Shared.Procedural.Prototypes;
using Content.Trauma.Shared.Procedural.Systems;
using Robust.Shared.Console;

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

    public override void Execute(IConsoleShell shell, string argStr, string[] args)
    {
        if (!_protoMan.TryIndex<ProceduralConfigPrototype>(args[0], out var proto))
        {
            shell.WriteLine(Help);
            return;
        }

        var map = _entMan.System<SharedMapSystem>().CreateMap();
        _entMan.System<ProceduralSystem>().EnsureProcedural(map, proto);
    }
}
