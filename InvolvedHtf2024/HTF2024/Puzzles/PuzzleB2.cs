using System.Net.Http.Json;

namespace HTF2024.Puzzles;

public class PuzzleB2(HttpClient httpClient) : IPuzzle
{
    public async Task Run(bool forReal = false)
    {
        Console.WriteLine("Running B2");
        var sampleOrPuzzle = forReal ? "puzzle" : "sample";

        await httpClient.GetAsync("b/medium/start");

        var commandsDto = await httpClient.GetFromJsonAsync<CommandsDto>($"b/medium/{sampleOrPuzzle}");
        if (commandsDto == null) throw new Exception();

        var position = 0;
        var depth = 0;
        var aim = 0;

        foreach (var command in commandsDto.Commands.Split('\n'))
        {
            var direction = command.Split(' ')[0];
            var x = int.Parse(command.Split(' ')[1]);

            switch (direction.ToLower())
            {
                case "forward":
                    position += x;
                    depth += x * aim;
                    break;
                case "down":
                    aim += x;
                    break;
                case "up":
                    aim -= x;
                    break;
                default: throw new ArgumentOutOfRangeException($"{direction} not handled");
            }
        }

        var result = position * depth;

        var response = await httpClient.PostAsJsonAsync($"b/medium/{sampleOrPuzzle}", result);

        try
        {
            response.EnsureSuccessStatusCode();
            Console.WriteLine($"B2 {sampleOrPuzzle} solved");
        }
        catch
        {
            Console.WriteLine($"B2 {sampleOrPuzzle} not solved");
        }
    }
}

class CommandsDto
{
    public string Commands { get; set; }
}