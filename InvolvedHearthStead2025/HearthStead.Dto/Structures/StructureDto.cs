using HearthStead.Dto.Enums;
using HearthStead.Dto.Materials;
using HearthStead.Dto.Upgrades;

namespace HearthStead.Dto.Structures;

public abstract class StructureDto
{
    public string Name { get; set; }
    public List<MaterialDto>? RepairCost { get; set; }
    public List<MaterialDto>? ProductionCost { get; set; }
    public List<UpgradeDto>? AvailableUpgrades { get; set; }
    public StructureStatusDto Status { get; set; }
    public MaterialDto? BaseProduction { get; set; }
    public bool Repairable { get; set; }
    public bool OnFire { get; set; }

    private string FireString = "VUUR";
    private string NoWorries = "ok";

    public string GetStuff()
    {
        var stuff =
            $"{Name}: {(OnFire ? FireString : NoWorries)} | {Status} | repair: ";

        RepairCost?.ForEach(r =>
            {
                stuff += r.MaterialType;
                stuff += '=';
                stuff += r.MaterialQuantity;
                stuff += "&";
            }
        );

        return stuff;
    }

    public string GetProduction()
    {
        var stuff =
            $"{Name}: {Status} | repair: ";

        RepairCost?.ForEach(r =>
            {
                stuff += r.MaterialType;
                stuff += '=';
                stuff += r.MaterialQuantity;
                stuff += "&";
            }
        );
        return stuff;

    }
}