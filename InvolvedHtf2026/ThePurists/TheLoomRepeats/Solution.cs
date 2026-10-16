namespace TheLoomRepeats;

public class Solution
{
    public char Forecast(IEnumerable<char> threadPulls, long targetIndex)
    {
        var firstIndices = new Dictionary<char, int>();
        var values = new List<char>();

        foreach (var value in threadPulls)
        {
            if (firstIndices.TryGetValue(value, out var loopStart))
            {
                var loopLength = values.Count - loopStart;
                var offset = (targetIndex - loopStart) % loopLength;
                return values[loopStart + (int)offset];
            }

            firstIndices.Add(value, values.Count);
            values.Add(value);

            if (targetIndex == values.Count - 1)
            {
                return value;
            }
        }

        throw new ArgumentException("The thread ended before the target or a repeating loop was found.", nameof(threadPulls));
    }
}