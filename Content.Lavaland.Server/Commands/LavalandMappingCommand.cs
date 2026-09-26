// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Lavaland.Server.Procedural.Systems;
using Content.Server.Administration;
using Content.Lavaland.Shared.Procedural.Prototypes;
using Content.Shared.Administration;
using Robust.Shared.Console;

namespace Content.Lavaland.Server.Commands;

[AdminCommand(AdminFlags.Mapping)]
public sealed partial class LavalandMappingCommand : IConsoleCommand
{
    [Dependency] private IPrototypeManager _proto = default!;
    [Dependency] private IEntityManager _entityManager = default!;

    public string Command => "spawnplanet";

    public string Description => $"Spawns a {nameof(PlanetPrototype)} with a specified ID. Be careful, this can cause freezes on runtime!";

    public string Help => "spawnplanet <prototype id> <seed (optional)>";

    public void Execute(IConsoleShell shell, string argStr, string[] args)
    {
        PlanetPrototype? lavalandProto;
        int? lavalandSeed = null;

        switch (args.Length)
        {
            case 0:
                shell.WriteLine(Loc.GetString($"Enter {nameof(PlanetPrototype)} ID as a first argument"));
                shell.WriteLine(Help);
                return;
            case 1:
                if (!_proto.TryIndex(args[0], out lavalandProto))
                {
                    shell.WriteLine(Loc.GetString($"Invalid {nameof(PlanetPrototype)}!"));
                    return;
                }
                break;
            case 2:
                if (!_proto.TryIndex(args[0], out lavalandProto))
                {
                    shell.WriteLine(Loc.GetString($"Invalid {nameof(PlanetPrototype)}!"));
                    return;
                }

                if (!ushort.TryParse(args[1], out var targetId))
                {
                    shell.WriteLine(Loc.GetString("shell-argument-must-be-number"));
                    return;
                }
                lavalandSeed = targetId;
                break;
            default:
                shell.WriteLine(Loc.GetString("cmd-playerpanel-invalid-arguments"));
                shell.WriteLine(Help);
                return;
        }
        var lavalandSys = _entityManager.System<LavalandPlanetSystem>();

        if (lavalandSys.GetPreloaderEntity() == null)
            lavalandSys.EnsurePreloaderMap();

        if (!lavalandSys.SetupLavalandPlanet(lavalandProto, out var lavaland, lavalandSeed))
            shell.WriteLine("Failed to load lavaland! Ensure that lavaland.enabled CVar is set to true and check server-side logs.");
        else
            shell.WriteLine($"Successfully created new lavaland map: {_entityManager.ToPrettyString(lavaland)}");
    }
}
