using InvolvedProLeaguePlayer;
using InvolvedProLeaguePlayer.Models;
using InvolvedProLeaguePlayer.Models.Results;
using InvolvedProLeaguePlayer.Models.States;
using Microsoft.AspNetCore.SignalR.Client;

const string tokenString = "";
const string url = "https://involved-pro-league.azurewebsites.net";

var client = new Client(tokenString, url);

var proLeagueConnection = new HubConnectionBuilder()
    .WithUrl($"{url}/InvolvedLeague",
        options => { options.AccessTokenProvider = async () => await Task.FromResult(tokenString); })
    .WithAutomaticReconnect()
    .Build();

NotifyDayResult? dayResult = null;
int? gameId = null;
List<PlayerScoutReport>? playerScoutReports = null;

proLeagueConnection.On("NotifyDayChanged", async (NotifyDayResult notifyDayResult) =>
{
    Console.WriteLine(
        $"{DateTime.Now}: Day changed - Week {notifyDayResult.Week} - Day {notifyDayResult.Day} - Money: {notifyDayResult.Club.Money} - Stadium tier: {notifyDayResult.Club.Stadium.Tier} upgrade price: {notifyDayResult.Club.Stadium.NextUpgradePrice}");

    dayResult = notifyDayResult;

    if (dayResult.Club.Money >= dayResult.Club.Stadium.NextUpgradePrice)
    {
        var response = await client.UpgradeStadium();
        if (response.IsSuccessStatusCode)
        {
            Console.WriteLine($"{DateTime.Now}: Upgraded stadium\n");
            dayResult.Club.Money -= dayResult.Club.Stadium.NextUpgradePrice;
        }
    }

    if (dayResult.ShouldBuyStaff(out var staffType) && staffType.HasValue)
    {
        var response = await client.BuyStaff(staffType.Value);
        if (response.IsSuccessStatusCode)
        {
            Console.WriteLine($"{DateTime.Now}: Bought {staffType.Value.ToString()}\n");
            dayResult.Club.Money -= dayResult.Club.PricePerStaffType[staffType.Value];
        }
    }

    if (notifyDayResult.HasStaff(StaffType.Scout) && playerScoutReports != null)
    {
        var response = await client.ScoutPlayers();
        if (response.IsSuccessStatusCode)
            Console.WriteLine($"{DateTime.Now}: Send out scout\n");
    }

    if (notifyDayResult.HasStaff(StaffType.Sales) && (playerScoutReports?.Any() ?? false))
    {
        playerScoutReports = playerScoutReports
            .OrderByDescending(r => r.GeneralRating)
            .ThenByDescending(r => r.PlayStyle)
            .ThenBy(r => r.Price)
            .ToList();

        var bestReport = playerScoutReports.FirstOrDefault();
        if (bestReport != null && bestReport.Price <= notifyDayResult.Club.Money)
        {
            var buyDto = new Client.BuyPlayerDto(bestReport.Id, bestReport.Price);
            var response = await client.BuyPlayer(buyDto);
            if (response.IsSuccessStatusCode)
                Console.WriteLine($"{DateTime.Now}: Requested buy player\n");
        }
    }
    
    if (notifyDayResult.Week == 1 && notifyDayResult.Day != 6)
    {
        await Train();
        return;
    }

    if (notifyDayResult.Day == 1 || notifyDayResult.Day == 6)
    {
        await Rest();
        return;
    }
        

    if (notifyDayResult.Club.Team.TeamSpirit < 65)
    {
        await TeamBuild();
        return;
    }

    var allPlayers = notifyDayResult.Club.Team.PlayerStates.Concat(notifyDayResult.Club.Team.BenchedPlayerStates);
    if (allPlayers.Average(p => p.Stamina) >= 75 || notifyDayResult.Week == 1)
    {
        await Train();
    }
    else
    {
        await Rest();
    }
});

proLeagueConnection.On("NotifyMinuteChanged", async (GameState gameState) =>
{
    Console.WriteLine(
        $"{DateTime.Now}: Minute {gameState.Minute} - Me: {(gameState.PlayingHome ? gameState.HomeScore : gameState.AwayScore)} - Opponent: {(!gameState.PlayingHome ? gameState.HomeScore : gameState.AwayScore)} ({gameState.TeamState.GameTactic.ToString()})\n");

    gameId = gameState.GameId;

    if (gameState.ShouldChangeTactic(out var gameTactic) && gameTactic.HasValue)
    {
        var response = await client.ChangeGameTactic(gameState.GameId, gameTactic.Value);
        if (response.IsSuccessStatusCode)
            Console.WriteLine($"{DateTime.Now}: Changed tactic - {gameTactic.Value.ToString()}\n");
    }

    if (gameState.ShouldSubstitute(out var playerOut, out var playerIn) && playerOut != null && playerIn != null)
    {
        var response = await client.Substitute(gameState.GameId, playerIn.PlayerId, playerOut.PlayerId);
        if (response.IsSuccessStatusCode)
            Console.WriteLine(
                $"{DateTime.Now}: Requested substitute player - {playerOut.Name} ({playerOut.PlayerId}) -> {playerIn.Name} ({playerIn.PlayerId})\n");
    }
});

proLeagueConnection.On("NotifySubstitutePlayer",
    (NotifySubstitutionDto notifySubstitution) =>
    {
        if (gameId.HasValue && notifySubstitution.GameId == gameId.Value)
            Console.WriteLine(
                $"{DateTime.Now}: Substitute player ({notifySubstitution.Minute}) - {notifySubstitution.Message}\n");
    });

proLeagueConnection.On("NotifyScoutReport", async (List<PlayerScoutReport> reports) =>
{
    Console.WriteLine($"{DateTime.Now}: Scout report\n");
    playerScoutReports = reports;
});

proLeagueConnection.On("NotifyOpponentScoutReport",
    async (OpponentScoutReport opponentScoutReport) =>
    {
        Console.WriteLine($"{DateTime.Now}: Opponent scout report\n");
    });

proLeagueConnection.On("NotifyStartGame", () => { Console.WriteLine($"{DateTime.Now}: Start game\n"); });

proLeagueConnection.On("NotifyGameEnd", () =>
{
    Console.WriteLine($"{DateTime.Now}: End game\n");
    gameId = null;
});

proLeagueConnection.On("NotifyStartWeek", async () => { Console.WriteLine($"{DateTime.Now}: Start week\n"); });

proLeagueConnection.On("NotifyTransfer",
    (string message) => { Console.WriteLine($"{DateTime.Now}: Transfer -> {message}\n"); });

await proLeagueConnection.StartAsync();

Console.WriteLine("Press any button when you want to stop the app");
Console.ReadLine();

async Task Train()
{
    var response = await client.SetTrainingDay();
    if (response.IsSuccessStatusCode)
    {
        Console.WriteLine($"{DateTime.Now}: Set training day\n");
    }
}

async Task Rest()
{
    var response = await client.SetRestDay();
    if (response.IsSuccessStatusCode)
    {
        Console.WriteLine($"{DateTime.Now}: Set rest day\n");
    }
}

async Task TeamBuild()
{
    var response = await client.SetTeamBuildingDay();
    if (response.IsSuccessStatusCode)
    {
        Console.WriteLine($"{DateTime.Now}: Set team building day\n");
    }
}