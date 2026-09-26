namespace Content.Lavaland.Server.Procedural;

/// <summary>
/// Marker event raised on planet maps in order to set up structures on them.
/// </summary>
[ByRefEvent]
public readonly record struct PlanetSetupEvent;
