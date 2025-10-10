using InvolvedHtf2025.Common;

var path = 'B';
var difficulty = Difficulty.Easy;
var sample = true;

var client = await Htf2025ClientFactory.GetAuthenticatedClient();

await client.Start(path, difficulty);

var data = await client.GetData<>(path, difficulty, sample);

//TODO
var answer = new object();

await client.SendAnswer(path, difficulty, answer, sample);