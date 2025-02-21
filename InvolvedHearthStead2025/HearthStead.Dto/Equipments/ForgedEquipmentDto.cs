using HearthStead.Dto.Materials;

namespace HearthStead.Dto.Equipments;

public class ForgedEquipmentDto
{
    public int Tier { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Type { get; set; }
    public MaterialDto? Cost { get; set; } 
}