namespace TheWayGatesBlindSpot;

public class Solution
{
    public string DecodeShift(string rawText, int shift)
    {
        var result = string.Empty;

        foreach(var letter in rawText) {
            var isLetter = (letter >= 'a' && letter <= 'z') || (letter >= 'A' && letter <= 'Z');

            if(!isLetter) {
                result += letter;
                continue;
            }

            var isUppercase = (letter >= 'A' && letter <= 'Z');

            var alphabetStart = isUppercase ? 'A' : 'a';
            var offset = letter - alphabetStart;

            var wrapped = ((offset - shift) % 26 + 26) % 26; 

            result += (char) (alphabetStart + wrapped);
        }
        return result;
    }

    public IEnumerable<Character> ParseParty(string rawPlan)
    {
        var characters = new List<Character>();
        var tags = rawPlan.Split('|');

        var name = string.Empty;
        var klasse = string.Empty;
        var hp = 0L;
        var strength = 0;
        var speed = 0;
        var location = new Location();


        for (int i = 0; i < tags.Length; i++)
        {
            var tag = tags[i];

            if(tag.ToLower().Contains("name")) {
                name = tag.Split('-')[1];
            }
            if(tag.ToLower().Contains("class")) {
                klasse = tag.Split('/')[1];
            }
            if(tag.ToLower().Contains("hp")) {
                hp = long.Parse(tag.Split(':')[1]);
            }
            if(tag.ToLower().Contains("str")) {
                strength = int.Parse(tag.Split('=')[1]);
            }
            if(tag.ToLower().Contains("spd")) {
                speed = int.Parse(tag.Split("==")[1]);
            }
            if(tag.ToLower().Contains("pos")) {
                var pos = tag.Split('@')[1];
                var coordinates = pos.Split(',');
                location = new Location() {
                    X = int.Parse(coordinates[0]),
                    Y = int.Parse(coordinates[1])
                };
            }

            if ((i + 1) % 6 == 0) {
                characters.Add(new Character() {
                    Name = name,
                    Class = klasse,
                    HP = hp,
                    Strength = strength,
                    Speed = speed,
                    Position = location
                });
            }
        }

        return characters;
    }

    public Location FindAvoidedLocation(IEnumerable<Character> characters, IEnumerable<Arrival> arrivals)
    {
        foreach(var character in characters) {
            var arrival = arrivals.FirstOrDefault(a => a.Name.ToLower() == character.Name.ToLower());
            if(arrival == null) continue;
            return new Location() {
                X = (character.Position.X + arrival.ArrivalLocation.X) / 2,
                Y = (character.Position.Y + arrival.ArrivalLocation.Y) / 2
            };
        }
        return new Location();
    }
}