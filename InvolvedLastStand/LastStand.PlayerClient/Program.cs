// See https://aka.ms/new-console-template for more information

using LastStand.Helpers;

var client = new LastStandClient();
var cache = new LastStandCache();
var sub = cache._subscriber;

//Config
const double
    HealthPercentageToRest =
        0.50; //The health percentage under which a player has to be to send him to rest (Default: 0.5)
const bool OverrideResourceLimit = false; //Always try farming the resource (default: false)
const int
    MaxReturnsResourceStable =
        15; //How many player returns a resource may be the same as before before farming the next resource
var baseSizeNumber = 8; //Sort of like a base variable to determine build amounts
const int towerBaseSizeIncrement = 1; //Default: 1
const int lighthouseBaseSizeIncrement = 2; //Default: 1


//await client.ResetGame();
await client.StartGame();

var game = cache.GetGame();

//First action
var firstPlayer = game.Players.First();
await client.SendOutAsync(new SendRequest
{
    CombatBehaviour = CombatBehaviour.ignore,
    Commands = new List<string> { $"{Constants.Collect} {Constants.Grain}" },
    Items = new List<string>(),
    PlayerName = firstPlayer.Name
});

if (game.Buildings.ContainsKey(Constants.Tower))
    baseSizeNumber += towerBaseSizeIncrement;

if (game.Buildings.ContainsKey(Constants.Lighthouse))
    baseSizeNumber += lighthouseBaseSizeIncrement;

//Variables to determine whether all resources of types have been farmed
var resourceAmounts = new Dictionary<string, int>
    { { Constants.Grain, 0 }, { Constants.Wood, 0 }, { Constants.Stone, 0 }, { Constants.Steel, 0 } };
var returnsResourceStable = new Dictionary<string, int>
    { { Constants.Grain, 0 }, { Constants.Wood, 0 }, { Constants.Stone, 0 }, { Constants.Steel, 0 } };

// A player has returned and is available again
sub.Subscribe(Setup.User + "-" + Constants.PlayerReturned).OnMessage(async x =>
{
    Console.WriteLine(x);

    await CheckAndBuildStorage();
    await CheckAndBuildHousing();
    await CheckAndBuildExpansion();
    await CheckAndBuildDefense();
    await CheckAndBuildSpecial();
    await CheckAndCraftItems();

    await SendOutPlayers();
});

// Attackers have appeared
sub.Subscribe(Setup.User + "-" + Constants.AttackerAppeared).OnMessage(async x =>
{
    Console.WriteLine(x);

    game = cache.GetGame();

    var usablePlayers = game.Players
        .Where(player =>
            //  (player.Name.Contains(Constants.Default) || player.Name.Contains(Constants.Peasant)) &&
            player.IsHome &&
            !player.Dead &&
            !player.Active)
        .ToList();
});
// Your base is being attacked
sub.Subscribe(Setup.User + "-" + Constants.BaseAttacked).OnMessage(async x =>
{
    Console.WriteLine(x);

    game = cache.GetGame();

    //Get strongest player closest to base
    var usablePlayers = game.Players
        .Where(player =>
            !player.Dead &&
            !player.Active)
        .OrderBy(player => player.DistanceToBase)
        .ThenByDescending(player => player.DerivedStats.Damage)
        .ThenByDescending(player => player.DerivedStats.Defense)
        .ThenByDescending(player => player.CurrentHealth)
        .ToList();

    foreach (var player in usablePlayers)
    {
        var command = "attack";
        await client.SendOutAsync(new SendRequest
        {
            PlayerName = player.Name,
            Commands = new List<string> { command },
            Items = player.EquipedItems.Values.ToList(),
            CombatBehaviour = CombatBehaviour.fight
        });
    }
});
// Game over
sub.Subscribe(Setup.User + "-" + Constants.GameEnded).OnMessage(x =>
{
    Console.WriteLine(x);

    game = cache.GetGame();

    Console.WriteLine(
        $"Rounds survived: {game.Round}, Highscore: {game.RoundHighScore}{(game.Round > game.RoundHighScore ? ", New record!" : string.Empty)}");
});

Console.WriteLine("Press enter to stop");
Console.ReadLine();

await client.StopGame();

