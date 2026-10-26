using Content.Server.Administration;
using Content.Shared.Administration;
using Content.Trauma.Shared.Procedural.Prototypes;
using Content.Trauma.Shared.Procedural.Systems;
using Robust.Shared.Toolshed;

namespace Content.Trauma.Server.Commands;

/// <summary>
/// Adds procedural config to the input entities.
/// </summary>
[ToolshedCommand, AdminCommand(AdminFlags.Debug)]
public sealed class ProceduralCommand : ToolshedCommand
{
    private ProceduralSystem Procedural
    {
        get
        {
            field ??= GetSys<ProceduralSystem>();
            return field;
        }
    }

    [CommandImplementation]
    public void Effect(
        [PipedArgument] EntityUid uid,
        [CommandArgument] ProtoId<ProceduralConfigPrototype> id)
    {
        Procedural.EnsureProcedural(uid, id);
    }

    // TODO make custom parser for ProceduralConfig and add another implementation with all of its fields
}
