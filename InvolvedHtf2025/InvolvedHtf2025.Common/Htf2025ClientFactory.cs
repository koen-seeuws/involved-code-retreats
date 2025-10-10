using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace InvolvedHtf2025.Common;

public static class Htf2025ClientFactory
{
    private const string BaseAddress = "https://htf2025.azurewebsites.net/api/";
    private const string Username = "Koen";
    private const string Password = "Neok";
    
    public static async Task<HttpClient> GetAuthenticatedClient()
    {
        var client = new HttpClient()
        {
            BaseAddress = new Uri(BaseAddress)
        };

        var tokenResponse = await client.GetFromJsonAsync<TokenResponse>($"Team/Token?teamName={Username}&password={Password}");
        
        if (tokenResponse is null)
        {
            throw new InvalidOperationException("Failed to retrieve token.");
        }
        
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenResponse.Token);

        return client;
    }
}