async Task SendOutPlayers()
{
    game = cache.GetGame();

    //Weakest players must collect
    var usablePlayers = game.Players
        .Where(player =>
            //  (player.Name.Contains(Constants.Default) || player.Name.Contains(Constants.Peasant)) &&
            player.IsHome &&
            !player.Dead &&
            !player.Active)
        .OrderBy(player => player.DerivedStats.Damage)
        .ThenBy(player => player.DerivedStats.Defense)
        .ToList();

    foreach (var player in usablePlayers)
    {
        game = cache.GetGame();
        var capacity = 1;
        if (player.Name.Contains("worker"))
            capacity = 3;
        if (player.Name.Contains("elite"))
            capacity = 5;
        var commands = new List<string>();


        if (player.CurrentHealth <= HealthPercentageToRest * player.MaxHealth)
            commands.Add("rest");

        var resources = new Dictionary<string, int>
        {
            { Constants.Grain, game.Resources.Grain }, { Constants.Wood, game.Resources.Wood },
            { Constants.Stone, game.Resources.Stone }, { Constants.Steel, game.Resources.Steel }
        };

        var resetResourceAmounts = game.Round % 100 <= 10;
        if (resetResourceAmounts)
            resourceAmounts = new Dictionary<string, int>
                { { Constants.Grain, 0 }, { Constants.Wood, 0 }, { Constants.Stone, 0 }, { Constants.Steel, 0 } };

        foreach (var resource in resources.Keys)
        {
            if (resourceAmounts[resource] == resources[resource])
                returnsResourceStable[resource]++;
            else
                returnsResourceStable[resource] = 0;

            var commandsMaxed = commands.Count >= capacity;
            var resourceMaxed = !OverrideResourceLimit && resources[resource] >= game.Resources.Limit;
            //var resourceStable = returnsResourceStable[resource] >= maxReturnsResourceStable;
            var resourceStable = false;

            if (!resourceMaxed && !commandsMaxed && !resourceStable)
                commands.Add($"{Constants.Collect} {resource}");
        }

        resourceAmounts = resources;

        //Get random item to give to player
        var availableItems = game.Inventory
            .Where(kvp => !string.IsNullOrWhiteSpace(kvp.Key) && kvp.Value > 0)
            .Select(kvp => kvp.Key)
            .ToArray();
        //TODO: Ordered best to worst 
        var orderedItems = Random.Shared.GetItems(availableItems, availableItems.Length);
        var itemsSelection = orderedItems
            .DistinctBy(ItemsHelper.GetItemType)
            .ToList();

        await client.SendOutAsync(new SendRequest
        {
            PlayerName = player.Name,
            Commands = commands,
            Items = itemsSelection,
            CombatBehaviour = CombatBehaviour.fight
        });
    }
}

async Task CheckAndBuildExpansion()
{
    game = cache.GetGame();

    var towerBuilt = game.Buildings.ContainsKey(Constants.Tower);
    var hasResourcesForTower = game.Resources.Wood >= 100;
    if (!towerBuilt && hasResourcesForTower)
    {
        await client.CommandAsync($"{Constants.Build} {Constants.Tower}");
        baseSizeNumber += towerBaseSizeIncrement;
        return;
    }

    var lighthouseBuilt = game.Buildings.ContainsKey(Constants.Lighthouse);
    var hasResourcesForLighthouse = game.Resources is { Grain: >= 200, Wood: >= 200, Stone: >= 100 };
    if (!lighthouseBuilt && hasResourcesForLighthouse)
    {
        await client.CommandAsync($"{Constants.Build} {Constants.Lighthouse}");
        baseSizeNumber += lighthouseBaseSizeIncrement;
        return;
    }
}

async Task CheckAndBuildSpecial()
{
    game = cache.GetGame();

    var barracksBuilt = game.Buildings.ContainsKey(Constants.Barracks);
    var hasResourcesForBarracks = game.Resources.Wood >= 100;
    if (!barracksBuilt && hasResourcesForBarracks)
    {
        await client.CommandAsync($"{Constants.Build} {Constants.Barracks}");
        return;
    }

    var sanctuaryBuilt = game.Buildings.ContainsKey(Constants.Sanctuary);
    var hasResourcesForSanctuary = game.Resources is { Wood: >= 100, Stone: >= 200 };
    if (!sanctuaryBuilt && hasResourcesForSanctuary)
    {
        await client.CommandAsync($"{Constants.Build} {Constants.Sanctuary}");
        return;
    }
}

