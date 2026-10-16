namespace TheLedgerOfBrokenVows;

public class Solution
{
    public long AuditLedger(IEnumerable<int> ledgerEntries)
    {
        long total = 0;
        var historyCount = 0;
        var previous = 0;
        var beforePrevious = 0;

        foreach (var entry in ledgerEntries)
        {
            var inRange = entry is >= 1 and <= 500;
            var corruptedByHistory = historyCount == 2 &&
                                     (entry == previous || (long)entry - previous == (long)previous - beforePrevious);

            if (inRange && !corruptedByHistory)
            {
                total += entry;
            }

            beforePrevious = previous;
            previous = entry;
            historyCount = Math.Min(historyCount + 1, 2);
        }

        return total;
    }
}