namespace TheWayGatesBlindSpot;

public sealed class Location
{
    public int X { get; set; }
    public int Y { get; set; }
}

public sealed class Character
{
    // Appears as: Name-Mira
    public string Name { get; set; } = string.Empty;

    // Appears as: Class/Mage ("Warrior", "Ranger", "Mage" or "Cleric")
    public string Class { get; set; } = string.Empty;

    // Appears as: HP:30
    public long HP { get; set; }

    // Appears as: STR=8
    public int Strength { get; set; }

    // Appears as: SPD==20
    public int Speed { get; set; }

    // Appears as: Pos@0,0
    public Location Position { get; set; } = new Location();
}

public sealed class Arrival
{
    public string Name { get; set; } = string.Empty;
    public Location ArrivalLocation { get; set; } = new Location();
}