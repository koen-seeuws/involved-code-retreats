using System.Net.Http.Json;

namespace HTF2024.Puzzles;

public class PuzzleA1(HttpClient httpClient) : IPuzzle
{
    public async Task Run(bool forReal = false)
    {
        Console.WriteLine("Running A1");
        var sampleOrPuzzle = forReal ? "puzzle" : "sample";
        await httpClient.GetAsync("a/easy/start");
        
        var alienMessageDto = await httpClient.GetFromJsonAsync<MessageTimeDto>($"a/easy/{sampleOrPuzzle}");
        if (alienMessageDto == null) throw new Exception();

        var distance = alienMessageDto.Distance * 2;
        var timeToTravel = TimeSpan.FromMinutes((double)distance  / alienMessageDto.TravelSpeed);

        var send = alienMessageDto.SendDateTime;
        
        
        timeToTravel /= alienMessageDto.DayLength;


     
        //var response = await httpClient.PostAsJsonAsync($"a/easy/{sampleOrPuzzle}", receiveDateTime);
        //response.EnsureSuccessStatusCode();
        
        Console.WriteLine($"A1 {sampleOrPuzzle} solved");;
        

    }
    
     class MessageTimeDto
    {
        public DateTime SendDateTime { get; set; }
        public int TravelSpeed { get; set; }
        public int Distance { get; set; }
        public int DayLength { get; set; }
    }
}