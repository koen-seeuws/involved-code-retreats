using HearthStead.Dto.Enums;

namespace HearthStead.Dto.Villagers;

public class SkillDto
{
    public string Name { get; set; } 
    public string Proficiency { get; set; } 
    public List<MaterialTypeDto> MaterialProficiencies { get; set; }
}