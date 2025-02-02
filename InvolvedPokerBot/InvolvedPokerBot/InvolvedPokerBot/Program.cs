using Microsoft.AspNetCore.SignalR.Client;
using System.Text;
using System.Text.Json;
using InvolvedPoker.Models.Events;
using InvolvedPoker.Models.Models;
using InvolvedPokerBot;

var baseUrl = "https://involvedpoker.azurewebsites.net";

var client = new HttpClient();
client.BaseAddress = new Uri(baseUrl);

var login = new Login
{
    UserName = "Koen",
    Password = ""
};

// Uncomment the next line to register first
var responseregister = await client.PostAsync("api/user/Register",
    new StringContent(JsonSerializer.Serialize(login), Encoding.UTF8, "application/json"), CancellationToken.None);

// fetch token
var response = await client.PostAsync("api/user/authenticate",
    new StringContent(JsonSerializer.Serialize(login), Encoding.UTF8, "application/json"), CancellationToken.None);
var token = await response.Content.ReadAsStringAsync();

// register to Involved hub
var connection = new HubConnectionBuilder().WithUrl($"{baseUrl}/InvolvedHoldemHub/",
    options => { options.AccessTokenProvider = () => Task.FromResult(token); }).Build();

var bot = new KoenPokerBot();

// start connection
await connection.StartAsync();

connection.On("HandStarted",
    async (HandStarted handStarted) =>
    {
        Console.WriteLine(
            $"Handed: {handStarted.FirstCard.Type.ToString()} {handStarted.FirstCard.Suit.ToString()}, {handStarted.SecondCard.Type.ToString()} {handStarted.SecondCard.Suit.ToString()}\n");
        bot.MyCards = new List<Card>() { handStarted.FirstCard, handStarted.SecondCard };
    });

connection.On("RoundStarted",
    async (RoundStarted roundStarted) =>
    {
        Console.WriteLine(
            $"Community: {string.Join(", ", roundStarted.CommunityCards.Select(cc => $"{cc.Type.ToString()} {cc.Suit.ToString()}"))}\n");
        bot.CommunityCards = roundStarted.CommunityCards;
    });

connection.On("TurnStarted", async (TurnStarted turnStarted) =>
{
    var (action, cards) = bot.DetermineAction(turnStarted);
    Console.WriteLine(
        $"Turn: {string.Join(", ", cards.Select(cc => $"{cc.Type.ToString()} {cc.Suit.ToString()}"))} -> {action.Action.ToString()} {(action.Action == ActionEnum.Raise ? " (Raise: " + action.Amount + ")" : "")}\n");
    await connection.SendAsync("Action", action);
});

connection.On("HandEnded", async (HandEnded handEnded) => { });

connection.On("RoundEnded", async (RoundEnded roundEnded) => { });

connection.On("GameStarted", async (GameStarted gameStarted) =>
{
    bot.Names = gameStarted.PlayerNames;
    Console.WriteLine("Game started\n");
});

connection.On("GameEnded",
    async (GameEnded gameEnded) => { Console.WriteLine($"Game ended, winner: {gameEnded.WinnerName}\n"); });

Console.ReadLine();

await connection.StopAsync();