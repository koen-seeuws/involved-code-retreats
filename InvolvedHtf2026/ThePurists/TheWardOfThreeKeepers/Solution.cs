namespace TheWardOfThreeKeepers;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Solution
{
    public IEnumerable<string> ReadRunes(IEnumerable<string> rawLines, int expectedLength, string allowedRunes)
    {
        var allowed = allowedRunes.ToHashSet();

        return rawLines.Where(line =>
            line.Length == expectedLength && line.All(allowed.Contains));
    }

    public IEnumerable<int> DistillWard(IEnumerable<string> validLines, int wordLength, IReadOnlyDictionary<char, int> runeValues)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(wordLength);

        var ward = new StringBuilder();
        var linePosition = 0;
        foreach (var line in validLines)
        {
            if (linePosition % 2 == 0)
            {
                ward.Append(line);
            }
            else
            {
                for (var i = line.Length - 1; i >= 0; i--)
                {
                    ward.Append(line[i]);
                }
            }

            linePosition++;
        }

        var wordSums = new List<int>();
        for (var start = 0; start + wordLength <= ward.Length; start += wordLength)
        {
            var sum = 0;
            for (var i = start; i < start + wordLength; i++)
            {
                sum += runeValues[ward[i]];
            }

            wordSums.Add(sum);
        }

        return wordSums;
    }

    public long SealTheWard(IEnumerable<int> wordSums, IEnumerable<int> counterSeals)
    {
        return wordSums
            .Zip(counterSeals)
            .Sum(pair => Math.Max(0L, (long)pair.First - pair.Second));
    }
}