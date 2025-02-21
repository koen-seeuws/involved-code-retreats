using HearthStead.Dto;
using HearthStead.Dto.Enums;
using HearthStead.Dto.Materials;

namespace HearthSteadCodeRetreat.BotClient;

public static class WoodWorkingStuff
{
    public static async Task<bool> ChopWood(HearthSteadHttpClient client, HearthSteadDto hearthSteadDto)
    {
        foreach (var material in hearthSteadDto.Storage.MaterialStorage.OrderBy(m => m.MaterialQuantity))
        {
            if (material.MaterialType != MaterialTypeDto.Wood ||
                material.MaterialQuantity >= hearthSteadDto.Storage.Capacity)
                continue;
            Console.WriteLine($"{client.GetVillagerName()}: Chopping Wood");
            await client.ChopWood();
            return true;
        }

        return false;
    }

    public static async Task<bool> CraftStuffUsingWood(HearthSteadHttpClient client, HearthSteadDto hearthSteadDto, bool random = true)
    {
        MaterialTypeDto[] materialTypes = [MaterialTypeDto.Charcoal, MaterialTypeDto.Planks];

        if (random)
        {
            Random.Shared.Shuffle(materialTypes);
        }
        
        var materials = hearthSteadDto.Storage.MaterialStorage
                .Where(m => materialTypes.Contains(m.MaterialType))
                .ToArray();
        
        foreach (var material in materials)
        {
            if (material.MaterialQuantity < hearthSteadDto.Storage.Capacity)
            {
                switch (material.MaterialType)
                {
                    case MaterialTypeDto.Planks:
                        if (hearthSteadDto.SawMill.CanProduce(hearthSteadDto.Storage))
                        {
                            Console.WriteLine($"{client.GetVillagerName()}: Crafting Planks");
                            await client.CraftingMakePlanks();
                            return true;
                        }

                        break;
                    case MaterialTypeDto.Charcoal:
                        if (hearthSteadDto.CharcoalKiln.CanProduce(hearthSteadDto.Storage))
                        {
                            Console.WriteLine($"{client.GetVillagerName()}: Crafting Charcoal");
                            await client.CraftingMakeCharcoal();
                            return true;
                        }
                        break;
                }
            }
        }

        return false;
    }

    public static async Task<bool> NickWood(HearthSteadHttpClient client, HearthSteadDto hearthSteadDto)
    {
        var actionDone= await CraftStuffUsingWood(client,hearthSteadDto);
        if(!actionDone)
            actionDone = await ChopWood(client,hearthSteadDto);
        return actionDone;
    }

    public static async Task<bool> KoenWood(HearthSteadHttpClient client, HearthSteadDto hearthSteadDto)
    {
        var actionDone = await CraftStuffUsingWood(client,hearthSteadDto);
        if(!actionDone)
            actionDone= await ChopWood(client,hearthSteadDto);
        return actionDone;
    }
}