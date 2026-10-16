namespace TheSeveredRoute;

public sealed class WaygateConnection
{
    // Return this from ExploreStep to jump there. One-way.
    public string DestinationId { get; set; } = string.Empty;

    // Scores the route you submit, not the walk you took to work it out.
    public int Cost { get; set; }
}

public sealed class WaygateNode
{
    // The Waygate you are standing on. Use it to name hops in your PATH.
    public string Id { get; set; } = string.Empty;

    // Only what this one gate can see. Nothing shows you the whole network.
    public List<WaygateConnection> Connections { get; set; } = [];
}

