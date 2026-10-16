namespace TheShateredLine;

public class GroupCharactersTests
{
    private readonly Solution _solution = new();

    [Fact]
    public void EmptyInput_ReturnsNoGroups()
    {
        var groups = _solution.GroupCharacters([], proximityRange: 1);

        Assert.Empty(groups);
    }

    [Fact]
    public void UsesManhattanDistanceAndMergesConnectedGroups()
    {
        var characters = new[]
        {
            CharacterTestData.Create("A", x: 0, y: 0),
            CharacterTestData.Create("B", x: 2, y: 0),
            CharacterTestData.Create("C", x: 4, y: 0),
            CharacterTestData.Create("D", x: 0, y: 3)
        };

        var groups = _solution.GroupCharacters(characters, proximityRange: 2)
            .Select(group => group.Select(character => character.Name).Order().ToArray())
            .ToList();

        Assert.Collection(
            groups,
            group => Assert.Equal(new[] { "A", "B", "C" }, group),
            group => Assert.Equal(new[] { "D" }, group));
    }

    [Fact]
    public void CharactersOutsideProximityRemainSeparate()
    {
        var groups = _solution.GroupCharacters(
            [CharacterTestData.Create("A", 0, 0), CharacterTestData.Create("B", 2, 2)],
            proximityRange: 3);

        Assert.Equal(2, groups.Count());
    }
}

public class MoveTowardNearestEnemyTests
{
    private readonly Solution _solution = new();

    [Fact]
    public void SelectsNearestLivingOpponentByManhattanDistance()
    {
        var mover = CharacterTestData.Create("mover", 0, 0);
        var opponents = new[]
        {
            CharacterTestData.Create("far", 5, 0),
            CharacterTestData.Create("near", 1, 1),
            CharacterTestData.Create("dead", 0, 0, hp: 0)
        };

        var destination = _solution.MoveTowardNearestEnemy(mover, opponents);

        Assert.Equal(1, destination.X);
        Assert.Equal(1, destination.Y);
    }

    [Fact]
    public void BreaksDistanceTiesByName()
    {
        var destination = _solution.MoveTowardNearestEnemy(
            CharacterTestData.Create("mover", 0, 0),
            [CharacterTestData.Create("Zed", 1, 0), CharacterTestData.Create("Amy", 0, 1)]);

        Assert.Equal(0, destination.X);
        Assert.Equal(1, destination.Y);
    }

    [Fact]
    public void NoLivingOpponents_StaysAtCurrentPosition()
    {
        var mover = CharacterTestData.Create("mover", 3, -2);

        var destination = _solution.MoveTowardNearestEnemy(
            mover,
            [CharacterTestData.Create("dead", 10, 10, hp: 0)]);

        Assert.Equal(3, destination.X);
        Assert.Equal(-2, destination.Y);
        Assert.NotSame(mover.Position, destination);
    }
}

public class CalculateGroupSpeedTests
{
    private readonly Solution _solution = new();

    [Fact]
    public void EmptyGroup_ReturnsZero()
    {
        Assert.Equal(0, _solution.CalculateGroupSpeed([]));
    }

    [Fact]
    public void FewerThanFourCharacters_CeilsAverageSpeed()
    {
        var group = new[]
        {
            CharacterTestData.Create("A", speed: 3),
            CharacterTestData.Create("B", speed: 4),
            CharacterTestData.Create("C", speed: 5)
        };

        Assert.Equal(4, _solution.CalculateGroupSpeed(group));
    }

    [Fact]
    public void FiltersSpeedOutliersBeforeAveraging()
    {
        var group = new[]
        {
            CharacterTestData.Create("A", speed: 1),
            CharacterTestData.Create("B", speed: 2),
            CharacterTestData.Create("C", speed: 3),
            CharacterTestData.Create("D", speed: 4),
            CharacterTestData.Create("E", speed: 5),
            CharacterTestData.Create("F", speed: 6),
            CharacterTestData.Create("G", speed: 7),
            CharacterTestData.Create("H", speed: 100)
        };

        Assert.Equal(4, _solution.CalculateGroupSpeed(group));
    }
}

public class CalculateGroupAttackTests
{
    private readonly Solution _solution = new();

