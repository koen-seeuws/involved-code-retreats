using System.Net.Http.Json;

namespace HTF2024.Puzzles;

public class PuzzleB1(HttpClient httpClient) : IPuzzle
{
    public async Task Run(bool forReal = false)
    {
        Console.WriteLine("Running B1");
        var sampleOrPuzzle = forReal ? "puzzle" : "sample";
        
        
        await httpClient.GetAsync("b/easy/start");

        var alphabet = await httpClient.GetFromJsonAsync<Dictionary<string, string>>("b/easy/alphabet");
        if (alphabet == null) throw new Exception();
        var alienMessageDto = await httpClient.GetFromJsonAsync<AlienMessageDto>($"b/easy/{sampleOrPuzzle}");
        var message = alienMessageDto?.AlienMessage;
        if (message == null) throw new Exception();
        
        Console.WriteLine(message);
        
        var result = string.Empty;

        foreach (var alienChar in message.ToCharArray())
        {
            var humanChar = alphabet.FirstOrDefault(kv => kv.Value == alienChar.ToString()).Key;
            result += humanChar != null ? humanChar : alienChar;
        }
        
        Console.WriteLine(result);
        
        var response = await httpClient.PostAsJsonAsync($"b/easy/{sampleOrPuzzle}", result);
        try
        {
            response.EnsureSuccessStatusCode();
            Console.WriteLine($"B1 {sampleOrPuzzle} solved");
        }
        catch
        {
            Console.WriteLine($"B1 {sampleOrPuzzle} not solved");;
        }
        
        
        
    }
}

 class AlienMessageDto
{
    public string AlienMessage { get; set; }
}