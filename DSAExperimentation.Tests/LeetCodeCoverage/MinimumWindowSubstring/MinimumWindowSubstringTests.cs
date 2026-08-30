using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumWindowSubstring;

// LeetCode 76. Minimum Window Substring: a single sliding-window pass tracking each
// needed character's remaining count in this repo's own HashMap<char,int> - the
// same primitive/precedent LongestSubstringWithoutRepeatingCharactersTests uses for
// its own sliding window, just counting "still needed" instead of "last seen".
// Characters absent from HashMap.TryGetValue simply aren't in t and are ignored,
// the same way the classic fixed-size-array solution leaves untouched slots at 0.
public sealed partial class MinimumWindowSubstringTests
{
    [Theory]
    [InlineData("ADOBECODEBANC", "ABC", "BANC")]
    [InlineData("a", "a", "a")]
    [InlineData("a", "aa", "")]
    [InlineData("ab", "b", "b")]
    public void MinWindow_SlidingWindowHashMap_ReturnsSmallestCoveringSubstring(string s, string t, string expected)
        => Assert.Equal(expected, MinWindow(s, t));

    private static string MinWindow(string s, string t)
    {
        var need = new HashMap<char, int>();

        foreach (var ch in t)
        {
            need.TryGetValue(ch, out var count);
            need.Set(ch, count + 1);
        }

        var missing = t.Length;
        var left = 0;
        var bestStart = 0;
        var bestLength = int.MaxValue;

        for (var right = 0; right < s.Length; right++)
        {
            var incoming = s[right];

            if (need.TryGetValue(incoming, out var remaining))
            {
                need.Set(incoming, remaining - 1);

                if (remaining > 0)
                {
                    missing--;
                }
            }

            while (missing == 0)
            {
                if (right - left + 1 < bestLength)
                {
                    bestStart = left;
                    bestLength = right - left + 1;
                }

                var outgoing = s[left];

                if (need.TryGetValue(outgoing, out var freed))
                {
                    need.Set(outgoing, freed + 1);

                    if (freed >= 0)
                    {
                        missing++;
                    }
                }

                left++;
            }
        }

        return bestLength == int.MaxValue ? string.Empty : s.Substring(bestStart, bestLength);
    }
}
