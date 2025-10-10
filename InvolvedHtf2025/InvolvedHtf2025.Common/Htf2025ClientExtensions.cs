using System.Net.Http.Json;

namespace InvolvedHtf2025.Common;

public static class Htf2025ClientExtensions
{
    public static async Task Start(this HttpClient httpClient, char path, Difficulty difficulty)
    {
        var response = await httpClient.GetAsync($"{path}/{difficulty}/start");
        response.EnsureSuccessStatusCode();
    }
    
    public static async Task<T> GetData<T>(this HttpClient httpClient, char path, Difficulty difficulty, bool sample = true)
    {
        var data = await httpClient.GetFromJsonAsync<T>($"{path}/{difficulty}/{(sample ? "sample" : "puzzle")}");
        if(data is null)
            throw new InvalidOperationException("Failed to retrieve data.");
        return data;
    }
    
    public static async Task SendAnswer(this HttpClient httpClient, char path, Difficulty difficulty, object answer, bool sample = true)
    {
        var response = await httpClient.PostAsJsonAsync($"{path}/{difficulty}/{(sample ? "sample" : "puzzle")}", answer);
        response.EnsureSuccessStatusCode();
        var rating = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"{path} - {difficulty} - {(sample ? "Sample" : "Puzzle")}: {rating}");
    }
}