using HearthStead.Dto;
using HearthStead.Dto.Materials;
using HearthStead.Dto.Structures;

namespace HearthSteadCodeRetreat.BotClient;

public static class UpgradeStuff
{
    public static async Task<bool> Upgrade(HearthSteadHttpClient client, HearthSteadDto hearthSteadDto, int offset = 0)
    {
        var structures = hearthSteadDto.GetStructures();

        foreach (var structure in structures.Concat(hearthSteadDto.Houses).Skip(offset))
        {
            if (structure.Repairable && structure.RepairCost != null &&
                Can(hearthSteadDto.Storage.MaterialStorage, structure.RepairCost))
            {
                Console.WriteLine($"{client.GetVillagerName()}: Repairing {structure.Name}");
                await client.Repair(structure.Name);
                return true;
            }
        }
        
        var allUpgrades = hearthSteadDto.GetStructures().Skip(offset)
            .Where(s => !s.Repairable && s.AvailableUpgrades != null)
            .SelectMany(s => s.AvailableUpgrades)
            .Where(u => !u.Unlocked && Can(hearthSteadDto.Storage.MaterialStorage, u.UpgradeCost))
            .OrderBy(a => a.UpgradeCost.Sum(v => v.MaterialQuantity));
        
        var upgrade = allUpgrades.FirstOrDefault();

        if (upgrade is not null)
        {
            Console.WriteLine($"{client.GetVillagerName()}: Upgrading {upgrade.Name}");
            await client.Upgrade(upgrade.Name);
            return true;
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