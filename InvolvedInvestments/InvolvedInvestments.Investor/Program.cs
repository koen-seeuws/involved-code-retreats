using InvolvedInvestments.Investor;
using InvolvedInvestments.Investor.Models;

foreach (var gameNumber in new []{1, 2, 3, 4})
{
    var outputFile =
        $"/Users/koenseeuws/Developer/Code Retreat/InvolvedInvestments/InvolvedInvestments.Investor/Output/Game {gameNumber}.txt";
    File.Delete(outputFile);

    var inputFile =
        $"/Users/koenseeuws/Developer/Code Retreat/InvolvedInvestments/InvolvedInvestments.Investor/Input/Game {gameNumber}.txt";

    var game = new Game(inputFile);

    Console.WriteLine("Successfully parsed game");

    var transactions = new List<Transaction>();

    var currentWallet = game.Budget;
    foreach (var today in game.AllDates.SkipLast(1))
    {
        var stockValuesToday = game.StockValues.Where(sv => sv.TimeStamp == today).ToArray();
        var nextTradingDay = game.AllDates.ElementAt(game.AllDates.IndexOf(today) + 1);
        var stockValuesNextTradingDay = game.StockValues.Where(sv => sv.TimeStamp == nextTradingDay).ToArray();

        var transactionsToday = new List<Transaction>();
        var allCompanies = stockValuesToday
            .OrderBy(s =>
            {
                var sn = stockValuesNextTradingDay.First(sn => sn.Company == s.Company);
                return sn.Value - s.Value;
            })
            .Select(s => s.Company)
            .Distinct()
            .ToArray();

        foreach (var company in allCompanies)
        {
            if (transactionsToday.Count >= game.MaxNumberOfTransactionsPerDay) break;

            var stockValueToday = stockValuesToday.First(s => s.Company == company);
            var stockValueNextTradingDay = stockValuesNextTradingDay.First(s => s.Company == company);

            Transaction? transaction = null;

            int amount = 500;
            
            var moneyForTransaction = amount * stockValueToday.Value;
            
            if (stockValueToday.Value < stockValueNextTradingDay.Value)
            {
                transaction = new Transaction
                {
                    Action = TransactionAction.Buy,
                    Amount = amount,
                    Company = company,
                    TimeStamp = today
                };
                currentWallet -= moneyForTransaction;
            }

            if (stockValueToday.Value > stockValueNextTradingDay.Value)
            {
                transaction = new Transaction
                {
                    Action = TransactionAction.Sell,
                    Amount = amount,
                    Company = company,
                    TimeStamp = today
                };
                currentWallet += moneyForTransaction;
            }

            if (transaction != null) transactionsToday.Add(transaction);
        }


        transactions.AddRange(transactionsToday);
    }

/*
if (allCompanies != null)
{
    var transactionsOnLastDay = new List<Transaction>();
    foreach (var company in allCompanies)
    {
        if (transactionsOnLastDay.Count >= game.MaxNumberOfTransactionsPerDay) continue;
        var transaction = new Transaction
        {
            Action = TransactionAction.Sell,
            Amount = int.MaxValue,
            Company = company,
            TimeStamp = game.AllDates.Last()
        };
        transactionsOnLastDay.Add(transaction);
    }
    transactions.AddRange(transactionsOnLastDay);
}
*/

    var outputLines = transactions
        .Select(t => $"{t.Action.ToString()[0]}-{t.Company}-{t.Amount}-{t.TimeStamp.ToString("yyyyMMdd")};");

    File.WriteAllText(outputFile, string.Join("", outputLines));

    Console.WriteLine("Successfully wrote output");
}