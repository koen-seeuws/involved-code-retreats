using System.Net.Http.Json;
using InvolvedHtf2025.A3;
using InvolvedHtf2025.Common;

var path = 'A';
var difficulty = Difficulty.Hard;
var sample = false;

var client = await Htf2025ClientFactory.GetAuthenticatedClient();

await client.Start(path, difficulty);

var frequencyMapResponse = await client.GetFromJsonAsync<Dictionary<string, int>>($"{path}/{difficulty}/frequencyMap");
if (frequencyMapResponse is null)
    throw new InvalidOperationException("Failed to get frequency map");

var frequencyMap = frequencyMapResponse
    .Select(r =>
    {
        var range = r.Key.Split("-");
        return (int.Parse(range[0]), int.Parse(range[1]), r.Value);
    })
    .ToList();

var data = await client.GetData<ResonanceStringsDto>(path, difficulty, sample);


var answer = new List<string>();


foreach (var resonanceString in data.ResonanceStrings)
{
    var sumBase10 = 0;
    var numbersBase33 = resonanceString.Split(' ');

    foreach (var numberBase33 in numbersBase33)
    {
        var loudIndex = int.Parse(numberBase33.Split('S')[0].Trim('L'));
        var softIndex = int.Parse(numberBase33.Split('S')[1]);

        var loud = frequencyMap.First(f => loudIndex >= f.Item1 && loudIndex <= f.Item2).Item3;
        var soft = frequencyMap.First(f => softIndex >= f.Item1 && softIndex <= f.Item2).Item3;

        sumBase10 += loud * 33 + soft;
    }

    var resultLoud = sumBase10 / 33;
    var resultSoft = sumBase10 % 33;

    var frequencyLoud = frequencyMap.First(f => f.Value == resultLoud);
    var frequencySoft = frequencyMap.First(f => f.Value == resultSoft);

    var resultLoudIndex = Random.Shared.Next(frequencyLoud.Item1, frequencyLoud.Item2);
    var resultSoftIndex = Random.Shared.Next(frequencySoft.Item1, frequencySoft.Item2);

    var sumBase33 = $"L{resultLoudIndex}S{resultSoftIndex}";

    answer.Add(sumBase33);
}

await client.SendAnswer(path, difficulty, answer, sample);