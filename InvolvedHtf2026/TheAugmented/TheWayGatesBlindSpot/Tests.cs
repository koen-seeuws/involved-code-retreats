namespace TheWayGatesBlindSpot;

public class DecodeShiftTests
{
    [Fact]
    public void DecodeShift_ShiftsLettersAndPreservesNonLetters()
    {
        var solution = new Solution();

        var result = solution.DecodeShift("Khoor, Zruog! 123", 3);

        Assert.Equal("Hello, World! 123", result);
    }

    [Fact]
    public void DecodeShift_WrapsAlphabetAndSupportsNegativeShifts()
    {
        var solution = new Solution();

        Assert.Equal("Za", solution.DecodeShift("Bc", 2));
        Assert.Equal("Bc", solution.DecodeShift("Za", -2));
    }
}

public class ParsePartyTests
{
    [Fact]
    public void ParseParty_ParsesAllFieldsForEachCharacter()
    {
        var solution = new Solution();
        const string rawPlan =
            "Name-Mira|Class/Mage|HP:30|STR=8|SPD==20|Pos@1,2|" +
            "Name-Torin|Class/Warrior|HP:45|STR=12|SPD==15|Pos@-3,4";

        var characters = solution.ParseParty(rawPlan).ToList();

        Assert.Equal(2, characters.Count);
        Assert.Equal("Mira", characters[0].Name);
        Assert.Equal("Mage", characters[0].Class);
        Assert.Equal(30L, characters[0].HP);
        Assert.Equal(8, characters[0].Strength);
        Assert.Equal(20, characters[0].Speed);
        Assert.Equal(1, characters[0].Position.X);
        Assert.Equal(2, characters[0].Position.Y);

        Assert.Equal("Torin", characters[1].Name);
        Assert.Equal("Warrior", characters[1].Class);
        Assert.Equal(45L, characters[1].HP);
        Assert.Equal(12, characters[1].Strength);
        Assert.Equal(15, characters[1].Speed);
        Assert.Equal(-3, characters[1].Position.X);
        Assert.Equal(4, characters[1].Position.Y);
    }
}

public class FindAvoidedLocationTests
{
    [Fact]
    public void FindAvoidedLocation_ReturnsMidpointForCaseInsensitiveNameMatch()
    {
        var solution = new Solution();
        var characters = new[]
        {
            new Character
            {
                Name = "Mira",
                Position = new Location { X = 2, Y = 4 }
            }
        };
        var arrivals = new[]
        {
            new Arrival
            {
                Name = "mIRA",
                ArrivalLocation = new Location { X = 8, Y = 10 }
            }
        };

        var result = solution.FindAvoidedLocation(characters, arrivals);

        Assert.Equal(5, result.X);
        Assert.Equal(7, result.Y);
    }

    [Fact]
    public void FindAvoidedLocation_ReturnsOriginWhenThereIsNoMatchingArrival()
    {
        var solution = new Solution();
        var characters = new[]
        {
            new Character { Name = "Mira" }
        };
        var arrivals = new[]
        {
            new Arrival { Name = "Torin" }
        };

        var result = solution.FindAvoidedLocation(characters, arrivals);

        Assert.Equal(0, result.X);
        Assert.Equal(0, result.Y);
    }
}