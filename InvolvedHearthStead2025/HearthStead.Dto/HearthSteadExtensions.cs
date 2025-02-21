using HearthStead.Dto.Enums;
using HearthStead.Dto.Structures;
using HearthStead.Dto.Villagers;

namespace HearthStead.Dto;

public static class HearthSteadExtensions
{
    public static VillagerDto GetMyVillager(this HearthSteadDto dto, string name)
    {
        return dto.Villagers.First(x => x.Name == name);
    }

    public static bool AmIDead(this HearthSteadDto dto, string name)
    {
        return dto.GetMyVillager(name).Health == HealthStatusDto.Moribund;
    }

    public static bool VillageNeedsProtection(this HearthSteadDto dto)
    {
        return dto.RaidSpotted && dto.DaysUntilRaid == 1;
    }

    public static int GetResourceCount(this HearthSteadDto dto, MaterialTypeDto type)
    {
        return dto.Storage.MaterialStorage.First(x => x.MaterialType == type).MaterialQuantity;
    }

    public static List<VillagerDto> GetAliveVillagers(this HearthSteadDto dto)
    {
        return dto.Villagers.Where(x => x.Health != HealthStatusDto.Moribund).ToList();
    }

    public static List<VillagerDto> GetDeadVillagers(this HearthSteadDto dto)
    {
        return dto.Villagers.Where(x => x.Health == HealthStatusDto.Moribund).ToList();
    }

    public static List<StructureDto> GetStructures(this HearthSteadDto dto)
    {
        return [dto.Storage, dto.Farm, dto.Forest, dto.OreMine,
            dto.StoneMine, dto.BlackSmith, dto.CharcoalKiln,
            dto.SawMill, dto.GuardTower, dto.TrainingGrounds, dto.Workshop];
    }
    public static List<StructureDto> GetBuringStructures(this HearthSteadDto dto)
    {
        return dto.GetStructures().Where(x => x.OnFire).ToList();
    }
}
