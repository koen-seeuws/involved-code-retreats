namespace InvolvedProLeaguePlayer.Models.Results;

public class NotifyDayResult(int week, int day, Club club)
{
    public int Week { get; set; } = week;
    public int Day { get; set; } = day;
    public Club Club { get; set; } = club;

    public bool ShouldBuyStaff(out StaffType? staffType)
    {
        foreach (var tempStaffType in Enum.GetValues<StaffType>())
        {
            if (CanBuyStaff(tempStaffType))
            {
                staffType = tempStaffType;
                return true;
            }
        }

        staffType = null;
        return false;
    }

    public bool HasStaff(StaffType staffType)
    {
        return Club.Staff.Any(s => s.StaffType == staffType);
    }

    public bool CanBuyStaff(StaffType staffType)
    {
        return !HasStaff(staffType) && Club.Money >= Club.PricePerStaffType[staffType];
    }

    
}