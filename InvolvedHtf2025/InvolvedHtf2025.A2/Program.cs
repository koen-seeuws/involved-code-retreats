using System.Net.Http.Json;
using InvolvedHtf2025.A2;
using InvolvedHtf2025.Common;

var path = 'A';
var difficulty = Difficulty.Medium;
var sample = true;

var client = await Htf2025ClientFactory.GetAuthenticatedClient();

await client.Start(path, difficulty);

var configuration = await client.GetFromJsonAsync<ConfigurationDto>($"{path}/{difficulty}/config");
if(configuration is null)
    throw new InvalidOperationException("Failed to get configuration");

var data = await client.GetData<LifeFormsDto>(path, difficulty, sample);

foreach (var lifeForm in data.LifeForms)
{
    
}



//TODO
var answer = new object();

await client.SendAnswer(path, difficulty, answer, sample);