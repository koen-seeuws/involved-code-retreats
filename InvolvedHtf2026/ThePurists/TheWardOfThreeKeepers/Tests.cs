namespace TheWardOfThreeKeepers;

public class ReadRunesTests
{
    [Theory]
    [InlineData(
        new[] { "ABCD", "AXBC", "DCAB", "AB", "BBBB", "CCDA" },
        4, "ABCD",
        new[] { "ABCD", "DCAB", "BBBB", "CCDA" })]
    [InlineData(
        new[] { "AB", "ABC", "ABCD", "ABCDE" },
        4, "ABCDE",
        new[] { "ABCD" })]
    [InlineData(
        new[] { "XABC", "AXBC", "ABXC", "ABCX", "ABCD" },
        4, "ABCD",
        new[] { "ABCD" })]
    [InlineData(
        new[] { "BBBB", "ABCD", "BBBB", "DCAB" },
        4, "ABCD",
        new[] { "BBBB", "ABCD", "BBBB", "DCAB" })]
    [InlineData(
        new[] { "ABCD", "abcd", "AbCD" },
        4, "ABCD",
        new[] { "ABCD" })]
    [InlineData(
        new[] { "ABCD", "DCAB" },
        4, "AABBCD",
        new[] { "ABCD", "DCAB" })]
    [InlineData(
        new string[] { },
        4, "ABCD",
        new string[] { })]
    [InlineData(
        new[] { "XXXX", "AB", "ABCDE" },
        4, "ABCD",
        new string[] { })]
    [InlineData(
        new[] { "A", "B" },
        1, "",
        new string[] { })]
    [InlineData(
        new[] { "", "A", "" },
        0, "",
        new[] { "", "" })]
    public void ReadRunes_KeepsOnlyMatchingLinesInOriginalOrder(
        string[] rawLines,
        int expectedLength,
        string allowedRunes,
        string[] expected)
    {
        var result = new Solution()
            .ReadRunes(rawLines, expectedLength, allowedRunes);

        Assert.Equal(expected, result);
    }
}

public class DistillWardTests
{
    [Fact]
    public void DistillWard_ReversesOddLinesAndSumsWordsAcrossBoundaries()
    {
        var lines = new[] { "ABCD", "DCAB", "BBBB", "CCDA" };
        var runeValues = new Dictionary<char, int>
        {
            ['A'] = 1,
            ['B'] = 2,
            ['C'] = 3,
            ['D'] = 4
        };

        var result = new Solution().DistillWard(lines, 8, runeValues);

        Assert.Equal(new[] { 20, 19 }, result);
    }

    [Fact]
    public void DistillWard_DropsTrailingRemainder()
    {
        var lines = new[] { "AB", "CD", "E" };
        var runeValues = new Dictionary<char, int>
        {
            ['A'] = 1,
            ['B'] = 2,
            ['C'] = 3,
            ['D'] = 4,
            ['E'] = 5
        };

        var result = new Solution().DistillWard(lines, 3, runeValues);

        Assert.Equal(new[] { 7 }, result);
    }

    [Fact]
    public void DistillWard_ReturnsNoSumsWhenThereIsNoCompleteWord()
    {
        var result = new Solution().DistillWard(
            new[] { "A", "B" },
            3,
            new Dictionary<char, int> { ['A'] = 1, ['B'] = 2 });

        Assert.Empty(result);
    }
}

public class SealTheWardTests
{
    [Fact]
    public void SealTheWard_SubtractsMatchingCounterSealsAndFloorsAtZero()
    {
        var result = new Solution().SealTheWard(
            new[] { 20, 19 },
            new[] { 15, 25 });

        Assert.Equal(5, result);
    }

    [Fact]
    public void SealTheWard_ReturnsZeroWhenThereAreNoWords()
    {
        var result = new Solution().SealTheWard(
            Array.Empty<int>(),
            new[] { 15, 25 });

        Assert.Equal(0, result);
    }

    [Fact]
    public void SealTheWard_PairsOnlyItemsAtMatchingPositions()
    {
        var result = new Solution().SealTheWard(
            new[] { 20, 19, 30 },
            new[] { 15, 25 });

        Assert.Equal(5, result);
    }
}