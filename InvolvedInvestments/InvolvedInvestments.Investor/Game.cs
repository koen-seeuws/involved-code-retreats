using System.Globalization;
using System.Security.Principal;
using InvolvedInvestments.Investor.Models;

namespace InvolvedInvestments.Investor;

public class Game
{
    public int MaxNumberOfTransactionsPerDay { get; }
    public DateOnly From { get; }
    public DateOnly Until { get; }
    public List<DateOnly> AllDates { get; }
    public decimal Budget { get; }
    public decimal Tax { get; }
    public ICollection<StockValue> StockValues { get; }

    public Game(string gameFilePath)
    {
        var fileLines = File.ReadLines(gameFilePath).ToArray();

        var firstLine = fileLines.First();
        var firstLineParts = firstLine.Split(';');

        MaxNumberOfTransactionsPerDay = int.Parse(firstLineParts[0].Split('=')[1]);
        From = DateOnly.ParseExact(firstLineParts[1].Split('=')[1],
            ["MM/dd/yyyy", "M/d/yyyy", "MM/d/yyyy", "M/dd/yyyy"]);
        Until = DateOnly.ParseExact(firstLineParts[2].Split('=')[1],
            ["MM/dd/yyyy", "M/d/yyyy", "MM/d/yyyy", "M/dd/yyyy"]);
        Budget = decimal.Parse(firstLineParts[3].Split('=')[1], CultureInfo.InvariantCulture);

        StockValues = fileLines
            .Skip(1)
            .Select(stockLine =>
            {
                var stockLineParts = stockLine.Split(',');
                var stockValue = new StockValue
                {
                    Company = stockLineParts[0],
                    TimeStamp = DateOnly.ParseExact(stockLineParts[1], "yyyyMMdd"),
                    Value = decimal.Parse(stockLineParts[2], CultureInfo.InvariantCulture)
                };
                return stockValue;
            })
            .OrderBy(sv => sv.TimeStamp)
            .ToList();

        AllDates = StockValues
            .Select(s => s.TimeStamp)
            .Distinct()
            .ToList();
    }
}