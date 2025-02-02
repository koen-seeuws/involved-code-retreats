using System.Net.Http.Headers;
using System.Net.Http.Json;
using InvolvedProLeaguePlayer.DataTransferObjects;
using InvolvedProLeaguePlayer.Models;

namespace InvolvedProLeaguePlayer;

public class Client
{
  private readonly HttpClient _client;

  public Client(string token, string url)
  {
    _client = new HttpClient();
    _client.DefaultRequestHeaders.Accept.Clear();
    _client.BaseAddress = new Uri($"{url}/api/");
    _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
  }

  public async Task<HttpResponseMessage> Substitute(int gameId, int playerInId, int playerOutId)
  {
    return await PostAsync($"game/{gameId}/substitute", new SubstitutePlayerDto(playerInId, playerOutId));
  }

  public async Task<HttpResponseMessage> ChangeGameTactic(int gameId, GameTactic gameTactic)
  {
    return await PostAsync($"game/{gameId}/change-game-tactic/{gameTactic}", null);
  }

  public async Task<HttpResponseMessage> SetRestDay()
  {
    return await PostAsync("club/rest-day", null);
  }

  public async Task<HttpResponseMessage> SetTeamBuildingDay()
  {
    return await PostAsync("club/teambuilding-day", null);
  }

  public async Task<HttpResponseMessage> SetTrainingDay()
  {
    return await PostAsync("club/training-day", null);
  }

  public async Task<HttpResponseMessage> ScoutPlayers()
  {
    return await PostAsync("club/scout-players", null);
  }

  public async Task<HttpResponseMessage> ScoutOpponent()
  {
    return await PostAsync("club/scout-opponent", null);
  }

  public record BuyPlayerDto(int MarketPlayerId, int Price);

  public async Task<HttpResponseMessage> BuyPlayer(BuyPlayerDto buyPlayer)
  {
    return await PostAsync("club/buy-player", buyPlayer);
  }

  public record SellPlayerDto(int PlayerId);

  public async Task<HttpResponseMessage> SellPlayer(SellPlayerDto sellPlayerDto)
  {
    return await PostAsync("club/sell-player", sellPlayerDto);
  }

  public async Task<HttpResponseMessage> UpgradeStadium()
  {
    return await PostAsync("club/upgrade-stadium", null);
  }

  public async Task<HttpResponseMessage> ChangePlayerName(int playerId, string name)
  {
    return await PostAsync($"player/{playerId}/change-name", name);
  }

  public record AddToSquadDto(int PlayerInId, int PlayerOutId);

  public async Task<HttpResponseMessage> AddToSquad(AddToSquadDto addToSquad)
  {
    return await PostAsync($"player/add-to-squad", addToSquad);
  }

  public async Task<HttpResponseMessage> BuyStaff(StaffType staffType)
  {
    return await PostAsync(   $"staff/buy-staff/{staffType}", null);
  }

  public async Task<string> GetContent(HttpResponseMessage response)
  {
    return await response.Content.ReadAsStringAsync();
  }

  private async Task<HttpResponseMessage> PostAsync(string requestUri, object content)
  {
    return await _client.PostAsJsonAsync(requestUri, content);
  }
}