    [Theory]
    [InlineData("Warrior", "Warrior", 18)]
    [InlineData("Warrior", "Ranger", 17)]
    [InlineData("Warrior", "Mage", 19)]
    [InlineData("Warrior", "Cleric", 18)]
    [InlineData("Ranger", "Warrior", 19)]
    [InlineData("Ranger", "Ranger", 18)]
    [InlineData("Ranger", "Mage", 17)]
    [InlineData("Ranger", "Cleric", 18)]
    [InlineData("Mage", "Warrior", 17)]
    [InlineData("Mage", "Ranger", 19)]
    [InlineData("Mage", "Mage", 18)]
    [InlineData("Mage", "Cleric", 18)]
    [InlineData("Cleric", "Warrior", 18)]
    [InlineData("Cleric", "Ranger", 18)]
    [InlineData("Cleric", "Mage", 18)]
    [InlineData("Cleric", "Cleric", 18)]
    public void AppliesClassMatchupMultiplier(
        string attackerClass,
        string defenderClass,
        long expectedHitPoints)
    {
        var defender = CharacterTestData.Create("defender", hp: 20, characterClass: defenderClass);
        var attacker = CharacterTestData.Create(
            "attacker",
            strength: 2,
            speed: 1,
            characterClass: attackerClass);

        var survivors = _solution.CalculateGroupAttack([attacker], [defender]).ToList();

        Assert.Same(defender, Assert.Single(survivors));
        Assert.Equal(expectedHitPoints, defender.HP);
    }

    [Fact]
    public void CeilsFractionalDamage()
    {
        var defender = CharacterTestData.Create("defender", hp: 5, characterClass: "Mage");
        var attacker = CharacterTestData.Create("attacker", strength: 3, characterClass: "Warrior");

        _solution.CalculateGroupAttack([attacker], [defender]).ToList();

        Assert.Equal(3, defender.HP);
    }

    [Fact]
    public void FasterAttackersActFirstAndTargetWeakestLivingDefender()
    {
        var weakestDefender = CharacterTestData.Create("weakest", hp: 5, characterClass: "Warrior");
        var nextDefender = CharacterTestData.Create("next", hp: 10, characterClass: "Warrior");
        var attackers = new[]
        {
            CharacterTestData.Create("slow", strength: 2, speed: 1, characterClass: "Warrior"),
            CharacterTestData.Create("fast", strength: 5, speed: 10, characterClass: "Warrior")
        };

        var survivors = _solution.CalculateGroupAttack(
            attackers,
            [nextDefender, weakestDefender]).ToList();

        Assert.Equal(0, weakestDefender.HP);
        Assert.Equal(8, nextDefender.HP);
        Assert.Same(nextDefender, Assert.Single(survivors));
    }

    [Fact]
    public void StopsWhenNoLivingDefendersRemain()
    {
        var defender = CharacterTestData.Create("defender", hp: 4);
        var attackers = new[]
        {
            CharacterTestData.Create("first", strength: 4, speed: 2),
            CharacterTestData.Create("second", strength: 10, speed: 1)
        };

        var survivors = _solution.CalculateGroupAttack(attackers, [defender]);

        Assert.Empty(survivors);
        Assert.Equal(0, defender.HP);
    }

    [Fact]
    public void IgnoresDefendersAlreadyDead()
    {
        var deadDefender = CharacterTestData.Create("dead", hp: 0);
        var livingDefender = CharacterTestData.Create("living", hp: 10);

        var survivors = _solution.CalculateGroupAttack(
            [CharacterTestData.Create("attacker", strength: 3)],
            [deadDefender, livingDefender]).ToList();

        Assert.Same(livingDefender, Assert.Single(survivors));
        Assert.Equal(0, deadDefender.HP);
        Assert.Equal(7, livingDefender.HP);
    }
}

internal static class CharacterTestData
{
    public static Character Create(
        string name,
        int x = 0,
        int y = 0,
        long hp = 100,
        int strength = 0,
        int speed = 0,
        string characterClass = "Warrior")
    {
        return new Character
        {
            Name = name,
            Class = characterClass,
            HP = hp,
            Strength = strength,
            Speed = speed,
            Position = new Location { X = x, Y = y }
        };
    }
}
