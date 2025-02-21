using HearthStead.Dto.ActionLogs;
using HearthStead.Dto.Messages;
using HearthStead.Dto.Structures;
using HearthStead.Dto.Villagers;

namespace HearthStead.Dto;

public class HearthSteadDto
{
    // Game State Properties
    public int CurrentDay { get; set; }
    public int DaysUntilRaid { get; set; }
    
    public bool RaidSpotted { get; set; }
    
    // villager action
    public List<ActionLogDto> ActionLogs { get; set; }
    
    // HearthStead Overview
    public List<VillagerDto> Villagers { get; set; }
    public List<HouseDto> Houses { get; set; }
    
    public MessageBoardDto MessageBoard { get; set; }
    
    // Harvesting Structures
    public FarmDto Farm { get; set; } 
    public ForestDto Forest { get; set; }
    public OreMineDto OreMine { get; set; }
    public StoneMineDto StoneMine { get; set; }
    
    // Crafting Structures
    public BlackSmithDto BlackSmith { get; set; }
    public CharcoalKilnDto CharcoalKiln { get; set; }
    public SawMillDto SawMill { get; set; }
    
    // Combat Structures
    public GuardTowerDto GuardTower { get; set; }
    public TrainingGroundsDto TrainingGrounds { get; set; }
    
    // Facilities
    public StorageDto Storage { get; set; }
    public WorkshopDto Workshop { get; set; }
}