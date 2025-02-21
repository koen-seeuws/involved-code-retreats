using HearthStead.Dto;
using HearthStead.Dto.Enums;
using static HearthSteadCodeRetreat.BotClient.HearthSteadExtensions;

namespace HearthSteadCodeRetreat.BotClient;

public static class FarmingStuff
{
    public static async Task<bool> Run(HearthSteadDto hearthSteadDto, HearthSteadHttpClient client)
    {
       
        var me = hearthSteadDto.Villagers.First(_ => _.Name == client.GetVillagerName());
        var side = me.Name == "Kabouter Wesley" ? FarmSide.Left : FarmSide.Right;

        var selectedFarmField = hearthSteadDto.GetFarmFieldToFarm(side);
        if (selectedFarmField != "")
        {
            Console.WriteLine($"{client.GetVillagerName()}: Farming {selectedFarmField}!!!");
            await client.Farm(selectedFarmField);
            return true;
        }

        return false;
    }
}