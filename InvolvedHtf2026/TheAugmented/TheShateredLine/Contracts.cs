namespace TheShateredLine;

public sealed class Location
{
    public int X { get; set; }
    public int Y { get; set; }
}

public sealed class Character
{
    // Settles ties alphabetically.
    public string Name { get; set; } = string.Empty;

    // "Warrior", "Ranger", "Mage" or "Cleric" — sets the damage multiplier.
    public string Class { get; set; } = string.Empty;

    // Alive means above 0. Damage never takes it below 0.
    public long HP { get; set; }

    // Damage before the class multiplier.
    public int Strength { get; set; }

    // Attackers act in descending order of this.
    public int Speed { get; set; }

    // Grid coordinate. All distances are Manhattan.
    public Location Position { get; set; } = new Location();
}