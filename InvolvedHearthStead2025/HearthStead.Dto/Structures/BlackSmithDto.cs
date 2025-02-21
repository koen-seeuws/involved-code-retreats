using HearthStead.Dto.Equipments;

namespace HearthStead.Dto.Structures;

public class BlackSmithDto : StructureDto
{
    public List<ForgedEquipmentDto> ForgeableEquipment { get; set; }
}