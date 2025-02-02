namespace InvolvedInvestments.Investor.Models;

public class StockValue
{
    public required string Company { get; set; }
    public required DateOnly TimeStamp { get; set; }
    public required decimal Value { get; set; }
}