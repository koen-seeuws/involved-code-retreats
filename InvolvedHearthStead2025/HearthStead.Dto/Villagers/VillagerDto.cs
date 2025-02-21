using HearthStead.Dto.Enums;
using HearthStead.Dto.Structures;

namespace HearthStead.Dto.Villagers;

public class VillagerDto
{
    public string Name { get; set; } 
    public HungerStatusDto Hunger { get; set; } 
    public HealthStatusDto Health { get; set; }
    public HouseDto ClaimedHouse { get; set; } 
    public List<SkillDto> Skills { get; set; } 
    
}