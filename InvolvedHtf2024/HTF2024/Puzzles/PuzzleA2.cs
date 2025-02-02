using System.Net.Http.Json;
using System.Text.Json;

namespace HTF2024.Puzzles;

public class PuzzleA2(HttpClient httpClient) : IPuzzle
{
    public async Task Run(bool forReal = false)
    {
        Console.WriteLine("Running A2");
        var sampleOrPuzzle = forReal ? "puzzle" : "sample";

        //await httpClient.GetAsync("a/medium/start");

        var battle = await httpClient.GetFromJsonAsync<BattleDto>($"a/medium/{sampleOrPuzzle}");
        var battleBeforeExecute = JsonSerializer.Serialize(battle);
        if (battle == null) throw new Exception();

        while (battle.TeamA.Count > 0 && battle.TeamB.Count > 0)
        {
            var alienA = battle.TeamA[0];
            var alienB = battle.TeamB[0];
            
            if (alienA.Speed >= alienB.Speed)
            {
                alienB.Health -= alienA.Strength;
                if (alienB.Health <= 0)
                    battle.TeamB.RemoveAt(0);
                else
                {
                    alienA.Health -= alienB.Strength;
                    if (alienA.Health <= 0)
                        battle.TeamA.RemoveAt(0);
                }
            }
            else
            {
                alienA.Health -= alienB.Strength;
                if (alienA.Health <= 0)
                    battle.TeamA.RemoveAt(0);
                else
                {
                     alienB.Health -= alienA.Strength;
                     if(alienB.Health <= 0)
                        battle.TeamB.RemoveAt(0);
                }
            }
        }

        var winningTeam = battle.TeamA.Count > 0 ? "TeamA" : "TeamB";

        var response = await httpClient.PostAsJsonAsync($"a/medium/{sampleOrPuzzle}", winningTeam);

        try
        {
            response.EnsureSuccessStatusCode();
            Console.WriteLine($"A2 {sampleOrPuzzle} solved");
        }
        catch
        {
            Console.WriteLine($"A2 {sampleOrPuzzle} not solved");
            Console.WriteLine(battleBeforeExecute);
        }
    }
}

class BattleDto
{
    public required List<AlienDto> TeamA { get; set; }
    public required List<AlienDto> TeamB { get; set; }
}

class AlienDto
{
    public int Strength { get; set; }
    public int Speed { get; set; }
    public int Health { get; set; }
}