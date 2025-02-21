using HearthStead.Dto.Equipments;
using HearthStead.Dto.Materials;

namespace HearthStead.Dto.Structures;

public class StorageDto : StructureDto
{
    public List<MaterialDto> MaterialStorage { get; set; }
    public List<ForgedEquipmentDto> AvailableForgedEquipment { get; set; }
    public int BaseCapacity { get; set; }
    public int Capacity { get; set; }
}
