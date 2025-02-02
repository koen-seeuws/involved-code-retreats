namespace InvolvedProLeaguePlayer.Models;

public class Club(
    int Money,
    Stadium Stadium,
    List<Staff> Staff,
    Dictionary<StaffType, int> PricePerStaffType,
    Team Team)
{
    public int Money {get; set;} = Money;
    public Stadium Stadium {get; set;} = Stadium;
    public List<Staff> Staff {get; set;} = Staff;
    public Dictionary<StaffType, int> PricePerStaffType {get; set;} = PricePerStaffType;
    public Team Team {get; set;} = Team;
}
  
  