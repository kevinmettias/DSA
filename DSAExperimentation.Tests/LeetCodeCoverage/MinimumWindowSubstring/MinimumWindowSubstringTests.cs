namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumWindowSubstring;

public sealed class MinimumWindowSubstringTests
{
    [Theory]
    [InlineData("ADOBECODEBANC", "ABC", "BANC")]
    [InlineData("a", "a", "a")]
    [InlineData("a", "aa", "")]
    public void MinWindow_SlidingCounts_ReturnsSmallestCoveringSubstring(string s, string t, string expected) => Assert.Equal(expected, MinWindow(s, t));

    private static string MinWindow(string s, string t)
    {
        var need = new int[128]; foreach (var ch in t) need[ch]++;
        var missing = t.Length; var bestStart = 0; var bestLen = int.MaxValue; var left = 0;
        for (var right = 0; right < s.Length; right++)
        {
            if (need[s[right]]-- > 0) missing--;
            while (missing == 0)
            {
                if (right - left + 1 < bestLen) { bestStart = left; bestLen = right - left + 1; }
                if (++need[s[left++]] > 0) missing++;
            }
        }
        return bestLen == int.MaxValue ? string.Empty : s.Substring(bestStart, bestLen);
    }
}
