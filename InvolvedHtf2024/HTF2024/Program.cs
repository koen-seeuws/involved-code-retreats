// See https://aka.ms/new-console-template for more information

using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.RegularExpressions;
using HTF2024;
using HTF2024.Puzzles;

const string username = "Koen";
const string password = "";

var test = "\u202fBedankt voor uw aanvraag\u202f Mail";

var fileName =
    Regex.Replace(test, @"[\\/:*?""<>|\u202F]", string.Empty)
        .Trim();

Console.WriteLine(test);
Console.WriteLine(fileName);


var httpClient = new HttpClient()
{
    BaseAddress = new Uri("https://app-htf-2024.azurewebsites.net/api/")
};

//Token
const string token =
    "eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6IjE2IiwibmJmIjoxNzMxNTc3ODY0LCJleHAiOjE3MzE2NjQyNjQsImlhdCI6MTczMTU3Nzg2NH0.MMfHp_jMmCmpkwOJVKgTDErW5DoPsJUv3uTjTUyLaJv89ewcuA3Jqj-YAZ6_wFkpF0pTOQQvcgXa4OIBM6wjoA";
httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

//await FlyInCircles();
await FlyRandom();

IPuzzle puzzleA1 = new PuzzleA1(httpClient);
IPuzzle puzzleA2 = new PuzzleA2(httpClient);
IPuzzle puzzleA3 = new PuzzleA3(httpClient);
IPuzzle puzzleB1 = new PuzzleB1(httpClient);
IPuzzle puzzleB2 = new PuzzleB2(httpClient);
IPuzzle puzzleB3 = new PuzzleB3(httpClient);

//Too much discussion: await puzzleA1.Run(false);
//await puzzleA2.Run(false);
//await puzzleA3.Run(true);

//await puzzleB1.Run(false);
//await puzzleB2.Run(false);

async Task FlyInCircles()
{
    while (true)
    {
        for (var i = 0; i < 360; i += 5)
        {
            var move = new MoveDto()
            {
                Angle = i,
                Speed = "L"
            };
            await httpClient.PostAsJsonAsync("Team/move", move);
        }
    }
}

async Task FlyRandom()
{
    string[] speeds = ["R", "L", "M", "H"];
    while (true)
    {
        var speed = speeds[Random.Shared.Next(0, 4)];
        var angle = Random.Shared.Next(1, 73);
        angle *= 5;
        var move = new MoveDto
        {
            Angle = angle,
            Speed = speed
        };
        await httpClient.PostAsJsonAsync("Team/move", move);
    }
}