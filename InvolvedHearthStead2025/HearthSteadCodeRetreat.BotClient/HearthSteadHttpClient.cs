using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using HearthStead.Dto.Messages;
using HearthStead.Dto.Villagers;

namespace HearthSteadCodeRetreat.BotClient;

public class HearthSteadHttpClient(string villagerName, string villagerSecret)
{
    public readonly string BaseUrl = "https://app-hearthstead.azurewebsites.net/";

    private string? _villagerToken;

    public async Task SettleVillager()
    {
        var villagerCharter = new VillagerCharterDto()
        {
            VillagerName = villagerName,
            VillagerSecret = villagerSecret
        };
        var responseRegister = await GetClient()
            .PostAsync(
                "api/villager/settle", new
                    StringContent(JsonSerializer.Serialize(villagerCharter), Encoding.UTF8, "application/json"),
                CancellationToken.None);
    }

    public async Task GetEndorsement()
    {
        var villagerCharter = new VillagerCharterDto()
        {
            VillagerName = villagerName,
            VillagerSecret = villagerSecret
        };
        var response = await GetClient().PostAsync("api/villager/endorse",
            new StringContent(JsonSerializer.Serialize(villagerCharter), Encoding.UTF8, "application/json"),
            CancellationToken.None);

        _villagerToken = await response.Content.ReadAsStringAsync();
    }

    public string? GetToken()
    {
        return _villagerToken;
    }

    public HttpClient GetClient()
    {
        var client = new HttpClient();
        client.BaseAddress = new Uri(BaseUrl);

        if (!string.IsNullOrEmpty(_villagerToken))
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _villagerToken);
        }

        return client;
    }

    public string GetVillagerName()
    {
        return villagerName;
    }



    public async Task PostMessage(string message)
    {
        var responseRegister = await GetClient()
            .PostAsJsonAsync(
                "api/MessageBoard/PostMessage", message, CancellationToken.None);
    }

    public async Task RemoveMessage(Guid messageId)
    {
        var responseRegister = await GetClient()
            .DeleteAsync(
                $"api/MessageBoard/RemoveMessage?messageId={messageId}",
                CancellationToken.None);
    }

    //Crafting
    public async Task CraftingMakeCharcoal()
    {
        var response = await GetClient().PostAsync("api/Crafting/MakeCharcoal", null);
        response.EnsureSuccessStatusCode();
    }

    public async Task CraftingMakePlanks()
    {
        var response = await GetClient().PostAsync("api/Crafting/MakePlanks", null);
        response.EnsureSuccessStatusCode();
    }

    public async Task CraftingForge(string equipment)
    {
        var response = await GetClient().PostAsJsonAsync("api/Crafting/Forge", equipment);
        response.EnsureSuccessStatusCode();
    }

    public async Task Guard()
    {
        var response = await GetClient().PostAsync("api/Combat/Guard", null);
        response.EnsureSuccessStatusCode();
    }

    public async Task Train()
    {
        var response = await GetClient().PostAsync("api/Combat/Train", null);
        response.EnsureSuccessStatusCode();
    }

    public async Task Defend()
    {
        var response = await GetClient().PostAsync("api/Combat/Defend", null);
        response.EnsureSuccessStatusCode();
    }

    public async Task Revitalize(string name)
    {
        var response = await GetClient().PostAsJsonAsync("api/Revitalize/Resurrect", name);
        response.EnsureSuccessStatusCode();
    }

    public async Task ChopWood()
    {
        await GetClient().PostAsync("api/Harvest/CutWood", null,
            CancellationToken.None);
    }

    public async Task MineOre()
    {
        await GetClient().PostAsync("api/Harvest/MineOre", null,
            CancellationToken.None);
    }

    public async Task MineStone()
    {
        await GetClient().PostAsync("api/Harvest/MineStone", null,
            CancellationToken.None);
    }

    public async Task Farm(string? farmField = null)
    {
        await GetClient().PostAsync("api/Harvest/Farm",
            new StringContent(JsonSerializer.Serialize(farmField), Encoding.UTF8, "application/json"), CancellationToken.None);
    }
    
    public async Task Extinguish(string structureName)
    {
        var response = await GetClient().PostAsync("api/Maintenance/Extinguishing",
            new StringContent(JsonSerializer.Serialize(structureName), Encoding.UTF8, "application/json"),
            CancellationToken.None);
        response.EnsureSuccessStatusCode();
    }

    public async Task Repair(string? structure = null)
    {
        await GetClient().PostAsync("api/maintenance/repair",
            new StringContent(JsonSerializer.Serialize(structure), Encoding.UTF8, "application/json"), CancellationToken.None);
    }

    public async Task Upgrade(string? structure = null)
    {
        await GetClient().PostAsJsonAsync("api/Maintenance/MakeUpgrade", structure, CancellationToken.None);
    }
}
