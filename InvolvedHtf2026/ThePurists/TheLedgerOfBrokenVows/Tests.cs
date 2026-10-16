namespace TheLedgerOfBrokenVows;

public class TheLedgerOfBrokenVowsTests
{
    [Theory]
    [InlineData(new int[] { }, 0L)]
    [InlineData(new[] { 1 }, 1L)]
    [InlineData(new[] { 500 }, 500L)]
    [InlineData(new[] { 0 }, 0L)]
    [InlineData(new[] { -1 }, 0L)]
    [InlineData(new[] { 501 }, 0L)]
    [InlineData(new[] { int.MinValue, int.MaxValue }, 0L)]
    [InlineData(new[] { 45, 45 }, 90L)]
    [InlineData(new[] { 0, 500 }, 500L)]
    [InlineData(new[] { 500, 501 }, 500L)]
    [InlineData(new[] { 10, 20, 20 }, 30L)]
    [InlineData(new[] { 5, 5, 5 }, 10L)]
    [InlineData(new[] { 10, 20, 30 }, 30L)]
    [InlineData(new[] { 44, 40, 36 }, 84L)]
    [InlineData(new[] { 10, 20, 31 }, 61L)]
    [InlineData(new[] { 10, 20, 30, 40 }, 30L)]
    [InlineData(new[] { 10, 20, 30, 30 }, 30L)]
    [InlineData(new[] { 0, 10, 20 }, 10L)]
    [InlineData(new[] { 502, 501, 500 }, 0L)]
    [InlineData(new[] { 10, 20, 20, 21 }, 51L)]
    [InlineData(new[] { int.MinValue, 1073741825, 2 }, 2L)]
    [InlineData(new[] { int.MaxValue, -1073741824, 1 }, 1L)]
    [InlineData(new[] { 45, 200, 200, 10, 20, 30, 501, 5, 5, 5, 90, 12, 44, 40, 36, 70 }, 536L)]
    public void AuditLedger_SumsOnlyEntriesThatObeyTheLaws(int[] entries, long expected)
    {
        var result = new Solution().AuditLedger(entries);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void AuditLedger_AccumulatesBeyondIntMaxValue()
    {
        var entries = Enumerable.Range(0, 4_500_000)
            .Select(index => (index % 3) switch
            {
                0 => 500,
                1 => 499,
                _ => 497
            });

        var result = new Solution().AuditLedger(entries);

        Assert.Equal(2_244_000_000L, result);
    }

    [Fact]
    public void AuditLedger_EnumeratesTheLedgerOnlyOnce()
    {
        var enumerationCount = 0;

        IEnumerable<int> Entries()
        {
            enumerationCount++;
            Assert.Equal(1, enumerationCount);
            yield return 10;
            yield return 20;
            yield return 30;
            yield return 45;
        }

        var result = new Solution().AuditLedger(Entries());

        Assert.Equal(75L, result);
        Assert.Equal(1, enumerationCount);
    }
}