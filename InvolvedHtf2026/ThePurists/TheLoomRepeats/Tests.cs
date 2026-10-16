namespace TheLoomRepeats;

public class TheLoomRepeatsTests
{
    [Theory]
    [InlineData("abc", "x1y2", 0L, 'a')]
    [InlineData("abc", "x1y2", 1L, 'b')]
    [InlineData("abc", "x1y2", 2L, 'c')]
    [InlineData("abc", "x1y2", 3L, 'x')]
    [InlineData("abc", "x1y2", 6L, '2')]
    [InlineData("abc", "x1y2", 7L, 'x')]
    [InlineData("abc", "x1y2", 10L, '2')]
    [InlineData("abc", "x1y2", 40_000_000_000L, '1')]
    [InlineData("abc", "x1y2", long.MaxValue, 'x')]
    [InlineData("", "abc", 0L, 'a')]
    [InlineData("", "abc", 2L, 'c')]
    [InlineData("", "abc", 3L, 'a')]
    [InlineData("", "abc", long.MaxValue, 'b')]
    [InlineData("", "x", long.MaxValue, 'x')]
    [InlineData("abc", "x", 40_000_000_000L, 'x')]
    [InlineData("a", "\0b", 2L, 'b')]
    [InlineData("a", "\0b", 3L, '\0')]
    public void Forecast_ReturnsTheValueAtTheTarget(string warmUp, string loop, long targetIndex, char expected)
    {
        var reads = 0;

        IEnumerable<char> Thread()
        {
            foreach (var value in warmUp)
            {
                Assert.True(++reads <= warmUp.Length + loop.Length + 1);
                yield return value;
            }

            while (true)
            {
                foreach (var value in loop)
                {
                    Assert.True(++reads <= warmUp.Length + loop.Length + 1);
                    yield return value;
                }
            }
        }

        var result = new Solution().Forecast(Thread(), targetIndex);

        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(1L, 2, 'b')]
    [InlineData(40_000_000_000L, 8, '1')]
    public void Forecast_UsesOneEnumeratorAndDisposesIt(long targetIndex, int expectedReads, char expected)
    {
        var enumerations = 0;
        var reads = 0;
        var disposed = false;

        IEnumerable<char> Thread()
        {
            Assert.Equal(1, ++enumerations);
            try
            {
                foreach (var value in "abcx1y2x")
                {
                    reads++;
                    yield return value;
                }

                Assert.Fail("The thread must not be read beyond its first repeated value.");
            }
            finally
            {
                disposed = true;
            }
        }

        var result = new Solution().Forecast(Thread(), targetIndex);

        Assert.Equal(expected, result);
        Assert.Equal(expectedReads, reads);
        Assert.Equal(1, enumerations);
        Assert.True(disposed);
    }
}