using TheShateredLine;

public class Solution
{
    public IEnumerable<IEnumerable<Character>> GroupCharacters(IEnumerable<Character> characters, int proximityRange)
    {
        var orderedCharacters = characters
            .OrderBy(c => c.Position.X)
            .ThenBy(c => c.Position.Y)
            .ToList();

        if(orderedCharacters.Count == 0) return new List<IEnumerable<Character>>();

        var groups = new List<List<Character>>() { 
            new List<Character>() {
                orderedCharacters.First()
            } 
        };

        foreach(var character in orderedCharacters.Skip(1)) {
            var groupsInProximity = groups.Where(g => g.Any(c => (Math.Abs(character.Position.X - c.Position.X) + Math.Abs(character.Position.Y - c.Position.Y)) <= proximityRange)).ToList();

            if(groupsInProximity.Count == 0) {
                groups.Add(new List<Character>() { character });
            } else if (groupsInProximity.Count == 1) {
                groupsInProximity.First().Add(character);
            } else {
                foreach(var groupInProximity in groupsInProximity) {
                    groups.Remove(groupInProximity);
                }
                groups.Add(groupsInProximity.SelectMany(c => c).Append(character).ToList());
            }
        }

        return groups;
    }

    public Location MoveTowardNearestEnemy(Character mover, IEnumerable<Character> allOpponents)
    {
        var myLocationX = mover.Position.X;
        var myLocationY = mover.Position.Y;

        var nearestOpponent = allOpponents
            .Where(x => x.HP > 0)
            .OrderBy(x => Math.Abs(x.Position.X - myLocationX) + Math.Abs(x.Position.Y - myLocationY))
            .ThenBy(x => x.Name)
            .FirstOrDefault();

        if (nearestOpponent == null)
        {
            return new Location { X = myLocationX, Y = myLocationY };
        }

        return new Location { X = nearestOpponent.Position.X, Y = nearestOpponent.Position.Y };
    }

    public int CalculateGroupSpeed(IEnumerable<Character> group)
    {
        var characters = group.ToList();
        
        if(characters.Count == 0) return 0;
        
        if (characters.Count < 4) return (int) Math.Ceiling(characters.Select(c => c.Speed).Average());

        var orderedSpeeds = characters
            .Select(c => c.Speed)
            .Order()
            .ToList();
        
        var count = orderedSpeeds.Count;
        var half = count / 2;

        // Median of the lower half
        double q1;
        if (half % 2 == 0)
            q1 = (orderedSpeeds[half / 2 - 1] + orderedSpeeds[half / 2]) / 2.0;
        else
            q1 = orderedSpeeds[half / 2];

        // Median of the upper half
        double q3;
        if (half % 2 == 0)
            q3 = (orderedSpeeds[half + half / 2 - 1] +
                  orderedSpeeds[half + half / 2]) / 2.0;
        else
            q3 = orderedSpeeds[half + half / 2];

        // Interquartile range
        var iqr = q3 - q1;

        // Calculate bounds
        var lowerBound = q1 - 1.5 * iqr;
        var upperBound = q3 + 1.5 * iqr;

        return (int)Math.Ceiling(
            characters
                .Where(c => c.Speed >= lowerBound && c.Speed <= upperBound)
                .Select(c => c.Speed)
                .Average()
        );
    }

    public IEnumerable<Character> CalculateGroupAttack(IEnumerable<Character> attackers, IEnumerable<Character> defenders)
    {
        var orderedAttackers = attackers.OrderByDescending(x => x.Speed);
        var livingDefenders = defenders.ToList();

        foreach (var attacker in orderedAttackers)
        {
            var weakestDefender = livingDefenders
                .Where(x => x.HP > 0)
                .OrderBy(x => x.HP)
                .FirstOrDefault();

            if (weakestDefender == null)
                break; // geen levende defenders meer over

            double attackMult = 1;

            if (attacker.Class.ToLower() == "warrior")
            {
                if (weakestDefender.Class.ToLower() == "warrior") attackMult = 1;
                if (weakestDefender.Class.ToLower() == "ranger") attackMult = 1.5;
                if (weakestDefender.Class.ToLower() == "mage") attackMult = 0.5;
                if (weakestDefender.Class.ToLower() == "cleric") attackMult = 1;
            }

            if (attacker.Class.ToLower() == "ranger")
            {
                if (weakestDefender.Class.ToLower() == "warrior") attackMult = 0.5;
                if (weakestDefender.Class.ToLower() == "ranger") attackMult = 1;
                if (weakestDefender.Class.ToLower() == "mage") attackMult = 1.5;
                if (weakestDefender.Class.ToLower() == "cleric") attackMult = 1;
            }

            if (attacker.Class.ToLower() == "mage")
            {
                if (weakestDefender.Class.ToLower() == "warrior") attackMult = 1.5;
                if (weakestDefender.Class.ToLower() == "ranger") attackMult = 0.5;
                if (weakestDefender.Class.ToLower() == "mage") attackMult = 1;
                if (weakestDefender.Class.ToLower() == "cleric") attackMult = 1;
            }

            if (attacker.Class.ToLower() == "cleric")
            {
                if (weakestDefender.Class.ToLower() == "warrior") attackMult = 1;
                if (weakestDefender.Class.ToLower() == "ranger") attackMult = 1;
                if (weakestDefender.Class.ToLower() == "mage") attackMult = 1;
                if (weakestDefender.Class.ToLower() == "cleric") attackMult = 1;
            }

            var damage = (int)Math.Ceiling(attacker.Strength * attackMult);

            weakestDefender.HP = Math.Max(0, weakestDefender.HP - damage);
        }

        return livingDefenders.Where(x => x.HP > 0);
    }
}