async Task CheckAndBuildStorage()
{
    game = cache.GetGame();

    var farmBuilt = game.Buildings.ContainsKey(Constants.Farm);
    var hasResourcesForFarm = game.Resources is { Limit: < 100, Grain: >= 50 };
    if (!farmBuilt && hasResourcesForFarm)
    {
        await client.CommandAsync($"{Constants.Build} {Constants.Farm}");
        return;
    }

    var windmillBuilt = game.Buildings.ContainsKey(Constants.Windmill);
    var hasResourcesForWindmill = game.Resources is { Limit: < 250, Grain: >= 100, Wood: >= 50 };
    if (!windmillBuilt && hasResourcesForWindmill)
    {
        await client.CommandAsync($"{Constants.Build} {Constants.Windmill}");
        return;
    }

    var quarryBuilt = game.Buildings.ContainsKey(Constants.Quarry);
    var hasResourcesForQuarry = game.Resources is { Limit: < 500, Grain: >= 200, Wood: >= 100, Stone: >= 50 };
    if (!quarryBuilt && hasResourcesForQuarry)
    {
        await client.CommandAsync($"{Constants.Build} {Constants.Quarry}");
        game = cache.GetGame();
    }

    var hasResourcesForStorage = game.Resources.Grain >= game.Resources.Limit &&
                                 game.Resources.Wood >= game.Resources.Limit &&
                                 game.Resources.Stone >= game.Resources.Limit &&
                                 game.Resources.Steel >= game.Resources.Limit;
    if (quarryBuilt && hasResourcesForStorage)
    {
        await client.CommandAsync($"{Constants.Build} {Constants.Storage}");
        //baseSizeNumber++;
        game = cache.GetGame();
    }
}

async Task CheckAndBuildHousing()
{
    game = cache.GetGame();

    var livingElites = game.Players
        .Where(player =>
            player.Name.Contains(Constants.Elite) &&
            !player.Dead)
        .ToList();
    var hasResourcesForCityHouse = game.Resources is { Grain: >= 200, Wood: >= 100, Stone: >= 50, Steel: >= 20 };
    if (livingElites.Count <= baseSizeNumber * 3 && hasResourcesForCityHouse)
    {
        await client.CommandAsync($"{Constants.Build} {Constants.CityHouse}");
        return;
    }

/*
    var livingWorkers = game.Players
        .Where(player =>
            player.Name.Contains(Constants.Worker) &&
            !player.Dead)
        .ToList();
    var hasResourcesForHouse = game.Resources is { Grain: >= 100, Wood: >= 50, Stone: >= 20 };
    if (livingWorkers.Count <= baseSizeNumber * 2 && hasResourcesForHouse)
    {
        await client.CommandAsync($"{Constants.Build} {Constants.House}");
        return;
    }

    var livingPeasants = game.Players
        .Where(player =>
            (player.Name.Contains(Constants.Default) || player.Name.Contains(Constants.Peasant)) &&
            !player.Dead)
        .ToList();
    var hasResourcesForTent = game.Resources is { Grain: >= 50, Wood: >= 20 };
    if (livingPeasants.Count <= baseSizeNumber && hasResourcesForTent)
    {
        await client.CommandAsync($"{Constants.Build} {Constants.Tent}");
        return;
    }*/
}

async Task CheckAndBuildDefense()
{
    game = cache.GetGame();

    game.Buildings.TryGetValue(Constants.Ballista, out var amountOfbalistas);
    var hasResourcesForBalista = game.Resources is { Wood: >= 100, Stone: >= 50, Steel: >= 60 };
    if (amountOfbalistas <= baseSizeNumber && hasResourcesForBalista)
    {
        await client.CommandAsync($"{Constants.Build} {Constants.Ballista}");
    }


    game.Buildings.TryGetValue(Constants.Trebuchet, out var trebuchets);
    var hasResourcesForTrebuchet = game.Resources is { Wood: >= 100, Stone: >= 100, Steel: >= 30 };
    if (trebuchets <= baseSizeNumber && hasResourcesForTrebuchet)
    {
        await client.CommandAsync($"{Constants.Build} {Constants.Trebuchet}");
    }
}


