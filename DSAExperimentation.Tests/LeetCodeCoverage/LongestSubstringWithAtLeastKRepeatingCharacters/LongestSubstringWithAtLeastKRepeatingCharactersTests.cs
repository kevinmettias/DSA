using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.Tests.LeetCodeCoverage.LongestSubstringWithAtLeastKRepeatingCharacters;

// LeetCode 395. Longest Substring with At Least K Repeating Characters: the classic
// divide-and-conquer over this repo's own HashMap<char,int> - same frequency-count
// primitive/precedent MinimumWindowSubstringTests uses for its own sliding window,
// just used here to find any character whose total count across the current range
// falls below k. Such a character can never appear in any valid answer inside this
// range, so it safely splits the range at every occurrence of it and recurses on
// both halves; a range with no such character is itself a valid answer.
public sealed partial class LongestSubstringWithAtLeastKRepeatingCharactersTests
{
    [Theory]
    [InlineData("aaabb", 3, 3)]
    [InlineData("ababbc", 2, 5)]
    [InlineData("weitong", 2, 0)]
    public void LongestSubstring_LeetCodeExamples_ReturnsLongestQualifyingLength(string s, int k, int expected)
    {
        var actual = LongestSubstring(s, k);
        Assert.Equal(expected, actual);
    }

    private static int LongestSubstring(string s, int k) => LongestSubstringInRange(s, 0, s.Length, k);

    private readonly record struct SubstringRange(int Start, int End);

    private static int LongestSubstringInRange(string s, int start, int end, int k)
    {
        if (end - start < k)
        {
            return 0;
        }

        var range = new SubstringRange(start, end);
        var counts = CountCharacters(s, range);
        var splitIndex = FindCharacterBelowThreshold(s, range, k, counts);

        if (splitIndex < 0)
        {
            return end - start;
        }

        var left = LongestSubstringInRange(s, start, splitIndex, k);
        var right = LongestSubstringInRange(s, splitIndex + 1, end, k);
        return Math.Max(left, right);
    }

    private static HashMap<char, int> CountCharacters(string s, SubstringRange range)
    {
        var counts = new HashMap<char, int>();

        for (var i = range.Start; i < range.End; i++)
        {
            counts.TryGetValue(s[i], out var count);
            counts.Set(s[i], count + 1);
        }

        return counts;
    }

    private static int FindCharacterBelowThreshold(string s, SubstringRange range, int k, HashMap<char, int> counts)
    {
        for (var i = range.Start; i < range.End; i++)
        {
            counts.TryGetValue(s[i], out var count);

            if (count < k)
            {
                return i;
            }
        }

        return -1;
    }
}
