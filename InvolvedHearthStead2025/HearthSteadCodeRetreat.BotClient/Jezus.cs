using HearthStead.Dto;
using HearthStead.Dto.Enums;

namespace HearthSteadCodeRetreat.BotClient;

public static class Jezus
{
    private static int _daysTrained = 0;

    public static async Task Run(HearthSteadDto hearthSteadDto, HearthSteadHttpClient client)
    {
        var daysUntilRaidCounter = hearthSteadDto.GuardTower.AvailableUpgrades!.Count(x => x.Unlocked) + 1;
        /*Console.WriteLine(daysUntilRaidCounter);
        Console.WriteLine(_daysTrained);
        Console.WriteLine("New day started for HearthStead");*/

        var myGuy = hearthSteadDto.Villagers.Single(x => x.Name == client.GetVillagerName());
        //Console.WriteLine("Health status: " + myGuy.Health);

        //Console.WriteLine("=======================");
        /*foreach (var myGuySkill in myGuy.Skills)
        {
            Console.WriteLine(myGuySkill.Name + " " + myGuySkill.Proficiency);
        }*/

        /*Console.WriteLine("=======================");
        Console.WriteLine("");*/

        if (hearthSteadDto is { RaidSpotted: true, DaysUntilRaid: 1 })
        {
            Console.WriteLine($"{client.GetVillagerName()}: Defending");
            await client.Defend();
            return;
        }

        if (_daysTrained < daysUntilRaidCounter && !hearthSteadDto.RaidSpotted &&
            hearthSteadDto.TrainingGrounds.AvailableUpgrades.Any(x => x.Unlocked))
        {
            if (myGuy.Skills.First(x => x.Name == "Combat").Proficiency != "Virtuoso")
            {
                Console.WriteLine($"{client.GetVillagerName()}: Training");
                await client.Train();
                return;
            }

            _daysTrained++;
        }

        if (_daysTrained >= daysUntilRaidCounter)
        {
            _daysTrained = 0;
        }

        if (hearthSteadDto.GuardTower.Status != StructureStatusDto.Dilapidated
            && hearthSteadDto.GuardTower.Status != StructureStatusDto.RunDown &&
            !hearthSteadDto.RaidSpotted && hearthSteadDto.DaysUntilRaid == 0)
        {
            Console.WriteLine($"{client.GetVillagerName()}: Guarding");
            await client.Guard();
            _daysTrained = 0;
            return;
        }

        if (hearthSteadDto.Storage.MaterialStorage.First(x => x.MaterialType == MaterialTypeDto.Charcoal)
                .MaterialQuantity <
            hearthSteadDto.Storage.Capacity)
        {
            Console.WriteLine($"{client.GetVillagerName()}: Chopping coal");
            await client.CraftingMakeCharcoal();
            return;
        }

        if (hearthSteadDto.Storage.MaterialStorage.First(x => x.MaterialType == MaterialTypeDto.Stone)
                .MaterialQuantity <
            hearthSteadDto.Storage.Capacity)
        {
            Console.WriteLine($"{client.GetVillagerName()}: Mining Stone");
            await client.MineStone();
            return;
        }

        if (hearthSteadDto.Storage.MaterialStorage.First(x => x.MaterialType == MaterialTypeDto.Ore).MaterialQuantity <
            hearthSteadDto.Storage.Capacity)
        {
            Console.WriteLine($"{client.GetVillagerName()}: Mining Ore");
            await client.MineOre();
            return;
        }

        if (hearthSteadDto.Storage.MaterialStorage.First(x => x.MaterialType == MaterialTypeDto.Wood).MaterialQuantity <
            hearthSteadDto.Storage.Capacity)
        {
            Console.WriteLine($"{client.GetVillagerName()}: Chopping Wood");
            await client.ChopWood();
            return;
        }

        Console.WriteLine($"{client.GetVillagerName()}: Guard because nothing else to do");
        await client.Guard();
    }
}