async Task CheckAndCraftItems()
{
    game = cache.GetGame();
    var barracksBuilt = game.Buildings.ContainsKey(Constants.Barracks);

    //Crating only possible with Barracks
    if (barracksBuilt)
    {
        //Craft farming items
        //if (game.Round < 1000)
        {
            /*
            if (game.Resources.Grain >= 100)
            {
                await client.CommandAsync($"{Constants.Craft} {Constants.Sickle}");
                return;
            }

            if (game.Resources is { Wood: >= 100, Stone: >= 50, Steel: >= 20 })
            {
                await client.CommandAsync($"{Constants.Craft} {Constants.Axe}");
                return;
            }

            if (game.Resources is { Wood: >= 50, Steel: >= 50 })
            {
                await client.CommandAsync($"{Constants.Craft} {Constants.Pickaxe}");
                return;
            }

            if (game.Resources is { Grain: >= 200, Wood: >= 50, Steel: >= 50 })
            {
                await client.CommandAsync($"{Constants.Craft} {Constants.Scythe}");
                return;
            }*/
        }

        /*
       if (game.Resources.Steel >= 2000)
       {

           var steelItems = new[]
           {
               Constants.PlateHelmet, Constants.PlateGloves, Constants.PlateBoots
               /*,Constants.IronShield,Constants.PlateArmor, Constants.PlatePants, Constants.Crossbow
           };

           var steelItem = Random.Shared.GetItems(steelItems, 1).FirstOrDefault();
           await client.CommandAsync($"{Constants.Craft} {steelItem}");
       }*/
        
        /*
         client.CommandAsync($"{Constants.Craft} {Constants.PlateBoots}");
         client.CommandAsync($"{Constants.Craft} {Constants.PlateBoots}");
         client.CommandAsync($"{Constants.Craft} {Constants.PlateBoots}");
         client.CommandAsync($"{Constants.Craft} {Constants.PlateBoots}");
         client.CommandAsync($"{Constants.Craft} {Constants.PlateBoots}");
         client.CommandAsync($"{Constants.Craft} {Constants.PlateBoots}");
         client.CommandAsync($"{Constants.Craft} {Constants.PlateBoots}");
         client.CommandAsync($"{Constants.Craft} {Constants.PlateBoots}");
         client.CommandAsync($"{Constants.Craft} {Constants.PlateBoots}");
         client.CommandAsync($"{Constants.Craft} {Constants.PlateBoots}");
         
         client.CommandAsync($"{Constants.Craft} {Constants.PlateGloves}");
         client.CommandAsync($"{Constants.Craft} {Constants.PlateGloves}");
         client.CommandAsync($"{Constants.Craft} {Constants.PlateGloves}");
         client.CommandAsync($"{Constants.Craft} {Constants.PlateGloves}");
         client.CommandAsync($"{Constants.Craft} {Constants.PlateGloves}");
         client.CommandAsync($"{Constants.Craft} {Constants.PlateGloves}");
         client.CommandAsync($"{Constants.Craft} {Constants.PlateGloves}");
         client.CommandAsync($"{Constants.Craft} {Constants.PlateGloves}");
         client.CommandAsync($"{Constants.Craft} {Constants.PlateGloves}");
         client.CommandAsync($"{Constants.Craft} {Constants.PlateGloves}");
         
         client.CommandAsync($"{Constants.Craft} {Constants.PlateHelmet}");
         client.CommandAsync($"{Constants.Craft} {Constants.PlateHelmet}");
         client.CommandAsync($"{Constants.Craft} {Constants.PlateHelmet}");
         client.CommandAsync($"{Constants.Craft} {Constants.PlateHelmet}");
         client.CommandAsync($"{Constants.Craft} {Constants.PlateHelmet}");
         client.CommandAsync($"{Constants.Craft} {Constants.PlateHelmet}");
         client.CommandAsync($"{Constants.Craft} {Constants.PlateHelmet}");
         client.CommandAsync($"{Constants.Craft} {Constants.PlateHelmet}");
         client.CommandAsync($"{Constants.Craft} {Constants.PlateHelmet}");
         client.CommandAsync($"{Constants.Craft} {Constants.PlateHelmet}");
         
         client.CommandAsync($"{Constants.Craft} {Constants.Crossbow}");
         client.CommandAsync($"{Constants.Craft} {Constants.Crossbow}");
         client.CommandAsync($"{Constants.Craft} {Constants.Crossbow}");
         client.CommandAsync($"{Constants.Craft} {Constants.Crossbow}");
         client.CommandAsync($"{Constants.Craft} {Constants.Crossbow}");
         client.CommandAsync($"{Constants.Craft} {Constants.Crossbow}");
         client.CommandAsync($"{Constants.Craft} {Constants.Crossbow}");
         client.CommandAsync($"{Constants.Craft} {Constants.Crossbow}");
         client.CommandAsync($"{Constants.Craft} {Constants.Crossbow}");
         client.CommandAsync($"{Constants.Craft} {Constants.Crossbow}");*/
    }

    //Random.Shared.Shuffle(craftableItems);
}