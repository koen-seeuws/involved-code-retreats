using System.Globalization;
using HearthStead.Dto.Materials;

namespace HearthStead.Dto.Upgrades;

public class UpgradeDto
{
    public string Name { get; set; } 
    public string Description { get; set; } 
    public List<MaterialDto> UpgradeCost { get; set; } 
    public bool Unlocked { get; set; }

    public string iet()
    {
        var stuff = $"{Name}; " + Unlocked.ToString();

        UpgradeCost.ForEach(r =>
        {
            stuff += r.MaterialType;
            stuff += '=';
            stuff += r.MaterialQuantity;
            stuff += "&";
        });

        return stuff;
    }
}