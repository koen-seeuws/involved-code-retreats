namespace InvolvedInvestments.Investor.Models;

public class Transaction
{
    public required TransactionAction Action { get; set; }
    public required string Company { get; set; }
    public required int Amount { get; set; }
    public required DateOnly TimeStamp { get; set; }
}