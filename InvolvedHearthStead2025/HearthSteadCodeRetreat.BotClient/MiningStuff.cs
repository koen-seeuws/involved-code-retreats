using HearthStead.Dto;
using HearthStead.Dto.Enums;
using HearthStead.Dto.Materials;

namespace HearthSteadCodeRetreat.BotClient;

public class MiningStuff
{
    public static async Task<bool> DoMining(HearthSteadDto state, HearthSteadHttpClient client)
    {
        var actionDone = await CraftStuffUsingOre(state, client);
        if (!actionDone)
        {
            var what = Random.Shared.Next(0, 2);

            actionDone = what == 0 ? await MineStone(state, client) : await MineOre(state, client);
            if (!actionDone)
            {
                actionDone = what == 1 ? await MineStone(state, client) : await MineOre(state, client);
            }
        }

        return actionDone;
    }


    private static async Task<bool> MineOre(HearthSteadDto state, HearthSteadHttpClient client)
    {
        foreach (var material in state.Storage.MaterialStorage.OrderBy(m => m.MaterialQuantity))
        {
            if (material.MaterialType != MaterialTypeDto.Ore ||
                material.MaterialQuantity >= state.Storage.Capacity)
                continue;
            Console.WriteLine($"{client.GetVillagerName()}: Mining Ore");
            await client.MineOre();
            return true;
        }

        return false;
    }

    private static async Task<bool> MineStone(HearthSteadDto state, HearthSteadHttpClient client)
    {
        foreach (var material in state.Storage.MaterialStorage.OrderBy(m => m.MaterialQuantity))
        {
            if (material.MaterialType != MaterialTypeDto.Stone ||
                material.MaterialQuantity >= state.Storage.Capacity)
                continue;
            Console.WriteLine($"{client.GetVillagerName()}: Mining Stone");
            await client.MineStone();
            return true;
        }

        return false;
    }

    public static async Task<bool> CraftStuffUsingOre(HearthSteadDto state, HearthSteadHttpClient client)
    {
        foreach (var material in
                 state.Storage.MaterialStorage.Where(m =>
                     m.MaterialType is MaterialTypeDto.Ingots))
        {
            if (material.MaterialQuantity < state.Storage.Capacity)
            {
                if (state.BlackSmith.CanProduce(state.Storage))
                {
                    Console.WriteLine($"{client.GetVillagerName()}: Blacksmithing Ingots");
                    await client.CraftingForge(null);
                    return true;
                }
            }
        }

        //var allEquipment = state.BlackSmith.ForgeableEquipment.Select(x => x.Name);
        //var max = state.Storage.AvailableForgedEquipment.GroupBy(x => x.Name).Max(x => x.Count());


        foreach (var equipment in state.BlackSmith.ForgeableEquipment)
        {
            if (!state.Storage.AvailableForgedEquipment.Any(x => x.Name == equipment.Name))
            {
                if (Can(state.Storage.MaterialStorage, [equipment.Cost]))
                {
                    Console.WriteLine($"{client.GetVillagerName()}: Blacksmithing {equipment.Name}");
                    await client.CraftingForge(equipment.Name);
                    return true;
                }
            }
        }

        return false;
    }

    private static bool Can(List<MaterialDto> owned, List<MaterialDto> needed)
    {
        foreach (var need in needed)
        {
            var own = owned.Single(o => o.MaterialType == need.MaterialType);
            if (need.MaterialQuantity > own.MaterialQuantity)
            {
                return false;
            }
        }

        return true;
    }
}