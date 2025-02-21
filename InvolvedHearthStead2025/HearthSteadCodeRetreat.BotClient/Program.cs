using HearthSteadCodeRetreat.BotClient;

var hearthSteadHttpClient1 = new HearthSteadHttpClient("Koen", "Seeuws");
var client1 = new BotClient(hearthSteadHttpClient1);
_ = client1.Run();

var hearthSteadHttpClient2 = new HearthSteadHttpClient("Bjert", "hahalol");
var client2 = new BotClient(hearthSteadHttpClient2);
_ = client2.Run();

var hearthSteadHttpClient3 = new HearthSteadHttpClient("Kevin", "ok");
var client3 = new BotClient(hearthSteadHttpClient3);
_ = client3.Run();

var hearthSteadHttpClient4 = new HearthSteadHttpClient("NickThys", "LOL");
var client4 = new BotClient(hearthSteadHttpClient4);
_ = client4.Run();

var hearthSteadHttpClient5 = new HearthSteadHttpClient("DeKoeke", "LOL");
var client5 = new BotClient(hearthSteadHttpClient5);
_ = client5.Run();

var hearthSteadHttpClient6 = new HearthSteadHttpClient("Cookie", "LOL");
var client6 = new BotClient(hearthSteadHttpClient6);
_ = client6.Run();

Console.ReadLine();